using Microsoft.VisualBasic.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordListBuilder.WordListControl.Hooks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WordListBuilder.WordListControl
{
    public partial class WordList : UserControl, IList<string>
    {

        /// <summary>
        /// The proportion of scroll heat that should be lost after 1 second
        /// </summary>
        private static float _scrollHeatDecay = 0.96f;
        /// <summary>
        /// An exponent that determines the impact that scroll heat has on the scrolling velocity.
        /// In general, lower values cause the scroll heat to have a lower effect.
        /// </summary> 
        private static float _scrollHeatExponent = 2.4f;
        //This control is primarily designed for huge lists with THOUSANDS of words,
        //so it's important that we can both scroll fairly precisely, but also very
        //very quickly when desired

        /// <summary>
        /// Event arguments for dragging strings into the list control.
        /// <see cref="input"/> provides the string that was dragged, and
        /// <see cref="output"/> can be used to set the string that will be
        /// inserted into the list.
        ///
        /// <para></para>
        /// E.g this can be used to capture a dragged file and interpret
        /// some details from it.
        /// </summary>
        public class StringDragArgs
        {
            /// <summary>
            /// The string being dropped into the control
            /// </summary>
            public string input;
            /// <summary>
            /// The string that the control should add
            /// </summary>
            public string? output;
            public StringDragArgs(string input)
            {
                this.input = input;
                output = input;
            }
        }

        /// <summary>
        /// Callback delegate for handling the insertion of strings into the control
        /// using the mouse cursor.
        /// </summary>
        /// <param name="args"></param>
        public delegate void StringDragHandler(StringDragArgs args);

        /// <summary>
        /// Called when a string is drag-dropped into the control using the mouse cursor
        /// </summary>
        public event StringDragHandler? StringDragged;

        /// <summary>
        /// A list placed conceptually to the left of this control. <para></para>
        /// The selected item is moved into this collection when the left arrow is pressed.
        /// </summary>
        public IList<string>? LeftNeighbor { get; set; } = null;


        /// <summary>
        /// A list placed conceptually to the right of this control. <para></para>
        /// The selected item is moved into this collection when the right arrow is pressed.
        /// </summary>
        public IList<string>? RightNeighbor { get; set; } = null;

        /// The height of each item, in pixels
        /// </summary>
        private float _itemHeight = 14f;
        /// <summary>
        /// Sets the height of items in the list
        /// </summary>
        public float ItemHeight
        {
            get => _itemHeight;
            set
            {
                _itemHeight = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The padding between each item, in pixels
        /// </summary>
        private float _itemPadding = 3f;
        public float ItemPadding
        {
            get => _itemPadding;
            set
            {
                _itemPadding = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The visual padding from the top of the control
        /// </summary>
        public float GetTopPadding => 4 + Padding.Top;

        /// <summary>
        /// The visual padding from the sides of the control
        /// </summary>
        public float GetSidePadding => 4 + Padding.Left;

        /// <summary>
        /// The number of items that can be fit into this display. Expect up to +1 to be partially visible
        /// due to rounding.
        /// </summary>
        public float GetItemsDisplayed => Height / (ItemHeight + ItemPadding);

        /// <summary>
        /// The internal list of words to use for this list.
        /// </summary>
        private List<string> _words = [];

        /// <summary>
        /// Gets or sets the list of words that are displayed by this list.
        /// 
        /// This setter <b>copies</b> the provided list, and <b>does not</b> modify its contents.
        /// </summary>
        public List<string> Words 
        { 
            get => _words; 
            set { 
                _words = new List<string>(value); 
                Resort(); 
            } 
        }

        /// <summary>
        /// The target scroll destination. This is the value we usually tell to the user,
        /// but the viewport is actually bound to <see cref="_scrollViewportOffset"/>
        /// </summary>
        private float _scrollTarget = 0f;
        /// <summary>
        /// The internal scroll position value, which is used to translate the viewport.
        /// 
        /// Viewport control should be handled via <see cref="_scrollTarget"/>, towards which this value
        /// is smoothed.
        /// </summary>
        private float _scrollViewportOffset = 0f;
        /// <summary>
        /// A value indicating how heavily the user has scrolled in a f window of time preceding
        /// the current moment
        /// </summary>
        private float _scrollHeat = 0f;

        /// <summary>
        /// Internal value for <see cref="SelectedIndex"/>
        /// </summary>
        int _selectedIndex;
        /// <summary>
        /// The index in the underlying list of the currently selected item.
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (Count == 0) _selectedIndex = -1;
                else _selectedIndex = Math.Min(Words.Count - 1, Math.Max(0, value));

                if (SelectedIndex < _scrollTarget + 0.5f) _scrollTarget = SelectedIndex;
                if (SelectedIndex > _scrollTarget + GetItemsDisplayed - 1.5f) _scrollTarget = SelectedIndex - GetItemsDisplayed + 1.5f;

                Invalidate();
            }
        }

        /// <summary> An internal measure of the last time the control was scrolled. </summary>
        private DateTime _lastScroll = DateTime.UtcNow;

        /// <summary>  The point at which we clicked, or PointF.Empty otherwise </summary>
        PointF _clickedPoint = PointF.Empty;

        /// <summary>  The point at which we clicked, or PointF.Empty otherwise </summary>
        PointF _dragTarget = PointF.Empty;

        /// <summary> A string value for the currently dragged word </summary>
        string _dragOfferedValue = "";

        /// <summary>
        /// The value that was grabbed by the mouse when dragging started
        /// </summary>
        string _dragGrabbedValue = "";


        /// <summary>
        /// Gets or sets the scroll position, where a value of N refers to the Nth word being at
        /// the top of visible area
        /// </summary>
        public float ScrollPosition
        {
            get => _scrollTarget;
            set {
                _scrollTarget = value;
                Invalidate();
            }
        }

        /// <summary>
        /// The number of words in this list
        /// </summary>
        public int Count => Words.Count;

        /// <summary>
        /// Whether this list is readonly
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Provides an internal getter/setter for the sort layout.
        /// 
        /// <para>Use with care when e.g providing cached presorted data.</para>
        /// </summary>
        /// <param name="b"></param>
        public void _setSortedSkipLayout(bool b)
        {
            _isSorted = b;
        }

        private bool _isSorted = true;
        /// <summary>
        /// Whether this list is sorted. Defaults to True.
        /// </summary>
        public bool IsSorted
        {
            get => _isSorted; set
            {
                if (value != _isSorted)
                {
                    _isSorted = value;
                    Resort();
                }
            }
        }

        /// <summary>
        /// Access this list of words via the given integer index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get => Words[index]; set
            {
                Words[index] = value; 
                Resort();
            }
        }

        /// <summary>
        /// Gets the index of this list under the given Y coordinate (in client space)
        /// </summary>
        /// <param name="relativeY"></param>
        /// <returns></returns>
        int GetListIndex(float relativeY)
        {
            return (int)float.Floor(_scrollViewportOffset + (relativeY - GetTopPadding) / (ItemHeight + ItemPadding));
        }


        /// <summary>
        /// Triggers the control to resort all of the words inside of it
        /// </summary>
        public void Resort()
        {
            if(IsSorted) _words.Sort();
            Refresh();
        }


        public WordList()
        {

            InitializeComponent();
            DoubleBuffered = true; //make it glidey and smooooth
            Paint += WordList_Paint;

            //During DragDrops, the drag drop thing captures mouse inputs.
            //Which prevents the list from being scrolled while a drag-drop is occurring.
            //To circumvent this, we can use a low level winAPI hook to capture the mouse event,
            //Then manually handle the scrolling with our own event
#if DEBUG
            //MouseWheel += WordList_MouseWheel;

#else
            //However, it's really annoying debugging input hooks and I'm not sure how to fix it
            //So we'll only enable this feature in release mode for the time being
            MouseHooks.MouseHook.ApplyHook(this, ScrollMouse);
#endif

            MouseHooks.MouseHook.ApplyMouseWheelHook(this, ScrollMouse);

            MouseDown += WordList_MouseDown;
            MouseMove += WordList_MouseMove;
            MouseUp += WordList_MouseUp;
            MouseDoubleClick += WordList_MouseDoubleClick;

            PreviewKeyDown += WordList_PreviewKeyDown;

            DragOver += WordList_DragOver;
            DragDrop += WordList_DragDrop;

            this.AllowDrop = true;
        }

        /// <summary>
        /// Catches the pressing of arrow keysies to move the selection bubble thingy
        /// or move items laterally.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordList_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {

            switch(e.KeyCode)
            {
                //handle arrow key up down selection
                case Keys.Up: SelectedIndex -= 1; break;
                case Keys.Down: SelectedIndex += 1; break;
                //handle left/right actions
                case Keys.Left: MoveSelectedLeft();  break;
                case Keys.Right: MoveSelectedRight(); break;
                //allow the delete key to borkify
                case Keys.Delete:
                    if (Count > 0 && SelectedIndex > 0 && SelectedIndex < Count)
                    {
                        RemoveAt(SelectedIndex);
                    }
                    break;
                default: break;
            }
        }
        
        private void _MoveTo(IList<string> target)
        {
            if (SelectedIndex >= 0 && SelectedIndex < Count)
            {
                //Add the item to the target list
                if (target is WordList w) w.Add(this[SelectedIndex], true);
                else target.Add(this[SelectedIndex]);

                RemoveAt(SelectedIndex);
                //Update the selected index accordingly
                if (Count == 0) SelectedIndex = -1;
                else SelectedIndex = Math.Min(Words.Count - 1, SelectedIndex);
                //refresh visuals
                //Resort();


                if (target is Control c) c.Invalidate();




            }
        }

        /// <summary>
        /// Moves the currently selected item to the left control
        /// as specified in <see cref="LeftNeighbor"/>
        /// </summary>
        public void MoveSelectedLeft()
        {
            //Move to the left... if it isn't null
            if (LeftNeighbor != null)
            {
                _MoveTo(LeftNeighbor);
            }
        }

        /// <summary>
        /// Moves the currently selected item to the control to the right
        /// as specified in <see cref="RightNeighbor"/>
        /// </summary>
        public void MoveSelectedRight()
        {
            if(RightNeighbor != null)
            {
                _MoveTo(RightNeighbor);
            }
        }


        private void WordList_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {

            }
        }

        /// <summary>
        /// Seeks to insert the given string to the list based on the y position in client
        /// coords.<para></para>Accounts for scrolling
        /// </summary>
        /// <param name="value">The string to insert</param>
        /// <param name="y">The y distance relative to the top of this control</param>
        private void InsertAtYPosition(string value, float y)
        {
            int n = GetListIndex(y);
            if (n < Count)
            {
                Insert(Math.Max(n, 0), value);
            }
            else Add(value);
        }

        private void WordList_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.Text) ?? false)
            {
                //get the dropped data
                var data = (string)(e.Data.GetData(DataFormats.Text) ?? "");
                StringDragArgs sda = new StringDragArgs(data);
                StringDragged?.Invoke(sda);
                if (sda.output != null)
                {
                    //set the effect appropriately
                    e.Effect = ModifierKeys.HasFlag(Keys.Control) ? DragDropEffects.Copy : DragDropEffects.Move;
                    //insert it accordingly
                    InsertAtYPosition(sda.output!, PointToClient(new(e.X, e.Y)).Y);
                }
            }
            else if(e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
            {
                var data = (string[])(e.Data.GetData(DataFormats.FileDrop) ?? new string[0]);
                foreach(string s in data)
                {
                    StringDragArgs sda = new StringDragArgs(s);
                    StringDragged?.Invoke(sda);
                    if (sda.output != null)
                    {
                        //set the effect appropriately
                        //insert it accordingly
                        InsertAtYPosition(sda.output!, PointToClient(new(e.X, e.Y)).Y);
                    }
                }
                e.Effect = DragDropEffects.Copy;

            }
        }

        /// <summary>
        /// Called when the word list is dragged over using the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordList_DragOver(object? sender, DragEventArgs e)
        {
            //Only allow this event to stem from other controls/windows
            if (_dragGrabbedValue != null && _dragGrabbedValue.Length > 0) return;
            //Only allow text
            if (e.Data?.GetDataPresent(DataFormats.Text) ?? false)
            {
                e.Effect = DragDropEffects.All; //all effects are allowed
                SelectedIndex = GetListIndex(PointToClient(new(e.X, e.Y)).Y);

                float dist = _selectedIndex - _scrollTarget;
                if (dist < 1.5) _scrollTarget -= float.Pow(2 - dist, 1.5f) / 2f; 
                else if(dist > GetItemsDisplayed - 2)
                {
                    _scrollTarget += float.Pow(GetItemsDisplayed - dist, 1.5f) / 2f;
                }

            }
            else if (e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
            {
                e.Effect = DragDropEffects.Copy;
                SelectedIndex = GetListIndex(PointToClient(new(e.X, e.Y)).Y);
                float dist = _selectedIndex - _scrollTarget;
                if (dist < 1.5) _scrollTarget -= float.Pow(2 - dist, 1.5f) / 2f;
                else if (dist > GetItemsDisplayed - 2)
                {
                    _scrollTarget += float.Pow(GetItemsDisplayed - dist, 1.5f) / 2f;
                }
            }
        }

        private void WordList_MouseUp(object? sender, MouseEventArgs e)
        {
            _clickedPoint = PointF.Empty;
            Invalidate();
        }

        private void WordList_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_clickedPoint == PointF.Empty) return;
            //calculate the distance of motion
            (float x, float y) dP = (_clickedPoint.X - e.X, _clickedPoint.Y - e.Y);
            float d = float.Sqrt(dP.x * dP.x + dP.y * dP.y);
            if(d > 3)
            {
                int n = GetListIndex(e.Y);
                if (n < 0 || n >= Count) return;

                _dragGrabbedValue = this[n];
                DataObject data = new DataObject(DataFormats.Text, _dragGrabbedValue);

                //perform the drag drop event
                var result = DoDragDrop(data, DragDropEffects.All);
                Debug.WriteLine($"Done Drag, {(int)result}");
                _clickedPoint = PointF.Empty;
                
                //possible results are
                //DragDropEffects.Scroll + Move or just Move
                List<DragDropEffects> goodResult = [DragDropEffects.Scroll | DragDropEffects.Move, DragDropEffects.Move];
                foreach (int nn in goodResult) Debug.WriteLine(nn);
                if (goodResult.Contains(result))
                {
                    if (n >= 0 && n < Count && _dragGrabbedValue.Equals(this[n]))
                    {
                        Words.RemoveAt(n);
                        Invalidate();
                    }
                }

                //reset the dragging thingy
                _dragGrabbedValue = "";
                _dragTarget = Point.Empty;

            }
        }


        private void WordList_MouseDown(object? sender, MouseEventArgs e)
        {
            //get the location of the click
            _clickedPoint = e.Location;
            //catch the scrolling view
            _scrollViewportOffset = Math.Max(0, _scrollViewportOffset);
            _scrollTarget = _scrollViewportOffset;
            _selectedIndex = GetListIndex(e.Y);
            Refresh();
        }

        /// <summary>
        /// Updates the scroll heat value according to its exponential decay function,
        /// heat = heat * time^x
        /// </summary>
        private void UpdateScrollHeat()
        {
            //We aren't guaranteed a predictable tick in the validation feedback loop,
            //So instead we can just project the scroll heat using a simple exponential decay
            double s = (DateTime.UtcNow - _lastScroll).TotalSeconds;
            _scrollHeat *= (float)double.Pow(1 - _scrollHeatDecay, s);
            if (float.Abs(_scrollHeat) < 1e-1) _scrollHeat = 0;
            _lastScroll = DateTime.UtcNow;
        }

        private void ScrollMouse(int delta)
        {
            //get the scroll amount and round it to good values
            //It's hard to know how Delta works, but this seems fine for Windows.
            float amount = delta / -120f;
            UpdateScrollHeat();

            //clamp the directionality
            if (float.Sign(amount) != float.Sign(_scrollHeat)) _scrollHeat = 0;
            //now scroll by the given amount
            _scrollHeat += amount;
            //scale the amount by the heat using the exponentiator
            amount *= float.Pow(float.Abs(_scrollHeat), _scrollHeatExponent);
            _scrollTarget += amount; //this invalidates the control
            _scrollTarget = float.Min(_scrollTarget, 1 + _words.Count - GetItemsDisplayed);
            _scrollTarget = float.Max(0, _scrollTarget);

            Refresh();
        }

        private void WordList_MouseWheel(object? sender, MouseEventArgs e)
        {
            ScrollMouse(e.Delta);
        }

        /// <summary>
        /// A validation feedback for updating the control
        /// </summary>
        private void WordList_Validated()
        {
            //Check if we need to update the scroller again (smoothing)
            if (float.Abs(_scrollViewportOffset - _scrollTarget) > 1e-2)
            {
                _scrollViewportOffset += (_scrollTarget - _scrollViewportOffset) * 0.03f;
                Invalidate();
                Update();
            }
            else _scrollViewportOffset = _scrollTarget;
        }

        /// <summary>
        /// Paints the word list, rendering all the things
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordList_Paint(object? sender, PaintEventArgs e)
        {
            using var f = new Font("Consolas", 8, FontStyle.Italic);
            if (DesignMode == true)
            {
                //draw the name of the wordlist and then escape
                e.Graphics.DrawString(this.Name, f, Brushes.Black, new PointF(GetSidePadding, GetTopPadding));
            }
            else
            {
                float x = GetSidePadding;

                //calculate the ideal range in the list
                int start = int.Max(0, (int)float.Floor(_scrollViewportOffset));
                int end = int.Min(Count - 1, start + (int)float.Ceiling(GetItemsDisplayed));

                //calculate the Y offset for drawing
                float offset = _scrollViewportOffset - float.Floor(_scrollViewportOffset);
                offset *= -(ItemHeight + ItemPadding);

                //make sure we're doing smooth drawing
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.SetClip(new Rectangle(0, 0, Width, Height));
                for (int i = int.Max(start - 1, 0); i <= end; i++)
                {
                    int j = i;
                    //get the position of the word in the box
                    float y = offset + GetTopPadding + (j - start) * (ItemHeight + ItemPadding);
                    //if this is the selected item, then highlight underneath it
                    if (SelectedIndex == i)
                    {
                        e.Graphics.FillRectangle(Brushes.CornflowerBlue, x, y, this.Width - 2 * x, ItemHeight + 2 * ItemPadding);
                    }

                    //draw the text
                    using var sb = new SolidBrush(this.ForeColor);
                    e.Graphics.DrawString(this[i], this.Font, sb, x, y);
                }

                //calculate the scrollbar position
                float realHeight = Words.Count;
                float scrollAmt = _scrollViewportOffset / realHeight;
                float barHeight = float.Max(32, Height * GetItemsDisplayed  / (float)realHeight);

                if(realHeight > Height)
                {
                    e.Graphics.FillRectangle(Brushes.CornflowerBlue, 
                        new RectangleF(Width - GetSidePadding - 16, (Height - barHeight) * scrollAmt, 
                        16, barHeight));
                }



                e.Graphics.ResetClip();
                //draw our border rectangle        
            }


            //draw a bounding box
            e.Graphics.DrawRectangle(Pens.Black, new(0, 0, Width-1, Height-1));
            //now notify that we've finished validating
            if(DesignMode == false) WordList_Validated();
            //This feedback loop lets us update the control roughly at the
            //framerate determined by the framework, and avoids the need for
            //silly things like timers.
        }



        public IEnumerator<string> GetEnumerator()
        {
            return Words.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Words.GetEnumerator();
        }

        public int IndexOf(string item)
        {
            return Words.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            Words.Insert(index, item);
            Resort();
        }

        public void RemoveAt(int index)
        {
            Words.RemoveAt(index);
        }

        public void Add(string item) => Add(item, false);

        public void Add(string item, bool x)
        {
            //find the right index
            if (Words.Count == 0 || !IsSorted)
            {
                Words.Add(item);
                if (x) SelectedIndex = Words.Count - 1;
            }
            //binary search to find the correct placement
            else
            {
                int index = Words.BinarySearch(item);
                if (index < 0) index = ~index;
                if (index >= Words.Count) Words.Add(item);
                else Words.Insert(index, item);

                if (x) SelectedIndex = index;


            }
            Invalidate();
        }



        public void Clear()
        {
            Words.Clear();
            Resort();
        }

        public bool Contains(string item)
        {
            return Words.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            Words.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            try
            {
                return Words.Remove(item);
            }
            finally
            {
            }
        }

    }
}

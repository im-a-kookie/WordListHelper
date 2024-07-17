
using System.Diagnostics;
using System.Diagnostics.Metrics;
using WordListBuilder.WordListControl.Hooks;
using static WordListBuilder.WordListControl.WordList;

namespace WordListBuilder
{
    public partial class MainWindow : Form
    {


        Dictionary<string, string> inputFiles = [];



        public MainWindow()
        {
            InitializeComponent();

            //set up the relations between the two boxes
            wordList1.RightNeighbor = wordList2;
            wordList2.LeftNeighbor = wordList1;

            //The lists will only accept files dragged into them
            //This delegate takes care of it for us
            var f = (StringDragArgs x) =>
            {
                lock (inputFiles)
                {
                    if (File.Exists(x.input))
                    {
                        x.output = Path.GetFileName(x.input);
                        inputFiles.Add(x.output, x.input);
                    }
                    else x.output = null;
                }
            };

            //add the delegate to the drag event
            wordList1.StringDragged += (x) => f(x);
            wordList2.StringDragged += (x) => f(x);


            bAdd.Click += BAdd_Click;

            button3.Click += Button3_Click;

        }

        private void Button3_Click(object? sender, EventArgs e)
        {
            LoadDictionaries();
        }

        private void LoadDictionaries()
        { 
            lock (this)
            {
                //By mutexing this region, and trapping it with p.ShowDialog
                //We can trap references to counter and wordLists within this scope,
                //And then spawn multiple threads in p while maintaining thread safety.
                //(scoped isolation)
                Progress p = new Progress();

                //counter to check how many threads are running/etc
                int counter = inputFiles.Count;
                Dictionary<string, WordCollection> wordLists = [];

                foreach (var k in inputFiles)
                {
                    var dict = new WordCollection(k.Value, k.Key);
                    wordLists.Add(k.Key, dict);
                }

                //when progress form loads, have it start spawning threads
                //We could use BlockingCollection and threadpool but in this case it's NBD.
                p.Load += (a, b) =>
                {
                    //one thread per wordlist file
                    foreach(var k in wordLists)
                    { 
                        Thread t = new Thread((x) =>
                        {
                            //ugh casting
                            if (x is WordCollection wc)
                            {
                                wc.Build();
                                //as above, this reference is now stable
                            }
                            Interlocked.Decrement(ref counter);
                            p.Invalidate();
                        });
                        t.Start(k.Value);
                        //decrement to indicate completion
                    }
                };

                //this will now be called when the form progress is completed
                p.Invalidated += (a, b) =>
                {
                    if (counter <= 0)
                        p.BeginInvoke(() => p.Close());
                };

                //now we can capture focus using the progress form, which locks the main form here.
                //This protects from race conditions
                p.ShowDialog();
                p.Dispose(); //cleanup?
                //Word lists now contains a populated list of word collections
                Debug.WriteLine(wordLists.Count);

                ListEditor editor = new ListEditor();
                editor.Prepare(wordLists.Values, []);
                editor.ShowDialog();


                //We can now push the wordList back into a synchronized context
            }

        }

        private void BAdd_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Text Files (txt) | *.txt",
                Multiselect = true
            })
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach(string path in ofd.FileNames)
                    {
                        if(File.Exists(path))
                        {
                            string pathName = Path.GetFileName(path);
                            inputFiles.TryAdd(pathName, path);
                        }
                    }

                }
            }

            
            var l = inputFiles.Keys.ToList();
            wordList1.Words = l;


        }
    }
}

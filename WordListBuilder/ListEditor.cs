using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using WordListBuilder.WordListControl;

namespace WordListBuilder
{
    public partial class ListEditor : Form
    {
        public WordList BungWords => wlRare;
        List<StringThing> words = new List<StringThing>();

        class StringThing : IComparable
        {
            public string word;
            public int count;

            public StringThing(string word)
            {
                this.word = word;
                this.count = 0;
            }

            public int CompareTo(object? obj)
            {
                if (obj is StringThing st) return word.CompareTo(st.word);
                else return 0;
            }
        }

        public void Prepare(IEnumerable<WordCollection> words, IEnumerable<WordCollection> badWords)
        {
            Dictionary<string, int> wordOccurrences = new();
            List<string> allWords = new List<string>();
            foreach (var wc in words)
            {
                foreach (var w in wc.Words)
                {
                    if (wordOccurrences.TryAdd(w, 0)) allWords.Add(w);
                    wordOccurrences[w] += 1;
                }
            }
            allWords.Sort();
            foreach(var s in allWords)
            {
                this.words.Add(new StringThing(s) { count = wordOccurrences[s] });
            }

        }

        public ListEditor()
        {
            InitializeComponent();

            var c = new WordList[] { wlNonsense, wlRare, wlSensible };
            for(int i = 0; i < c.Length; i++)
            {
                if (i < c.Length - 1) c[i].RightNeighbor = c[i + 1];
                if (i > 0) c[i].LeftNeighbor = c[i - 1];
            }

            this.Load += (s, e) => nupbThresholdRare.Value = 2;
            nupbThresholdRare.ValueChanged += (s, e) =>
            {
                Reproc();
            };

            nupdNonsense.ValueChanged += (s, e) =>
            {
                Reproc();
            };

            Reproc();


            bBuild.Click += Button1_Click;

        }

        private void Button1_Click(object? sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var s in wlRare)
            {
                sb.AppendLine(s);
            }
            File.WriteAllText("extraWords.txt", sb.ToString().Trim());

            sb.Clear();

            foreach(var s in wlSensible)
            {
                sb.AppendLine(s);
            }
            File.WriteAllText("commonWords.txt", sb.ToString().Trim());

        }

        int _counter = 0;
        int _oldN = 0;
        int _oldR = 0;

        public async Task<List<string>[]> _procListCaller(int[] thresholds)
        {
            return await Task.Run(() => ProcLists(thresholds));
        }

        public List<string>[] ProcLists(int[] thresholds)
        {
            List<string>[] nrs = new List<string>[] { new(), new(), new() };
            foreach (var k in words)
            {
                bool has = false;
                for(int i = 0; i < thresholds.Length && i < nrs.Length; ++i)
                {
                    if (k.count < thresholds[i])
                    {
                        nrs[i].Add(k.word);
                        has = true;
                        break;
                    }
                }
                if (!has) nrs.Last().Add(k.word);
            }
            return nrs;
        }

        public void Reproc()
        {
            if (_counter > 0) return;
            ++_counter;
            var c = new WordList[] { wlNonsense, wlRare, wlSensible };
            foreach (var cc in c)
            {
                cc.Clear();
                cc.IsSorted = false;
            }

            _oldN = (int)nupdNonsense.Value;
            _oldR = (int)nupbThresholdRare.Value;


            _procListCaller([(int)nupdNonsense.Value, (int)nupbThresholdRare.Value]).ContinueWith((x) =>
                {
                    var r = x.Result;
                    lock (this)
                    {
                        for (int i = 0; i < c.Length && i < r.Length; ++i)
                        {
                            c[i].Words.AddRange(r[i]);
                            c[i]._setSortedSkipLayout(true);
                        }
                        --_counter;

                        if (_oldN != nupdNonsense.Value || _oldR != nupbThresholdRare.Value)
                        {
                            Reproc();
                        }
                    }

                    this.Invoke(() =>
                    {
                        lock (this)
                        {
                            gbRare.Text = $"Rare Words ({wlRare.Count})";
                            gbCommon.Text = $"Common Words ({wlSensible.Count})";
                            gbNonsense.Text = $"Nonsense Words ({wlNonsense.Count})";
                        }
                    });


                });

        }

    }
}

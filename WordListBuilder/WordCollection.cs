using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WordListBuilder
{
    public class WordCollection
    {
        string file;
        string name;
        public HashSet<string> Words = [];

        public WordCollection(string file, string name) {
            this.file = file;
            this.name = name;
        }
        
        public void Build()
        {
            lock (this)
            {
                string s = File.ReadAllText(file).ToLower();
                //convert to span
                ReadOnlySpan<char> span = s.AsSpan();

                //iterate from newline to newline
                while (span.Length > 0)
                {
                    int n = span.IndexOf('\n');
                    if (n < 0) n = span.Length;
                    int a = 0;
                    //do some preliminary whitespace trimming
                    while (a < span.Length &&( span[a] == '\r' || span[a] == ' ')) a += 1;
                    while (n >= 1 && (span[n - 1] == '\r' || span[n - 1] == ' ')) n -= 1;
                    if(a < 0 || a >= span.Length || n - a <= 0)
                    {
                        span = span.Slice(n + 1);
                        continue;
                    }

                    //grab the slicy thing
                    var word = span.Slice(a, n - a);
                    bool valid = true;
                    //check that all the letters are actually letters
                    for (int i = 0; i < word.Length; i++)
                    {
                        if (word[i] < 'a' || word[i] > 'z')
                        {
                            valid = false;
                            break;
                        }
                    }
                    //only add words that contain only letters
                    if(valid)
                    {
                        Words.Add(word.ToString());
                    }

                    //step frowards
                    span = span.Slice(n);

                }
            }
        }


    }
}

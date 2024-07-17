using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using WordListBuilder;
using WordListBuilder.WordListControl;

namespace Tests
{
    public class TestWordListUControl
    {

        WordList control;


        static class Unsorted
        {
            /// <summary>
            /// The original list. D A B
            /// </summary>
            public static List<string> original = ["d", "a", "b"];
            /// <summary>
            /// The modified (appended D A B C)
            /// </summary>
            public static List<string> appended = ["d", "a", "b", "c"];
            /// <summary>
            /// The modified list (inserted at 1, D C A B)
            /// </summary>
            public static List<string> inserted = ["d", "c", "a", "b"];

        }

        static class Sorted
        {
            /// <summary>
            /// The original sorted list
            /// </summary>
            public static List<string> original = ["a", "b", "d"];
            /// <summary>
            /// The appended sorted list
            /// </summary>
            public static List<string> appended = ["a", "b", "c", "d"];
        }



        [SetUp]
        public void Setup()
        {
            if (control != null && !control.IsDisposed) control.Dispose();
            control = new WordList();
        }

        [TearDown]
        public void TearDown()
        {
            control?.Dispose();
        }

        /// <summary>
        /// Ensure that indexing the control, correctly indexes the underlying list
        /// </summary>
        [Test]
        public void TestIndexing()
        {
            control.Words = Sorted.original;

            for(int i = 0; i < control.Count; i++)
            {
                Assert.That(control[i], Is.EqualTo(control.Words[i]));
            }
        }

        /// <summary>
        /// Ensure that the control reports length correctly
        /// </summary>
        [Test]
        public void TestCount()
        {
            control.Words = Sorted.original;
            //make sure the length is correct
            Assert.That(control.Count, Is.EqualTo(control.Words.Count));

        }

        [Test]
        public void TestEnumator()
        {
            control.Words = Sorted.original;
            int i = 0;
            foreach (string w in control)
            { 
                Assert.That(w, Is.EqualTo(Sorted.original[i++]));
            }
        }


        /// <summary>
        /// Test the setting of words via standard IList accessors in a
        /// sorted control (sorted is the default setting).
        /// </summary>
        [Test]
        public void SortedSetting()
        {
            //set the words to d,a,b
            control!.Words = Unsorted.original;

            Assert.That(control.Count, Is.EqualTo(Unsorted.original.Count));

            //we expect them to be sorted to a,b,c
            for (int i = 0; i < Unsorted.original.Count; i++)
            {
                Assert.That(Sorted.original[i], Is.EqualTo(control[i]));
            }

            //We also expect the original list to be unaffected
            for (int i = 0; i < Sorted.original.Count; i++)
            {
                Assert.That(Unsorted.original[i], Is.Not.EqualTo(Sorted.original[i]));
            }

        }


        /// <summary>
        /// Test the IList interface implementation
        /// </summary>
        [Test]
        public void SortedAdd()
        {
            //set the words to the unsorted list
            //which we expect now to be a,b,d
            control.Words = Unsorted.original;
            var l = new List<string>(control);

            //add the letter c
            control.Add("c");
            //make sure the length is correct
            Assert.That(control.Count, Is.EqualTo(Sorted.appended.Count));

            //we now expect a,b,c,d
            for (int i = 0; i < Sorted.appended.Count; i++)
            {
                Assert.That(Sorted.appended[i], Is.EqualTo(control[i]));
            }

        }

        /// <summary>
        /// Test the IList interface implementation
        /// </summary>
        [Test]
        public void SortedRemove()
        {
            //set the words to the extended sorted list
            control.Words = Sorted.appended;
            var l = new List<string>(control);

            //remove it
            control.Remove("c");
            Assert.That(control.Count, Is.EqualTo(Sorted.original.Count));
            for (int i = 0; i < Sorted.original.Count; i++)
            {
                Assert.That(Sorted.original[i], Is.EqualTo(control[i]));
            }

        }


        /// <summary>
        /// Tests list indexation stuff
        /// </summary>
        [Test]
        public void SortedIndexOf()
        {
            Assert.NotNull(control);
            control.Words = Sorted.appended;
            Assert.That(control.IndexOf("c"), Is.EqualTo(2));
        }

        /// <summary>
        /// Test that Insert works correctly
        /// </summary>
        [Test]
        public void SortedInsert()
        {
            //add it at an arbitrary index
            control.Words = Sorted.original;
            //insert the new letter
            control.Insert(1, "c");
            Assert.That(control.Count, Is.EqualTo(Sorted.appended.Count));

            //The list is inherently sorteed. So we still expect a,b,c,d.
            for (int i = 0; i < Sorted.appended.Count; i++)
            {
                Assert.That(Sorted.appended[i], Is.EqualTo(control[i]));
            }
        }

        /// <summary>
        /// Tests that words are correctly set in an unsorted list
        /// </summary>
        [Test]
        public void UnsortedSetting()
        {
            control.IsSorted = false;
            control.Words = Unsorted.original;
            Assert.That(control.Count, Is.EqualTo(Unsorted.original.Count));
            for(int i = 0; i < Unsorted.original.Count; i++)
            {
                Assert.That(control[i], Is.EqualTo(Unsorted.original[i]));
            }
        }


        /// <summary>
        /// Test the IList interface implementation
        /// </summary>
        [Test]
        public void UnsortedAdd()
        {
            control.IsSorted = false;

            //set the words to the unsorted list
            //which we expect now to be a,b,d
            control.Words = Unsorted.original;
            var l = new List<string>(control);

            //add the letter c
            control.Add("c");
            //make sure the length is correct
            Assert.That(control.Count, Is.EqualTo(Unsorted.appended.Count));

            //we now expect a,b,c,d
            for (int i = 0; i < Unsorted.appended.Count; i++)
            {
                Assert.That(Unsorted.appended[i], Is.EqualTo(control[i]));
            }

        }

        /// <summary>
        /// Test the IList interface implementation
        /// </summary>
        [Test]
        public void UnortedRemove()
        {
            control.IsSorted = false;
            //set the words to the extended sorted list
            control.Words = Unsorted.appended;
            var l = new List<string>(control);

            //remove it
            control.Remove("c");
            Assert.That(control.Count, Is.EqualTo(Unsorted.original.Count));
            for (int i = 0; i < Unsorted.original.Count; i++)
            {
                Assert.That(Unsorted.original[i], Is.EqualTo(control[i]));
            }

        }

        /// <summary>
        /// Test that Insert works correctly
        /// </summary>
        [Test]
        public void UnortedInsert()
        {
            control.IsSorted = false;

            //add it at an arbitrary index
            control.Words = Unsorted.original;
            //insert the new letter
            control.Insert(1, "c");
            Assert.That(control.Count, Is.EqualTo(Unsorted.inserted.Count));

            //The list is inherently sorteed. So we still expect a,b,c,d.
            for (int i = 0; i < Unsorted.inserted.Count; i++)
            {
                Assert.That(Unsorted.inserted[i], Is.EqualTo(control[i]));
            }
        }

        [Test]
        public void UnsortedRemoveAt()
        {
            control.IsSorted = false;
            //add it at an arbitrary index
            control.Words = Unsorted.inserted;

            //remove it based on the sorted position
            control.RemoveAt(1);
            Assert.That(control.Count, Is.EqualTo(Unsorted.original.Count));
            for (int i = 0; i < Unsorted.original.Count; i++)
            {
                Assert.That(Unsorted.original[i], Is.EqualTo(control[i]));
            }
        }


        /// <summary>
        /// Tests list indexation stuff
        /// </summary>
        [Test]
        public void UnsortedIndexOf()
        {
            control.IsSorted = false;
            Assert.NotNull(control);
            control.Words = Unsorted.original;
            for(int i = 0; i < Unsorted.original.Count; i++)
            {
                Assert.That(control.IndexOf(Unsorted.original[i]), Is.EqualTo(i));
            }
            Assert.That(control.IndexOf("c"), Is.EqualTo(-1));
        }




        [OneTimeTearDown]
        public void FinalTearDown()
        {
            try
            {
                control?.Dispose();
            }
            catch
            {

            }
        }




    }
}
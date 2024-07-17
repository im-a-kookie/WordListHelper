# WordListBuilder

A simple project in C#.NET made to help with putting together word lists for my word puzzles ideas.

It's very simple, and essentially just filters word lists based on the frequency of word occurences.

Of note, handling multiple files with hundreds of thousands of lines, presents some interesting conditions for performance and UI smoothness. To this end, the program uses threading and asynchronous processing in a couple of areas, and demonstrates some simple techniques for threadsafety (such as scope isolation).

I also found the .NET ListBox class to be lacking, and have implemented my own custom control (the WordList UserControl) to help keep things working smoothly.

Notably, list.Sort() imposes a significant cost when the list is 500,000 items in length, so this is avoided for most insertions in favor of list.BinarySearch(). The WordList also implements some handy drag-drop functionality, and shows how Windows low level input hooks can be used to handle scrolling inputs during DragDrop events (which capture input and prevent wheel-based scrolling).

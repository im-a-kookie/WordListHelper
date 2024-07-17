namespace WordListBuilder
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            gbWordLists = new GroupBox();
            wordList1 = new WordListControl.WordList();
            tableLayoutPanel1 = new TableLayoutPanel();
            bRemove = new Button();
            bAdd = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            groupBox1 = new GroupBox();
            wordList2 = new WordListControl.WordList();
            tableLayoutPanel2 = new TableLayoutPanel();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            gbWordLists.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            menuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // gbWordLists
            // 
            gbWordLists.Controls.Add(wordList1);
            gbWordLists.Controls.Add(tableLayoutPanel1);
            gbWordLists.Location = new Point(12, 40);
            gbWordLists.Name = "gbWordLists";
            gbWordLists.Size = new Size(268, 423);
            gbWordLists.TabIndex = 3;
            gbWordLists.TabStop = false;
            gbWordLists.Text = "Extended Words";
            // 
            // wordList1
            // 
            wordList1.AllowDrop = true;
            wordList1.BackColor = SystemColors.ControlLightLight;
            wordList1.Dock = DockStyle.Fill;
            wordList1.IsSorted = true;
            wordList1.ItemHeight = 13F;
            wordList1.ItemPadding = 2F;
            wordList1.LeftNeighbor = null;
            wordList1.Location = new Point(3, 23);
            wordList1.Name = "wordList1";
            wordList1.RightNeighbor = null;
            wordList1.ScrollPosition = -1F;
            wordList1.SelectedIndex = -1;
            wordList1.Size = new Size(262, 358);
            wordList1.TabIndex = 5;
            wordList1.Words = (List<string>)resources.GetObject("wordList1.Words");
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel1.Controls.Add(bRemove, 0, 0);
            tableLayoutPanel1.Controls.Add(bAdd, 4, 0);
            tableLayoutPanel1.Dock = DockStyle.Bottom;
            tableLayoutPanel1.Location = new Point(3, 381);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(262, 39);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // bRemove
            // 
            bRemove.Location = new Point(3, 3);
            bRemove.Name = "bRemove";
            bRemove.Size = new Size(74, 29);
            bRemove.TabIndex = 7;
            bRemove.Text = "Remove";
            bRemove.UseVisualStyleBackColor = true;
            // 
            // bAdd
            // 
            bAdd.Location = new Point(185, 3);
            bAdd.Name = "bAdd";
            bAdd.Size = new Size(74, 29);
            bAdd.TabIndex = 6;
            bAdd.Text = "Add";
            bAdd.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.ControlLight;
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(577, 28);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(wordList2);
            groupBox1.Controls.Add(tableLayoutPanel2);
            groupBox1.Location = new Point(286, 40);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(268, 423);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Profanities";
            // 
            // wordList2
            // 
            wordList2.AllowDrop = true;
            wordList2.BackColor = SystemColors.ControlLightLight;
            wordList2.Dock = DockStyle.Fill;
            wordList2.IsSorted = true;
            wordList2.ItemHeight = 13F;
            wordList2.ItemPadding = 2F;
            wordList2.LeftNeighbor = null;
            wordList2.Location = new Point(3, 23);
            wordList2.Name = "wordList2";
            wordList2.RightNeighbor = null;
            wordList2.ScrollPosition = -1F;
            wordList2.SelectedIndex = -1;
            wordList2.Size = new Size(262, 358);
            wordList2.TabIndex = 5;
            wordList2.Words = (List<string>)resources.GetObject("wordList2.Words");
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 5;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel2.Controls.Add(button1, 0, 0);
            tableLayoutPanel2.Controls.Add(button2, 4, 0);
            tableLayoutPanel2.Dock = DockStyle.Bottom;
            tableLayoutPanel2.Location = new Point(3, 381);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(262, 39);
            tableLayoutPanel2.TabIndex = 5;
            // 
            // button1
            // 
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(74, 29);
            button1.TabIndex = 7;
            button1.Text = "Remove";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(185, 3);
            button2.Name = "button2";
            button2.Size = new Size(74, 29);
            button2.TabIndex = 6;
            button2.Text = "Add";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(460, 469);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 5;
            button3.Text = "Process";
            button3.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(577, 512);
            Controls.Add(button3);
            Controls.Add(groupBox1);
            Controls.Add(gbWordLists);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainWindow";
            Text = "Form1";
            gbWordLists.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox gbWordLists;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private WordListBuilder.WordListControl.WordList wordList1;
        private Button bRemove;
        private Button bAdd;
        private GroupBox groupBox1;
        private WordListControl.WordList wordList2;
        private TableLayoutPanel tableLayoutPanel2;
        private Button button1;
        private Button button2;
        private Button button3;
    }
}

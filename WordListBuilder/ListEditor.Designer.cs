namespace WordListBuilder
{
    partial class ListEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListEditor));
            wlRare = new WordListControl.WordList();
            gbRare = new GroupBox();
            label1 = new Label();
            nupbThresholdRare = new NumericUpDown();
            gbCommon = new GroupBox();
            wlSensible = new WordListControl.WordList();
            gbNonsense = new GroupBox();
            label2 = new Label();
            wlNonsense = new WordListControl.WordList();
            nupdNonsense = new NumericUpDown();
            bBuild = new Button();
            gbRare.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nupbThresholdRare).BeginInit();
            gbCommon.SuspendLayout();
            gbNonsense.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nupdNonsense).BeginInit();
            SuspendLayout();
            // 
            // wlRare
            // 
            wlRare.AllowDrop = true;
            wlRare.BackColor = SystemColors.ControlLightLight;
            wlRare.IsSorted = true;
            wlRare.ItemHeight = 14F;
            wlRare.ItemPadding = 3F;
            wlRare.LeftNeighbor = null;
            wlRare.Location = new Point(6, 26);
            wlRare.Name = "wlRare";
            wlRare.RightNeighbor = null;
            wlRare.ScrollPosition = -1F;
            wlRare.SelectedIndex = -1;
            wlRare.Size = new Size(226, 405);
            wlRare.TabIndex = 0;
            wlRare.Words = (List<string>)resources.GetObject("wlRare.Words");
            // 
            // gbRare
            // 
            gbRare.Controls.Add(label1);
            gbRare.Controls.Add(nupbThresholdRare);
            gbRare.Controls.Add(wlRare);
            gbRare.Location = new Point(268, 12);
            gbRare.Name = "gbRare";
            gbRare.Size = new Size(250, 480);
            gbRare.TabIndex = 1;
            gbRare.TabStop = false;
            gbRare.Text = "Rare Words";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(58, 436);
            label1.Name = "label1";
            label1.Size = new Size(77, 20);
            label1.TabIndex = 4;
            label1.Text = "Threshold:";
            // 
            // nupbThresholdRare
            // 
            nupbThresholdRare.Location = new Point(141, 434);
            nupbThresholdRare.Name = "nupbThresholdRare";
            nupbThresholdRare.Size = new Size(91, 27);
            nupbThresholdRare.TabIndex = 3;
            nupbThresholdRare.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // gbCommon
            // 
            gbCommon.Controls.Add(wlSensible);
            gbCommon.Location = new Point(524, 12);
            gbCommon.Name = "gbCommon";
            gbCommon.Size = new Size(250, 480);
            gbCommon.TabIndex = 1;
            gbCommon.TabStop = false;
            gbCommon.Text = "Common Words";
            // 
            // wlSensible
            // 
            wlSensible.AllowDrop = true;
            wlSensible.BackColor = SystemColors.ControlLightLight;
            wlSensible.IsSorted = true;
            wlSensible.ItemHeight = 14F;
            wlSensible.ItemPadding = 3F;
            wlSensible.LeftNeighbor = null;
            wlSensible.Location = new Point(6, 28);
            wlSensible.Name = "wlSensible";
            wlSensible.RightNeighbor = null;
            wlSensible.ScrollPosition = -1F;
            wlSensible.SelectedIndex = -1;
            wlSensible.Size = new Size(226, 405);
            wlSensible.TabIndex = 0;
            wlSensible.Words = (List<string>)resources.GetObject("wlSensible.Words");
            // 
            // gbNonsense
            // 
            gbNonsense.Controls.Add(label2);
            gbNonsense.Controls.Add(wlNonsense);
            gbNonsense.Controls.Add(nupdNonsense);
            gbNonsense.Location = new Point(12, 12);
            gbNonsense.Name = "gbNonsense";
            gbNonsense.Size = new Size(250, 480);
            gbNonsense.TabIndex = 1;
            gbNonsense.TabStop = false;
            gbNonsense.Text = "Nonsense Words";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(58, 441);
            label2.Name = "label2";
            label2.Size = new Size(77, 20);
            label2.TabIndex = 4;
            label2.Text = "Threshold:";
            // 
            // wlNonsense
            // 
            wlNonsense.AllowDrop = true;
            wlNonsense.BackColor = SystemColors.ControlLightLight;
            wlNonsense.IsSorted = true;
            wlNonsense.ItemHeight = 14F;
            wlNonsense.ItemPadding = 3F;
            wlNonsense.LeftNeighbor = null;
            wlNonsense.Location = new Point(6, 28);
            wlNonsense.Name = "wlNonsense";
            wlNonsense.RightNeighbor = null;
            wlNonsense.ScrollPosition = -1F;
            wlNonsense.SelectedIndex = -1;
            wlNonsense.Size = new Size(226, 405);
            wlNonsense.TabIndex = 0;
            wlNonsense.Words = (List<string>)resources.GetObject("wlNonsense.Words");
            // 
            // nupdNonsense
            // 
            nupdNonsense.Location = new Point(141, 439);
            nupdNonsense.Name = "nupdNonsense";
            nupdNonsense.Size = new Size(91, 27);
            nupdNonsense.TabIndex = 3;
            nupdNonsense.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // button1
            // 
            bBuild.Location = new Point(680, 498);
            bBuild.Name = "button1";
            bBuild.Size = new Size(94, 29);
            bBuild.TabIndex = 1;
            bBuild.Text = "Build";
            bBuild.UseVisualStyleBackColor = true;
            // 
            // ListEditor
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(799, 533);
            Controls.Add(bBuild);
            Controls.Add(gbCommon);
            Controls.Add(gbNonsense);
            Controls.Add(gbRare);
            Name = "ListEditor";
            Text = "ListEditor";
            gbRare.ResumeLayout(false);
            gbRare.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nupbThresholdRare).EndInit();
            gbCommon.ResumeLayout(false);
            gbNonsense.ResumeLayout(false);
            gbNonsense.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nupdNonsense).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private WordListControl.WordList wlRare;
        private GroupBox gbRare;
        private NumericUpDown nupbThresholdRare;
        private Label label1;
        private GroupBox gbCommon;
        private WordListControl.WordList wlSensible;
        private GroupBox gbNonsense;
        private WordListControl.WordList wlNonsense;
        private Label label2;
        private NumericUpDown nupdNonsense;
        private Button bBuild;
    }
}
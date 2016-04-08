namespace TileDetection
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.numericUpDown1_upper_blue = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown2_epsilonApproxPolyDP = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown1_lower_blue = new System.Windows.Forms.NumericUpDown();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.loadImageButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.originalImageBox = new Emgu.CV.UI.ImageBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.out_image1ImageBox = new Emgu.CV.UI.ImageBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.out_image0ImageBox = new Emgu.CV.UI.ImageBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.out_image2ImageBox = new Emgu.CV.UI.ImageBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_upper_blue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2_epsilonApproxPolyDP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_lower_blue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.originalImageBox)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.out_image1ImageBox)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.out_image0ImageBox)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.out_image2ImageBox)).BeginInit();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.numericUpDown1_upper_blue);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.trackBar1);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.numericUpDown2_epsilonApproxPolyDP);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.numericUpDown1_lower_blue);
            this.panel1.Controls.Add(this.fileNameTextBox);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.loadImageButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1059, 124);
            this.panel1.TabIndex = 0;
            // 
            // numericUpDown1_upper_blue
            // 
            this.numericUpDown1_upper_blue.Location = new System.Drawing.Point(115, 83);
            this.numericUpDown1_upper_blue.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown1_upper_blue.Name = "numericUpDown1_upper_blue";
            this.numericUpDown1_upper_blue.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1_upper_blue.TabIndex = 9;
            this.numericUpDown1_upper_blue.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown1_upper_blue.ValueChanged += new System.EventHandler(this.numericUpDown1_canny_high_thresh_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(46, 85);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "upper_blue:";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(241, 49);
            this.trackBar1.Maximum = 500;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(104, 45);
            this.trackBar1.TabIndex = 7;
            this.trackBar1.Value = 30;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_canny_low_thresh_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(394, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(111, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "epsilonApproxPolyDP:";
            // 
            // numericUpDown2_epsilonApproxPolyDP
            // 
            this.numericUpDown2_epsilonApproxPolyDP.Location = new System.Drawing.Point(511, 50);
            this.numericUpDown2_epsilonApproxPolyDP.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown2_epsilonApproxPolyDP.Name = "numericUpDown2_epsilonApproxPolyDP";
            this.numericUpDown2_epsilonApproxPolyDP.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown2_epsilonApproxPolyDP.TabIndex = 5;
            this.numericUpDown2_epsilonApproxPolyDP.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2_epsilonApproxPolyDP.ValueChanged += new System.EventHandler(this.numericUpDown2_epsilonApproxPolyDP_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(49, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "lower_blue:";
            // 
            // numericUpDown1_lower_blue
            // 
            this.numericUpDown1_lower_blue.Location = new System.Drawing.Point(115, 52);
            this.numericUpDown1_lower_blue.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown1_lower_blue.Name = "numericUpDown1_lower_blue";
            this.numericUpDown1_lower_blue.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1_lower_blue.TabIndex = 3;
            this.numericUpDown1_lower_blue.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDown1_lower_blue.ValueChanged += new System.EventHandler(this.numericUpDown1_canny_low_thresh_ValueChanged);
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Location = new System.Drawing.Point(49, 14);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.ReadOnly = true;
            this.fileNameTextBox.Size = new System.Drawing.Size(546, 20);
            this.fileNameTextBox.TabIndex = 2;
            this.fileNameTextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "File:";
            // 
            // loadImageButton
            // 
            this.loadImageButton.Location = new System.Drawing.Point(601, 12);
            this.loadImageButton.Name = "loadImageButton";
            this.loadImageButton.Size = new System.Drawing.Size(30, 23);
            this.loadImageButton.TabIndex = 0;
            this.loadImageButton.Text = "...";
            this.loadImageButton.UseVisualStyleBackColor = true;
            this.loadImageButton.Click += new System.EventHandler(this.loadImageButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 124);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(1059, 614);
            this.splitContainer1.SplitterDistance = 520;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.originalImageBox);
            this.splitContainer2.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.out_image1ImageBox);
            this.splitContainer2.Panel2.Controls.Add(this.panel4);
            this.splitContainer2.Size = new System.Drawing.Size(520, 614);
            this.splitContainer2.SplitterDistance = 311;
            this.splitContainer2.TabIndex = 0;
            // 
            // originalImageBox
            // 
            this.originalImageBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.originalImageBox.Cursor = System.Windows.Forms.Cursors.Cross;
            this.originalImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.originalImageBox.Location = new System.Drawing.Point(0, 25);
            this.originalImageBox.Name = "originalImageBox";
            this.originalImageBox.Size = new System.Drawing.Size(520, 286);
            this.originalImageBox.TabIndex = 3;
            this.originalImageBox.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(520, 25);
            this.panel2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Original Image";
            // 
            // out_image1ImageBox
            // 
            this.out_image1ImageBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.out_image1ImageBox.Cursor = System.Windows.Forms.Cursors.Cross;
            this.out_image1ImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.out_image1ImageBox.Location = new System.Drawing.Point(0, 25);
            this.out_image1ImageBox.Name = "out_image1ImageBox";
            this.out_image1ImageBox.Size = new System.Drawing.Size(520, 274);
            this.out_image1ImageBox.TabIndex = 4;
            this.out_image1ImageBox.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(520, 25);
            this.panel4.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "out_image1";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.out_image0ImageBox);
            this.splitContainer3.Panel1.Controls.Add(this.panel3);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.out_image2ImageBox);
            this.splitContainer3.Panel2.Controls.Add(this.panel5);
            this.splitContainer3.Size = new System.Drawing.Size(535, 614);
            this.splitContainer3.SplitterDistance = 311;
            this.splitContainer3.TabIndex = 0;
            // 
            // out_image0ImageBox
            // 
            this.out_image0ImageBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.out_image0ImageBox.Cursor = System.Windows.Forms.Cursors.Cross;
            this.out_image0ImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.out_image0ImageBox.Location = new System.Drawing.Point(0, 25);
            this.out_image0ImageBox.Name = "out_image0ImageBox";
            this.out_image0ImageBox.Size = new System.Drawing.Size(535, 286);
            this.out_image0ImageBox.TabIndex = 4;
            this.out_image0ImageBox.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(535, 25);
            this.panel3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "out_image0";
            // 
            // out_image2ImageBox
            // 
            this.out_image2ImageBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.out_image2ImageBox.Cursor = System.Windows.Forms.Cursors.Cross;
            this.out_image2ImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.out_image2ImageBox.Location = new System.Drawing.Point(0, 25);
            this.out_image2ImageBox.Name = "out_image2ImageBox";
            this.out_image2ImageBox.Size = new System.Drawing.Size(535, 274);
            this.out_image2ImageBox.TabIndex = 4;
            this.out_image2ImageBox.TabStop = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(535, 25);
            this.panel5.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "out_image2";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 738);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.Text = "Tile Detection";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_upper_blue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2_epsilonApproxPolyDP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1_lower_blue)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.originalImageBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.out_image1ImageBox)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.out_image0ImageBox)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.out_image2ImageBox)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label4;
        private Emgu.CV.UI.ImageBox originalImageBox;
        private Emgu.CV.UI.ImageBox out_image1ImageBox;
        private Emgu.CV.UI.ImageBox out_image0ImageBox;
        private Emgu.CV.UI.ImageBox out_image2ImageBox;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button loadImageButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.NumericUpDown numericUpDown1_lower_blue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown2_epsilonApproxPolyDP;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.NumericUpDown numericUpDown1_upper_blue;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
    }
}


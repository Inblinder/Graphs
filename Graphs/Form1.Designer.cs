
namespace Graphs
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.algLabel = new System.Windows.Forms.Label();
            this.rightBtn = new System.Windows.Forms.Button();
            this.leftBtn = new System.Windows.Forms.Button();
            this.panelMatrix = new System.Windows.Forms.Panel();
            this.copyBtn = new System.Windows.Forms.Button();
            this.pasteBox = new System.Windows.Forms.TextBox();
            this.insertBtn = new System.Windows.Forms.Button();
            this.matrix = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dragUpdate = new System.Windows.Forms.Timer(this.components);
            this.fixedUpdate = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panelMatrix.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(252)))));
            this.splitContainer.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            this.splitContainer.Panel1.Controls.Add(this.label2);
            this.splitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.splitContainer.Panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Clicked);
            this.splitContainer.Panel1.MouseEnter += new System.EventHandler(this.fixedUpdate_Start);
            this.splitContainer.Panel1.MouseLeave += new System.EventHandler(this.fixedUpdate_End);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(252)))));
            this.splitContainer.Panel2.Controls.Add(this.algLabel);
            this.splitContainer.Panel2.Controls.Add(this.rightBtn);
            this.splitContainer.Panel2.Controls.Add(this.leftBtn);
            this.splitContainer.Panel2.Controls.Add(this.panelMatrix);
            this.splitContainer.Panel2.Controls.Add(this.label1);
            this.splitContainer.Panel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.splitContainer.Size = new System.Drawing.Size(1332, 778);
            this.splitContainer.SplitterDistance = 830;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(252)))));
            this.label2.Location = new System.Drawing.Point(13, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "click to create vertex";
            // 
            // algLabel
            // 
            this.algLabel.AutoSize = true;
            this.algLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.algLabel.Location = new System.Drawing.Point(47, 70);
            this.algLabel.MinimumSize = new System.Drawing.Size(60, 25);
            this.algLabel.Name = "algLabel";
            this.algLabel.Size = new System.Drawing.Size(60, 25);
            this.algLabel.TabIndex = 8;
            this.algLabel.Text = "Choose";
            // 
            // rightBtn
            // 
            this.rightBtn.Location = new System.Drawing.Point(112, 70);
            this.rightBtn.Name = "rightBtn";
            this.rightBtn.Size = new System.Drawing.Size(25, 25);
            this.rightBtn.TabIndex = 7;
            this.rightBtn.Text = ">";
            this.rightBtn.UseVisualStyleBackColor = true;
            // 
            // leftBtn
            // 
            this.leftBtn.Location = new System.Drawing.Point(18, 70);
            this.leftBtn.Name = "leftBtn";
            this.leftBtn.Size = new System.Drawing.Size(25, 25);
            this.leftBtn.TabIndex = 6;
            this.leftBtn.Text = "<";
            this.leftBtn.UseVisualStyleBackColor = true;
            // 
            // panelMatrix
            // 
            this.panelMatrix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelMatrix.Controls.Add(this.copyBtn);
            this.panelMatrix.Controls.Add(this.pasteBox);
            this.panelMatrix.Controls.Add(this.insertBtn);
            this.panelMatrix.Controls.Add(this.matrix);
            this.panelMatrix.Location = new System.Drawing.Point(3, 453);
            this.panelMatrix.Name = "panelMatrix";
            this.panelMatrix.Size = new System.Drawing.Size(288, 322);
            this.panelMatrix.TabIndex = 5;
            // 
            // copyBtn
            // 
            this.copyBtn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.copyBtn.Location = new System.Drawing.Point(230, 3);
            this.copyBtn.Name = "copyBtn";
            this.copyBtn.Size = new System.Drawing.Size(55, 27);
            this.copyBtn.TabIndex = 5;
            this.copyBtn.Text = "Copy";
            this.copyBtn.UseVisualStyleBackColor = true;
            this.copyBtn.Click += new System.EventHandler(this.copyBtn_Click);
            // 
            // pasteBox
            // 
            this.pasteBox.Location = new System.Drawing.Point(3, 3);
            this.pasteBox.Name = "pasteBox";
            this.pasteBox.PlaceholderText = "Paste adjacency matrix";
            this.pasteBox.Size = new System.Drawing.Size(168, 27);
            this.pasteBox.TabIndex = 3;
            // 
            // insertBtn
            // 
            this.insertBtn.Location = new System.Drawing.Point(173, 3);
            this.insertBtn.Name = "insertBtn";
            this.insertBtn.Size = new System.Drawing.Size(55, 27);
            this.insertBtn.TabIndex = 4;
            this.insertBtn.Text = "Insert";
            this.insertBtn.UseVisualStyleBackColor = true;
            // 
            // matrix
            // 
            this.matrix.AutoSize = true;
            this.matrix.Location = new System.Drawing.Point(3, 33);
            this.matrix.MaximumSize = new System.Drawing.Size(280, 280);
            this.matrix.MinimumSize = new System.Drawing.Size(280, 280);
            this.matrix.Name = "matrix";
            this.matrix.Size = new System.Drawing.Size(280, 280);
            this.matrix.TabIndex = 1;
            this.matrix.Text = "[...]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(18, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "G = (V, E)";
            // 
            // dragUpdate
            // 
            this.dragUpdate.Interval = 20;
            this.dragUpdate.Tick += new System.EventHandler(this.dragUpdate_Tick);
            // 
            // fixedUpdate
            // 
            this.fixedUpdate.Interval = 20;
            this.fixedUpdate.Tick += new System.EventHandler(this.fixedUpdate_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1332, 778);
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(900, 550);
            this.Name = "Form1";
            this.Text = "Graphs";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panelMatrix.ResumeLayout(false);
            this.panelMatrix.PerformLayout();
            this.ResumeLayout(false);

        }

        private void Panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label matrix;
        private System.Windows.Forms.Timer dragUpdate;
        private System.Windows.Forms.TextBox pasteBox;
        private System.Windows.Forms.Button insertBtn;
        private System.Windows.Forms.Panel panelMatrix;
        private System.Windows.Forms.Button copyBtn;
        private System.Windows.Forms.Timer fixedUpdate;
        private System.Windows.Forms.Label algLabel;
        private System.Windows.Forms.Button rightBtn;
        private System.Windows.Forms.Button leftBtn;
    }
}



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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.vertexLabel = new System.Windows.Forms.Label();
            this.basicMsg = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboVertices = new System.Windows.Forms.ComboBox();
            this.labelSource = new System.Windows.Forms.Label();
            this.algDescription = new System.Windows.Forms.Label();
            this.resultMsg = new System.Windows.Forms.Label();
            this.resultLabel = new System.Windows.Forms.Label();
            this.startBtn = new System.Windows.Forms.Button();
            this.resultMatrix = new System.Windows.Forms.Label();
            this.algLabel = new System.Windows.Forms.Label();
            this.rightBtn = new System.Windows.Forms.Button();
            this.leftBtn = new System.Windows.Forms.Button();
            this.panelMatrix = new System.Windows.Forms.Panel();
            this.exampleMsg = new System.Windows.Forms.Label();
            this.errorMsg = new System.Windows.Forms.Label();
            this.exampleBtn = new System.Windows.Forms.Button();
            this.copyBtn = new System.Windows.Forms.Button();
            this.pasteBox = new System.Windows.Forms.TextBox();
            this.insertBtn = new System.Windows.Forms.Button();
            this.matrix = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dragUpdate = new System.Windows.Forms.Timer(this.components);
            this.fixedUpdate = new System.Windows.Forms.Timer(this.components);
            this.writeTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelMatrix.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(252)))));
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            this.splitContainer.Panel1.Controls.Add(this.vertexLabel);
            this.splitContainer.Panel1.Controls.Add(this.basicMsg);
            this.splitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.splitContainer.Panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Clicked);
            this.splitContainer.Panel1.MouseEnter += new System.EventHandler(this.fixedUpdate_Start);
            this.splitContainer.Panel1.MouseLeave += new System.EventHandler(this.fixedUpdate_End);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(252)))));
            this.splitContainer.Panel2.Controls.Add(this.panel1);
            this.splitContainer.Panel2.Controls.Add(this.panelMatrix);
            this.splitContainer.Panel2.Controls.Add(this.label1);
            this.splitContainer.Panel2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.splitContainer.Size = new System.Drawing.Size(1402, 823);
            this.splitContainer.SplitterDistance = 947;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.TabStop = false;
            // 
            // vertexLabel
            // 
            this.vertexLabel.AutoSize = true;
            this.vertexLabel.BackColor = System.Drawing.Color.White;
            this.vertexLabel.Enabled = false;
            this.vertexLabel.Location = new System.Drawing.Point(210, 13);
            this.vertexLabel.Name = "vertexLabel";
            this.vertexLabel.Size = new System.Drawing.Size(0, 20);
            this.vertexLabel.TabIndex = 10;
            // 
            // basicMsg
            // 
            this.basicMsg.AutoSize = true;
            this.basicMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(252)))));
            this.basicMsg.Location = new System.Drawing.Point(13, 13);
            this.basicMsg.Name = "basicMsg";
            this.basicMsg.Size = new System.Drawing.Size(145, 20);
            this.basicMsg.TabIndex = 0;
            this.basicMsg.Text = "click to create vertex";
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel1.Controls.Add(this.comboVertices);
            this.panel1.Controls.Add(this.labelSource);
            this.panel1.Controls.Add(this.algDescription);
            this.panel1.Controls.Add(this.resultMsg);
            this.panel1.Controls.Add(this.resultLabel);
            this.panel1.Controls.Add(this.startBtn);
            this.panel1.Controls.Add(this.resultMatrix);
            this.panel1.Controls.Add(this.algLabel);
            this.panel1.Controls.Add(this.rightBtn);
            this.panel1.Controls.Add(this.leftBtn);
            this.panel1.Location = new System.Drawing.Point(14, 70);
            this.panel1.MinimumSize = new System.Drawing.Size(320, 350);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(422, 350);
            this.panel1.TabIndex = 9;
            // 
            // comboVertices
            // 
            this.comboVertices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboVertices.FormattingEnabled = true;
            this.comboVertices.Location = new System.Drawing.Point(116, 131);
            this.comboVertices.Name = "comboVertices";
            this.comboVertices.Size = new System.Drawing.Size(60, 28);
            this.comboVertices.TabIndex = 14;
            // 
            // labelSource
            // 
            this.labelSource.AutoSize = true;
            this.labelSource.Location = new System.Drawing.Point(13, 134);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(97, 20);
            this.labelSource.TabIndex = 13;
            this.labelSource.Text = "Pick a source:";
            // 
            // algDescription
            // 
            this.algDescription.AutoSize = true;
            this.algDescription.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.algDescription.Location = new System.Drawing.Point(13, 38);
            this.algDescription.MaximumSize = new System.Drawing.Size(400, 80);
            this.algDescription.MinimumSize = new System.Drawing.Size(400, 80);
            this.algDescription.Name = "algDescription";
            this.algDescription.Size = new System.Drawing.Size(400, 80);
            this.algDescription.TabIndex = 12;
            this.algDescription.Text = resources.GetString("algDescription.Text");
            // 
            // resultMsg
            // 
            this.resultMsg.AutoSize = true;
            this.resultMsg.Location = new System.Drawing.Point(13, 163);
            this.resultMsg.Name = "resultMsg";
            this.resultMsg.Size = new System.Drawing.Size(58, 20);
            this.resultMsg.TabIndex = 11;
            this.resultMsg.Text = "Results:";
            // 
            // resultLabel
            // 
            this.resultLabel.AutoSize = true;
            this.resultLabel.Location = new System.Drawing.Point(80, 188);
            this.resultLabel.MaximumSize = new System.Drawing.Size(100, 100);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(0, 20);
            this.resultLabel.TabIndex = 10;
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(354, 130);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(59, 29);
            this.startBtn.TabIndex = 9;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // resultMatrix
            // 
            this.resultMatrix.AutoSize = true;
            this.resultMatrix.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.resultMatrix.Location = new System.Drawing.Point(243, 167);
            this.resultMatrix.MaximumSize = new System.Drawing.Size(170, 170);
            this.resultMatrix.MinimumSize = new System.Drawing.Size(170, 170);
            this.resultMatrix.Name = "resultMatrix";
            this.resultMatrix.Size = new System.Drawing.Size(170, 170);
            this.resultMatrix.TabIndex = 8;
            this.resultMatrix.Text = "[...]";
            // 
            // algLabel
            // 
            this.algLabel.AutoSize = true;
            this.algLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.algLabel.Location = new System.Drawing.Point(80, 3);
            this.algLabel.MinimumSize = new System.Drawing.Size(250, 30);
            this.algLabel.Name = "algLabel";
            this.algLabel.Size = new System.Drawing.Size(250, 30);
            this.algLabel.TabIndex = 8;
            this.algLabel.Text = "DFS";
            this.algLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rightBtn
            // 
            this.rightBtn.Location = new System.Drawing.Point(336, 5);
            this.rightBtn.Name = "rightBtn";
            this.rightBtn.Size = new System.Drawing.Size(25, 25);
            this.rightBtn.TabIndex = 1;
            this.rightBtn.Text = "▶";
            this.rightBtn.UseVisualStyleBackColor = false;
            this.rightBtn.Click += new System.EventHandler(this.rightBtn_Click);
            // 
            // leftBtn
            // 
            this.leftBtn.Location = new System.Drawing.Point(49, 5);
            this.leftBtn.Name = "leftBtn";
            this.leftBtn.Size = new System.Drawing.Size(25, 25);
            this.leftBtn.TabIndex = 2;
            this.leftBtn.Text = "◀";
            this.leftBtn.UseVisualStyleBackColor = false;
            this.leftBtn.Click += new System.EventHandler(this.leftBtn_Click);
            // 
            // panelMatrix
            // 
            this.panelMatrix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panelMatrix.Controls.Add(this.exampleMsg);
            this.panelMatrix.Controls.Add(this.errorMsg);
            this.panelMatrix.Controls.Add(this.exampleBtn);
            this.panelMatrix.Controls.Add(this.copyBtn);
            this.panelMatrix.Controls.Add(this.pasteBox);
            this.panelMatrix.Controls.Add(this.insertBtn);
            this.panelMatrix.Controls.Add(this.matrix);
            this.panelMatrix.Location = new System.Drawing.Point(3, 470);
            this.panelMatrix.MinimumSize = new System.Drawing.Size(422, 350);
            this.panelMatrix.Name = "panelMatrix";
            this.panelMatrix.Size = new System.Drawing.Size(422, 350);
            this.panelMatrix.TabIndex = 5;
            // 
            // exampleMsg
            // 
            this.exampleMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exampleMsg.AutoSize = true;
            this.exampleMsg.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.exampleMsg.Location = new System.Drawing.Point(364, 281);
            this.exampleMsg.Name = "exampleMsg";
            this.exampleMsg.Size = new System.Drawing.Size(52, 30);
            this.exampleMsg.TabIndex = 15;
            this.exampleMsg.Text = " Import\r\nexample";
            // 
            // errorMsg
            // 
            this.errorMsg.AutoSize = true;
            this.errorMsg.Location = new System.Drawing.Point(347, 6);
            this.errorMsg.MaximumSize = new System.Drawing.Size(80, 60);
            this.errorMsg.Name = "errorMsg";
            this.errorMsg.Size = new System.Drawing.Size(0, 20);
            this.errorMsg.TabIndex = 8;
            // 
            // exampleBtn
            // 
            this.exampleBtn.Location = new System.Drawing.Point(375, 314);
            this.exampleBtn.Name = "exampleBtn";
            this.exampleBtn.Size = new System.Drawing.Size(30, 27);
            this.exampleBtn.TabIndex = 9;
            this.exampleBtn.Text = "🡳";
            this.exampleBtn.UseVisualStyleBackColor = true;
            this.exampleBtn.Click += new System.EventHandler(this.exampleBtn_Click);
            // 
            // copyBtn
            // 
            this.copyBtn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.copyBtn.Location = new System.Drawing.Point(272, 3);
            this.copyBtn.Name = "copyBtn";
            this.copyBtn.Size = new System.Drawing.Size(55, 27);
            this.copyBtn.TabIndex = 5;
            this.copyBtn.Text = "Copy";
            this.copyBtn.UseVisualStyleBackColor = true;
            this.copyBtn.Click += new System.EventHandler(this.copyBtn_Click);
            // 
            // pasteBox
            // 
            this.pasteBox.Location = new System.Drawing.Point(13, 3);
            this.pasteBox.Name = "pasteBox";
            this.pasteBox.PlaceholderText = "Paste adjacency matrix";
            this.pasteBox.Size = new System.Drawing.Size(200, 27);
            this.pasteBox.TabIndex = 7;
            // 
            // insertBtn
            // 
            this.insertBtn.Location = new System.Drawing.Point(215, 3);
            this.insertBtn.Name = "insertBtn";
            this.insertBtn.Size = new System.Drawing.Size(55, 27);
            this.insertBtn.TabIndex = 4;
            this.insertBtn.Text = "Insert";
            this.insertBtn.UseVisualStyleBackColor = true;
            this.insertBtn.Click += new System.EventHandler(this.insertBtn_Click);
            // 
            // matrix
            // 
            this.matrix.AutoSize = true;
            this.matrix.Location = new System.Drawing.Point(13, 44);
            this.matrix.MaximumSize = new System.Drawing.Size(280, 280);
            this.matrix.MinimumSize = new System.Drawing.Size(300, 300);
            this.matrix.Name = "matrix";
            this.matrix.Size = new System.Drawing.Size(300, 300);
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
            // writeTimer
            // 
            this.writeTimer.Interval = 500;
            this.writeTimer.Tick += new System.EventHandler(this.writeTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1402, 823);
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(1350, 825);
            this.Name = "Form1";
            this.Text = "Graphs";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.Label basicMsg;
        private System.Windows.Forms.Label matrix;
        private System.Windows.Forms.Timer dragUpdate;
        private System.Windows.Forms.TextBox pasteBox;
        private System.Windows.Forms.Button insertBtn;
        private System.Windows.Forms.Panel panelMatrix;
        private System.Windows.Forms.Button copyBtn;
        private System.Windows.Forms.Timer fixedUpdate;
        private System.Windows.Forms.Timer writeTimer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label algLabel;
        private System.Windows.Forms.Button rightBtn;
        private System.Windows.Forms.Button leftBtn;
        private System.Windows.Forms.Label resultMatrix;
        private System.Windows.Forms.Label resultMsg;
        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Label algDescription;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.Label vertexLabel;
        private System.Windows.Forms.ComboBox comboVertices;
        private System.Windows.Forms.Label errorMsg;
        private System.Windows.Forms.Label exampleMsg;
        private System.Windows.Forms.Button exampleBtn;
    }
}


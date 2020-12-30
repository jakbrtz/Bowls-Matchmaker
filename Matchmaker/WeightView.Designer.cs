namespace Matchmaker
{
    partial class WeightView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LBLname = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TBRmultiplier = new CustomControls.ClearTrackBar();
            this.TBRvalue = new CustomControls.ClearTrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.TBRmultiplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TBRvalue)).BeginInit();
            this.SuspendLayout();
            // 
            // LBLname
            // 
            this.LBLname.AutoSize = true;
            this.LBLname.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBLname.Location = new System.Drawing.Point(4, 4);
            this.LBLname.Name = "LBLname";
            this.LBLname.Size = new System.Drawing.Size(316, 25);
            this.LBLname.TabIndex = 0;
            this.LBLname.Text = "Description of what weight does";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Not as important";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Avoid consecutive games";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(559, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Spread games evenly";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(559, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Very important";
            // 
            // TBRmultiplier
            // 
            this.TBRmultiplier.AutoSize = false;
            this.TBRmultiplier.BackColor = System.Drawing.SystemColors.Control;
            this.TBRmultiplier.Location = new System.Drawing.Point(142, 71);
            this.TBRmultiplier.Maximum = 99;
            this.TBRmultiplier.Name = "TBRmultiplier";
            this.TBRmultiplier.Size = new System.Drawing.Size(411, 24);
            this.TBRmultiplier.TabIndex = 3;
            this.TBRmultiplier.TickFrequency = 25;
            this.TBRmultiplier.Scroll += new System.EventHandler(this.TBRmultiplier_Scroll);
            this.TBRmultiplier.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TBR_MouseWheel);
            // 
            // TBRvalue
            // 
            this.TBRvalue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TBRvalue.AutoSize = false;
            this.TBRvalue.BackColor = System.Drawing.SystemColors.Control;
            this.TBRvalue.Location = new System.Drawing.Point(142, 41);
            this.TBRvalue.Maximum = 100;
            this.TBRvalue.Name = "TBRvalue";
            this.TBRvalue.Size = new System.Drawing.Size(411, 24);
            this.TBRvalue.TabIndex = 1;
            this.TBRvalue.TickFrequency = 25;
            this.TBRvalue.Scroll += new System.EventHandler(this.TBRvalue_Scroll);
            this.TBRvalue.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TBR_MouseWheel);
            // 
            // WeightView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TBRmultiplier);
            this.Controls.Add(this.TBRvalue);
            this.Controls.Add(this.LBLname);
            this.Name = "WeightView";
            this.Size = new System.Drawing.Size(689, 70);
            ((System.ComponentModel.ISupportInitialize)(this.TBRmultiplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TBRvalue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBLname;
        private CustomControls.ClearTrackBar TBRvalue;
        private CustomControls.ClearTrackBar TBRmultiplier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

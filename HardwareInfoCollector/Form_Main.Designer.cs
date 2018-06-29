namespace HardwareInfoCollector
{
    partial class Form_Main
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
            this.button_CollectAndSave = new System.Windows.Forms.Button();
            this.textBox_Name = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_CollectAndSave
            // 
            this.button_CollectAndSave.Location = new System.Drawing.Point(12, 39);
            this.button_CollectAndSave.Name = "button_CollectAndSave";
            this.button_CollectAndSave.Size = new System.Drawing.Size(210, 61);
            this.button_CollectAndSave.TabIndex = 1;
            this.button_CollectAndSave.Text = "Collect And Save";
            this.button_CollectAndSave.UseVisualStyleBackColor = true;
            this.button_CollectAndSave.Click += new System.EventHandler(this.button_CollectAndSave_Click);
            // 
            // textBox_Name
            // 
            this.textBox_Name.Location = new System.Drawing.Point(13, 13);
            this.textBox_Name.Name = "textBox_Name";
            this.textBox_Name.Size = new System.Drawing.Size(209, 20);
            this.textBox_Name.TabIndex = 0;
            // 
            // Form_Main
            // 
            this.AcceptButton = this.button_CollectAndSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 112);
            this.Controls.Add(this.textBox_Name);
            this.Controls.Add(this.button_CollectAndSave);
            this.Name = "Form_Main";
            this.Text = "H.I.C.";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_CollectAndSave;
        private System.Windows.Forms.TextBox textBox_Name;
    }
}


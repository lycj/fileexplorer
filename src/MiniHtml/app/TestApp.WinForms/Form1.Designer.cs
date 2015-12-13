namespace TestApp.WinForms
{
    partial class Form1
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
            this.mhLabel1 = new QuickZip.MiniHtml.Controls.mhLabel();
            this.SuspendLayout();
            // 
            // mhLabel1
            // 
            this.mhLabel1.BackColor = System.Drawing.Color.White;
            this.mhLabel1.Css = "";
            this.mhLabel1.Html = "<b>This is a test</b>";
            this.mhLabel1.Location = new System.Drawing.Point(252, 242);
            this.mhLabel1.Name = "mhLabel1";
            this.mhLabel1.ParseMode = QuickZip.MiniHtml.parseMode.Html;
            this.mhLabel1.Size = new System.Drawing.Size(439, 232);
            this.mhLabel1.TabIndex = 0;
            this.mhLabel1.Load += new System.EventHandler(this.mhLabel1_Load);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1046, 724);
            this.Controls.Add(this.mhLabel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private QuickZip.MiniHtml.Controls.mhLabel mhLabel1;
    }
}


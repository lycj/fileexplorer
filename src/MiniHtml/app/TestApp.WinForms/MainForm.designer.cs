/*
 * Created by SharpDevelop.
 * User: Joseph Leung
 * Date: 8/26/2006
 * Time: 4:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace TestApp.WinForms
{
	partial class MainForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.mhLabel1 = new QuickZip.MiniHtml.Controls.mhLabel();
			this.mhEditor1 = new QuickZip.MiniHtml.Controls.mhEditor();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// mhLabel1
			// 
			this.mhLabel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.mhLabel1.Css = "b {color:silver;}\r\np {background-color:Cornsilk;}\r\np.border {border: 1px double r" +
			"ed;}\r\nli {list-style-type:upper-roman}";
			this.mhLabel1.Html = resources.GetString("mhLabel1.Html");
			this.mhLabel1.Location = new System.Drawing.Point(34, 21);
			this.mhLabel1.Name = "mhLabel1";
			this.mhLabel1.ParseMode = QuickZip.MiniHtml.parseMode.Html;
			this.mhLabel1.Size = new System.Drawing.Size(298, 99);
			this.mhLabel1.TabIndex = 0;
			// 
			// mhEditor1
			// 
			this.mhEditor1.Css = "b.day {color:red;}";
			this.mhEditor1.Html = resources.GetString("mhEditor1.Html");
			this.mhEditor1.Location = new System.Drawing.Point(32, 137);
			this.mhEditor1.Name = "mhEditor1";
			this.mhEditor1.ParseMode = QuickZip.MiniHtml.parseMode.Html;
			this.mhEditor1.Size = new System.Drawing.Size(300, 109);
			this.mhEditor1.TabIndex = 1;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(32, 282);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "About";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(145, 282);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 3;
			this.button2.Text = "Message Box";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.Button2Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(257, 282);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 4;
			this.button3.Text = "Editor";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.Button3Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(363, 329);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.mhEditor1);
			this.Controls.Add(this.mhLabel1);
			this.Name = "MainForm";
			this.Text = "CompiledDemo - qzMiniHtml.Net";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private QuickZip.MiniHtml.Controls.mhEditor mhEditor1;
		private QuickZip.MiniHtml.Controls.mhLabel mhLabel1;
	}
}

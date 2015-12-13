/*
 * Created by SharpDevelop.
 * User: Joseph Leung
 * Date: 8/17/2006
 * Time: 1:28 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using QuickZip.MiniHtml;

namespace QuickZip.MiniHtml.Controls
{
	/// <summary>
	/// Description of Dialog.
	/// </summary>
	public class mhEditorDialog : System.Windows.Forms.Form
	{
		private string pHtml;
		private string pCss;
		private Int32 lastIndex = 0;
		public mhEditorDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			mhView.Width = Width;	
			mhView.ParseMode = parseMode.Html;
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mhEditorDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this.bCancel = new System.Windows.Forms.Button();
			this.bOK = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.mhTabControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.mhView = new QuickZip.MiniHtml.Controls.mhEditor();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.mhSource = new System.Windows.Forms.TextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.mhCss = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.mhTabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.bCancel);
			this.panel1.Controls.Add(this.bOK);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 201);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(398, 24);
			this.panel1.TabIndex = 1;
			// 
			// bCancel
			// 
			this.bCancel.Location = new System.Drawing.Point(235, 2);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75, 15);
			this.bCancel.TabIndex = 1;
			this.bCancel.Text = "Cancel";
			this.bCancel.Click += new System.EventHandler(this.BCancelClick);
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(316, 2);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(75, 14);
			this.bOK.TabIndex = 0;
			this.bOK.Text = "OK";
			this.bOK.Click += new System.EventHandler(this.BOKClick);
			// 
			// panel2
			// 
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(398, 20);
			this.panel2.TabIndex = 2;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.mhTabControl);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 20);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(398, 181);
			this.panel3.TabIndex = 3;
			// 
			// mhTabControl
			// 
			this.mhTabControl.Controls.Add(this.tabPage1);
			this.mhTabControl.Controls.Add(this.tabPage2);
			this.mhTabControl.Controls.Add(this.tabPage3);
			this.mhTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mhTabControl.Location = new System.Drawing.Point(0, 0);
			this.mhTabControl.Name = "mhTabControl";
			this.mhTabControl.SelectedIndex = 0;
			this.mhTabControl.Size = new System.Drawing.Size(398, 181);
			this.mhTabControl.TabIndex = 1;
			this.mhTabControl.TabIndexChanged += new System.EventHandler(this.MhTabControlTabIndexChanged);
			this.mhTabControl.SelectedIndexChanged += new System.EventHandler(this.MhTabControlSelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.mhView);
			this.tabPage1.Location = new System.Drawing.Point(4, 21);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(390, 156);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Preview";
			// 
			// mhView
			// 
			this.mhView.Css = "b.day {color:red;}";
			this.mhView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mhView.Html = resources.GetString("mhView.Html");
			this.mhView.Location = new System.Drawing.Point(0, 0);
			this.mhView.Name = "mhView";
			this.mhView.Size = new System.Drawing.Size(390, 156);
			this.mhView.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.mhSource);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(390, 125);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Html";
			// 
			// mhSource
			// 
			this.mhSource.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mhSource.Location = new System.Drawing.Point(0, 0);
			this.mhSource.Multiline = true;
			this.mhSource.Name = "mhSource";
			this.mhSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.mhSource.Size = new System.Drawing.Size(390, 125);
			this.mhSource.TabIndex = 0;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.mhCss);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(390, 125);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Css";
			// 
			// mhCss
			// 
			this.mhCss.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mhCss.Location = new System.Drawing.Point(0, 0);
			this.mhCss.Multiline = true;
			this.mhCss.Name = "mhCss";
			this.mhCss.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.mhCss.Size = new System.Drawing.Size(390, 125);
			this.mhCss.TabIndex = 2;
			// 
			// mhEditorDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.ClientSize = new System.Drawing.Size(398, 225);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "mhEditorDialog";
			this.Text = "Html Editor";
			this.SizeChanged += new System.EventHandler(this.MhDialogSizeChanged);
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.mhTabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TextBox mhCss;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.TextBox mhSource;
		private QuickZip.MiniHtml.Controls.mhEditor mhView;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabControl mhTabControl;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel1;
		#endregion
		
		public string Html
		{
			get
			{
				return pHtml;
			}
			set
			{
				pHtml = value;
				mhView.Html = value;
				mhSource.Text = value;
			}
		}
		
		public string Css
		{
			get
			{
				return pCss;
			}
			set
			{
				if (pCss != value)
				{					
					pCss = value;
					mhCss.Text = value;
					mhView.Css = value;
					mhView.Html = "";
					mhView.Html = pHtml;					
				}	
			}
		}
		
		void MhTabControlTabIndexChanged(object sender, System.EventArgs e)
		{

		}
		
		void MhTabControlSelectedIndexChanged(object sender, System.EventArgs e)
		{			
			UpdateVar();
			lastIndex = mhTabControl.SelectedIndex;
			
			switch (lastIndex)
			{
				case (0):
					mhView.Css = pCss;
					mhView.Html = "";
					mhView.Html = pHtml;					
					break;
				case (1):					
					mhSource.Text = pHtml;
					break;		
				case (2):
					mhCss.Text = pCss;
					break;
			}
		}
		
		void UpdateVar()
		{
			switch (lastIndex)
			{
				case (0):
					pHtml = mhView.Html;						
					break;
				case (1):
					pHtml = mhSource.Text;
					break;		
				case (2):
					Css = mhCss.Text;
					pCss = mhCss.Text;
					break;
			}						 
			
		}
		
		void BOKClick(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK; 
			UpdateVar();
			Close();
		}
		
		void BCancelClick(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel; 
			Close();
		}		
						
		
		void MhDialogSizeChanged(object sender, System.EventArgs e)
		{
			panel1.Height = 30;
			panel2.Height = 30;
			bCancel.Height = 29;
			bOK.Height = 29;
			bOK.Left = Width - bOK.Width - 15;
			bCancel.Left = bOK.Left - bCancel.Width - 10;
			
			mhView.Width = Width;
			
		}
	}
}

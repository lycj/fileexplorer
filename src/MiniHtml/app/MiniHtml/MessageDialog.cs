/*
 * Created by SharpDevelop.
 * User: Joseph Leung
 * Date: 8/18/2006
 * Time: 1:11 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuickZip.MiniHtml.Controls
{
	/// <summary>
	/// Description of mhMessageDialog.
	/// </summary>
	public class mhMessageDialog : System.Windows.Forms.Form
	{	
		public string Html
		{
			get
			{
				return mhView.Html;
			}
			set
			{				
				mhView.Html = value;			
				Height = panel1.Height + mhView.requiredHeight + 70;
			}

		}
		
		public string Css
		{
			get
			{
				return mhView.Css;
			}
			set
			{				
				mhView.Css = value;									
			}
		}
		
		private MessageBoxButtons pButtons;
		public MessageBoxButtons Buttons
		{
			get
			{
				return pButtons;
			}
			set
			{
				pButtons = value;
				switch (pButtons)
				{
					case (MessageBoxButtons.OK):						
						b01.Visible = false;
						b02.Visible = true;
						b03.Visible = false;
						b02.Text = "OK";	
						DialogResult = DialogResult.OK;
						this.AcceptButton = b02;
						this.CancelButton = null;
					break;
					case (MessageBoxButtons.OKCancel):
						b01.Visible = true;
						b02.Visible = false;
						b03.Visible = true;	
						b01.Text = "OK";
						b03.Text = "Cancel";
						DialogResult = DialogResult.Cancel;
						this.AcceptButton = b01;
						this.CancelButton = b03;
					break;
					case (MessageBoxButtons.RetryCancel):
						b01.Visible = true;
						b02.Visible = false;
						b03.Visible = true;										
						b01.Text = "Retry";
						b03.Text = "Cancel";
						DialogResult = DialogResult.Cancel;
						this.AcceptButton = b01;
						this.CancelButton = b03;
					break;
					case (MessageBoxButtons.YesNo):
						b01.Visible = true;
						b02.Visible = false;
						b03.Visible = true;										
						b01.Text = "Yes";
						b03.Text = "No";
						DialogResult = DialogResult.No;
						this.AcceptButton = b01;
						this.CancelButton = b03;
					break;
					case (MessageBoxButtons.YesNoCancel):
						b01.Visible = true;
						b02.Visible = true;
						b03.Visible = true;					
						b01.Text = "Yes";
						b02.Text = "No";
						b03.Text = "Cancel";
						DialogResult = DialogResult.Cancel;
						this.AcceptButton = b01;
						this.CancelButton = b03;
					break;
				}
			}
		}
		
		public override Color BackColor {
			get {
				return base.BackColor;
			}
			set {								
				base.BackColor = value;	
				mhView.BackColor = value;																		
			}
		}
				
		
		public mhMessageDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			#if mh_dotNet_20
			this.mhView.Padding = new System.Windows.Forms.Padding(10);
			#endif
			panel1.Height = 30;
			b01.Height = 24;
			b02.Height = 24;
			b03.Height = 24;
			Buttons = MessageBoxButtons.YesNoCancel;
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.mhView = new QuickZip.MiniHtml.Controls.mhLabel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.b03 = new System.Windows.Forms.Button();
			this.b01 = new System.Windows.Forms.Button();
			this.b02 = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// mhView
			// 
			this.mhView.Css = "";
			this.mhView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mhView.Html = "";
			this.mhView.Location = new System.Drawing.Point(0, 0);
			this.mhView.Name = "mhView";
			this.mhView.Size = new System.Drawing.Size(412, 208);
			this.mhView.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.b03);
			this.panel1.Controls.Add(this.b01);
			this.panel1.Controls.Add(this.b02);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 192);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(412, 16);
			this.panel1.TabIndex = 1;
			// 
			// b03
			// 
			this.b03.Location = new System.Drawing.Point(290, 3);
			this.b03.Name = "b03";
			this.b03.Size = new System.Drawing.Size(86, 12);
			this.b03.TabIndex = 2;
			this.b03.Text = "OK";
			this.b03.Visible = false;
			this.b03.Click += new System.EventHandler(this.B03Click);
			// 
			// b01
			// 
			this.b01.Location = new System.Drawing.Point(52, 3);
			this.b01.Name = "b01";
			this.b01.Size = new System.Drawing.Size(86, 12);
			this.b01.TabIndex = 1;
			this.b01.Text = "OK";
			this.b01.Visible = false;
			this.b01.Click += new System.EventHandler(this.B01Click);
			// 
			// b02
			// 
			this.b02.Location = new System.Drawing.Point(173, 3);
			this.b02.Name = "b02";
			this.b02.Size = new System.Drawing.Size(86, 12);
			this.b02.TabIndex = 0;
			this.b02.Text = "OK";
			this.b02.Click += new System.EventHandler(this.B02Click);
			// 
			// mhMessageDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.ClientSize = new System.Drawing.Size(412, 208);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.mhView);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "mhMessageDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.SizeChanged += new System.EventHandler(this.MhMessageDialogSizeChanged);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button b01;
		private System.Windows.Forms.Button b03;
		private System.Windows.Forms.Button b02;
		private System.Windows.Forms.Panel panel1;
		private QuickZip.MiniHtml.Controls.mhLabel mhView;
		#endregion
			
		void MhMessageDialogSizeChanged(object sender, System.EventArgs e)
		{
			b02.Left = (Width - b02.Width) / 2;
			b01.Left = (b02.Left - b01.Width) / 2;			
			b03.Left =  (b02.Left - b01.Left) + b02.Left;
		}
		
		void B01Click(object sender, System.EventArgs e)
		{
				switch (pButtons)
				{
					case (MessageBoxButtons.OK):						
					DialogResult = DialogResult.OK;
					break;
					case (MessageBoxButtons.OKCancel):
					DialogResult = DialogResult.OK;
					break;
					case (MessageBoxButtons.RetryCancel):
					DialogResult = DialogResult.Retry;
					break;
					case (MessageBoxButtons.YesNo):
					DialogResult = DialogResult.Yes;
					break;
					case (MessageBoxButtons.YesNoCancel):
					DialogResult = DialogResult.Yes;
					break;
				}
				Close();
		}
		
		void B03Click(object sender, System.EventArgs e)
		{
				switch (pButtons)
				{
					case (MessageBoxButtons.OK):						
					DialogResult = DialogResult.OK;
					break;
					case (MessageBoxButtons.OKCancel):
					DialogResult = DialogResult.Cancel;
					break;
					case (MessageBoxButtons.RetryCancel):
					DialogResult = DialogResult.Cancel;
					break;
					case (MessageBoxButtons.YesNo):
					DialogResult = DialogResult.No;
					break;
					case (MessageBoxButtons.YesNoCancel):
					DialogResult = DialogResult.Cancel;
					break;
				}		
				Close();
		}
		
		void B02Click(object sender, System.EventArgs e)
		{
				switch (pButtons)
				{
					case (MessageBoxButtons.OK):						
					DialogResult = DialogResult.OK;
					break;
					case (MessageBoxButtons.OKCancel):
					DialogResult = DialogResult.OK;
					break;
					case (MessageBoxButtons.RetryCancel):
					DialogResult = DialogResult.Retry;
					break;
					case (MessageBoxButtons.YesNo):
					DialogResult = DialogResult.Yes;
					break;
					case (MessageBoxButtons.YesNoCancel):
					DialogResult = DialogResult.No;
					break;
				}	
				Close();
		}
		
		public static DialogResult Show(string aTitle, string aHtml, string aCss, MessageBoxButtons aButtons, Color aBackColor)
		{
			mhMessageDialog msg = new mhMessageDialog();
			msg.Text = aTitle;
			msg.BackColor = aBackColor;
			msg.Css = aCss;
			msg.Html = aHtml;
			msg.Buttons = aButtons;			
			return msg.ShowDialog();
		}
	}
}

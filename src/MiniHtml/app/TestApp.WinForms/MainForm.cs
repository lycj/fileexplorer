/*
 * Created by SharpDevelop.
 * User: Joseph Leung
 * Date: 8/26/2006
 * Time: 4:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickZip.MiniHtml;
using QuickZip.MiniHtml.Controls;

namespace TestApp.WinForms
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

		}
		
		void Button1Click(object sender, System.EventArgs e)
		{
			Utils.AboutScreen();
		}
		
		void Button2Click(object sender, System.EventArgs e)
		{			
			if (mhMessageDialog.Show("Title", "<p align=\"centre\">This is a <b>YesNo Dialog</b><br>" +
			                         "Want to see more?</p>",
			                         "", MessageBoxButtons.YesNo, System.Drawing.SystemColors.Control) == DialogResult.Yes)
			{
				mhMessageDialog.Show("qzMiniHtml Demo by LYCJ", "<p align=\"centre\">This is a <b>Message Dialog</b></p>",
			                     "b {color:YellowGreen;}", MessageBoxButtons.OK, System.Drawing.SystemColors.Control);
							
				mhMessageDialog.Show("qzMiniHtml Demo by LYCJ", "<p align=\"centre\">This is a <b>Confirmation Dialog</b></p>",
			                     "b {color:Green;}", MessageBoxButtons.OKCancel, System.Drawing.SystemColors.Control);
				
				mhMessageDialog.Show("qzMiniHtml Demo by LYCJ", "<p align=\"centre\">This is a <b>Retry Dialog</b></p>",
			                     "b {color:Red;}", MessageBoxButtons.RetryCancel, System.Drawing.SystemColors.Control);
			}
		}
		
		void Button3Click(object sender, System.EventArgs e)
		{
			mhEditorDialog editor = new mhEditorDialog();
			editor.Html = mhLabel1.Html;
			editor.Css = mhLabel1.Css;
			if (editor.ShowDialog() == DialogResult.OK)
			{
				mhLabel1.Html = editor.Html;
				mhLabel1.Css = editor.Css;
			}
		}
	}
}

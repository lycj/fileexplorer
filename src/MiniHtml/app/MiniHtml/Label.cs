/*
 * Created by SharpDevelop.
 * User: Joseph Leung
 * Date: 8/9/2006
 * Time: 2:28 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using QuickZip.MiniHtml;

namespace QuickZip.MiniHtml.Controls
{
	/// <summary>
	/// Description of UserControl1.	
	/// </summary>		
	#if mh_dotNet_20
	[Designer(typeof(mhDesigner))]
	[Category("qzMiniHtml2")]
	#endif
	#if mh_dotNet_10
	[Category("qzMiniHtml1")]
	#endif
	public class mhLabel : System.Windows.Forms.UserControl
	{
		private MiniHtml mh;
		private string pHtml = "";
		private string pCss = "";
		private parseMode pParseMode = parseMode.Html;
		
		public parseMode ParseMode
		{
			get { return pParseMode; }
			set { pParseMode = value; 
					UpdateHtml(); }
		}
		
		[Category("Appearance")]
		public string Html
		{
			get				
			{
				return pHtml;
			}
			set
			{
				pHtml = value;
				UpdateHtml();
			}
		}
		
		[Category("Appearance")]
		public string Css
		{
			get				
			{
				return pCss;
			}
			set
			{
				pCss = value;
				UpdateHtml();
			}
		}	
		
		public override Color BackColor {
			get {
				return base.BackColor;
			}
			set {								
				base.BackColor = value;	
				if (mh.documentOutput.backColor == value) return;				
				mh.documentOutput.backColor = value;								
				UpdateHtml();						
			}
		}	
		
		
		
		public Int32 requiredHeight
		{
			get
			{
				return (Int32)mh.documentOutput.Height();
			}
		}
		
		public mhLabel()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			mh = new MiniHtml();
			BackColor = Color.White;
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.mhOutput = new System.Windows.Forms.PictureBox();
			//((System.ComponentModel.ISupportInitialize)(this.mhOutput)).BeginInit();
			this.SuspendLayout();
			// 
			// mhOutput
			// 
			this.mhOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mhOutput.Location = new System.Drawing.Point(0, 0);
			this.mhOutput.Name = "mhOutput";
			this.mhOutput.Size = new System.Drawing.Size(292, 266);
			this.mhOutput.TabIndex = 0;
			this.mhOutput.TabStop = false;
			this.mhOutput.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MhOutputMouseDown);
			this.mhOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MhOutputMouseMove);
			// 
			// mhLabel
			// 
			this.Controls.Add(this.mhOutput);
			this.Name = "mhLabel";
			this.Size = new System.Drawing.Size(292, 266);
			this.SizeChanged += new System.EventHandler(this.MhLabelSizeChanged);
			//((System.ComponentModel.ISupportInitialize)(this.mhOutput)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.PictureBox mhOutput;
		#endregion
		
		
		public void UpdateHtml()
		{
			mh = new MiniHtml();
			mh.widthLimit = Math.Max(10,mhOutput.Width);
			mh.ParseMode = ParseMode;			
			mh.parser.Parse(pHtml);
			mh.parser.ParseCss(pCss);
			
			mh.documentOutput.Update();
			mhOutput.Image = new Bitmap(mh.widthLimit, (Int32)(mh.documentOutput.Height()));
            
			GraphicsOutputType output = (GraphicsOutputType)(mh.documentOutput);
			
            output.drawCursor = false;
            output.backColor = this.BackColor;
			output.outputGraphics = Graphics.FromImage(mhOutput.Image);
            output.ClearBackground();            
            
            mh.documentOutput.Output();            
		}
		
		public void About()
		{
			Html = "<b>qzMiniHtml.Net <sup>ver 2.00</sup></b> <p>" +
				"Copyright &copy; (2005-2006) Leung Yat Chun Joseph (lycj) <p> <p>"+
				"email: <a href=\"mailto://\">author2004@quickzip.org</a><p>"+
				"www: <a href=\"http://www.quickzip.org\">http://www.quickzip.org</a>";
		}
				
	
		void MhLabelSizeChanged(object sender, System.EventArgs e)
		{
			if ((Visible) && (Width > 0))
				UpdateHtml();
		}
				
		
		void MhOutputMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			mh.documentOutput.selection.SetStartCursor(new PointF(e.X, e.Y));
			if (mh.documentOutput.selection.startCursor.TargetURL() != "")
				this.Cursor = Cursors.Hand; 
			else this.Cursor = Cursors.Default;
		}
		
		void MhOutputMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			mh.documentOutput.selection.SetStartCursor(new PointF(e.X, e.Y));			
			string URL = mh.documentOutput.selection.startCursor.TargetURL();
			if (URL != "")
			{
				this.Cursor = Cursors.Default;
				Utils.Run(URL, "");
			}
				 
		}		
		
	}
}

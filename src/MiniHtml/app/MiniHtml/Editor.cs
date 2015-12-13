/*
 * Created by SharpDevelop.
 * User: Joseph Leung
 * Date: 8/15/2006
 * Time: 11:23 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
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
	public class mhEditor : System.Windows.Forms.UserControl
	{
		private MiniHtml mh;
		private bool dragging = false;
		private PointF currentPos1, currentPos2, lastPos;
		private string pHtml = "", pCss = "";
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
				return mh.Html();
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
				if (pCss == value) return;
				pCss = value;		
				
				UpdateHtml();
			}
		}
		
		public override Color BackColor {
			get {
				return base.BackColor;
			}
			set {
				if (mh.documentOutput.backColor == value) return;
				
				mh.documentOutput.backColor = value;
				base.BackColor = value;					
				UpdateHtml();						
			}
		}
		
		
		
		void UpdateScrollBar()
		{
			vScrollBar.Minimum = 0;
			vScrollBar.Value = 0;
			vScrollBar.Maximum = mh.documentOutput.Height();
			vScrollBar.Visible = (mh.documentOutput.Height() > Height);
		}
		void UpdateHtml()
		{					
			mh.widthLimit = Width;
			mh.masterTag.childTags.Clear();
			mh.ParseMode = ParseMode;	
			mh.parser.Parse(pHtml);
			mh.parser.ParseCss(pCss);			
			mh.documentOutput.Update();	
				
			mh.documentOutput.selection.SetStartCursor();
			mh.documentOutput.selection.SetEndCursor();
			mh.documentOutput.selection.UpdateSelected(true);  

			mh.documentOutput.basePoint = new PointF(0,0);
			UpdateScrollBar();
					
			Invalidate();						
		}
		
		public mhEditor()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();						
			
			pHtml = "<indent ind=\"15\">abcdefg hijklmn opqrstuv wxyz<b class=\"day\">opqrstuv</b>wxyz</indent>" +
                "<sup>12345</sup>67<sub>890</sub><a href=\"aLink\">link</a>" +
                "<font color=\"#00FF00\" size=\"25\" name=\"Lucida Console\">font</font>";
			pCss = "b.day {color:red;}";
			pHtml = "";
						
			mh = new MiniHtml();
			mh.ParseMode = parseMode.Text;
			mh.widthLimit = Width;
			mh.parser.Parse(pHtml);
			mh.parser.ParseCss(pCss);	
			
			mh.documentOutput.backColor = Color.White;
			mh.documentOutput.selectionColor = Color.LightGreen;		
			
			#if   mh_dotNet_10
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
            			  ControlStyles.DoubleBuffer, true);
			#elif mh_dotNet_20			
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
            			  ControlStyles.OptimizedDoubleBuffer, true);			
			#endif
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// vScrollBar
			// 
			this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar.Location = new System.Drawing.Point(271, 0);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(21, 266);
			this.vScrollBar.TabIndex = 0;
			this.vScrollBar.ValueChanged += new System.EventHandler(this.VScrollBarValueChanged);
			// 
			// mhEditor
			// 
			this.Controls.Add(this.vScrollBar);
			this.Name = "mhEditor";
			this.Size = new System.Drawing.Size(292, 266);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MhEditorMouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MhEditorMouseMove);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MhEditorKeyPress);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MhEditorPaint);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MhEditorMouseUp);
			this.SizeChanged += new System.EventHandler(this.MhEditorSizeChanged);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.VScrollBar vScrollBar;
		#endregion
		
		void MhEditorPaint(object sender, System.Windows.Forms.PaintEventArgs e)
		{			    						
			mh.widthLimit = this.Width - vScrollBar.Width;
			mh.documentOutput.Update();    
			
			GraphicsOutputType output = ((GraphicsOutputType)(mh.documentOutput));            
							   		
			
			output.outputGraphics = e.Graphics;			
			try
			{
				e.Graphics.FillRectangle(new SolidBrush(output.backColor), e.ClipRectangle);
            	output.Output();  
			}
			finally
			{
				output.outputGraphics = null;
			}
		}
		
		void UpdateCursor()
		{			
			mh.documentOutput.selection.SetStartCursor(currentPos1);
            mh.documentOutput.selection.SetEndCursor(currentPos2);
            mh.documentOutput.selection.UpdateSelected(true);                    
		}
		
		void MhEditorMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			        		
			if (e.Button == MouseButtons.Left)
			{
				dragging = true;
				currentPos1 = new PointF(e.X, e.Y + vScrollBar.Value);				
				lastPos = currentPos1;
			}						
		}
		
		void MhEditorMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (dragging)
			{				
				currentPos2 = new PointF(e.X, e.Y + vScrollBar.Value);
				if (Math.Abs(lastPos.X - currentPos2.X) + Math.Abs(lastPos.Y - currentPos2.Y) > 10)
				{
					UpdateCursor();
					Invalidate();				
					lastPos = currentPos2;					
				}
			}
		}
		
		void MhEditorMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				dragging = false;
				currentPos2 = new PointF(e.X, e.Y + vScrollBar.Value);
				UpdateCursor();
				Invalidate();					
			}						
		}
		
		void MhEditorKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{			
			mh.documentOutput.selection.ReplaceText(e.KeyChar + "");					
			Invalidate();	
		}
		
		protected override bool ProcessDialogKey(Keys keyData)
    	{      
      		switch (keyData) 
      		{
      			case Keys.Shift | Keys.Right:
      				mh.documentOutput.selection.endCursor = mh.documentOutput.selection.endCursor.RightShiftText();
      				break;      	
      			case Keys.Shift | Keys.Left:
      				mh.documentOutput.selection.endCursor = mh.documentOutput.selection.endCursor.LeftShiftText();
		      		break;     
		      	case Keys.Shift | Keys.Enter:
		      		mh.documentOutput.selection.InsertBreak(false);		         	
		         	break;		         				         
				case Keys.Left:
		          	mh.documentOutput.selection.LeftShift(); 		          	
		          	break;
		        case Keys.Right:
		          	mh.documentOutput.selection.RightShift();            	
		          	break;
		         case Keys.Space:
		          	mh.documentOutput.selection.ReplaceText(" ");
		          	break;
		         case Keys.Back:
		          	mh.documentOutput.selection.BackSpace();          	
		          	break;
		         case Keys.Enter:
		         	mh.documentOutput.selection.InsertBreak(true);	         	
		         	break;		         				         
		         case Keys.Delete:
		         	mh.documentOutput.selection.Delete();
		        	break;
		        default:
					return base.ProcessDialogKey(keyData);        
		      }
				
		      Invalidate();
		      UpdateScrollBar();
		      return true;
		}
		
		void MhEditorSizeChanged(object sender, System.EventArgs e)
		{
			if ((Visible) && (Width > 0))
				UpdateHtml();
		}
		
		void VScrollBarValueChanged(object sender, System.EventArgs e)
		{
			mh.documentOutput.basePoint = new PointF(0, vScrollBar.Value * -1);			
			Invalidate();
		}
	}
}

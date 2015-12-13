using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace QuickZip.MiniHtml
{
    /// <summary>
    /// Html page in graphical form.
    /// </summary>
    public abstract class DocumentOutputType
    {        
        internal RegionTag masterTag;                           //Owner of all tag, mirror from parentHtml
        internal MiniHtml parentHtml;                           //Reference to parentHtml
        public bool drawCursor = false;							//Specify to draw cursor or not
        public Color cursorColor = Color.Navy;					//Cursor Color
        public Color selectionColor = Color.Gainsboro;			//Selection Color
        public Color backColor = Color.White;					//Background Color
        public PointF basePoint = new PointF(0,0);				//Starting point to draw inside MiniHtml
        public RectangleF outputBounds;							//Region of where output draw to.

        	
        public Selection selection;                             //Start and end selection
        /// <summary>
        /// return space allocator of masterTag
        /// </summary>        
        internal SpaceAllocator spaceAlloc()
        {
            return masterTag.spaceAlloc;
        }                 
        /// <summary>
        /// constructor
        /// </summary>                  
        internal DocumentOutputType(RegionTag aMasterTag)
        {
            masterTag = aMasterTag;
            parentHtml = aMasterTag.parentHtml;
            selection = new Selection(aMasterTag.parentHtml);
            outputBounds = new RectangleF(0,0,99999, 99999);   
        }

        /// <summary>
        /// update masterTag and it's childTags
        /// </summary>
        public void Update()
        {
            CurrentStateType curState = new CurrentStateType(parentHtml, parentHtml.widthLimit);            
            masterTag.Update(ref curState);
        }        
        /// <summary>
        /// clean up
        /// </summary>
        public void ClearBackground()
        {
        	OutputRect(backColor, new RectangleF(0, 0, Width() , Height()), true);
        }
        /// <summary>
        /// output an allocatedItem
        /// </summary>
        private void Output(RegionTag aTag, anAllocatedItem item, PointF basePt)
        {
            item.allocatedTag.needRedraw = false;           
            //TODO: Clean up code
            if (item.allocatedTag is TextTag)
            {
            	PointF itemBasePoint = new PointF(item.actualRect().X + basePt.X, item.actualRect().Y + basePt.Y);
            	RectangleF itemRect = new RectangleF(itemBasePoint, new SizeF(item.allocatedRect.Width, item.allocatedRect.Height));
				outputTextTag(((TextTag)(item.allocatedTag)), itemRect, item.extraInfo);
            }   
            else
            if (item.allocatedTag is ElementTag)
            {
            	PointF itemBasePoint = new PointF(item.actualRect().X + basePt.X, item.actualRect().Y + basePt.Y);
            	RectangleF itemRect = new RectangleF(itemBasePoint, new SizeF(item.allocatedRect.Width, item.allocatedRect.Height));
				outputElementTag(((ElementTag)(item.allocatedTag)), itemRect);
            } 
            else
                if (item.allocatedTag is RegionTag)
                {
            		RectangleF rect = ((RegionTag)(item.allocatedTag)).state.DrawableRect(item.allocatedRect, true);
            		rect.Offset(basePt);
                    {
            			output((RegionTag)item.allocatedTag,rect);                    	       
                    }

                }
        }
        
        /// <summary>
        /// output text
        /// </summary>
        private void outputTextTag(TextTag aTag, RectangleF aRect, Int32 id)
        {
			if ((aTag.TagIndex() >= selection.firstCursor.TagPosition()) &&
                (aTag.TagIndex() <= selection.secondCursor.TagPosition()))                
        		this.OutputRect(selectionColor, aTag.GetSelectionRange(id, aRect), true);
        	
//            if (DrawDebugRectangle)
//            	outputGraphics.DrawRectangle(new Pen(cursorColor), aRect.Left, aRect.Top, aRect.Width, aRect.Height);
            
            OutputString(aTag[id], aTag.font, aTag.textColor, new PointF(aRect.Left, aRect.Top));
        }
        
        private void outputElementTag(ElementTag aTag, RectangleF aRect)
        {
        	if (selection.isBetween(aTag))
        		OutputImage(aTag.invertedImage, new PointF(aRect.Left, aRect.Top));
        	else
        		OutputImage(aTag.elementImage, new PointF(aRect.Left, aRect.Top));
        }
        /// <summary>
        /// output bullets
        /// </summary>
        private void outputBulletTag(RegionTag aTag, RectangleF baseRect)
        {
        	if ((aTag.type == RegionTag.tagType.listItem) && aTag.StartTag())
        	{        		
        		switch (aTag.LastRegionTag().type)
        		{
        			case RegionTag.tagType.orderedList:
        				Int32 idx = aTag.parentTag.childTags.IndexOfType(aTag);
        				String idxString = Utils.Number2BulletValue((UInt32)idx, aTag.state.bulletType);
        					
        				float txtWidth = TextSize(idxString, aTag.state.font).Width;
        				OutputString(idxString, aTag.state.font, Color.Black, 
        		             new PointF(baseRect.Left - (Defines.defaultListIndent / 3) - txtWidth,
        		                        baseRect.Top));
        				break;        				
        		}
        		
        	}        	            	
        }
        /// <summary>
        /// output all tag in specified regionTag
        /// </summary>
        private void output(RegionTag aTag, RectangleF baseRect)
        {        	        	
        	PointF basePt = new PointF(baseRect.X, baseRect.Y);
//        	RectangleF itemRect = new RectangleF(basePt.X + aTag.state.startPosition, basePt.Y, 
//        	                                     aTag.state.endPosition - aTag.state.startPosition, aTag.spaceAlloc.Height());
//        	itemRect = baseRect;
			outputBulletTag(aTag, baseRect);
        	OutputRect(aTag.state.bkColor, baseRect, true);
        	PointF pt1 = new PointF(baseRect.X, baseRect.Y);
        	PointF pt2 = new PointF(baseRect.X, baseRect.Y + baseRect.Height);
        	PointF pt3 = new PointF(baseRect.X + baseRect.Width, baseRect.Y + baseRect.Height);
        	PointF pt4 = new PointF(baseRect.X + baseRect.Width, baseRect.Y);        	        	
        	OutputLine(aTag.state.borderColor[(Int32)fourSide._top], pt1, pt2, aTag.state.borderWidth[(Int32)fourSide._top]);
        	OutputLine(aTag.state.borderColor[(Int32)fourSide._right], pt2, pt3, aTag.state.borderWidth[(Int32)fourSide._right]);
        	OutputLine(aTag.state.borderColor[(Int32)fourSide._bottom], pt3, pt4, aTag.state.borderWidth[(Int32)fourSide._bottom]);
        	OutputLine(aTag.state.borderColor[(Int32)fourSide._left], pt4, pt1, aTag.state.borderWidth[(Int32)fourSide._left]);
        	                   	            
            foreach (anAllocatedItem item in aTag.spaceAlloc.allocList)
            {
                Output(aTag, item, basePt);
            }
            foreach (anAllocatedItem item in aTag.spaceAlloc.floatList)
            {
                Output(aTag, item, basePt);
            }
        }
        /// <summary>
        /// output to display.
        /// </summary>
        public void Output()
        {
        	BeginOutput();
        	PointF basePt = new PointF(basePoint.X + outputBounds.X, basePoint.Y + outputBounds.Y);        	
        	RectangleF rect = new RectangleF(basePt, new SizeF(Width(), Height()));
            output(masterTag, rect);
            OutputCursor();
            EndOutput();
        }
        
        
        /// <summary>
        /// measure Text size, can override
        /// </summary>
        public virtual SizeF TextSize(string aText, Font aFont)
        {
            return new SizeF(aText.Length, 1);
        }
        /// <summary>
        /// output selection.startCursor and endCursor
        /// </summary>
        public virtual void OutputCursor()
        {

        }
        /// <summary>
        /// Output a filled/unfilled rectangle 
        /// </summary>
        public void OutputRect(Color aColor, RectangleF aRect, bool Filled)
        {
        	if (aColor == Color.Transparent) return;
        	OutputRect(aColor, aRect, Filled, 1F);
        }
        /// <summary>
        /// Output a filled/unfilled rectangle 
        /// </summary>
        public virtual void OutputRect(Color aColor, RectangleF aRect, bool Filled, float aSize)
        {
        	if (aColor == Color.Transparent) return;
        	
        	PointF pt1 = new PointF(aRect.Left, aRect.Top);
        	PointF pt2 = new PointF(aRect.Left + aRect.Width, aRect.Top);
        	PointF pt3 = new PointF(aRect.Left + aRect.Width, aRect.Top + aRect.Height);
        	PointF pt4 = new PointF(aRect.Left, aRect.Top + aRect.Height);
        	        
        	OutputLine(aColor, pt1, pt2, aSize);
        	OutputLine(aColor, pt2, pt3, aSize);
        	OutputLine(aColor, pt3, pt4, aSize);
        	OutputLine(aColor, pt4, pt1, aSize);
        }
        
        /// <summary>
        /// Output a line
        /// </summary>
        public void OutputLine(Color aColor, PointF startPt, PointF endPt, float aSize)
        {
        	if (aColor == Color.Transparent) return;
        	OutputLine(new SolidBrush(aColor), startPt, endPt, aSize);
        }
        /// <summary>
        /// Output a line
        /// </summary>
        public virtual void OutputLine(Brush aBrush, PointF startPt, PointF endPt, float aSize)
        {
        	
        }        
        
        
        public virtual void OutputString(string text, Font font, Color textColor, PointF point)
        {

        }
        
        public virtual void OutputImage(Image img, PointF point)
        {        	
        	
        }
        /// <summary>
        /// run this when before Output(), can override;
        /// </summary>
        public virtual void BeginOutput()
        {

        }
        /// <summary>
        /// run this when finished Output(), can override;
        /// </summary>
        public virtual void EndOutput()
        {

        }

        
        internal HtmlTag GetFocusTag(ref float x, ref float y, ref Int32 extraInfo)
        {
            return masterTag.GetFocusTag(ref x, ref y, ref extraInfo);
        }
        internal HtmlTag GetFocusTag(float x, float y)
        {
            return masterTag.GetFocusTag(x, y);
        }
        internal TextTag GetFocusText(float x,float y, ref Int32 aLine, ref Int32 aChar)
        {            
            HtmlTag retVal = GetFocusTag(ref x, ref y, ref aLine);
            if ((retVal == null) || (!(retVal is TextTag)))
                return null;

            ((TextTag)(retVal)).GetFocusedText(x, y, aLine, ref aChar);
            return ((TextTag)(retVal));
        }
        internal PointF GetFocusPosition(HtmlTag aTag, Int32 aLine, Int32 aChar)
        {
            RectangleF tagPos = aTag.LastSpaceAllocator().GetTagRect(aTag, aLine);            
            RectangleF basePos = aTag.LastSpaceAllocator().absoluteBaseRect();           
                                    
            basePos = ((RegionTag)(aTag.LastRegionTag())).state.DrawableRect(basePos, true);
            
            if (aTag is TextTag)
            {
                TextTag tag = (TextTag)aTag;
                
                float txtLength;
                if (aChar > 0)
                	txtLength = TextSize(tag[aLine].Substring(0, aChar), tag.font).Width;
                else txtLength = 0;
                
                return new PointF(tagPos.X + basePos.X + txtLength,
                    tagPos.Y + basePos.Y);
            }

            return new PointF(tagPos.X + basePos.X, tagPos.Y + basePos.Y);
        }
        internal PointF GetFocusPosition(HtmlTag aTag, out Int32 aLine, ref Int32 aChar)
        {
            aLine = -1;
            if (aTag is TextTag)
            {
                TextTag tag = (TextTag)aTag;
                tag.LocateText(out aLine, ref aChar);
            }
            return GetFocusPosition(aTag, aLine, aChar);
        }
        public Int32 Height()
        {
        	return (Int32)(spaceAlloc().Height()) + 1 + (Int32)outputBounds.Y;
        }
        public Int32 Width()
        {
        	return parentHtml.widthLimit + (Int32)outputBounds.X;
        }
        private void PrintItems(RegionTag aTag)
        {
            aTag.spaceAlloc.allocList.PrintItems();
            aTag.spaceAlloc.floatList.PrintItems();
            foreach (anAllocatedItem item in aTag.spaceAlloc.allocList)
            {
                if (item.allocatedTag is RegionTag)
                {
                    PrintItems((RegionTag)(item.allocatedTag));
                }
            }
            foreach (anAllocatedItem item in aTag.spaceAlloc.floatList)
            {
                if (item.allocatedTag is RegionTag)
                {
                    PrintItems((RegionTag)(item.allocatedTag));
                }
            }                        
        }
        public void PrintItems()
        {
            PrintItems(masterTag);
        }
        
        /// <summary>
        /// return if specified font is usable.
        /// </summary>
        public virtual bool FontExists(string fntName)
        {
        	return false;
        }                        
    }
    /// <summary>
    /// output HtmlTag to Graphics
    /// </summary>
    public class GraphicsOutputType : DocumentOutputType
    {
        Bitmap tempBitmap = null;
        Graphics tempGraphics = null;
        public Graphics outputGraphics = null;
        public bool DrawDebugRectangle = false;
        

        /// <summary>
        /// constructor
        /// </summary>        
        internal GraphicsOutputType(RegionTag aMasterTag)
            : base(aMasterTag)
        {

        }        
        /// <summary>
        /// check if tempGraphics constructed, create if not
        /// </summary>
        private void checkGraphics()
        {
            if (tempGraphics == null)
            {
                tempBitmap = new Bitmap(10, 10);
                tempGraphics = Graphics.FromImage(tempBitmap);
            }
        }
        /// <summary>
        /// free up graphics object when completed.
        /// </summary>
        public void FreeGraphics()
        {
            try
            {
                if (tempGraphics != null)
                    tempGraphics.Dispose();
                if (tempBitmap != null)
                    tempBitmap.Dispose();                
            }
            finally
            {
                tempGraphics = null;
                tempBitmap = null;
            }
        }
        /// <summary>
        /// return Textsize
        /// </summary>
        public override SizeF TextSize(string aText, Font aFont)
        {
            checkGraphics();
            return Utils.TextSize(tempGraphics, aText, aFont);            
        }        
        /// <summary>
        /// run this when before Output(), can override;
        /// </summary>
        public override void BeginOutput()
        {        	
        	base.BeginOutput();
        	outputGraphics.Clip = new Region(outputBounds);        	
        	checkGraphics();
        }
        /// <summary>
        /// execute when Output() finished, free up resources.
        /// </summary>
        public override void EndOutput()
        {            
        	base.EndOutput();            
        	outputGraphics.Clip = new Region();
        	FreeGraphics();
        }
        /// <summary>
        /// Output a filled/unfilled rectangle 
        /// </summary>
        public override void OutputRect(Color aColor, RectangleF aRect, bool Filled, float aSize)
        {
        	if (Filled)
        		outputGraphics.FillRectangle(new SolidBrush(aColor),aRect);
        	else
        		outputGraphics.DrawRectangle(new Pen(aColor, aSize), aRect.X, aRect.Y, aRect.Width, aRect.Height);        	
        }        
        /// <summary>
        /// output a line
        /// </summary>
        public override void OutputLine(Brush aBrush, PointF startPt, PointF endPt, float aSize)
        {
        	outputGraphics.DrawLine(new Pen(aBrush, aSize), startPt, endPt);
        }
        
        public override void OutputString(string text, Font font, Color textColor, PointF aPoint)
        {
        	Utils.DrawString(outputGraphics, text, font,
                new SolidBrush(textColor), aPoint);
        }
        
        public override void OutputImage(Image img, PointF point)
        {        	
        	Utils.DrawImage(outputGraphics, img, point);
        }
        
        public override void OutputCursor()
        {        	
        	if (selection.CursorMode)
        	{        		
        		selection.startCursor.UpdatePosition();        		
        		if (selection.startCursor.selectedTag is VisibleTag)
        			outputGraphics.FillRectangle(new SolidBrush(cursorColor),
        		                             selection.startCursor.position.X - 0.5F,
        		                             selection.startCursor.position.Y - Math.Abs(basePoint.Y),
        		                             1.0F,
        		                             ((VisibleTag)(selection.startCursor.selectedTag)).visibleHeight);
        	}
        }
        
        //ToDO: Complete FontExists to check UserFont!
        /// <summary>
        /// return if specified font is usable.
        /// </summary>
        public override bool FontExists(string fntName)
        {        	
        	checkGraphics();
        	return Utils.FontExists(fntName, tempGraphics);
        }
        
    }
}

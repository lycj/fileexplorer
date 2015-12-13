using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace QuickZip.MiniHtml
{
    /// <summary>
    /// Represent an allocated item in SpaceAllocator List
    /// </summary>
    public class anAllocatedItem
    {
        public Int32 extraInfo;
        public Int32 currentLine;
        public hAlignType align;
        internal HtmlTag allocatedTag;        
        public RectangleF allocatedRect;
        private bool calActualRect = false;
        private RectangleF pActualRect;
        internal anAllocatedItem link;					// Link to first item

        internal anAllocatedItem(HtmlTag aTag, hAlignType anAlign, RectangleF aRect, Int32 aCurrentLine, 
                                 Int32 anExtraInfo, anAllocatedItem aLink)
        {
            allocatedTag = aTag;
            align = anAlign;
            allocatedRect = aRect;
            currentLine = aCurrentLine;
            extraInfo = anExtraInfo;
            link = aLink;            
        }
        internal anAllocatedItem(HtmlTag aTag, hAlignType anAlign, float aLeft, float aTop, float aWidth, float aHeight, 
                                 Int32 aCurrentLine, Int32 anExtraInfo, anAllocatedItem aLink)
        {
            allocatedTag = aTag;
            align = anAlign;
            allocatedRect = new RectangleF(aLeft,aTop,aWidth,aHeight);
            currentLine = aCurrentLine;
            extraInfo = anExtraInfo;
            link = aLink;
        }
        ~anAllocatedItem()
        {
            allocatedTag = null;
        }
        public RectangleF actualRect()
        {
        	if (calActualRect)
        		return pActualRect;
        	
            if (allocatedTag is RegionTag)
            { pActualRect = ((RegionTag)allocatedTag).state.UpdateRect(allocatedRect, true); }
            else
            	if (allocatedTag is VisibleTag)
            	{       
            		PointF itemBasePoint;
            		switch (((VisibleTag)(allocatedTag)).vAlign)
            		{
            			case vAlignType.Bottom:
            				{
            					itemBasePoint = new PointF(allocatedRect.Left,
                        		allocatedRect.Top + allocatedTag.LastSpaceAllocator().lineInfo[currentLine].lineHeight 
                        		- allocatedRect.Height);
                        		break;
            				}
            			case vAlignType.Top:
            				{
            					itemBasePoint = new PointF(allocatedRect.Left,
                        		allocatedRect.Top);
                        		break;
            				}
            			default:
            				{
            					itemBasePoint = new PointF(allocatedRect.Left,
                        		allocatedRect.Top + (allocatedTag.LastSpaceAllocator().lineInfo[currentLine].lineHeight 
            					                    - allocatedRect.Height) / 2);
                        		break;
            				}
            		}
            		pActualRect = new RectangleF(itemBasePoint,
                    new SizeF(allocatedRect.Width, allocatedRect.Height));
            	}
            	else
            		{ pActualRect = allocatedRect; }
            	
            	calActualRect = true;
            	return pActualRect;
        }
        public bool isIn(float x, float y)
        {			
            RectangleF actualrect = actualRect();     

            if ((actualrect.Left <= x) && (actualrect.Top <= y) &&
                (actualrect.Right >= x) && (actualrect.Bottom >= y))
            {
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// A list of anAllocatedItem
    /// </summary>
    public class AllocatedList : CollectionBase
    {
        private anAllocatedItem getItem(HtmlTag aTag)
        {
        	for (int i = 0; i < this.Count; i++)
        	{
        		if (this[i].allocatedTag == aTag)
        			return this[i];
        	}
        	return null;
        }
    	private anAllocatedItem getItem(Int32 aIndex)
        {
            return (anAllocatedItem)List[aIndex];
        }
        private void setItem(Int32 aIndex, anAllocatedItem value)
        {
            List[aIndex] = value;
        }
        private void VerifyType(object value)
        {
        	if (!(value is anAllocatedItem))
                if (value == null)
                { throw new ArgumentException("Null exception"); }
                else
                { throw new ArgumentException("Invalid Type " + value.ToString()); }
        }
        protected override void OnInsert(Int32 aIndex, object value)
        {
            VerifyType(value);
        }
        protected override void OnSet(Int32 aIndex, object oldValue, object newValue)
        {
            VerifyType(newValue);
        }
        protected override void OnValidate(object value)
        {
            VerifyType(value);
        }
        ~AllocatedList()
        {
            List.Clear();
        }
        public Int32 Add(anAllocatedItem value)
        {
            List.Add(value);
            return List.Count - 1;
        }
        internal Int32 Add(HtmlTag aTag, hAlignType anAlign, float aLeft, float aTop, float aWidth,
            float aHeight, Int32 aCurrentLine, Int32 anExtraInfo)
        {        	        	         
        	return Add(new anAllocatedItem(aTag, anAlign,
        	                               new RectangleF(aLeft, aTop, aWidth, aHeight), 
        	                               aCurrentLine, anExtraInfo, this[aTag]));
        }
        internal void Insert(Int32 index, anAllocatedItem value)
        {
            List.Insert(index, value);
        }
        public void Remove(anAllocatedItem value)
        {
            List.Remove(value);
        }
        public bool Contains(anAllocatedItem value)
        {
            return List.Contains(value);
        }
        internal anAllocatedItem this[HtmlTag aTag]
        {
            get
            {
            	return getItem(aTag);
            }
        }
        public anAllocatedItem this[Int32 index]
        {
            get
            {
                return getItem(index);
            }
            set
            {
                setItem(index, value);
            }
        }
        internal Int32 GetLineIndex(HtmlTag aTag, Int32 anExtraInfo)
        {
        	foreach (anAllocatedItem item in this)
        		if ((item.allocatedTag == aTag) && (item.extraInfo == anExtraInfo))
        			return item.currentLine;
        	return -1;
        }
        public void PrintItems()
        {
            foreach (anAllocatedItem item in this)
            {
                Console.WriteLine(String.Format("TagName: {0}, Line {5}, Position: ({1},{2}),({3},{4})",
                    item.allocatedTag.name, item.allocatedRect.Left, item.allocatedRect.Top, 
                    item.allocatedRect.Width, item.allocatedRect.Height, item.currentLine));
            }            
        }        
    }
    /// <summary>
    /// Represent a line in allocate list.
    /// </summary>
    public class aLine
    {
        public float lineTop, lineWidth, lineWidthLimit, lineHeight;
        public Int32 lineNumber;
        private SpaceAllocator spaceAlloc;
        public aLine(SpaceAllocator aSpaceAlloc, float aLineWidthLimit, float aLineTop, float aLineHeight, Int32 aLineNumber)
        {
        	spaceAlloc = aSpaceAlloc;
            lineWidthLimit = aLineWidthLimit;
            lineTop = aLineTop;
            lineHeight = aLineHeight;
            lineNumber = aLineNumber;
        }
        public void UpdateHeight(float newHeight)
        {
            if (newHeight > lineHeight)
            {
                lineHeight = newHeight;
            }
        }
        public float GetLeftOffset(hAlignType anAlign, float aWidthLimit)
        {
            switch (anAlign)
            {
            	//TODO: Figure out what Align.right goes wrong
                case hAlignType.Right:
                    return lineWidthLimit - lineWidth - 10;                    
                case hAlignType.Centre:
                    return (lineWidthLimit - lineWidth) / 2;                    
                default:
                    return 0;                    
            }
        }
        /// <summary>
        /// Return a list of allocatedItem in current line
        /// </summary>
        public AllocatedList TagList()
        {
        	AllocatedList retVal = new AllocatedList();
        	foreach (anAllocatedItem item in spaceAlloc.allocList)
        		if (item.currentLine == lineNumber)
        			retVal.Add(item);
        	return retVal;
        }
        
        internal HtmlTag LastTag()
        {
        	AllocatedList aList = TagList();
        	if (aList.Count == 0) return null;
        	return aList[aList.Count-1].allocatedTag;
        }
        
        internal HtmlTag FirstTag()
        {
        	AllocatedList aList = TagList();
        	if (aList.Count == 0) return null;
        	return aList[aList.Count-1].allocatedTag;      	
        }
    }
    /// <summary>
    /// A list of Lines (aLine)
    /// </summary>
    public class LineInfoList : CollectionBase
    {    	
        private aLine getLine(Int32 aIndex)
        {
            return (aLine)List[aIndex];
        }
        public void setLine(Int32 aIndex, aLine value)
        {
            List[aIndex] = value;
        }
        public void VerifyType(object value)
        {
            if (!(value is aLine))
            {
                throw new ArgumentException("Invalid Type, Not a line");
            }
        }
        protected override void OnInsert(Int32 aIndex, object value)
        {
            VerifyType(value);
        }
        protected override void OnSet(Int32 aIndex, object oldValue, object newValue)
        {
            VerifyType(newValue);
        }
        protected override void OnValidate(object value)
        {
            VerifyType(value);
        }
        ~LineInfoList()
        {
            List.Clear();
        }
        public Int32 Add(aLine value)
        {
            List.Add(value);
            return List.Count - 1;
        }
        public void Insert(Int32 index, aLine value)
        {
            List.Insert(index, value);
        }
        public void Remove(aLine value)
        {
            List.Remove(value);
        }
        public bool Contains(aLine value)
        {
            return List.Contains(value);
        }
        public aLine this[Int32 index]
        {
            get
            {
                return getLine(index);
            }
            set
            {
                setLine(index, value);
            }
        }
        public float getLineHeight(Int32 aIndex)
        {
            return this[aIndex].lineHeight;
        }
        public float getLineTop(Int32 aIndex)
        {
            return this[aIndex].lineTop;
        }
        public void setTopMargin(Int32 value)
        {
            if (Count == 1)
            {
                this[0].lineTop = value;
            }
        }
    }
    /// <summary>
    /// Keep track of which tag occupying space, and make sure they dont overlap each other.
    /// </summary>
    public class SpaceAllocator
    {
        float maxWidth, maxHeight;      //Maxium widthLimit and height
        public RectangleF baseRect;     //The position of tag rect, updated by it's own regiontag
        public float currentX, currentY;//Current position for allocate.
        public LineInfoList lineInfo;   //Line top and height information
        Int32 currentLine = -1;         //Current Line number (Default = -1)
        internal RegionTag ownerTag;    //ownerTag should be regionTag
        public AllocatedList allocList; //A List of item that required to draw
        public AllocatedList floatList; //A list for items that is have cssPositiontype

        public RectangleF absoluteBaseRect()
        {
        	PointF basePt = new PointF(baseRect.X, baseRect.Y);
        	RegionTag currentTag = ownerTag.LastRegionTag();
        	
        	while (!(currentTag is MasterTag))
        	{
        		RectangleF tagPos = currentTag.LastSpaceAllocator().GetTagRect(currentTag, -1);
        		basePt = new PointF(basePt.X + tagPos.X, basePt.Y + tagPos.Y);        		                            		
        		currentTag = currentTag.LastRegionTag();
        	}
        	
        	return new RectangleF(basePt.X, basePt.Y, baseRect.Width, baseRect.Height);        	
        }
        	
        /// <summary>
        /// Return max allocatable widthLimit.
        /// </summary>        
        public float WidthLimit()
        {
            CurrentStateType tagState = ownerTag.state;
            return tagState.endPosition - tagState.startPosition - 
            	tagState.GetBorderSpace(fourSide._left, true) - 
            	tagState.GetBorderSpace(fourSide._right, true);
        }    
        /// <summary>
        /// Return current widthLimit.
        /// </summary>        
        public float Width()
        {            
            if (WidthLimit() > maxWidth)
            {
                return maxWidth;
            }
            else
            {
                return WidthLimit();
            }
        }
        /// <summary>
        /// 
        /// </summary>        
        public float EndPosition()
        {
            CurrentStateType tagState = ownerTag.state;
            return tagState.endPosition - tagState.padding[(Int32)fourSide._left] -
                tagState.padding[(Int32)fourSide._right] 
                - tagState.margin[(Int32)fourSide._left] - tagState.margin[(Int32)fourSide._right];                        
        }   
        /// <summary>
        /// Move to next line and reset currentX
        /// </summary>
        /// <param name="startPos">a start position</param>
        /// <returns>current line number</returns>
        public Int32 NewLine(float startPos)
        {            	
            if (currentLine == -1) { lineInfo.Add(new aLine(this, WidthLimit(), 0, 0, 0)); } else
            if (currentX == startPos) return lineInfo.Count-1; else
            {
                aLine line = lineInfo[lineInfo.Count-1];
                lineInfo.Add(new aLine(this, WidthLimit(), line.lineTop + line.lineHeight, 0, line.lineNumber + 1));
            }
            currentLine ++;
            currentX = startPos;
            currentY = lineInfo[lineInfo.Count-1].lineTop;
            return lineInfo.Count-1;
        }         
        /// <summary>
        /// Move to next line and reset currentX
        /// </summary>
        /// <returns>Current line number</returns>
        public Int32 NewLine()
        {
            CurrentStateType tagState = ownerTag.state;
            return NewLine(tagState.startPosition);
        }                       
        /// <summary>
        /// Indicate whether can allocate specified widthLimit in current line, without exceeding WidthLimit()
        /// </summary>
        /// <param name="aWidth">a widthLimit to test</param>        
        public bool CanAllocateSpace(float aWidth)
        {
            return (currentX + aWidth <= EndPosition());
        }  
        /// <summary>
        /// Allocate a tag
        /// </summary>
        /// <param name="aTag">a tag</param>
        /// <param name="aWidth">widthLimit</param>
        /// <param name="aHeight">height</param>
        /// <param name="anExtraInfo">extrainfo to be stored</param>
        /// <returns>relative position where the tag should be drawn</returns>
        internal RectangleF AllocateSpace(HtmlTag aTag, float aWidth, float aHeight, Int32 anExtraInfo)   
        {
            if (!(CanAllocateSpace(aWidth))) { NewLine();}
            lineInfo[currentLine].UpdateHeight(aHeight);
            allocList.Add(aTag, hAlignType.Unknown, currentX, currentY, aWidth, aHeight, currentLine, anExtraInfo);
            
            lineInfo[currentLine].lineWidth += aWidth;
            currentX += aWidth + 1;            
            if (maxWidth < currentX) { maxWidth = currentX; }

            return allocList[allocList.Count - 1].allocatedRect;
        }    
        /// <summary>
        /// Allocate a tag
        /// </summary>
        /// <param name="aTag">a tag</param>
        /// <param name="anAlign">hAlign</param>
        /// <param name="aWidth">widthLimit</param>
        /// <param name="aHeight">height</param>
        /// <param name="anExtraInfo">extrainfo to be stored</param>
        /// <returns>relative position where the tag should be drawn</returns>
        internal RectangleF AllocateSpace(HtmlTag aTag, hAlignType anAlign, float aWidth, float aHeight, Int32 anExtraInfo)
        {
            if ((anAlign == hAlignType.Left) && (!(CanAllocateSpace(aWidth)))) { NewLine(); }
            if ((anAlign == hAlignType.Right) && (currentX + aWidth > WidthLimit())) { NewLine(); }
            lineInfo[currentLine].UpdateHeight(aHeight);

            switch (anAlign)
            {
                case hAlignType.Right:
                    allocList.Add(aTag, anAlign, WidthLimit() - aWidth, currentY, aWidth, aHeight, currentLine, anExtraInfo);
                    currentX += (WidthLimit() - aWidth) + 1;
                    NewLine();
                    break;
                case hAlignType.Centre:
                    allocList.Add(aTag, anAlign, (WidthLimit() - aWidth) /2, currentY, aWidth, aHeight, currentLine, anExtraInfo);
                    currentX += (WidthLimit() - aWidth) /2 + 1;
                    NewLine();
                    break;
                default:
                    allocList.Add(aTag, anAlign, currentX, currentY, aWidth, aHeight, currentLine, anExtraInfo);
                    currentX += aWidth + 1;
                    break;
            }
            return allocList[allocList.Count - 1].allocatedRect;
        }
        /// <summary>
        /// Allocate a tag to any spot in the current space allocator 
        /// Used for tag with Css
        /// </summary>
        /// <param name="aTag">a tag</param>
        /// <param name="aLeft">left</param>
        /// <param name="aTop">top</param>
        /// <param name="aWidth">widthLimit</param>
        /// <param name="aHeight">height</param>
        /// <param name="anExtraInfo">extrainfo to be stored</param>
        /// <returns>relative position where the tag should be drawn<</returns>
        internal RectangleF AllocateFloat(HtmlTag aTag, Int32 aLeft, Int32 aTop, float aWidth, float aHeight, Int32 anExtraInfo)
        {
            floatList.Add(aTag, hAlignType.Unknown,  aLeft, aTop, aWidth, aHeight, -1, anExtraInfo);
            if (maxWidth < aLeft + aWidth) { maxWidth = aLeft + aWidth; }
            if (maxHeight < aTop + aHeight) { maxHeight = aTop + aHeight; }

            return floatList[allocList.Count - 1].allocatedRect;
        }
        /// <summary>
        /// Clear all allocated tag list
        /// </summary>
        public void ClearAllocateList()
        {
            currentLine = -1;
            lineInfo.Clear();
            NewLine(0);
            allocList.Clear();
            floatList.Clear();
            maxWidth = 0;
            maxHeight = 0;
            currentX = 0;
            currentY = 0;
        }
        /// <summary>
        /// Return height (allocated) of the spaceAllocator
        /// </summary>        
        public float Height()
        {
            float retVal;
            if (currentLine > -1)
            { retVal = currentY + lineInfo[currentLine].lineHeight; }
            else
            { retVal = currentY; }

            if (retVal < maxHeight) { return maxHeight; }
            else
            { return retVal; }
        }        
        /// <summary>
        /// Return the relative posiition of an allocated tag.
        /// </summary>
        internal RectangleF GetTagRect(HtmlTag aTag, Int32 extraInfo)
        {
            RectangleF retVal = new RectangleF(0, 0, 0, 0);
            foreach (anAllocatedItem item in allocList)
            {
                if (item.allocatedTag.Equals(aTag))
                    if (item.extraInfo == extraInfo)
                { return item.actualRect(); }
            }
            foreach (anAllocatedItem item in floatList)
            {
                if (item.allocatedTag.Equals(aTag))
                    if (item.extraInfo == extraInfo)
                { return item.actualRect(); }
            }
            return retVal;
        }
        /// <summary>
        /// Return the relative posiition of an allocated tag.
        /// </summary>
        /// <param name="aTag">tag</param>
        /// <param name="first">return first found item</param>
        /// <returns></returns>
        internal RectangleF GetTagRect(HtmlTag aTag, bool first)
        {
            RectangleF retVal = new RectangleF(0, 0, 0, 0);
            foreach (anAllocatedItem item in allocList)
            {
                if (item.allocatedTag.Equals(aTag))
                { if (first) {return item.allocatedRect;} else {retVal = item.allocatedRect;} }
            }
            foreach (anAllocatedItem item in floatList)
            {
                if (item.allocatedTag.Equals(aTag))
                { if (first) {return item.allocatedRect;} else {retVal = item.allocatedRect;} }
            }
            return retVal;
        }
        /// <summary>
        /// Return the relative posiition of an allocated tag.
        /// </summary>
        /// <param name="aTag">tag</param>
        /// <returns></returns>
        internal RectangleF GetTagRect(HtmlTag aTag)
        {
            return GetTagRect(aTag, true);
        }
        /// <summary>
        /// Return an updated RectangleF for specified rect
        /// Useful when hAlign is centre / right
        /// </summary>
        /// <param name="currentLine">line tag allocated</param>
        /// <param name="originalRect">original tag allocated location</param>
        /// <returns></returns>
        private RectangleF applyRectAlign(Int32 currentLine, RectangleF originalRect)
        {
            float Offset = 0;

            if (currentLine < lineInfo.Count)
            { Offset = lineInfo[currentLine].GetLeftOffset(ownerTag.hAlign, WidthLimit()); }

            return new RectangleF(originalRect.Left + Offset, originalRect.Top, originalRect.Width, originalRect.Height);
        }
        /// <summary>
        /// Update all allocated tag's allocatedRect for centre/right hAlign.
        /// </summary>
        public void FinishAllocate()
        {
            foreach (anAllocatedItem item in allocList)
            {
                if ((!(item.allocatedTag is ElementTag)) || (((ElementTag)item.allocatedTag).objAlign == hAlignType.Unknown))
                {
                    item.allocatedRect = applyRectAlign(item.currentLine, item.allocatedRect);
                }
            }
        }
        /// <summary>
        /// Return top (relative to pMastertag) of current space allocator.
        /// </summary>
        /// <returns></returns>
        public float SpaceAllocTop()
        {
            RegionTag cTag = ownerTag;
            float RetVal = 0;

            while (cTag != cTag.parentHtml.masterTag)
            {
                RetVal += cTag.LastSpaceAllocator().GetTagRect(cTag).Top;
                cTag = cTag.LastRegionTag();
            }

            return RetVal;
        }
		/// <summary>
		/// Constructor
		/// </summary>
        internal SpaceAllocator(RegionTag anOwnerTag)
        {
            ownerTag = anOwnerTag;
            allocList = new AllocatedList();
            floatList = new AllocatedList();
            lineInfo = new LineInfoList();
            lineInfo.Clear();

            currentLine = -1;
            currentX = 0;
            currentY = 0;
            NewLine();
        }
        
        public static void DebugUnit()
        {
            RegionTag rTag = new RegionTag(null, "Debug", null, 0, 0);
            CurrentStateType cState = new CurrentStateType(new MiniHtml(),100);
            rTag.Update(ref cState);
            SpaceAllocator spaceAlloc = new SpaceAllocator(rTag);               
            HtmlTag aTag = new HtmlTag(); aTag.name = "abc";
            spaceAlloc.AllocateSpace(aTag, 20, 15, -1);
            aTag = new HtmlTag(); aTag.name = "cde";
            spaceAlloc.AllocateSpace(aTag, 70, 15, -1);
            aTag = new HtmlTag(); aTag.name = "xyz";
            spaceAlloc.AllocateSpace(aTag, 50, 15, -1);
            spaceAlloc.allocList.PrintItems();
            spaceAlloc.floatList.PrintItems();
            

        }
    }

//    /// <summary>
//    /// Helper class to allocate cells (td) to SpaceAllocator in TableTag
//    /// This is not replacement of SpaceAllocator
//    /// </summary>
//    public class TableSpaceAllocator
//    {
//    	Int32 maxCol, maxRow;      		//Maxium column and row.
//        //public LineInfoList rowInfo;  //Line top and height information
//        Int32 currentRow = -1;          //Current Line number (Default = -1)
//        Int32 currentCol = -1;  
//        internal TableTag ownerTag;    	//ownerTag should be regionTag
//        private anAllocatedItem[,] allocList; 	//A list of Table Cells        
//     
//        /// <summary>
//        /// Constructor
//        /// </summary>
//        internal TableSpaceAllocator(TableTag anOwnerTag, Int32 aMaxCol, Int32 aMaxRow)
//        {
//        	ownerTag = anOwnerTag;
//        	maxCol = aMaxCol;
//        	maxRow = aMaxRow;
//        	allocList = new anAllocatedItem[aMaxCol, aMaxRow];
//        	
//        	newRow();
//        }
//        
//        /// <summary>
//        /// Move to next row
//        /// </summary>        
//        public Int32 newRow()
//        {
//        	currentCol = 0;
//        	currentRow += 1;        	
//        	
//        	return currentRow;
//        }
//        
//        /// <summary>
//        /// Allociate a cell
//        /// </summary>
//        internal void AllocateSpace(TableTag aTag, hAlignType anAlign, float aWidth, float aHeight, Int32 anExtraInfo)
//        {
//        	while (allocList[currentCol, currentRow] != null)
//        	{ currentCol += 1; }
//        	
//        	anAllocatedItem initItem = new anAllocatedItem(aTag, anAlign,
//        		                    new RectangleF(0, 0, aWidth / aTag.colSpan, aHeight / aTag.rowSpan), //Left and Top is unassigned.
//        		                    currentRow, currentCol, null);
//        	
//        	allocList[currentCol, currentRow] = initItem;        		
//        	currentCol += 1;
//        	
//        	#region ColSpan        	
//        	for (Int32 i = 1; i < aTag.colSpan; i++)
//        	{
//        		anAllocatedItem newItem = new anAllocatedItem(aTag, anAlign,
//        		                    new RectangleF(0, 0, aWidth / aTag.colSpan, aHeight / aTag.rowSpan), //Left and Top is unassigned.
//        		                    currentRow, currentCol, initItem);
//        		allocList[currentCol + i, currentRow] = newItem;   
//        	}
//        	#endregion
//        	
//        	#region RowSpan        	
//        	for (Int32 i = 1; i < aTag.rowSpan; i++)
//        	{
//        		anAllocatedItem newItem = new anAllocatedItem(aTag, anAlign,
//        		                    new RectangleF(0, 0, aWidth / aTag.colSpan, aHeight / aTag.rowSpan), //Left and Top is unassigned.
//        		                    currentRow, currentCol, initItem);
//        		allocList[currentCol, currentRow + i] = newItem;   
//        	}
//        	#endregion
//
//        }
//        
//        public float WidthLimit()
//        {
//            CurrentStateType tagState = ownerTag.state;
//            return tagState.endPosition - tagState.startPosition - 
//            	tagState.GetBorderSpace(fourSide._left, true) - 
//            	tagState.GetBorderSpace(fourSide._right, true);
//        }    
//     
//        
//        /// <summary>
//        /// Calculate ColWidth and RowHeight, then allocate it to SpaceAllocator
//        /// </summary>
//        public void FinishAllocate()
//        {
//        	#region Calculate Max requested rowHeight and colWidth        	        	
//        	float[] maxRowHeight = new float[maxRow];
//        	for (Int32 y = 0; y < maxRow; y++)
//        	{
//        		maxRowHeight[y] = 0;
//        		
//        		for (Int32 x = 0; x < maxCol; x++)
//        			if ((allocList[x,y] != null) &&
//        			    (allocList[x, y].allocatedRect.Height > maxRowHeight[y]))
//        				maxRowHeight[y] = allocList[x, y].allocatedRect.Height;
//        	}
//        		
//        	float[] maxColWidth = new float[maxCol];
//        	bool[] lockCol = new bool[maxCol];        	        	
//        	
//        	for (Int32 x = 0; x < maxCol; x++)
//        	{
//        		maxColWidth[x] = 0;
//        		lockCol[x] = false;
//        		
//        		for (Int32 y = 0; y < maxRow; y++)
//        		{
//        			if ((allocList[x,y] != null) && !lockCol[x])
//        			{
//        			    if (allocList[x, y].allocatedRect.Width > maxColWidth[y])
//        					maxColWidth[y] = allocList[x, y].allocatedRect.Width;
//        			    if (((TableTag)allocList[x,y].allocatedTag).state.width != -1)
//        			    {
//        			    	lockCol[x] = true;
//        			    	allocList[x, y].allocatedRect.Width = 
//        			    	 	((TableTag)allocList[x,y].allocatedTag).state.width / 
//        			    		((TableTag)allocList[x,y].allocatedTag).colSpan;        			    	
//        			    }
//        			}
//        			    
//        			
//        			
//        		}
//        	}
//        	#endregion
//        	
//        	#region Re-calculate colWidth based on maxColWidth
//        	float[] maxAllocatedWidth = new float[maxCol];
//        	float[] maxAllocatedRatio = new float[maxCol];
//        	float maxUnlockedWidth = 0;
//        	float maxLockedWidth = 0;
//        	Int32 maxUnlockedCol = 0;
//        	                                      
//        	for (Int32 x = 0; x < maxCol; x++)
//        	{
//        		if (!lockCol[x])
//        		{
//        			maxUnlockedWidth += maxColWidth[x];
//        			maxAllocatedWidth[x] = maxColWidth[x];
//        			maxUnlockedCol += 1;
//        		} else
//        		{
//        			maxLockedWidth += maxColWidth[x];	
//        			maxAllocatedWidth[x] = maxColWidth[x];
//        		}
//        	}
//        	
//        	if (maxUnlockedWidth + maxLockedWidth > WidthLimit())
//        	{
//        		float avaliableWidth = WidthLimit() - maxLockedWidth;
//        		for (Int32 x = 0; x < maxCol; x++)
//        		{
//        			if (!lockCol[x])
//        			{
//        				maxAllocatedRatio[x] = maxColWidth[x] / maxUnlockedWidth;
//        				maxAllocatedWidth[x] = avaliableWidth * maxAllocatedRatio[x];
//        			}        			        			        			
//        		}
//        		
//				//TODO: Handle with situation that maxLockedWidth > ownerTag.spaceAlloc.Width
//        	}
//        	
//        	#endregion
//        	
//        	#region Call Table cells.update with new width
//        	
//        	
//        	#endregion
//        	
//        	
//        }
//        
//    }
}

using System;
using System.Drawing;
using System.Diagnostics;
using System.Text;

namespace QuickZip.MiniHtml
{
	/// <summary>
	/// One way to locate position from MiniHtmlCursor
	/// </summary>
	public class CursorPosition : IComparable
	{
		public Int32 tagIndex, tagLine, tagChar;
		public CursorPosition(Int32 aTagIndex, Int32 aTagLine, Int32 aTagChar)
		{
			tagIndex = aTagIndex;
			tagLine = aTagLine;
			tagChar = aTagChar;
		}		
		public int CompareTo(object item)
		{
			CursorPosition Compare = (CursorPosition)item;
			if (tagIndex != Compare.tagIndex) return Compare.tagIndex - tagIndex;
			if (tagLine != Compare.tagLine) return Compare.tagLine - tagLine;
			return Compare.tagChar - tagChar;
		}
	}
    /// <summary>
    /// Represent startCursor or endCursor
    /// </summary>
    public class MiniHtmlCursor
    {
        internal MiniHtml parentHtml;                           //Parent MiniHtml
        public PointF position;                                 //Position of selection
        internal HtmlTag selectedTag;                           //Tag selected
        public Int32 selectedLine, selectedChar;                //Selected line and char if TextTag        
        /// <summary>
        /// constructor
        /// </summary>        
        public MiniHtmlCursor(MiniHtml mh)
        {
            parentHtml = mh;
            SetCursor();
        }   
        /// <summary>
        /// set Cursor position
        /// </summary>
        public void SetCursor()
        {
            selectedTag = parentHtml.masterTag;
            selectedLine = -1;
            selectedChar = -1;
            position = new PointF(0, 0);
        }
        /// <summary>
        /// set Cursor position
        /// </summary>
        public void SetCursor(PointF aPos)
        {
            position = aPos;
            selectedTag = parentHtml.documentOutput.GetFocusText(aPos.X, aPos.Y, ref selectedLine, ref selectedChar);
            if (selectedTag == null)
            	selectedTag = parentHtml.documentOutput.GetFocusTag(aPos.X, aPos.Y);
        }
        /// <summary>
        /// set Cursor position
        /// </summary>
        internal void SetCursor(HtmlTag aTag, Int32 aChar)
        {
            selectedTag = aTag;
            selectedLine = -1;
            selectedChar = aChar;
            position = parentHtml.documentOutput.GetFocusPosition(aTag, out selectedLine, ref selectedChar);
        }
        /// <summary>
        /// set Cursor position
        /// </summary>
        internal void SetCursor(HtmlTag aTag, Int32 aLine, Int32 aChar)
        {
            selectedTag = aTag;
            selectedLine = aLine;
            selectedChar = aChar;
            position = parentHtml.documentOutput.GetFocusPosition(aTag, selectedLine, selectedChar);
        }
        /// <summary>
        /// set Cursor position
        /// </summary>
        public void SetCursor(Int32 aTagIndex, Int32 aLine, Int32 aChar)
        {
        	if (parentHtml.tagPositionList.Count > aTagIndex)
        	{
        		selectedTag = parentHtml.tagPositionList[aTagIndex];
        		selectedLine = aLine;
        		selectedChar = aChar;
            	position = parentHtml.documentOutput.GetFocusPosition(selectedTag, selectedLine, selectedChar);
        	}
        }
        /// <summary>
        /// set Cursor position
        /// </summary>
        public void SetCursor(CursorPosition cur)
        {
        	SetCursor(cur.tagIndex, cur.tagLine, cur.tagChar);
        }
        /// <summary>
        /// update cursor position
        /// </summary>
        public void UpdatePosition()
        {
        	position = parentHtml.documentOutput.GetFocusPosition(selectedTag, selectedLine, selectedChar);
        }
        /// <summary>
        /// return position of tag in parentHtml.tagPositionList
        /// </summary>
        public Int32 TagPosition()
        {
            return parentHtml.tagPositionList.IndexOf(selectedTag);
        }
        /// <summary>
        /// return if the selection is valid
        /// </summary>        
        private bool isSelected()
        {
            if (selectedTag != null)
                if (selectedTag is TextTag)
                {return ((selectedLine != -1) && (selectedChar != -1));}
                else
                {return true;}
            return false;
        }
        /// <summary>
        /// return if the selection is valid
        /// </summary>        
        public bool Selected
        {
        	get
        	{
        		return isSelected();
        	}
        	set
        	{
        		selectedTag = null;
        		selectedLine = -1;
        		selectedChar = -1;
        	}
        }
        /// <summary>
        /// update selection between startCursor and endCursor
        /// </summary>
        public static void UpdateSelected(MiniHtmlCursor startCursor, MiniHtmlCursor endCursor, bool Changed)
        {
            Int32 startPos = startCursor.TagPosition();
            Int32 endPos = endCursor.TagPosition();
            
            if (endPos > startPos)
            {
            	Int32 temp = startPos;
            	startPos = endPos;
            	endPos = startPos;
            }
            	
            if ((endPos == -1) || (startPos == -1))
            {return;}
            for (int i = startPos; i <= endPos; i++)
                startCursor.parentHtml.tagPositionList[i].needRedraw = Changed;
        }        
    	/// <summary>
        /// return a cursor that point to the tag left side of current
        /// </summary>
        public MiniHtmlCursor LeftShift(bool fullTag)
        {
        	MiniHtmlCursor retVal = new MiniHtmlCursor(parentHtml);
        	
        	if ((selectedTag is TextTag) && !(fullTag))
        	{
        		TextTag currentTag = (TextTag)(selectedTag);
        		Int32 pos = currentTag.LocateText(selectedLine, selectedChar);
        		if (pos <= 0)
        			fullTag = true; 
        		else
        		{
        			Int32 aLine = 0, aChar = pos -1;        			
        			currentTag.LocateText(out aLine, ref aChar);
        			retVal.SetCursor(selectedTag, aLine, aChar);
					return retVal;        			
        		}
        	}
        	
        	HtmlTag prevTag = selectedTag.prevTag();
        	if (prevTag == null)
        		return null;
        	else
        	{
        		if (!(prevTag is TextTag))
        			retVal.SetCursor(prevTag, -1);
        		else
        		{        			
        			retVal.SetCursor(prevTag, ((TextTag)(prevTag)).text.Length);
        		}
        	}
        	return retVal;
        }        
        /// <summary>
        /// return a cursor that point to the tag right side of current
        /// </summary>
        public MiniHtmlCursor RightShift(bool fullTag)
        {
            MiniHtmlCursor retVal = new MiniHtmlCursor(parentHtml);
        	
        	if ((selectedTag is TextTag) && !(fullTag))
        	{
        		TextTag currentTag = (TextTag)(selectedTag);
        		Int32 pos = currentTag.LocateText(selectedLine, selectedChar);
        		if (pos >= currentTag.text.Length)
        			fullTag = true; 
        		else
        		{
        			Int32 aLine = 0, aChar = pos +1;        			
        			currentTag.LocateText(out aLine, ref aChar);
        			retVal.SetCursor(selectedTag, aLine, aChar);
					return retVal;        			
        		}
        	}
        	
        	HtmlTag nextTag = selectedTag.nextTag();
        	if (nextTag == null)
        		return null;
        	else
        	{
        		if (!(nextTag is TextTag))
        			retVal.SetCursor(nextTag, -1);
        		else
        		{        			
        			retVal.SetCursor(nextTag, 0);
        		}
        	}
        	return retVal;
        }
        
        private static void leftShiftVisible(ref MiniHtmlCursor shiftCursor)
        {
        	do 
        		shiftCursor = shiftCursor.LeftShift(false);
        	while ((shiftCursor.selectedTag.TagIndex() > -1) && !(shiftCursor.selectedTag is VisibleTag));        	        	
        }
        private static void rightShiftVisible(ref MiniHtmlCursor shiftCursor)
        {
        	do        		
        		shiftCursor = shiftCursor.RightShift(false);
        	while ((shiftCursor.selectedTag.TagIndex() < shiftCursor.parentHtml.tagPositionList.Count ) && !(shiftCursor.selectedTag is VisibleTag));
        }
        /// <summary>
        /// Is the cursor pointing to last char of currentTag?
        /// </summary>
        public static bool isLastChar(MiniHtmlCursor cursor)
        {
        	if (cursor.selectedTag is TextTag)
        		return ((TextTag)(cursor.selectedTag))[cursor.selectedLine].Length == cursor.selectedChar;
        	return false;
        }
        /// <summary>
        /// Is the cursor pointing to last tag of current Line?
        /// </summary>
        public static bool isLastTag(MiniHtmlCursor cursor)
        {
        	if (cursor.selectedTag is TextTag)
        		return ((TextTag)(cursor.selectedTag)).AllocatedLine(cursor.selectedLine).LastTag() != cursor.selectedTag;
        	return false;
        }
        /// <summary>
        /// Return a cursor that point to the last visible tag
        /// Ignore non-Visible tag and last character of TextTag (except if it's last tag in current line)
        /// </summary>
        public MiniHtmlCursor LeftShiftText()
        {
        	if (selectedTag.TagIndex() < 0 ) return null;
        	
        	MiniHtmlCursor currentCursor = new MiniHtmlCursor(parentHtml);
        	currentCursor.SetCursor(CursorPosition());
        	MiniHtmlCursor tempCursor = new MiniHtmlCursor(parentHtml);        	
        	MiniHtmlCursor.leftShiftVisible(ref currentCursor);
        		
        	if (currentCursor.selectedTag is TextTag) 
        	{
        		bool shiftAgain = true;
        		TextTag currentTag = (TextTag)(currentCursor.selectedTag);
        		tempCursor.SetCursor(currentCursor.CursorPosition());
        		try
        		{
        			MiniHtmlCursor.leftShiftVisible(ref tempCursor);	
        		}
        		catch
        		{
        			if (currentCursor == null)
        				currentCursor.SetCursor(CursorPosition());        			
        		}
        		
        		        		
        		if (tempCursor != null) 
        		{       			
        			shiftAgain = (MiniHtmlCursor.isLastChar(currentCursor) &&
        			              MiniHtmlCursor.isLastTag(tempCursor)) &&
        						  !(currentCursor.selectedTag.nextTag() is ElementTag);
        				
        			if (shiftAgain)
        				return tempCursor;
        		}
        	}
        	
        	return currentCursor;        		
        }
        /// <summary>
        /// Return a cursor that point to the next visible tag
        /// Ignore non-Visible tag and last character of TextTag (except if it's last tag in current line)
        /// </summary>                
        public MiniHtmlCursor RightShiftText()
        {        	        	
        	MiniHtmlCursor currentCursor = new MiniHtmlCursor(parentHtml);
        	currentCursor.SetCursor(CursorPosition());
        	try 
        	{
        		MiniHtmlCursor.rightShiftVisible(ref currentCursor);
        	}
        	catch
        	{
        		currentCursor.SetCursor(CursorPosition());
        	}
        	        	
        	if (currentCursor.selectedTag is TextTag) 
        	{
        		bool shiftAgain = true;

        		shiftAgain = MiniHtmlCursor.isLastChar(currentCursor) &&
        				MiniHtmlCursor.isLastTag(currentCursor) &&
        			!(currentCursor.selectedTag.nextTag() is ElementTag);
        				
        		if (shiftAgain)
        			MiniHtmlCursor.rightShiftVisible(ref currentCursor);
        	}
        		
        	return currentCursor;
        }
        /// <summary>
        /// Return current line height
        /// </summary>
        public float LineHeight()
        {
        	if ((selectedTag != null) && (selectedTag is VisibleTag))
        	{
        		VisibleTag currentTag = (VisibleTag)(selectedTag);        		
        		return currentTag.LastSpaceAllocator().lineInfo[selectedLine].lineHeight;
        	}
        	return 0;
        }
        /// <summary>
        /// Return URL of selection
        /// </summary>        
        public string TargetURL()
        {
        	if ((selectedTag != null) && (selectedTag is VisibleTag))
        	{
        		VisibleTag currentTag = (VisibleTag)(selectedTag);
        		return currentTag.targetURL;        		
        	}
        	return "";
        }
        /// <summary>
        /// Return information related to current selection
        /// </summary>        
        public string Info()
        {
        	string retVal = '(' + this.position.X.ToString() + ',' + this.position.Y.ToString() + ") ";
        	if ((selectedTag != null) && (selectedTag is TextTag))
        	{
                retVal += ((TextTag)selectedTag)[selectedLine] + '-' + selectedChar.ToString();
        	}
            else
            	if (selectedTag != null)
            		retVal += selectedTag.name;
            return retVal;
        }
        
        public CursorPosition CursorPosition()
        {
        	if (selectedTag == null)
        		return new CursorPosition(0, selectedLine, selectedChar);
        	return new CursorPosition(selectedTag.TagIndex(), selectedLine, selectedChar);
        }
    }
    /// <summary>
    /// Contain both startCursor and endCursor
    /// </summary>
    public class Selection
    {
    	internal MiniHtml parentHtml;
        public MiniHtmlCursor startCursor, endCursor;           //Start Cursor and End Cursor
        /// <summary>
        /// Constructor
        /// </summary>        
        public Selection(MiniHtml mh)
        {
            startCursor = new MiniHtmlCursor(mh);
            endCursor = new MiniHtmlCursor(mh);
            parentHtml = mh;
        }
        public MiniHtmlCursor firstCursor
        {
        	get
        	{
        		if (startCursor.CursorPosition().CompareTo(endCursor.CursorPosition()) > 0)
        		    return startCursor;
        		return endCursor;
        	}
        	set
        	{
        		if (startCursor.CursorPosition().CompareTo(endCursor.CursorPosition()) > 0)
        		    startCursor = value;
        		else endCursor = value;
        	}
        }
        public MiniHtmlCursor secondCursor
        {
        	get
        	{
        		if (startCursor.CursorPosition().CompareTo(endCursor.CursorPosition()) <= 0)
        		    return startCursor;
        		return endCursor;
        	}
        	set
        	{
        		if (startCursor.CursorPosition().CompareTo(endCursor.CursorPosition()) <= 0)
        		    startCursor = value;
        		else endCursor = value;
        	}

        }
        /// <summary>
        /// Return if aTag is between startCursor and endCursor
        /// </summary>
        internal bool isBetween(HtmlTag aTag)
        {
            Int32 startPos = firstCursor.TagPosition();
            Int32 endPos = secondCursor.TagPosition();
            if ((endPos < startPos) || (endPos == -1) || (startPos == -1))
            { return false; }
            for (int i = startPos; i <= endPos; i++)
                if (startCursor.parentHtml.tagPositionList[i] == aTag)
                    return true;
            return false;
        }
        /// <summary>
        /// Return if startCursor = endCursor
        /// </summary>        
        private bool isCursorMode()
        {
            bool retVal = startCursor.Selected && endCursor.Selected;
            if (retVal)
                retVal = ((startCursor.selectedTag == endCursor.selectedTag) &&
                            (startCursor.selectedLine == endCursor.selectedLine) &&
                            (startCursor.selectedChar == endCursor.selectedChar));
            return retVal;
        }
        /// <summary>
        /// Return if startCursor = endCursor
        /// </summary>
        public bool CursorMode
        {
        	get
        	{
        		return isCursorMode();
        	}
        	set
        	{
        		if (value)
        			SetStartCursor(endCursor.selectedTag, 
        			               endCursor.selectedLine, endCursor.selectedChar);
        	}        	
        }
        /// <summary>
        /// Return if startCursor != endCursor
        /// </summary>        
        private bool isSelectionMode()
        {
            bool retVal = startCursor.Selected && endCursor.Selected;
            if (retVal)
                retVal = ((startCursor.selectedTag != endCursor.selectedTag) ||
                            (startCursor.selectedLine != endCursor.selectedLine) ||
                            (startCursor.selectedChar != endCursor.selectedChar));
            return retVal;
        }
        /// <summary>
        /// Return if startCursor != endCursor
        /// </summary>
        public bool SelectionMode
        {
        	get
        	{
        		return isSelectionMode();
        	}      	
        }
        /// <summary>
        /// Update selection between startCursor and endCursor
        /// </summary>        
        public void UpdateSelected(bool Changed)
        {
            MiniHtmlCursor.UpdateSelected(firstCursor, secondCursor, Changed);
        }
        /// <summary>
        /// Set Start Cursor position
        /// </summary>
        public void SetStartCursor()
        {
            startCursor.SetCursor();
        }
        /// <summary>
        /// Set Start Cursor position
        /// </summary>
        public void SetStartCursor(PointF aPos)
        {
            startCursor.SetCursor(aPos);
        }
        /// <summary>
        /// Set Start Cursor position
        /// </summary>
        internal void SetStartCursor(HtmlTag aTag, Int32 aChar)
        {
            startCursor.SetCursor(aTag, aChar);
        }
        /// <summary>
        /// Set Start Cursor position
        /// </summary>
        internal void SetStartCursor(HtmlTag aTag, Int32 aLine, Int32 aChar)
        {
            startCursor.SetCursor(aTag, aLine, aChar);
        }
        /// <summary>
        /// Set Start Cursor position
        /// </summary>
        public void SetStartCursor(CursorPosition cur)
        {
        	startCursor.SetCursor(cur);
        }
        /// <summary>
        /// Set End Cursor position
        /// </summary>
        public void SetEndCursor()
        {
            endCursor.SetCursor();
        }
        /// <summary>
        /// Set End Cursor position
        /// </summary>
        public void SetEndCursor(PointF aPos)
        {
            endCursor.SetCursor(aPos);
        }
        /// <summary>
        /// Set End Cursor position
        /// </summary>
        internal void SetEndCursor(HtmlTag aTag, Int32 aChar)
        {
            endCursor.SetCursor(aTag, aChar);
        }
        /// <summary>
        /// Set End Cursor position
        /// </summary>
        internal void SetEndCursor(HtmlTag aTag, Int32 aLine, Int32 aChar)
        {
            endCursor.SetCursor(aTag, aLine, aChar);
        }
        /// <summary>
        /// Set End Cursor position
        /// </summary>
        public void SetEndCursor(CursorPosition cur)
        {
        	startCursor.SetCursor(cur);
        }
        /// <summary>
        /// Remove selected items.
        /// </summary>
        public void RemoveSelection(bool TextOnly)
        {        	
        	if ((startCursor.selectedTag == null) || (endCursor.selectedTag == null))
        		return;
        	
        	MiniHtmlCursor firstCur = new MiniHtmlCursor(parentHtml);
        	MiniHtmlCursor secondCur = new MiniHtmlCursor(parentHtml);
        	firstCur.SetCursor(firstCursor.selectedTag, firstCursor.selectedLine, firstCursor.selectedChar);
        	secondCur.SetCursor(secondCursor.selectedTag, secondCursor.selectedLine, secondCursor.selectedChar);
        	
//        	if (!(startCursor.selectedTag is TextTag) || !(endCursor.selectedTag is TextTag))
//        		return;
        	        	
        	if (firstCur.selectedTag != secondCur.selectedTag)
      		{        		        
        		if (firstCur.selectedTag is TextTag)
        			((TextTag)(secondCur.selectedTag)).ReplaceText(firstCur, secondCursor, "");        			
        		secondCur = secondCur.LeftShift(true);
        	}
        		
        	while (secondCur.selectedTag.TagIndex() > firstCur.selectedTag.TagIndex())
        	{
        		HtmlTag currentTag = secondCur.selectedTag;
        		secondCur = secondCur.LeftShift(true);
        		if ((currentTag is TextTag) || (!(TextOnly) && (currentTag is RegionTag)))
        		{
//        			Console.WriteLine("removing " + currentTag.name);
//        			if (currentTag is TextTag) Console.WriteLine(((TextTag)currentTag).text);
        			currentTag.RemoveSelf();        		        		
        		}
        	}
        	        	
        	if (firstCur.selectedTag is RegionTag)
        	{
        		HtmlTag currentTag = firstCur.selectedTag;
        		
        		firstCur = firstCur.RightShiftText();
        			
        		SetEndCursor(firstCur.selectedTag,0);   
        		
        		currentTag.RemoveSelf();         		
        	}
        	
        	if ((firstCur.selectedTag is TextTag) && (firstCur.selectedTag == secondCur.selectedTag))
        	{        	
        		TextTag currentTag = (TextTag)firstCur.selectedTag;
        		Int32 sPos = currentTag.LocateText(firstCur.selectedLine, 
        		        		firstCur.selectedChar);
        		currentTag.ReplaceText(firstCur, secondCur, "");  
        		        		
        		parentHtml.Invalidate(); 
        		parentHtml.documentOutput.Update();
        		        		
        		SetStartCursor(currentTag, sPos );    
        		SetEndCursor(startCursor.selectedTag, startCursor.selectedLine, startCursor.selectedChar);        		
        	}        	        	        		        		        		        	        	
        }
        
        /// <summary>
        /// Remove selected items.
        /// </summary>
        public void RemoveSelection()
        {
        	RemoveSelection(true);
        }
        
        /// <summary>
        /// Insert text to current cursor.
        /// </summary>
        public void InsertText(string aText)
        {
        	if ((startCursor.selectedTag is TextTag) &&(startCursor.selectedTag == endCursor.selectedTag))
        	{        	
        		TextTag currentTag = (TextTag)startCursor.selectedTag;
        		Int32 sPos = currentTag.LocateText(firstCursor.selectedLine, 
        		        		firstCursor.selectedChar);        		
        		currentTag.ReplaceText(firstCursor, secondCursor, aText);  
        		
        		parentHtml.documentOutput.Update();
        		
        		SetStartCursor(currentTag, sPos + aText.Length);   
        		SetEndCursor(currentTag, startCursor.selectedLine, startCursor.selectedChar);   
        	}
        }
        
        /// <summary>
        /// Replace selection to specified text
        /// </summary>
        private void replace(bool textOnly, string aText)
        {
        	if ((startCursor.selectedTag == null) || (endCursor.selectedTag == null))
        		return;
        	
        	if (startCursor.selectedTag != endCursor.selectedTag)
      		{        		
        		((TextTag)(secondCursor.selectedTag)).ReplaceText(firstCursor, secondCursor, "");        			
        		endCursor = endCursor.LeftShift(true);
        	}
        		
        	while (secondCursor.selectedTag.TagIndex() > firstCursor.selectedTag.TagIndex())
        	{
        		HtmlTag currentTag = secondCursor.selectedTag;
        		secondCursor = secondCursor.LeftShift(true);
        		if ((currentTag is TextTag) || !(textOnly))
        			currentTag.RemoveSelf();
        	}
        	
        	if ((startCursor.selectedTag is TextTag) &&(startCursor.selectedTag == endCursor.selectedTag))
        	{        	
        		TextTag currentTag = (TextTag)startCursor.selectedTag;
        		Int32 sPos = currentTag.LocateText(startCursor.selectedLine, 
        		        		startCursor.selectedChar);
        		currentTag.ReplaceText(firstCursor, secondCursor, aText);  
        		
        		parentHtml.documentOutput.Update();
        		
        		SetStartCursor(currentTag, sPos + aText.Length);
        		SetEndCursor(currentTag, startCursor.selectedLine, startCursor.selectedChar);      		
        	}        	
        }    
        /// <summary>
        /// Replace text only
        /// </summary>
        public void ReplaceText(string aText)
        {
        	RemoveSelection();
        	InsertText(aText);
     
        }
        /// <summary>
        /// Replace everything
        /// </summary>
        public void ReplaceAll(string aText)
        {
        	replace(false, aText);
        }
        
        /// <summary>
        /// remove previous removable element.
        /// </summary>
        public void BackSpace()
        {
        	if (!(CursorMode)) return;
			firstCursor = firstCursor.LeftShiftText();	
			CursorPosition cur = firstCursor.CursorPosition();
        	RemoveSelection(false);        
        	
        	parentHtml.parser.Parse(parentHtml.Html());
        	parentHtml.documentOutput.Update(); 
            SetStartCursor(cur);
            SetEndCursor(startCursor.selectedTag, startCursor.selectedLine, startCursor.selectedChar);
        }                
        
        /// <summary>
        /// remove next removable element.
        /// </summary>
        public void Delete()
        {
        	if (!(CursorMode)) { ReplaceText(""); return; }
        	if (firstCursor.selectedTag.TagIndex() >= parentHtml.tagPositionList.Count ) return;
        	
        	CursorPosition cur = secondCursor.CursorPosition();
        	secondCursor = secondCursor.RightShiftText();    
        	
        	RemoveSelection(false);
        	
        	parentHtml.parser.Parse(parentHtml.Html());
        	parentHtml.documentOutput.Update(); 
            SetStartCursor(cur);
            SetEndCursor(startCursor.selectedTag, startCursor.selectedLine, startCursor.selectedChar);
        }
        
        /// <summary>
        /// remove and return text after cursor 
        /// </summary>
        /// <returns></returns>
        public string SplitText()
        {
        	if (!(firstCursor.selectedTag is TextTag)) return "";
        	if (!(CursorMode)) return "";
        	
        	TextTag currentTag = (TextTag)(firstCursor.selectedTag);
        	
        	string beforeSplit = currentTag.Html(true, true, false, false);
        	string afterSplit = currentTag.Html(false, false, true, false);
			       	
        	currentTag.text = beforeSplit;
        	return afterSplit;        	        	 	             	
        }
        
        /// <summary>
        /// insert a linebreak at cursor
        /// </summary>        
        public void InsertBreak(bool paragraph)
        {        	        	        	
        	//TODO: Make paragraph InsertBreak not require full reload.
        	if (paragraph)
        	{			
				CursorPosition cur = firstCursor.CursorPosition();
//				string seperator = "<p>";				
//				if (firstCursor.selectedTag.HaveLastTagNamed("p"))
//					seperator = "</p><p>";
				
				string seperator = firstCursor.selectedTag.LastRegionTag().name;
				if (firstCursor.selectedTag.HaveLastTagNamed(seperator))
					seperator = String.Format("</{0}><{0}{1}> ",seperator, firstCursor.selectedTag.LastRegionTag().variables.Html());
				else
					seperator = String.Format("<{0}> ",seperator);
					
				string BeforeSel = parentHtml.HtmlBeforeSelection();
				string AfterSel = parentHtml.HtmlAfterSelection();
				
				parentHtml.parser.Parse(BeforeSel + seperator + AfterSel);
                parentHtml.documentOutput.Update(); 
                SetStartCursor(cur);
                startCursor = startCursor.RightShiftText();
                SetEndCursor(startCursor.selectedTag, startCursor.selectedLine, startCursor.selectedChar);    
        	}
        	else
        	{
        		TextTag txtTag = new TextTag(parentHtml, SplitText());
        		//if (txtTag.text == "") return;
        		firstCursor.selectedTag.parentTag.AddTag(firstCursor.selectedTag.ParentTagIndex()+1,
        		                                         txtTag);
        		
        		Int32 tagID = Utils.LocateTag(Utils.RemoveFrontSlash("br"));
        		RegionTag newTag = new RegionTag(parentHtml, "br", new PropertyList(), tagID, 0);
        		firstCursor.selectedTag.parentTag.AddTag(firstCursor.selectedTag.ParentTagIndex()+1, 
        	    	                                     newTag);   
        		parentHtml.Invalidate();
        		parentHtml.documentOutput.Update();  
        		
        		SetStartCursor(txtTag, 0);        		
        		SetEndCursor(startCursor.selectedTag, startCursor.selectedLine, startCursor.selectedChar);    
        	}
        	
        }
        
        
        public void LeftShift()
        {
        	if (endCursor.selectedTag.TagIndex() < 0 ) return;
 			
        	try
        	{
			startCursor = startCursor.LeftShiftText();
        	endCursor.SetCursor(startCursor.selectedTag, startCursor.selectedLine,
        	                    startCursor.selectedChar);
        	}
        	catch { }
        }
        
        public void RightShift()
        {
        	if (endCursor.selectedTag.TagIndex() >= parentHtml.tagPositionList.Count ) return;
        	
        	try
        	{
			endCursor = endCursor.RightShiftText();
        	startCursor.SetCursor(endCursor.selectedTag, endCursor.selectedLine,
        	                    endCursor.selectedChar);
        	}
        	catch { }
        }
                
    }
}

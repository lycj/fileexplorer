using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace QuickZip.MiniHtml
{
    /// <summary>
    /// Represent an element (Text/Tag) in Html code
    /// </summary>
    internal class HtmlTag
    {
        public Int32 id;                                        //HtmlTag ID based on BuiltinTags in Defines
        public string name;                                     //HtmlTag name without <>
        public Int32 level;                                     //Default tag level (e.g. td should have lower level than table)
        public PropertyList variables = new PropertyList();     //Variable List and values        
        public bool lineBreakNow, lineBreakAfter;               //Do a linebreak now or after this tag
        public bool mustCompleteTag;                            //Will keep adding to child tag till endTag found.      
        public HtmlTag parentTag;                               //Parent HtmlTag    
        public MiniHtml parentHtml;                             //Reference main component
        public TagList childTags = new TagList();               //A List of tags inside
//        public float baseX, baseY;                              //Record it's current location when drawing
        public bool requireUpdate = true;                       //Require update or not.        
        private SpaceAllocator lastCacheSpaceAllocator = null;  //Cache it when first use lastSpaceAllocator
        private RegionTag lastCacheRegionTag = null;            //Cache it when first use lastRegionTag        

        internal bool pNeedUpdate = true;                       //Need update?
        public bool needUpdate
        {
            get { return pNeedUpdate; }
            set { pNeedUpdate = value; 
                if ((!(parentTag == null)) && (value == true)) { parentTag.needUpdate = value; } 
                if (value) 
                { lastCacheSpaceAllocator = null; lastCacheRegionTag = null;}
            	}	
        }
        internal bool pNeedRedraw;                              //Need redraw?
        public bool needRedraw
        {
            get { return pNeedRedraw; }
            set { pNeedRedraw = value; if ((!(parentTag == null)) && (value == true)) { parentTag.needRedraw = value; } }
        }

        /// <summary>
        /// Return last parent tag with specified name.
        /// </summary>
        public HtmlTag LastTagNamed(string aTagName)
        {
            HtmlTag aTag = parentTag;
            while ((aTag != null) && (!(aTag.name == aTagName)))
            { aTag = aTag.parentTag; }

            if (aTag == null)
            { return null; }
            else
                return aTag;
        }
        public bool HaveLastTagNamed(string aTagName)
        {
        	return (LastTagNamed(aTagName) != null);
        }
        /// <summary>
        /// Return last parent region tag.
        /// </summary>        
        public RegionTag LastRegionTag()
        {
            if (this is MasterTag) { return (RegionTag)this; }
            if (lastCacheRegionTag == null)
            {
                HtmlTag aTag = parentTag;
                while ((aTag != null) && (!(aTag is RegionTag)))
                { aTag = aTag.parentTag; }

                if (aTag == null)
                { lastCacheRegionTag = null; }
                else
                    lastCacheRegionTag =(RegionTag)(aTag);
            }
            return lastCacheRegionTag;
        }
        /// <summary>
        /// Return spaceAlloc owned by last parent region tag.
        /// Will cache to lastCacheSpaceAllocator once run once.
        /// </summary>        
        public SpaceAllocator LastSpaceAllocator()
        {
            if (lastCacheSpaceAllocator == null)
            {
                RegionTag rt = LastRegionTag();
                if (rt != null)
                { lastCacheSpaceAllocator = rt.spaceAlloc; }
            }
            return lastCacheSpaceAllocator;
        }

        /// <summary>
        /// Initialite procedure, can be used by child tags.
        /// </summary>
        protected void init(MiniHtml aMiniHtml, string aName, PropertyList aVariables, Int32 aID, Int32 aLevel)
        {            
            parentHtml = aMiniHtml;            
            name = aName.ToLower();
            if (aVariables == null)
                variables = new PropertyList();
            else
                variables = aVariables;
            id = Math.Abs(aID);
            level = Math.Abs(aLevel);

            lineBreakNow = false;
            lineBreakAfter = false;
            mustCompleteTag = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlTag()
        {
            //Console.WriteLine("Warning : Constructor HtmlTag() is for debug only");
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlTag(MiniHtml aMiniHtml, string aName, PropertyList aVariables, Int32 aID, Int32 aLevel)
        {
            init(aMiniHtml, aName, aVariables, aID, aLevel);
        }
        /// <summary>
        /// Indicate if it is a startTag. e.g. <B>
        /// </summary>
        public bool StartTag()
        {
            return !(name.StartsWith("/"));
        }
        /// <summary>
        /// Indicate if it is an endTag. e.g. </B> or <Image />
        /// </summary>
        public bool EndTag()
        {
            return ((name.IndexOf('/') == 0) ||(variables.Contains('/')));
        }

        public Int32 TagIndex()
        {
            return parentHtml.tagPositionList.IndexOf(this);
        }
        
        public Int32 ParentTagIndex()
        {
        	if (parentTag != null)
        	{
        		Int32 idx = parentTag.childTags.IndexOf(this);
        		if (idx != -1)
        			return idx;
        	}        		
        	return 0;
        }
        /// <summary>
        /// Used by AddTag()
        /// </summary>
        private HtmlTag doAddChild(Int32 index, HtmlTag value)
        {
            value.parentTag = this;
//            if (index == -1)
//            	parentHtml.tagPositionList.Add(value);
//            else
//            	parentHtml.tagPositionList.Insert(childTags[index].TagIndex(), value);
            
            childTags.Insert(index, value);  
            if (value.id == 0)
                return this;
            else
                return value;
        }
        /// <summary>
        /// Used by AddTag()
        /// </summary>
        private HtmlTag haveParentTagNamed(string aName)
        {
            HtmlTag aTag = parentTag;
            while ((aTag != null) && (aTag.name.Equals(aName)))
            {
                aTag = aTag.parentTag;
            }
            if (aTag != null)
            {
                if (aTag.name.Equals(aName))
                    return aTag;
            }
            return null;
        }
        /// <summary>
        /// Add the tag to child if value.tagLevel is lower then currentTag's
        /// Otherwise look for parent tag that can hold value.
        /// Some exception, e.g. TableTag and ListTag 
        /// </summary>
        public HtmlTag AddTag(HtmlTag value)
        {
        	return AddTag(-1, value);
        }
        public HtmlTag AddTag(Int32 index, HtmlTag value)
        {
        	Int32 pIndex = ParentTagIndex();
        	if (index == -1) pIndex = -1;
        	
            if (value.name == '/' + name)
            {            	            		
                doAddChild(index, value);
                return parentTag;
            }
            else
            {
                HtmlTag aTag = haveParentTagNamed(Utils.RemoveFrontSlash(value.name));
                if (aTag != null)
                {
                    aTag.AddTag(value);
                    return parentTag;
                }
            }

            if (EndTag())
            	return parentTag.AddTag(pIndex, value);

            if (value.level < level)
                return doAddChild(index, value);

//            if ((this is ListTag) && (value is ListTag))
//                if (((ListTag)this).type == ListTag.tagType.list)
//                    if (((ListTag)value).type == ListTag.tagType.list)
//                    {
//                        return doAddChild(index, value);
//                    }
//            if ((value is TableTag) && (((TableTag)value).type == TableTag.tagType.table))
//                return doAddChild(index, value);

            return parentTag.AddTag(pIndex, value);
        }
        /// <summary>
        /// Return focused tag based on Space allocator info
        /// </summary>        
        public virtual HtmlTag GetFocusTag(ref float x,ref float y, ref Int32 extraInfo)
        {
            //x -= LastRegionTag().state.GetBorderSpace(fourSide._left, false);
            //y -= LastRegionTag().state.GetBorderSpace(fourSide._top, false);
            return this;            
        }
        /// <summary>
        /// Return focused tag based on Space allocator info
        /// </summary>
        public HtmlTag GetFocusTag(float x, float y)
        {
            float xx = x;
            float yy = y;
            Int32 e = 0;
            return GetFocusTag(ref xx, ref yy, ref e);
        }        

        /// <summary>
        /// Do the necessary update, e.g. Allocate space for text, Add/Remove a tag to stateTagList        
        /// </summary>        
        public virtual void Update(ref CurrentStateType curState)
        {
            pNeedRedraw = true;
            CurrentStateType.UpdateState(ref curState);
            	
            if (!(this is TextTag))
            	parentHtml.parentCss.ApplyCssStyle(this, ref curState, true);            	

            foreach (HtmlTag aTag in childTags)
                aTag.Update(ref curState);           
        }
        /// <summary>
        /// Update the included curState, , called from CurrentStateType.UpdateState()
        /// </summary>        
        public virtual void UpdateState(ref CurrentStateType curState)
        {      
        	parentHtml.parentCss.ApplyCssStyle(this, ref curState, false);
        }
        /// <summary>
        /// Remove itself from parentTags
        /// </summary>
        public void RemoveSelf(bool removeChilds)
        {        	
        	needUpdate = true;
        	//Remove related tags
        	if (StartTag() && !EndTag())
        	{
        		//Remove last related tag if found, or remove the one from childlist.
        		if ((prevTag() != null) && (prevTag().name == "/" + name))
        			prevTag().RemoveSelf(removeChilds);
        		else if (childTags.IndexOf("/" + name) != -1)
        			childTags[childTags.IndexOf("/" + name)].RemoveSelf(removeChilds);
        	}
        	
        	Int32 id = parentTag.childTags.IndexOf(this);
        	parentTag.childTags.Remove(this);        	
        	if (!(removeChilds))
        	{
        		//Add childTags to parentTag
        		for (Int32 i = childTags.Count - 1; i >= 0; i--)
        		{
        			parentTag.childTags.Insert(id, childTags[i]);
        			childTags[i].parentTag = parentTag;
        			childTags[i].needUpdate = true;
        		}   
        	}
        	else
        	{
        		//Remove childTags from tagPositionList
//        		foreach (HtmlTag tag in childTags)
//        		{
//        			parentHtml.tagPositionList.Remove(tag);
//        		}        		
        	}
        	//Remove Self from tagPositionList
//        	parentHtml.tagPositionList.Remove(this);        	
			parentHtml.RebuildTagPositionList();
        }                 
        /// <summary>
        /// Remove itself from parentTags
        /// </summary>
        public void RemoveSelf()
        {
        	RemoveSelf(false);
        }
        /// <summary>
        /// return previous tag based on tagPositionList
        /// </summary>
        public HtmlTag prevTag()
        {
        	if (TagIndex() > 0)
        		return parentHtml.tagPositionList[TagIndex()-1];
        	else
        		return null;        
        }
        /// <summary>
        /// return next tag based on tagPositionList
        /// </summary>        
        public HtmlTag nextTag()
        {
        	if (TagIndex() < parentHtml.tagPositionList.Count)
        		return parentHtml.tagPositionList[TagIndex()+1];
        	else
        		return null;        
        }
        /// <summary>
        /// toggle needUpdate of current and all childTags to true
        /// </summary>
        public void Invalidate()
        {
        	needUpdate = true;
        	foreach (HtmlTag item in childTags)
        	{
        		item.needUpdate = true;
        		item.Invalidate();
        	}
        }
        
        public Int32 ParentTagsCount()
        {
        	HtmlTag currentTag = this.parentTag;
        	Int32 retVal = 0;
        	
        	while (currentTag != null)
        	{
        		currentTag = currentTag.parentTag;
        		retVal += 1;
        	}
        	
        	return retVal;
        }
        /// <summary>
        /// return html syntax of current tag
        /// </summary>        
        public virtual string Html(bool beforeSelection, bool betweenSelection, bool afterSelection, bool aFormatted)
        {
        	string breakStr = "";
        	if ((aFormatted) && !(this.TagIndex() == 0) /*&& !(this.EndTag())*/)
        	{        		
        		breakStr = Defines.lineBreak;
        		Int32 count = ParentTagsCount();
        		for (Int32 i = 0; i < count-1; i++)
        			breakStr += Defines.formattedSpacing;
        	}
        	string retVal = breakStr;
        	
        	MiniHtmlCursor firstCursor = parentHtml.documentOutput.selection.firstCursor;
        	MiniHtmlCursor secondCursor = parentHtml.documentOutput.selection.secondCursor;
        	
        	bool retHtml = true;
        	if ((firstCursor.Selected) && (secondCursor.Selected))
        	{
        		retHtml = false;
        		        		
        		Int32 firstIndex = firstCursor.selectedTag.TagIndex();
        		Int32 secondIndex = secondCursor.selectedTag.TagIndex();
        		Int32 index = TagIndex();
        		
        		if (beforeSelection && (index < firstIndex)) retHtml = true;
        		else if (betweenSelection && ((index > firstIndex) && (index < secondIndex))) retHtml = true;
        		else if (afterSelection && (index > secondIndex)) retHtml = true;        		
        	}        	        	        	
        	
        	if (retHtml)
        	{
        		retVal += "<" + name;
        		for (Int32 i = 0; i < variables.Count; i++)
        		{
        			PropertyItemType item = variables[i];
        			retVal += " " + item.key + "=\"" + item.value + "\"";
        		}
        		retVal += ">";
        	}
        	
        	foreach (HtmlTag item in childTags)
        		retVal += item.Html(beforeSelection, betweenSelection, afterSelection, aFormatted);
        	
        	return retVal;
        }
        
        /// <summary>
        /// return bbcode syntax of current tag
        /// </summary>        
        public virtual string BBCode(bool beforeSelection, bool betweenSelection, bool afterSelection)
        {
        	string breakStr = "";
        	string retVal = breakStr;
        	
        	MiniHtmlCursor firstCursor = parentHtml.documentOutput.selection.firstCursor;
        	MiniHtmlCursor secondCursor = parentHtml.documentOutput.selection.secondCursor;
        	
        	bool retHtml = true;
        	if ((firstCursor.Selected) && (secondCursor.Selected))
        	{
        		retHtml = false;
        		        		
        		Int32 firstIndex = firstCursor.selectedTag.TagIndex();
        		Int32 secondIndex = secondCursor.selectedTag.TagIndex();
        		Int32 index = TagIndex();
        		
        		if (beforeSelection && (index < firstIndex)) retHtml = true;
        		else if (betweenSelection && ((index > firstIndex) && (index < secondIndex))) retHtml = true;
        		else if (afterSelection && (index > secondIndex)) retHtml = true;        		
        	}        	        	        	
        	
        	if (retHtml)
        	{
        		retVal += "[" + name;
        		if (variables.Contains("value"))
        			retVal += "=" + variables["value"].value;
        		retVal += "]";
        	}
        	
        	foreach (HtmlTag item in childTags)
        		retVal += item.BBCode(beforeSelection, betweenSelection, afterSelection);
        	
        	return retVal;
        }
        /// <summary>
        /// Debug use.
        /// </summary>
        internal static void DebugUnit()
        {
            MiniHtml mh = new MiniHtml();
            HtmlTag pMaster = new MasterTag(mh, "master", new PropertyList(), 1, 30);
            HtmlTag p1 = new HtmlTag(mh, "p1", new PropertyList(), 1, 15);
            HtmlTag p2 = new HtmlTag(mh, "p2", new PropertyList(), 1, 10);
            HtmlTag p1close = new HtmlTag(mh, "/p1", new PropertyList(), 1, 15);
            HtmlTag p4 = new HtmlTag(mh, "p4", new PropertyList(), 1, 10);

            pMaster.AddTag(p1).AddTag(p2).AddTag(p1close).AddTag(p4);
            pMaster.childTags.PrintItems();
        }

    }
    /// <summary>
    /// Represent an area
    /// </summary>
    internal class RegionTag : InvisibleTag
    {    
        internal enum tagType { unknown, head, title, body, paragraph, 
    		div, html, span, blockQuote, indent, linebreak, 
    		unorderedList, orderedList, listItem, centre}
        public tagType type = tagType.unknown;

        public hAlignType hAlign = hAlignType.Left;             //Align of this region
        public SpaceAllocator spaceAlloc;                       //Store the space allocation of childTags
        public CurrentStateType state;                          //Store current state                    
        
        public RectangleF DrawableRect(bool includePadding)
        {
            return state.DrawableRect(spaceAlloc.baseRect, includePadding);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public RegionTag() : base()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public RegionTag(MiniHtml aMiniHtml, string aName, PropertyList aVariables, Int32 aID, Int32 aLevel)
        {
            init(aMiniHtml, aName, aVariables, aID, aLevel);
            #region Assign Tag Type
            switch (Utils.RemoveFrontSlash(aName.ToLower()))
            {
                case "head":
                    type = tagType.head;
                    break;
                case "title":
                    type = tagType.title;
                    break;
                case "body":
                    type = tagType.body;
                    break;
                case "p":
                    type = tagType.paragraph;
                    lineBreakNow = true;
                    lineBreakAfter = true;
                    break;
                case "div":
                    type = tagType.div;
                    lineBreakNow = true;
                    lineBreakAfter = true;
                    break;
                case "html":
                    type = tagType.html;
                    break;
                case "span":
                    type = tagType.span;
                    break;
                case "blockquote":
                    type = tagType.blockQuote;
                    break;
                case "indent":
                    type = tagType.indent;
                    lineBreakNow = true;
                    lineBreakAfter = true;
                    break;
                case "br":
                	type = tagType.linebreak;
                	lineBreakNow = true;
                	break;
                case "ol":
                	type = tagType.orderedList;
                	lineBreakNow = true;
                    lineBreakAfter = true;
                	break;
                case "ul":
                	type = tagType.unorderedList;
                	lineBreakNow = true;
                    lineBreakAfter = true;
                	break;
                case "li":
                	type = tagType.listItem;
                	lineBreakNow = true;
                    lineBreakAfter = true;
                	break;
                case "centre":
                	type = tagType.centre;
                	lineBreakNow = true;
                    lineBreakAfter = true;
                	break;
            }
            #endregion
        }
        /// <summary>
        /// Return focused tag based on Space allocator info
        /// </summary>        
        public override HtmlTag GetFocusTag(ref float x,ref float y, ref Int32 extraInfo)
        {
            foreach (anAllocatedItem item in spaceAlloc.allocList)
                if (!(item.allocatedTag.EndTag()))
                {
                    RectangleF newRect = state.DrawableRect(spaceAlloc.baseRect, true);

                    float leftBorderSpace = state.GetBorderSpace(fourSide._left, false);
                    float xx = x; //- newRect.Left;
                    float yy = y;// - (spaceAlloc.lineInfo[item.currentLine].lineHeight - item.allocatedRect.Height);
                    extraInfo = item.extraInfo;
                    if (item.isIn(xx, yy))
                    {
                    	x = xx - item.actualRect().Left;
                        y = yy - item.actualRect().Top;
                        return item.allocatedTag.GetFocusTag(ref x, ref  y, ref  extraInfo);
                    }
                }
              //TODO: Handle GetFocusTag when Out of bounds.
//            if (spaceAlloc.allocList.Count > 0)
//            {
//                anAllocatedItem item = spaceAlloc.allocList[0];
//                RectangleF newRect = state.DrawableRect(spaceAlloc.baseRect, true);
//                float xx = x - newRect.Left;
//                float yy = y - (spaceAlloc.lineInfo[item.currentLine].lineHeight - item.allocatedRect.Height);
//                x = xx - item.allocatedRect.Left;
//                y = yy - item.allocatedRect.Top;
//                return item.allocatedTag.GetFocusTag(ref x, ref  y, ref  extraInfo);
//            }
            return this;
        }
        
        /// <summary>
        /// Used by Update, add current tag to curState and update it.
        /// </summary>   
        internal override void updateStateList(ref CurrentStateType curState)
    	{    	
    	    if (curState != null)
            {
                curState.RemoveTag(Utils.RemoveFrontSlash(name));
                if (StartTag())
                {
                    curState.AddTag(this);
                }
                
                state = curState.Clone();
                
                CurrentStateType.UpdateState(ref state);                   
                parentHtml.parentCss.ApplyCssStyle(this, ref state, true);
                
                if (variables.Contains("align")) 
        			hAlign = Utils.StrAlign2Align(variables["align"].value);
        			else hAlign = curState.hAlign;
        		if (type == tagType.centre) 
        			hAlign = hAlignType.Centre;
            }
            else
            {
                curState = state;
            }
    	}     
        	
        /// <summary>
        /// Update childTags, and allocate self to parent region spaceAllocaor
        /// </summary>        
        public override void Update(ref CurrentStateType curState)
        {        	               	
        	updateStateList(ref curState);

            #region Update self and child if pNeedUpdate            
            if (pNeedUpdate) 
            {
                pNeedUpdate = false;				
                spaceAlloc = new SpaceAllocator(this);     
                if (lineBreakNow) { LastSpaceAllocator().NewLine(); } 
                base.Update(ref curState);               
            }
            
			if (!state.visible) return;                        
			
            if ((spaceAlloc.ownerTag == this) && !(this.EndTag()))
                	spaceAlloc.FinishAllocate();
            #endregion
            
            #region Calculate Height and Width of current tag, and allocate            
            float w = spaceAlloc.Width() + state.GetBorderSpace(fourSide._left) + state.GetBorderSpace(fourSide._right);
            float h = spaceAlloc.Height() + state.GetBorderSpace(fourSide._top) + state.GetBorderSpace(fourSide._bottom);
            if ((hAlign == hAlignType.Right) || (hAlign == hAlignType.Centre) || lineBreakAfter)
            	w = state.endPosition - curState.startPosition;
             
            // Make sure still allocate if Height or Width = 0 //
            if (!EndTag() || (type == tagType.linebreak))
            {
            	w = Math.Max(w, 0.1F);
            	h = Math.Max(h,parentHtml.documentOutput.TextSize("QZ",curState.font).Height);
            	h = Math.Max(h, 5.0F);
            }
            
            // Allocate self to lastSpaceAllocator //
            if (!(this is MasterTag)) { spaceAlloc.baseRect = 
            		LastSpaceAllocator().AllocateSpace(this, w, h, -1) ;}

            #endregion
            
            if (lineBreakAfter) { LastSpaceAllocator().NewLine(curState.startPosition); }
        }

        /// <summary>
        /// Update the included curState, called from CurrentStateType.UpdateState()
        /// </summary>        
        public override void UpdateState(ref CurrentStateType curState)
        {
        	base.UpdateState(ref curState);
        	
            if (StartTag())
            {
            	if (curState.stateTagList.IndexOf(this) != curState.stateTagList.Count -1)
                	return;
            	            	         
                switch (type)
                {                	
                	case tagType.head:
                		curState.visible = false;
                		curState.isHeader = true;
                		break;
                	case tagType.paragraph:
                		if (variables["bgcolor"].value != "")
                			curState.bkColor = Utils.WebColor2Color(variables["bgcolor"].value);                		
                		break;
                    case tagType.indent:                		
                        Int32 ind;
                        if ((variables.Contains("ind")) && (variables["ind"].value != ""))
                        { ind = Convert.ToInt32(variables["ind"].value); }
                        else
                        { ind = 0; }
                        curState.startPosition += ind;
                        break;
                    case tagType.orderedList:
                        if (curState.stateTagList.IndexOf(this) == curState.stateTagList.IndexOf(name))
                        	curState.startPosition += Defines.defaultListIndent;
//                       	if (spaceAlloc.currentX < curState.startPosition) 
//                        	spaceAlloc.currentX = curState.startPosition;
                        break;
                    case tagType.unorderedList:
                        if (curState.stateTagList.IndexOf(this) == curState.stateTagList.IndexOf(name))
                        	curState.startPosition += Defines.defaultListIndent;
//                        if (spaceAlloc.currentX < curState.startPosition) 
//                        	spaceAlloc.currentX = curState.startPosition;
                        break;                                  
                }
            }
            
            curState.hAlign = hAlign;                                       
        }
    }
    /// <summary>
    /// Owner of all tag
    /// </summary>
    internal class MasterTag : RegionTag
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MasterTag(MiniHtml aMiniHtml, string aName, PropertyList aVariables, Int32 aID, Int32 aLevel)
            : base(aMiniHtml, aName, aVariables, aID, aLevel) { lineBreakNow = true; }
        /// <summary>
        /// Return focused tag based on Space allocator info
        /// </summary>  
        public override HtmlTag GetFocusTag(ref float x, ref float y, ref Int32 extraInfo)
        {
            float xx = x; float yy = y; Int32 ex = extraInfo;
            HtmlTag retVal = base.GetFocusTag(ref xx, ref yy, ref ex);
            if (retVal != null)
            { x = xx; y = yy; extraInfo = ex; return retVal; }
            
            foreach (anAllocatedItem item in spaceAlloc.floatList)
                if (!(item.allocatedTag.EndTag()))
                {
                    RectangleF newRect = state.DrawableRect(spaceAlloc.baseRect, true);
                    xx = x - newRect.Left;
                    yy = y - newRect.Top - (spaceAlloc.lineInfo[item.currentLine].lineHeight - item.allocatedRect.Height);
                    extraInfo = item.extraInfo;                    
                    if (item.isIn(xx, yy))
                    {
                        x = xx;
                        y = yy;
                        return item.allocatedTag.GetFocusTag(ref x,ref y, ref extraInfo);
                    }
                }
            return null;
        }
        	
        /// <summary>
        /// return html syntax of current tag
        /// </summary>        
        public override string Html(bool beforeSelection, bool betweenSelection, bool afterSelection, bool aFormatted)
        {
        	string retVal = "";
        	        	
        	foreach (HtmlTag item in childTags)
        		retVal += item.Html(beforeSelection, betweenSelection, afterSelection, aFormatted);
        	
        	return retVal;
        }
        public override string BBCode(bool beforeSelection, bool betweenSelection, bool afterSelection)
    	{
    		string retVal = "";
        	        	
        	foreach (HtmlTag item in childTags)
        		retVal += item.BBCode(beforeSelection, betweenSelection, afterSelection);
        	
        	return retVal.Replace("[br]",Defines.lineBreak);        	
    	}
    }
    internal class InvisibleTag : HtmlTag
    {
    	public InvisibleTag(MiniHtml aMiniHtml, string aName, PropertyList aVariables, Int32 aID, Int32 aLevel)
            : base(aMiniHtml, aName, aVariables, aID, aLevel)
    	{    		
    	}
    	public InvisibleTag() : base()
        {
        }
    	/// <summary>
        /// Used by Update, add current tag to curState and update it.
        /// </summary>   
    	internal virtual void updateStateList(ref CurrentStateType curState)
    	{    	
    	    if (StartTag())
            {
                curState.AddTag(this);                
            }
            else
            {
                curState.RemoveTag(Utils.RemoveFrontSlash(name));
            }
    	}
    }
    /// <summary>
    /// Abstract class of Visible element
    /// </summary>	
    internal class VisibleTag : HtmlTag
    {

        #region State variables
        public bool visible;                                    //Is the text visible?
        public hAlignType hAlign;                               //Horizontal Align of text.
        public vAlignType vAlign;                               //Vertical Align of text.
        public string targetURL;                                //Is the text link to a URL?
        public float visibleHeight = 0;							//Height of a Line
        #endregion

        public VisibleTag() { }
        public VisibleTag(MiniHtml aMiniHtml, string aName, PropertyList aVariables, Int32 aID, Int32 aLevel)
            : base(aMiniHtml, aName, aVariables, aID, aLevel) { }
        
        public override void Update(ref CurrentStateType curState)
        {
        	if (!pNeedUpdate || !curState.visible) return;
            
            pNeedUpdate = false;            
            visible = curState.visible;            
            targetURL = curState.targetURL;            
            hAlign = curState.hAlign;
            vAlign = curState.vAlign;            
            base.Update(ref curState);
        }   
        
        public aLine AllocatedLine()
        {
        	Int32 lineIndex = LastSpaceAllocator().allocList.GetLineIndex(this, 0);
        	if (lineIndex != -1)
        		return LastSpaceAllocator().lineInfo[lineIndex];
        	return null;
        }
    }
    /// <summary>
    /// Represent text
    /// </summary>
    internal class TextTag : VisibleTag
    {
        static float approxRatio = 0.98F;
        static Int32 defCount = 50;

        private StringBuilder pText;                            //Text in this region
        private ArrayList textList = new ArrayList();
        public string text
        {
            get { return pText.ToString(); }
            set { pText = new StringBuilder(value) ; needUpdate = true; }
        }

        #region State variables
        public Font font;                                       //Font of text        
        public Color textColor;                                 //Color of text
        public bool isScript;                                   //Is the text script? (not visible)
        public bool subScript, supScript;                       //Sub or Super script
        public textTransformType textTransform;                 //Transform the text to upper/lower/capitalize        
        #endregion        
          
        /// <summary>
        /// Process the text variable to outputable form.
        /// </summary>        
        public string outputText()
        {
            return Utils.TransformText(text, textTransform);
        }
        /// <summary>
        /// Update state variables and allocate space.
        /// </summary>        
        public override void Update(ref CurrentStateType curState)
        {
            if (!pNeedUpdate || !curState.visible) 
            {
            	if (curState.isTitle())
            		parentHtml.title = this.text;
            	return;
            }
            base.Update(ref curState);
            pNeedUpdate = false;

            //Debug.WriteLine("Updating TextTag, text = " + text);

            CurrentStateType.UpdateState(ref curState);
                        
            textColor = curState.TextColor();            
            isScript = curState.isScript;
            subScript = curState.subScript;
            supScript = curState.supScript;
            textTransform = curState.textTransform;            

            if ((subScript) || (supScript))
            {
                font = new Font(curState.font.FontFamily, curState.font.Size / 3 * 2);
                if (subScript)
                { vAlign = vAlignType.Bottom; }
                else
                { vAlign = vAlignType.Top; }
            }
            else
            {
                font = (Font)(curState.font.Clone());
            }

            visibleHeight = parentHtml.documentOutput.TextSize("QZ", font).Height;
            //LastSpaceAllocator().AllocateSpace(this, hAlign, textSize.Width, textSize.Height, -1);
            textList.Clear();
            if (text == "")
            {            	
            	textList.Add("");
            	//HACK: Allocate 0 as height instead of visibleHeight
            	LastSpaceAllocator().AllocateSpace(this, hAlign, 2, 0, 0);
            }
            else
            	MultiLineUpdate(curState);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public TextTag()
        {
            pText = new StringBuilder();
            Console.WriteLine("Warning : Constructor TextTag() is for debug only");
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public TextTag(MiniHtml aMiniHtml, string aText) 
            : base(aMiniHtml, "Text", null, 0, 0)
        {       
            pText = new StringBuilder(aText.Replace((char)10, ' ').Replace((char)13, ' '));            
        }
		/// <summary>
		/// Return if specified line in selection, and what is selected.
		/// Internal use only
		/// </summary>
        private bool getSelectionRange(Int32 aLine, ref Int32 startChar, ref Int32 endChar)
        {
            Selection sel = parentHtml.documentOutput.selection;
            bool retVal = sel.isBetween(this);

            if (retVal)
                if ((sel.firstCursor.selectedTag != this) || (sel.firstCursor.selectedLine <= aLine))
                    if ((sel.secondCursor.selectedTag != this) || (sel.secondCursor.selectedLine >= aLine))
                    {
                        startChar = 0;
                        if (sel.firstCursor.selectedTag == this)
                            if (sel.firstCursor.selectedLine == aLine)
                            { startChar = sel.firstCursor.selectedChar; }

                        endChar = this[aLine].Length;
                        if (sel.secondCursor.selectedTag == this)
                            if (sel.secondCursor.selectedLine == aLine)
                            { endChar = sel.secondCursor.selectedChar; }                            
                    }
                    else return false;
                else return false;
            return retVal;
        }
        /// <summary>
        /// Return the selected rectangle in specified line.
        /// </summary>
        public RectangleF GetSelectionRange(Int32 aLine, RectangleF baseRect)
        {
            Int32 startChar = 0, endChar = 0;
            if (getSelectionRange(aLine, ref startChar, ref endChar))
            {
                float sPos = 0, ePos = 0;
                //if (startChar == 1)
                //    startChar = 0;
                if (startChar > 0)
                    sPos = parentHtml.documentOutput.TextSize(this[aLine].Substring(0, startChar), font).Width;
                if (endChar > 0)
                    ePos = parentHtml.documentOutput.TextSize(this[aLine].Substring(0, endChar), font).Width;

                PointF startPos = new PointF(baseRect.Left + sPos, baseRect.Top);
                PointF endPos = new PointF(baseRect.Left + ePos, baseRect.Bottom);
                return new RectangleF(startPos.X, startPos.Y, endPos.X - startPos.X, endPos.Y - startPos.Y);
            }
            else
            { return new RectangleF(0, 0, 0, 0); }
        }
		/// <summary>
		/// Return approx how many char and width in current line.
		/// Internal use only (MultiLineUpdate)
		/// </summary>
        private void lineTextCount(string s, bool currentLine, ref Int32 textCount,ref float lineWidth)
        {
            Int32 sampleCount;


            if (s.Length > defCount)
            { sampleCount = defCount; }
            else { sampleCount = s.Length; }

            string sample = s.Substring(0, sampleCount);
            float sampleWidth = Utils.TextSize(sample, font).Width;

            if (currentLine)
            { lineWidth = Math.Max(0, (LastSpaceAllocator().EndPosition() - LastSpaceAllocator().currentX) * approxRatio); }
            else
            { lineWidth = Math.Max(1, (LastSpaceAllocator().EndPosition()) * approxRatio); }

            if (text.Trim() == "")
            { textCount = text.Length; }
            else
            { textCount = (Int32)(lineWidth / (sampleWidth / sampleCount)) + 1; }           
        }
        /// <summary>
        /// Return next line to add.
        /// Internal use only (MultiLineUpdate)
        /// </summary>
        private string readNextLine(string s, Int32 lineCount, float lineWidth, ref Int32 workingTextPos)
        {
            string retVal;
            if (s.Length - workingTextPos <= lineCount)
            {
                retVal = s.Substring(workingTextPos);
                workingTextPos = s.Length - 1;
                return retVal;
            }
            Int32 lastSpacePos = s.LastIndexOf(' ', lineCount + workingTextPos, lineCount + workingTextPos) + 1;
            if (lastSpacePos <= 0)
                if (lineCount == 0)                
                { lastSpacePos = s.Length-1; }
            	else
            		//if (lineCount == fullLineCount)
            			{lastSpacePos = workingTextPos + lineCount-1; }

            if (lastSpacePos - workingTextPos == 0)
            {
            	workingTextPos = s.Length;
            	return s;            	
            }
            
            retVal = s.Substring(workingTextPos, lastSpacePos - workingTextPos);

            float lnWidth = LastRegionTag().spaceAlloc.WidthLimit() * approxRatio;
            while ((Utils.TextSize(retVal, font).Width > lnWidth) && (retVal.Trim().Length > 1))
            {
                lastSpacePos = retVal.LastIndexOf(' ');
                if (lastSpacePos == -1)
                	if (retVal.Length - 2 > 0) 
                    	lastSpacePos = retVal.Length - 2;
                	else lastSpacePos = retVal.Length + 1;
                	
                retVal = retVal.Substring(0, lastSpacePos - 1);
            }            

            lastSpacePos = retVal.Length;
            workingTextPos += lastSpacePos;
            return retVal;
        }
        /// <summary>
        /// Add a line to LastSpaceAllocator.
        /// Internal use only (MultiLineUpdate)
        /// </summary>        
        private void addLine(string s, float lineWidth, float lineHeight)
        {
            if (s == "") { return; }

            SizeF lineSize = parentHtml.documentOutput.TextSize(s, font);
            textList.Add(s);
            LastSpaceAllocator().AllocateSpace(this, lineWidth, lineHeight, textList.Count - 1);                        
            //    (CurState as CurrentStateType).updateState;
            ////    LineTextCount(workingText,workingTextPos,next,nLWidth);
        }
        /// <summary>
        /// Add a line to LastSpaceAllocator.
        /// Internal use only (MultiLineUpdate)
        /// </summary> 
        private void addLine(string s, Font font)
        {
            if (s == "") { return; }

            SizeF lineSize = parentHtml.documentOutput.TextSize(s, font);
            addLine(s, lineSize.Width, lineSize.Height);            
        }
        /// <summary>
        /// Update function for TextTag.
        /// Internal use only 
        /// </summary> 
        private void MultiLineUpdate(CurrentStateType curState)
        {
            Int32 workingTextPos = 0;
            string workingText = outputText();
            //float lineHeight = Utils.TextSize(text, font).Height;
            Int32 lineCount=0;
            Int32 fullLineCount=0;
            float lineWidth=0;
            float fullLineWidth=0;
			
            
            textList.Clear();
            lineTextCount(workingText, true, ref lineCount,ref lineWidth);
            lineTextCount(workingText, false,ref fullLineCount,ref fullLineWidth);

            string nextLine = readNextLine(workingText, lineCount, lineWidth, ref workingTextPos);
            addLine(nextLine, font);
            
            while (workingText.Length > workingTextPos + 1)
            {               
                nextLine = readNextLine(workingText, fullLineCount, fullLineWidth, ref workingTextPos);                
                addLine(nextLine, font);                
            }
        }
        /// <summary>
        /// Return TextPosition based on x axis and aLine. 
        /// </summary>
        public void GetFocusedText(float x, float y, Int32 aLine, ref Int32 aChar)
        {
            aChar = 0;
            if ((aLine == -1) || (x < 0))
                return;            
                
            string currentLine = (String)(textList[aLine]);
            
            Bitmap b = new Bitmap((Int32)(LastSpaceAllocator().Width()),(Int32)(LastSpaceAllocator().Height()));
            Graphics g = Graphics.FromImage(b);

            try
            {
                aChar = Utils.TextPosition2(g, currentLine, font, x);
            }
            finally
            {
                g.Dispose();
                b.Dispose();
            }
        }
		/// <summary>
		/// Return a line of text.
		/// </summary>
        public string this[Int32 id]
        {
            get
            {            	
                return (string)(textList[id]);
            }
        }
        /// <summary>
        /// Line/Char Conversion tools.
        /// </summary>
        public void LocateText(out Int32 aLine, ref Int32 aChar)
        {
            Int32 count = aChar;
            for (int i = 0; i < textList.Count; i++)
            {
                string currentLine = (string)textList[i];
                if (count > currentLine.Length)
                { count -= currentLine.Length; }
                else
                { 
                    aLine = i;
                    aChar = count;
                    return;
                }
            }
            aLine = 0;
            aChar = 0;            
        }
        /// <summary>
        /// Line/Char Conversion tools.
        /// </summary>
        public Int32 LocateText(Int32 aLine, Int32 aChar)
        {
            Int32 retVal = 0;
            if ((aLine != 0) || (aChar != 0))
            for (int i = 0; i <= aLine; i++)
            {
                string currentLine = (string)textList[i];
                if (i != aLine)
                { retVal += currentLine.Length; }
                else
                { retVal += aChar; }
            }
            return retVal;
        }
        /// <summary>
        /// Replace text in texttag.
        /// </summary>
        public void ReplaceText(Int32 startPos, Int32 endPos, string aText)
        {
        	if (startPos > endPos)
        		return;
        	if (endPos - startPos < 0)
        		return;
        	if (startPos <= -1)
        		return;
        	
            pText.Remove(startPos, endPos - startPos);
			pText.Insert(startPos, aText);
            
            //TODO: Remove needUpdate information
            foreach (HtmlTag item in parentHtml.tagPositionList)
                	item.needUpdate = true;
            needUpdate = true;
        }
        /// <summary>
        /// Replace text in texttag.
        /// </summary>
        public void ReplaceText(Int32 startLine, Int32 startChar, Int32 endLine, Int32 endChar, string aText)
        {
        	ReplaceText(LocateText(startLine, startChar),
        	            LocateText(endLine, endChar),
        	           	aText);
        }
        /// <summary>
        /// Replace text in texttag.
        /// </summary>
        public void ReplaceText(MiniHtmlCursor startCursor, MiniHtmlCursor endCursor, string aText)
        {
        	Int32 startLine = 0, startChar = 0;
        	Int32 endLine = textList.Count - 1;
        	Int32 endChar = this[endLine].Length-1;
        	if (startCursor.selectedTag == this)
        	{
        		startLine = startCursor.selectedLine;
        		startChar = startCursor.selectedChar;
        	}
        	if (endCursor.selectedTag == this)
        	{
        		endLine = endCursor.selectedLine;
        		endChar = endCursor.selectedChar;
        	}
        	
        	ReplaceText(startLine, startChar, endLine, endChar, aText);        	
        }
        /// <summary>
        /// Replace text in texttag.
        /// </summary>
        public void ReplaceText(string aText)
        {
        	if ((parentHtml.documentOutput.selection.CursorMode) &&
        	    (parentHtml.documentOutput.selection.startCursor.selectedTag == this))
        	ReplaceText(parentHtml.documentOutput.selection.firstCursor, 
        	            parentHtml.documentOutput.selection.secondCursor, 
        	            aText);
        }  
        
        public aLine AllocatedLine(Int32 aLineNumber)
        {
        	Int32 lineIndex = LastSpaceAllocator().allocList.GetLineIndex(this, aLineNumber);
        	if (lineIndex != -1)
        		return LastSpaceAllocator().lineInfo[lineIndex];
        	return null;
        }                
                             
        /// <summary>
        /// return html syntax of current tag
        /// </summary>        
        public override string Html(bool beforeSelection, bool betweenSelection, bool afterSelection, bool aFormatted)
        {
        	string breakStr = "";
        	if (aFormatted)
        	{        		
        		breakStr = Defines.lineBreak;
        		Int32 count = ParentTagsCount();
        		for (Int32 i = 0; i < count-1; i++)
        			breakStr += Defines.formattedSpacing;
        	}
        	
        	MiniHtmlCursor firstCursor = parentHtml.documentOutput.selection.firstCursor;
        	MiniHtmlCursor secondCursor = parentHtml.documentOutput.selection.secondCursor;
        	
        	Int32 index = TagIndex();
        	Int32 firstIndex = 0; 
        	Int32 secondIndex = parentHtml.tagPositionList.Count -1; 
        	
        	if ((firstCursor.Selected) && (secondCursor.Selected))
        	{
        		firstIndex = firstCursor.selectedTag.TagIndex();
        		secondIndex = secondCursor.selectedTag.TagIndex();
        	}
        	else return breakStr + text;  //Not valid selection, return everything.
        	        	
        	if (((firstIndex > index) && beforeSelection) ||
        	    ((secondIndex < index) && afterSelection) ||
        		(beforeSelection && betweenSelection && afterSelection))
        		return breakStr + text;
        	
        	Int32 startIndex = 0;
        	if (secondIndex < index)
        		startIndex = -1;
        	else
        	if (firstIndex == index)
        		if (!(beforeSelection))
        		{
        			if (afterSelection)
        			startIndex = LocateText(secondCursor.selectedLine, secondCursor.selectedChar);
        			if (betweenSelection)
        			startIndex = LocateText(firstCursor.selectedLine, firstCursor.selectedChar);
        		}
        		        		        	
        	Int32 endIndex = text.Length;
        	if (firstIndex > index)
        		endIndex = -1;
        	else
        	if (secondIndex == index)
        		if (!(afterSelection))
        		{
        			if (beforeSelection)
        				endIndex = LocateText(firstCursor.selectedLine, firstCursor.selectedChar);
        			if (betweenSelection)
        				endIndex = LocateText(secondCursor.selectedLine, secondCursor.selectedChar);
        			
        		}

        	if ((startIndex == -1) || (endIndex == -1))
        		return "";
        	return breakStr + text.Substring(startIndex, endIndex - startIndex);
        }
        /// <summary>
    	/// return BBCode syntax of current tag
    	/// </summary>    
    	public override string BBCode(bool beforeSelection, bool betweenSelection, bool afterSelection)
    	{
    		return Html(beforeSelection, betweenSelection, afterSelection, false);
    	}
        
    }
    
    
    /// <summary>
    /// Represent a tag that change style of font.
    /// </summary>
    internal class FontStyleTag : InvisibleTag
    {
        internal enum tagType { unknown, bold, underline, italic, strikeOut, sub, sup, strong, anchor, font, color, size };        
        public tagType type = tagType.unknown;

        /// <summary>
        /// Constructor
        /// </summary>
        public FontStyleTag(MiniHtml aMiniHtml, string aName, PropertyList aVariables, Int32 aID, Int32 aLevel)
            : base(aMiniHtml, aName, aVariables, aID, aLevel)
        {
            #region Assign Tag Type
            switch (Utils.RemoveFrontSlash(aName.ToLower()))
            {
                case "a":
                    type = tagType.anchor;
                    break;
                case "b":
                    type = tagType.bold;
                    break;
                case "i":
                    type = tagType.italic;
                    break;
                case "u":
                    type = tagType.underline;
                    break;
                case "s":
                    type = tagType.strikeOut;
                    break;
                case "sub":
                    type = tagType.sub;
                    break;
                case "sup":
                    type = tagType.sup;
                    break;
                case "strong":
                    type = tagType.strong;
                    break;
                case "font":
                    type = tagType.font;
                    break;
                case "color":
                	type = tagType.color;
                	break;
                case "size":
                	type = tagType.size;
                	break;
            }
            #endregion
        }
        /// <summary>
        /// Add tag to stateTagList if StartTag(), remove if EndTag()
        /// </summary>        
        public override void Update(ref CurrentStateType curState)
        {            
        	updateStateList(ref curState);
        	
            if (!(pNeedUpdate)) return;
            pNeedUpdate = false;

            base.Update(ref curState);
        }
        /// <summary>
        /// Update font style in curState
        /// </summary>        
        public override void UpdateState(ref CurrentStateType curState)
        {
        	base.UpdateState(ref curState);
        	
            if (StartTag())
            {
                switch (type)
                {
                    case tagType.bold :
                        curState.font = new Font(curState.font, curState.font.Style | FontStyle.Bold);
                        break;
                    case tagType.italic :
                        curState.font = new Font(curState.font, curState.font.Style | FontStyle.Italic);
                        break;
                    case tagType.strikeOut:
                        curState.font = new Font(curState.font, curState.font.Style | FontStyle.Strikeout);
                        break;
                    case tagType.underline:
                        curState.font = new Font(curState.font, curState.font.Style | FontStyle.Underline);
                        break;
                    case tagType.sup:
                        curState.supScript = true;
                        break;
                    case tagType.sub:
                        curState.subScript = true;
                        break;
                    case tagType.strong:
                        curState.font = new Font(curState.font, curState.font.Style | FontStyle.Bold);
                        break;
                    case tagType.anchor:
                        if (variables["href"].value != "")
                        {
                            curState.targetURL = variables["href"].value;
                            curState.font = new Font(curState.font, curState.font.Style | FontStyle.Underline);
                            curState.fntColor = curState.URLColor;                            
                        }
                        break;
                    case tagType.font:
                        if (variables["name"].value != "")
                        {
                            curState.font = new Font(variables["name"].value, curState.font.Size);
                        }
                        if (variables["size"].value != "")
                        {
                            curState.font = new Font(curState.font.FontFamily,(float)(Convert.ToInt32(variables["size"].value)));
                        }
                        if (variables["color"].value != "")
                        {
                            curState.fntColor = Utils.WebColor2Color(variables["color"].value);
                        }
                        break;
                    case tagType.color:   
                        if (variables["value"].value != "")
                        {
                            curState.fntColor = Utils.WebColor2Color(variables["value"].value);
                        }
                        break;
                    case tagType.size:   
                        if (variables["value"].value != "")
                        {
                            curState.font = new Font(curState.font.FontFamily,(float)(Convert.ToInt32(variables["value"].value)));
                        }
                        break;
                        
                }
                
            }

        }
    }
    /// <summary>
    /// Represent variables thats not visible on screen, like meta, doctype
    /// </summary>       
    internal class VariableTag : InvisibleTag
    {
    	internal enum tagType { unknown, meta }
    	public tagType type = tagType.unknown;
    	
    	public VariableTag(MiniHtml aMiniHtml, string aName, PropertyList aVariables, Int32 aID, Int32 aLevel)
        {
            init(aMiniHtml, aName, aVariables, aID, aLevel);
            #region Assign Tag Type
            switch (Utils.RemoveFrontSlash(aName.ToLower()))
            {
                case "meta":
                    type = tagType.meta;
                    break;
            }            
            #endregion
        }
    	
    	public override void Update(ref CurrentStateType curState)
        {  
    		switch (type)
    		{
    			case tagType.meta:
    				foreach (PropertyItemType pie in variables)
    					parentHtml.metaData.Add(pie.key, pie.value);
    				break;
    				
    		}
    	}
    }
   
    internal class ElementTag : VisibleTag    	
    {
    	internal enum tagType { unknown, img }
    	public tagType type = tagType.unknown;
    	
    	private Image imgCache, invImgCache;
    	public Size imgSize;
    	public string imgSrc = "";
    	public string imgAlt = "";    	    	    	
    	public hAlignType objAlign = hAlignType.Left;    	
    	
    	public Image elementImage
    	{
    		get { return imgCache; }
    	}    
    	public Image invertedImage
    	{
    		get { return invImgCache; }
    	}    
    	
    	public ElementTag(MiniHtml aMiniHtml, string aName, PropertyList aVariables, Int32 aID, Int32 aLevel)
        {
            init(aMiniHtml, aName, aVariables, aID, aLevel);
            #region Assign Tag Type
            switch (Utils.RemoveFrontSlash(aName.ToLower()))
            {
                case "img":
                    type = tagType.img;
                    if (variables.Contains("src")) 
    					imgSrc = variables["src"].value;
    				if (variables.Contains("alt")) 
    					imgAlt = variables["alt"].value;
    				if (variables.Contains("align"))
    					objAlign = Utils.StrAlign2Align(variables["align"].value);
                    break;
            }            
            #endregion
        }
    	
    	public override void Update(ref CurrentStateType curState)
        {  
    		if (!pNeedUpdate || !curState.visible) 
            {
            	return;
            }
            base.Update(ref curState);
            pNeedUpdate = false;            

            CurrentStateType.UpdateState(ref curState);
    		
    		switch (type)
    		{
    			case tagType.img:  
    				Int32 width  = 10;
    				Int32 height = 10;
    				try
    				{
    					imgCache = new Bitmap(imgSrc);
    					    						
    					if (variables.Contains("width"))
    						width = Convert.ToInt32(variables["width"].value,10);
    					else if (imgCache != null)
    						width = imgCache.Width;
    				
    					if (variables.Contains("height"))
    						height = Convert.ToInt32(variables["height"].value,10);
    					else if (imgCache != null)
    						height = imgCache.Height;
    				
    					imgSize = new Size(width, height);
    					
    					if ((width != imgCache.Width) || (height != imgCache.Height))
    						imgCache = Utils.ResizeImage(imgCache, width, height);
    					invImgCache = Utils.NegativeImage(imgCache);
    				}
    				finally
    				{
    					LastSpaceAllocator().AllocateSpace(this, objAlign, width, height, -1);
    				}    				
    			break;    				
    		}
    	}
    }
    
//    internal class TableTag : InvisibleTag
//    {
//        public enum tagType { unknown, table, head, row, cell };
//        public tagType type = tagType.unknown;
//        public Int32 colSpan, rowSpan;
//        public TableSpaceAllocator tableSpaceAlloc;
//        private TableTag lastCacheTableTag;
//        private TableSpaceAllocator lastCacheTableSpaceAllocator;      
//        public CurrentStateType state;    
//        
//        /// <summary>
//        /// Return last parent table tag.
//        /// </summary>        
//        public TableTag LastTableTag()
//        {            
//            if (lastCacheTableTag == null)
//            {
//                HtmlTag aTag = parentTag;
//                while ((aTag != null) && (!(aTag is TableTag)))
//                { aTag = aTag.parentTag; }
//
//                if (aTag == null)
//                { lastCacheTableTag = null; }
//                else
//                    lastCacheTableTag =(TableTag)(aTag);
//            }
//            return lastCacheTableTag;
//        }
//        /// <summary>
//        /// Return spaceAlloc owned by last parent region tag.
//        /// Will cache to lastCacheSpaceAllocator once run once.
//        /// </summary>        
//        public TableSpaceAllocator LastTableSpaceAllocator()
//        {
//            if (lastCacheTableSpaceAllocator == null)
//            {
//                TableTag rt = LastTableTag();
//                if (rt != null)
//                { lastCacheTableSpaceAllocator = rt.tableSpaceAlloc; }
//            }
//            return lastCacheTableSpaceAllocator;
//        }
//        
//        
//        /// <summary>
//        /// Constructor
//        /// </summary>
//        public TableTag(MiniHtml aMiniHtml, string aName, PropertyList aVariables, 
//                        Int32 aID, Int32 aLevel) 
//        {
//            init(aMiniHtml, aName, aVariables, aID, aLevel);
//            #region Assign Tag Type
//            switch (Utils.RemoveFrontSlash(aName.ToLower()))
//            {
//                case "table":
//                    type = tagType.table;
//                    break;
//                case "th":
//                    type = tagType.head;
//                    break;
//                case "tr":
//                    type = tagType.row;
//                    break;
//                case "td":
//                    type = tagType.cell;
//                    if (variables.Contains("colSpan")) 
//                    	colSpan = Convert.ToInt32(variables["colSpan"].value,1);
//                    if (variables.Contains("rowSpan")) 
//                    	rowSpan = Convert.ToInt32(variables["rowSpan"].value,1);
//                    break;            
//            }
//            #endregion
//        }
//        
//        private void updateTable(ref CurrentStateType curState)
//        {
//        	#region 1. Scan through childTags and calculate amount of Column / Row.        	
//       		Int32 colCount = 0;
//            Int32 rowCount = 0;
//                
//            foreach (HtmlTag aTag in childTags)
//            {
//            	if (aTag is TableTag) 
//            	{
//            		TableTag tTag = (TableTag)(aTag);
//             		tagType aType = tTag.type;
//             		if ((aType == tagType.head) || (aType == tagType.row))
//             		{
//             			rowCount += 1;
//             			if (colCount < tTag.colCount())
//             				colCount = tTag.colCount();
//             		}
//             	}
//            }	
//            #endregion
//            
//            #region 2. Create new tableSpaceAllocator, allocate array
//            tableSpaceAlloc = new TableSpaceAllocator(this, colCount, rowCount);
//            #endregion            
//            
//            #region 3. Estimate size for each cell
//            
//            foreach (HtmlTag rowTag in childTags)
//            {
//            	if ((rowTag is TableTag) && (((TableTag)rowTag).type == tagType.row))
//            		foreach (HtmlTag cellTag in rowTag.childTags)
//            		{
//            			if ((cellTag is TableTag) && (((TableTag)rowTag).type == tagType.row))
//            			{
//            				SizeF cellSize = ((TableTag)cellTag).estimateCellSize(ref curState);
//            				tableSpaceAlloc.AllocateSpace((TableTag)cellTag,
//            				                              hAlignType.Left, cellSize.Width, cellSize.Height, -1);
//            				                              
//            			}
//            		}
//            }
//            
//            #endregion
//            
//        }
//        
//        public SizeF estimateCellSize(ref CurrentStateType curState)
//        {
//        	updateStateList(ref curState);
//        	parentHtml.parentCss.ApplyCssStyle(this, ref curState, true);    
//        	
//        	spaceAlloc = new SpaceAllocator(this);  
//        	            	        	
//            foreach (HtmlTag aTag in childTags)
//                aTag.Update(ref curState);   
//        	
//        	float w = spaceAlloc.Width() + state.GetBorderSpace(fourSide._left) + state.GetBorderSpace(fourSide._right);
//            float h = spaceAlloc.Height() + state.GetBorderSpace(fourSide._top) + state.GetBorderSpace(fourSide._bottom);
//            
//            return new SizeF(w, h);
//        }
//        
//        
//        public void updateCell()
//        {        	               	
////        	updateStateList(ref curState);
////
////            #region Update self and child if pNeedUpdate            
////            if (pNeedUpdate) 
////            {
////                pNeedUpdate = false;				
////                spaceAlloc = new SpaceAllocator(this);     
////                if (lineBreakNow) { LastSpaceAllocator().NewLine(); } 
////                base.Update(ref curState);               
////            }
////            
////			if (!state.visible) return;                        
////			
////            if ((spaceAlloc.ownerTag == this) && !(this.EndTag()))
////                	spaceAlloc.FinishAllocate();
////            #endregion
////            
////            #region Calculate Height and Width of current tag, and allocate            
////            float w = spaceAlloc.Width() + state.GetBorderSpace(fourSide._left) + state.GetBorderSpace(fourSide._right);
////            float h = spaceAlloc.Height() + state.GetBorderSpace(fourSide._top) + state.GetBorderSpace(fourSide._bottom);
////            if ((hAlign == hAlignType.Right) || (hAlign == hAlignType.Centre) || lineBreakAfter)
////            	w = state.endPosition - curState.startPosition;
////             
////            // Make sure still allocate if Height or Width = 0 //
////            if (!EndTag() || (type == tagType.linebreak))
////            {
////            	w = Math.Max(w, 0.1F);
////            	h = Math.Max(h,parentHtml.documentOutput.TextSize("QZ",curState.font).Height);
////            	h = Math.Max(h, 5.0F);
////            }
////            
////            // Allocate self to lastSpaceAllocator //
////            if (!(this is MasterTag)) { spaceAlloc.baseRect = 
////            		LastSpaceAllocator().AllocateSpace(this, w, h, -1) ;}
////
////            #endregion
//         
//        }
//               
//        public Int32 colCount()
//        {
//        	if ((type == tagType.head) || (type == tagType.row))
//        		return childTags.CountName("td"); 
//        	else
//        		throw new Exception("colCount can be called by <th> or <td> only.");
//        		
//        }
//        
//        /// <summary>
//        /// Update childTags, and allocate self to parent region spaceAllocaor
//        /// </summary>        
//        public override void Update(ref CurrentStateType curState)
//        {   
//        	base.updateStateList(ref curState);
//        	
//        	
//            #region Update self and child if pNeedUpdate            
//            if (pNeedUpdate) 
//            {
//                pNeedUpdate = false;				
//                
                
//                Int32 rowCount = this.childTags.CountName("th") + this.childTags.CountName("tr");
                
//                spaceAlloc = new SpaceAllocator(this);     
//                if (lineBreakNow) { LastSpaceAllocator().NewLine(); } 
//                base.Update(ref curState);               
//            }
            
//			if (!state.visible) return;                        
//			
//            if ((spaceAlloc.ownerTag == this) && !(this.EndTag()))
//                	spaceAlloc.FinishAllocate();
//            #endregion
//            
//            #region Calculate Height and Width of current tag, and allocate            
//            float w = spaceAlloc.Width() + state.GetBorderSpace(fourSide._left) + state.GetBorderSpace(fourSide._right);
//            float h = spaceAlloc.Height() + state.GetBorderSpace(fourSide._top) + state.GetBorderSpace(fourSide._bottom);
//            if ((hAlign == hAlignType.Right) || (hAlign == hAlignType.Centre) || lineBreakAfter)
//            	w = state.endPosition - curState.startPosition;
//             
//            // Make sure still allocate if Height or Width = 0 //
//            if (!EndTag() || (type == tagType.linebreak))
//            {
//            	w = Math.Max(w, 0.1F);
//            	h = Math.Max(h,parentHtml.documentOutput.TextSize("QZ",curState.font).Height);
//            	h = Math.Max(h, 5.0F);
//            }
//            
//            // Allocate self to lastSpaceAllocator //
//            if (!(this is MasterTag)) { spaceAlloc.baseRect = 
//            		LastSpaceAllocator().AllocateSpace(this, w, h, -1) ;}
//
//            #endregion
//            
//            if (lineBreakAfter) { LastSpaceAllocator().NewLine(curState.startPosition); }
//        }
//    }
    
}

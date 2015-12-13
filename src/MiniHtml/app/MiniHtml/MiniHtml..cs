using System;
using System.Drawing;
using System.Text;
using QuickZip.MiniCss;

namespace QuickZip.MiniHtml
{   
/// <summary>
/// Quick Zip MiniHtml - Html Parser and Printer
/// </summary>
    public class MiniHtml
    {
        internal MasterTag masterTag;                           //Owner of all tags
        public Int32 widthLimit = Defines.defaultWidth;         //Default Width Limit

        public DocumentOutputType documentOutput;               //Control the visible element
        public TagList tagPositionList;                         //Maintain a list of HtmlTags
        public QuickZip.MiniCss.MiniCss parentCss;				//Casade Style Sheets
        public CustomParaser parser;                            //Parse Html source to usable form
        public string title = "";
        public PropertyList metaData = new PropertyList();
        
        public parseMode ParseMode
        {
        	get 
        	{
        		if (parser is BBCodeParaser)
        			return parseMode.BBCode;
        		return parseMode.Html;
        	}
        	set
        	{
        		switch (value)
        		{
        			case parseMode.BBCode:
        				parser = new BBCodeParaser(this, parentCss);  	
        				break;
        			case parseMode.Html:
        				parser = new InternalParaser(this, parentCss);
        				break;
        			default: 
        				parser = new TextParaser(this, parentCss);
        				break;
        		}	
        	}
        }
        
        /// <summary>
        /// Contstructor
        /// </summary>
        public MiniHtml()
        {
            masterTag = new MasterTag(this, "Master", new PropertyList(), -1, 50);
            documentOutput = new GraphicsOutputType(masterTag);
            tagPositionList = new TagList();
            parentCss = new QuickZip.MiniCss.MiniCss(this);
            parser = new InternalParaser(this, parentCss);            
        }
        /// <summary>
        /// Parse Html from string
        /// </summary>
        public static MiniHtml FromString(string html, string css,  Int32 width)
        {
            MiniHtml mh = new MiniHtml();
            mh.widthLimit = width;
            mh.parser.Parse(html);
            if (css != "")
	            mh.parser.ParseCss(css);
            return mh;
        }
        
        /// <summary>
        /// Parse Html from string
        /// </summary>
        public static MiniHtml FromString(string html, Int32 width)
        {
        	return MiniHtml.FromString(html, "", width);
        }
        
        /// <summary>
        /// Parse BBCode from string
        /// </summary>
        public static MiniHtml FromBBCode(string bbCode, string css,  Int32 width)
        {        	
            MiniHtml mh = new MiniHtml();
            mh.ParseMode = parseMode.BBCode;            
            mh.widthLimit = width;            
            mh.parser.Parse(bbCode);
            if (css != "")
	            mh.parser.ParseCss(css);
            return mh;
        }
        
        
        private void rebuildTagPositionList(HtmlTag tag)
        {
        	tagPositionList.Add(tag);
        	foreach (HtmlTag item in tag.childTags)
        		rebuildTagPositionList(item);
        }
        
        public void RebuildTagPositionList()
        {
        	tagPositionList.Clear();
        	foreach (HtmlTag item in masterTag.childTags)
        		rebuildTagPositionList(item);
        }
        
        public void RebuildTagTreeList()
        {
        	HtmlTag currentTag = masterTag;
        	currentTag.childTags.Clear();
        	foreach (HtmlTag item in tagPositionList)
        	{
        		item.childTags.Clear();
        		currentTag = currentTag.AddTag(item);
        	}
        		
        }

        /// <summary>
        /// toggle every tag.needUpdate to true
        /// </summary>
        public void Invalidate()
        {
        	masterTag.Invalidate();
        	RebuildTagPositionList();
        }
        
        public void Reload(Int32 width)
        {
        	widthLimit = width;
        	CursorPosition sPos = documentOutput.selection.startCursor.CursorPosition();
        	CursorPosition ePos = documentOutput.selection.endCursor.CursorPosition();
        	parser.Parse(Html());
        	documentOutput.Update();
        	documentOutput.selection.SetStartCursor(sPos);
        	documentOutput.selection.SetEndCursor(ePos);        	        	
        }
        
        public void Reload()
        {
        	Reload(widthLimit);
        }        
        
        public string Html(bool Formatted)
        {
        	switch (ParseMode)
        	{
        		case parseMode.Html : 
        			return masterTag.Html(true, true, true, Formatted);
        		case parseMode.BBCode :
        			return masterTag.BBCode(true, true, true);        		
        		default : return "";
        	}        	
        }
        
        public string Html()
        {
        	switch (ParseMode)
        	{
        		case parseMode.Html : 
        			return masterTag.Html(true, true, true, false);
        		case parseMode.BBCode :
        			return masterTag.BBCode(true, true, true);        		
        		default : return "";
        	}            	   	
        }
        
        public string HtmlSelected()
        {
        	switch (ParseMode)
        	{
        		case parseMode.Html : 
        			return masterTag.Html(false, true, false, false);
        		case parseMode.BBCode :
        			return masterTag.BBCode(false, true, false);        		
        		default : return "";
        	}           	
        }
        
        public string HtmlBeforeSelection()
        {
        	switch (ParseMode)
        	{
        		case parseMode.Html : 
        			return masterTag.Html(true, false, false, false);
        		case parseMode.BBCode :
        			return masterTag.BBCode(true, false, false);        		
        		default : return "";
        	}            	
        }
        
        public string HtmlAfterSelection()
        {
        	switch (ParseMode)
        	{
        		case parseMode.Html : 
        			return masterTag.Html(false, false, true, false);
        		case parseMode.BBCode :
        			return masterTag.BBCode(false, false, true);        		
        		default : return "";
        	}         	
        }
        
        
        public static Bitmap DebugUnit()
        {
        	MiniHtml mh = MiniHtml.FromString("QzMiniHtml.Net (c) 2005-2006 Leung Yat Chun Joseph", 200);
            mh.documentOutput.Update();

            Bitmap b = new Bitmap(mh.widthLimit, (Int32)(mh.documentOutput.Height()));
            ((GraphicsOutputType)(mh.documentOutput)).outputGraphics = Graphics.FromImage(b);
            ((GraphicsOutputType)(mh.documentOutput)).outputGraphics.FillRectangle(Brushes.Wheat,
                new RectangleF(0, 0, mh.widthLimit, mh.documentOutput.Height()));


            mh.documentOutput.Output();
            mh.documentOutput.PrintItems();

            //b.Save("MiniHtml.Debug.bmp");
            return b;
        }

    }
}

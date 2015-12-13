using System;
using System.IO;
using System.Text;
using QuickZip.MiniCss;

namespace QuickZip.MiniHtml
{
    /// <summary>
    /// A Abstract class for a Html Paraser
    /// </summary>
    public class CustomParaser
    {
        internal MiniHtml parentHtml;
        internal QuickZip.MiniCss.MiniCss parentCss;
        /// <summary>
        /// Parse Html, Override this procedure for custom implementation
        /// </summary> 
        public virtual void Parse(TextReader reader)
        {   
        	parentHtml.masterTag.needUpdate = true;            
            parentHtml.masterTag.childTags.Clear();
            parentHtml.tagPositionList.Clear();   
            parentHtml.title = "";
            parentHtml.metaData.Clear();
        }        
        /// <summary>
        /// Parse Html
        /// </summary>        
        public void Parse(String s)
        {
        	if (s != null)
            	Parse(new StringReader(s));
        }
        /// <summary>
        /// Parse Html
        /// </summary>        
        public void Parse(Stream stream)
        {
            TextReader reader = new StreamReader(stream);
            Parse(reader);
        }
        /// <summary>
        /// Parse Css, Override this procedure for custom implementation
        /// </summary> 
        public virtual void ParseCss(TextReader reader)
        {
        	parentCss.ClearCssStyle();

            string input = reader.ReadToEnd();			
			string style, afterStyle;
			
            while (input != "")
            {
            	readNextStyle(input, out style, out afterStyle);
            	if (style.Trim() != "")
            		parentCss.AddCssStyle(style);
            	input = afterStyle;            
            }
        }
        
        private void readNextStyle(string text, out string style, out string afterStyle)
        {
        	style = "";
        	afterStyle = "";
        	Int32 pos1 = text.IndexOf('{');
        	Int32 pos2 = text.IndexOf('}');
        	if ((pos1 > -1) && (pos2 > pos1))
        	{
        		afterStyle = text;
        		style = Utils.ExtractBefore(ref afterStyle, '}') + '}';
        	}
        }
               
        /// <summary>
        /// Parse Css
        /// </summary>        
        public void ParseCss(String s)
        {
        	if (s != null)
            	ParseCss(new StringReader(s));
        }
        /// <summary>
        /// Parse Css
        /// </summary>        
        public void ParseCss(Stream stream)
        {
            TextReader reader = new StreamReader(stream);
            ParseCss(reader);
        }
        
         
        
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomParaser(MiniHtml mh, QuickZip.MiniCss.MiniCss ms)
        {
            parentHtml = mh;
            parentCss = ms;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomParaser(MiniHtml mh)
        {
            parentHtml = mh;
            parentCss = new QuickZip.MiniCss.MiniCss(mh);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomParaser(QuickZip.MiniCss.MiniCss ms)
        {
            parentHtml = null;
            parentCss = ms;
        }
    }
    /// <summary>
    /// MiniHtml internal Html Paraser, used since D7 version of TQzHtmlLabel2,
    /// not too efficient as it does a lot of string swapping.
    /// </summary>
    public class InternalParaser : CustomParaser
    {        
        internal HtmlTag previousTag = null;
        /// <summary>
        /// Constructor
        /// </summary>        
        public InternalParaser(MiniHtml mh) : base(mh)
        { }
        public InternalParaser(MiniHtml mh, QuickZip.MiniCss.MiniCss ms) : base(mh, ms)
        { }
        public InternalParaser(QuickZip.MiniCss.MiniCss ms) : base(ms)
        { }
        /// <summary>
        /// Return true if both < and > found in input
        /// </summary>        
        private bool haveClosingTag(string input)
        {
            if ((input.IndexOf('<') != -1) && (input.IndexOf('>') != -1))
                return false;
            return true;
        }
        /// <summary>
        /// Add a Non TextTag to Tag List
        /// </summary>        
        internal void addTag(HtmlTag aTag)
        {
            if (previousTag == null) { previousTag = parentHtml.masterTag; }
            previousTag = previousTag.AddTag(aTag);
        }
        /// <summary>
        /// Add TextTag to Tag List
        /// </summary>        
        protected void addTextTag(string text)
        {            
        	bool spaceStart = false;
        	bool spaceEnd = false;
        	
            if (text.EndsWith(" "))
            	spaceEnd = true;
            if (text.StartsWith(" "))
            	spaceStart = true;

            text = text.Trim();
            if (spaceStart) text = " " + text;
            if (spaceEnd) text = text + " ";
            
            text = Utils.DecodeSymbol(text);
            
            //if (text.Trim() != "")
            addTag(new TextTag(parentHtml, text));
        }
        /// <summary>
        /// Parse a string and return text before a tag, the tag and it's variables, and the string after that tag.
        /// </summary>
        private static void readNextTag(string s, ref string beforeTag, ref string afterTag, ref string tagName, 
                                          ref string tagVars, char startBracket, char endBracket)
        {
            Int32 pos1 = s.IndexOf(startBracket);
            Int32 pos2 = s.IndexOf(endBracket);

            if ((pos1 == -1) || (pos2 == -1) || (pos2 < pos1))
            {
                tagVars = "";
                beforeTag = s;
                afterTag = "";
            }
            else
            {
                String tagStr = s.Substring(pos1 + 1, pos2 - pos1 - 1);
                beforeTag = s.Substring(0, pos1);
                afterTag = s.Substring(pos2 + 1, s.Length - pos2 - 1);

                Int32 pos3 = tagStr.IndexOf(' ');
                if ((pos3 != -1) && (tagStr != ""))
                {
                    tagName = tagStr.Substring(0, pos3);
                    tagVars = tagStr.Substring(pos3+1, tagStr.Length-pos3-1);
                }
                else
                {
                    tagName = tagStr;
                    tagVars = "";
                }

                if (tagName.StartsWith("!--"))
                {
                    if ((tagName.Length < 6) || (!(tagName.EndsWith("--"))))
                    {
                        Int32 pos4 = afterTag.IndexOf("-->");
                        if (pos4 != -1)
                            afterTag = afterTag.Substring(pos4 + 2, afterTag.Length-pos4-1);
                    }
                    tagName = "";
                    tagVars = "";
                }

            }    
        }     
        /// <summary>
        /// Parse a string and return text before a tag, the tag and it's variables, and the string after that tag.
        /// </summary>
       	private static void readNextTag(string s, ref string beforeTag, ref string afterTag, ref string tagName, ref string tagVars)
       	{
       		InternalParaser.readNextTag(s, ref beforeTag, ref afterTag, ref tagName, ref tagVars, '<','>');
       	}
        /// <summary>
        /// Recrusive paraser.
        /// </summary>        
        private void parseHtml(ref string s)
        {
            string beforeTag="", afterTag="", tagName="", tagVar="";
            readNextTag(s, ref beforeTag, ref afterTag, ref tagName, ref tagVar);
            
//            if (beforeTag != "") { addTextTag(beforeTag); }
            //HACK: Always add a textTag
            if (beforeTag != "" || 
                (previousTag == null) 
                /*(!previousTag.EndTag())*/) { addTextTag(beforeTag); }
//            addTextTag(beforeTag);
            
            if (tagName != "")
            {
                Int32 tagID = Utils.LocateTag(Utils.RemoveFrontSlash(tagName));
                if (tagID != -1)
                {
                    Int32 tagLv = Defines.BuiltinTags[tagID].tagLevel;
                    switch (Defines.BuiltinTags[tagID].flags)
                    {
                    	case HTMLFlag.Element:
                    		addTag(new ElementTag(parentHtml, tagName, PropertyList.FromString(tagVar),tagID, tagLv));
                            break;
                        case HTMLFlag.TextFormat:
                            addTag(new FontStyleTag(parentHtml, tagName, PropertyList.FromString(tagVar),tagID, tagLv));
                            break;
                        case HTMLFlag.Region:
                            addTag(new RegionTag(parentHtml, tagName, PropertyList.FromString(tagVar), tagID, tagLv));
                            break;
                        case HTMLFlag.Variable:
                            addTag(new VariableTag(parentHtml, tagName, PropertyList.FromString(tagVar), tagID, tagLv));
                            break;                            
                    }
                }
            }
            s = afterTag;
        }
        /// <summary>
        /// Parse Html
        /// </summary>        
        public override void Parse(TextReader reader)
        {
        	base.Parse(reader);
        	previousTag = null;

            string input = reader.ReadToEnd();

            while (input != "")
                parseHtml(ref input);    
            
            parentHtml.RebuildTagPositionList();
        }

        public static void DebugUnit()
        {
            //string beforeTag="", afterTag="", tagName="", tagVar="";
            //readNextTag("<!-- xyz --><a href=\"xyz\"><b>", ref beforeTag, ref afterTag, ref tagName, ref tagVar);
            //readNextTag(afterTag, ref beforeTag, ref afterTag, ref tagName, ref tagVar);
            //Console.WriteLine(beforeTag);
            //Console.WriteLine(afterTag);
            //Console.WriteLine(tagName);
            //Console.WriteLine(tagVar);
            string Html = "<b>test</b>";
            MiniHtml mh = new MiniHtml();
            mh.parser.Parse((new StringReader(Html)));
            mh.masterTag.childTags.PrintItems();
        }
    }

    public class TextParaser : CustomParaser
    {
    	/// <summary>
        /// Constructor
        /// </summary>        
        public TextParaser(MiniHtml mh) : base(mh)
        { }       
        public TextParaser(MiniHtml mh, QuickZip.MiniCss.MiniCss ms) : base(mh, ms)
        { }
        public TextParaser(QuickZip.MiniCss.MiniCss ms) : base(ms)
        { }
    	public override void Parse(TextReader reader)
        {
        	base.Parse(reader);            
        	
        	string input = reader.ReadToEnd();
			
        	parentHtml.masterTag.AddTag(new TextTag(parentHtml, input));        	                                           
            
            parentHtml.RebuildTagPositionList();
        }
    }
    
    public class BBCodeParaser : CustomParaser
    {
    	internal HtmlTag previousTag = null;
    	
    	/// <summary>
        /// Constructor
        /// </summary>        
        public BBCodeParaser(MiniHtml mh) : base(mh)
        { }       
        public BBCodeParaser(MiniHtml mh, QuickZip.MiniCss.MiniCss ms) : base(mh, ms)
        { }
        public BBCodeParaser(QuickZip.MiniCss.MiniCss ms) : base(ms)
        { }
        /// <summary>
        /// Add a Non TextTag to Tag List
        /// </summary>        
        internal void addTag(HtmlTag aTag)
        {
            if (previousTag == null) { previousTag = parentHtml.masterTag; }
            previousTag = previousTag.AddTag(aTag);
        }
        /// <summary>
        /// Add TextTag to Tag List
        /// </summary>        
        protected void addTextTag(string text)
        {            
        	bool spaceStart = false;
        	bool spaceEnd = false;
        	
            if (text.EndsWith(" "))
            	spaceEnd = true;
            if (text.StartsWith(" "))
            	spaceStart = true;

            text = text.Trim();
            if (spaceStart) text = " " + text;
            if (spaceEnd) text = text + " ";
            
            text = Utils.DecodeSymbol(text);
            
            //if (text.Trim() != "")
            addTag(new TextTag(parentHtml, text));
        }
        /// <summary>
        /// Parse a string and return text before a tag, the tag and it's variables, and the string after that tag.
        /// </summary>
        protected static void readNextTag(string s, ref string beforeTag, ref string afterTag, ref string tagName, 
                                          ref string tagVars, char startBracket, char endBracket)
        {
            Int32 pos1 = s.IndexOf(startBracket);
            Int32 pos2 = s.IndexOf(endBracket);

            if ((pos1 == -1) || (pos2 == -1) || (pos2 < pos1))
            {
                tagVars = "";
                beforeTag = s;
                afterTag = "";
            }
            else
            {
                String tagStr = s.Substring(pos1 + 1, pos2 - pos1 - 1);
                beforeTag = s.Substring(0, pos1);
                afterTag = s.Substring(pos2 + 1, s.Length - pos2 - 1);

                Int32 pos3 = tagStr.IndexOf(' ');
                if ((pos3 != -1) && (tagStr != ""))
                {
                    tagName = tagStr.Substring(0, pos3);
                    tagVars = tagStr.Substring(pos3+1, tagStr.Length-pos3-1);
                }
                else
                {
                    tagName = tagStr;
                    tagVars = "";
                }

                if (tagName.StartsWith("!--"))
                {
                    if ((tagName.Length < 6) || (!(tagName.EndsWith("--"))))
                    {
                        Int32 pos4 = afterTag.IndexOf("-->");
                        if (pos4 != -1)
                            afterTag = afterTag.Substring(pos4 + 2, afterTag.Length-pos4-1);
                    }
                    tagName = "";
                    tagVars = "";
                }

            }    
        }     
        /// <summary>
        /// Parse a string and return text before a tag, the tag and it's variables, and the string after that tag.
        /// </summary>
       	private static void readNextTag(string s, ref string beforeTag, ref string afterTag, ref string tagName, ref string tagVars)
       	{
       		Int32 pos1 = s.IndexOf('[');
            Int32 pos2 = s.IndexOf(']');
            
       		if ((pos1 == -1) || (pos2 == -1) || (pos2 < pos1))
            {
                tagVars = "";
                beforeTag = s;
                afterTag = "";
            }
            else
            {
                String tagStr = s.Substring(pos1 + 1, pos2 - pos1 - 1);
                beforeTag = s.Substring(0, pos1);
                afterTag = s.Substring(pos2 + 1, s.Length - pos2 - 1);

                Int32 pos3 = tagStr.IndexOf('=');
                if ((pos3 != -1) && (tagStr != ""))
                {
                    tagName = tagStr.Substring(0, pos3);
                    tagVars = "value=" + tagStr.Substring(pos3+1, tagStr.Length-pos3-1);
                }
                else
                {
                    tagName = tagStr;
                    tagVars = "";
                }
            }    
       	}
       	
       	/// <summary>
        /// Recrusive paraser.
        /// </summary>        
        private void parseBBCode(ref string s)
        {
            string beforeTag="", afterTag="", tagName="", tagVar="";
            readNextTag(s, ref beforeTag, ref afterTag, ref tagName, ref tagVar);
            
//            if (beforeTag != "") { addTextTag(beforeTag); }
            //HACK: Always add a textTag
            if (beforeTag != "" || 
                (previousTag == null) 
                /*(!previousTag.EndTag())*/) { addTextTag(beforeTag); }
//            addTextTag(beforeTag);
            
            if (tagName != "")
            {
                Int32 tagID = Utils.LocateBBCode(Utils.RemoveFrontSlash(tagName));
                if (tagID != -1)
                {
                    Int32 tagLv = Defines.BuiltinBBCodes[tagID].tagLevel;
                    switch (Defines.BuiltinBBCodes[tagID].flags)
                    {
                        case HTMLFlag.TextFormat:
                            addTag(new FontStyleTag(parentHtml, tagName, PropertyList.FromString(tagVar),tagID, tagLv));
                            break;
                        case HTMLFlag.Region:
                            addTag(new RegionTag(parentHtml, tagName, PropertyList.FromString(tagVar), tagID, tagLv));
                            break;
                    }
                }
            }
            s = afterTag;
        }
        /// <summary>
        /// Parse Html
        /// </summary>        
        public override void Parse(TextReader reader)
        {
        	base.Parse(reader);            
        	previousTag = null;

        	
        	string input = reader.ReadLine();

            while (input != null)
            {
            	while (input != "")
            	{
            		parseBBCode(ref input);    
            	}
            	
            	Int32 tagID = Utils.LocateBBCode(Utils.RemoveFrontSlash("br"));
            	Int32 tagLv = Defines.BuiltinBBCodes[tagID].tagLevel;
            	addTag(new RegionTag(parentHtml, "br", PropertyList.FromString(""), tagID, tagLv));
            	      
            	input = reader.ReadLine();
            }
                
            
            parentHtml.RebuildTagPositionList();
        }
    	
    }
}

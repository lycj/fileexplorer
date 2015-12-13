using System;
using System.Collections;
using System.Drawing;
using System.Text;
using QuickZip.MiniHtml;

namespace QuickZip.MiniCss
{   
/// <summary>
/// Quick Zip MiniCss - Css Parser and Printer
/// </summary>
    public class MiniCss
    {
    	internal TagCssStyleDictionary styleList;
    	internal QuickZip.MiniHtml.MiniHtml parentHtml;
    		   
    	private void loadStyle(string key, string value, ref CurrentStateType curState,
    	                  	   bool loadNonInherited)
    	{
    		Int32 idx = Utils.LocateStyle(key.ToLower());
    		ArrayList aList;
    		fourSide f;
    		
    		switch (idx)
    		{
    			#region Style that not affected by loadNonInherit    			
    			case (-1): 
    				#region unknown (-1)    				
    				break;
    				#endregion
    			case (28): 
    				#region color (28)    				
    				curState.fntColor = Utils.WebColor2Color(value);    				
    				break;
    				#endregion
    			case (29):
    				#region cursor (29)    				
    				curState.cursor = Utils.StrCursor2CursorType(value);
    				break;
    				#endregion
    			case (33):
    				#region font-family (33)    				
    				aList = Utils.ExtractList(value,',');
    				string curFont = "";
    				for (Int32 i = aList.Count -1; i >= 0; i--)
    				{
    					string fntName = (string)aList[i];
    					if (parentHtml.documentOutput.FontExists(fntName))
    						curFont = fntName;	
    						
    				}
    				if (curFont != "")
    					curState.font = new Font(curFont, curState.font.Size, 
    						                         curState.font.Style);
    				break;
    				#endregion
    			case (34):
    				#region font-size (34)    				
    				curState.font = new Font(curState.font.FontFamily, 
    				                         Utils.StrSize2PixelSize(value, Defines.defaultFntSize),	
    				                         curState.font.Style);
    				break;
    				#endregion
    			case (35):
    				#region font-style (35)
    				if ((value == "italic") || (value == "oblique"))
    					curState.font = new Font(curState.font, curState.font.Style & FontStyle.Italic);
    				break;
    				#endregion
    			case (36):
    				#region font-variant (36)    				
    				curState.textTransform = Utils.StrFontVariant2TextTransformType(value);
    				break;
    				#endregion
    			case (37):
    				#region font-weight (37)    				
    				if ((value == "bold") || (value == "700") || (value == "600") || (value == "500") ||
    				    (value == "bolder") || (value == "800") || (value == "900"))    					
    					curState.font = new Font(curState.font, curState.font.Style & FontStyle.Bold);
    					else
    						if ((value == "400") || (value == "300") || (value == "200") || (value == "100") ||
    						    (value == "lighter") || (value == "normal"))
    						curState.font = new Font(curState.font, curState.font.Style | FontStyle.Bold);    					
    				break;
    				#endregion
    			case (45):
    				#region list-style-type (45)
    				curState.bulletType = Utils.StrBullet2BulletType(value);
    				break;
    				#endregion
    			case (71):
    				#region text-align (71)
    				curState.hAlign = Utils.StrAlign2Align(value);
    				break;
    				#endregion
    			case (74):
    				#region text-transform (74)    				
    				curState.textTransform = Utils.StrTransform2TextTransformType(value);
    				break;
    				#endregion
    			case (80):
    				#region width (80)    				
    				curState.width = Utils.StrSize2PixelSize(value, parentHtml.widthLimit);
    				curState.endPosition = curState.startPosition + curState.width;
    				break;
    				#endregion
    			#endregion
    		}
    		
     		if (loadNonInherited)
    		switch (idx)
    		{
    			#region Style affected by loadNonInherit
    			case (01):
    				#region background-color (01)    			
    				curState.bkColor = Utils.WebColor2Color(value);    			
					break;
					#endregion
				case (05):
					#region border (05)
					aList = Utils.ExtractList(value, ' ');					
					for (f = fourSide._default; f <= fourSide._bottom; f++)
					{
						if (aList.Count >= 1) curState.borderWidth[(Int32)f] = 
							Utils.StrSize2PixelSize((string)aList[0],1);
						if (aList.Count >= 2) curState.borderStyle[(Int32)f] =
							Utils.StrBorder2BorderType((string)aList[1]);
						if (aList.Count >= 3) curState.borderColor[(Int32)f] =
							Utils.WebColor2Color((string)aList[2]);
					}						     						
					break;
					#endregion
				case (06):
					#region border-color (06)					
					for (f = fourSide._default; f <= fourSide._bottom; f++)
						curState.borderColor[(Int32)f] = Utils.WebColor2Color(value);
					break;
					#endregion
				case (08):
					#region border-style (08)					
					for (f = fourSide._default; f <= fourSide._bottom; f++)
						curState.borderStyle[(Int32)f] = Utils.StrBorder2BorderType(value);
					break;
					#endregion
				case (25):
					#region border-width (25)					
					for (f = fourSide._default; f <= fourSide._bottom; f++)
						curState.borderWidth[(Int32)f] = Utils.StrSize2PixelSize(value,1);
					break;
					#endregion						
				case (38):
					#region height (38)
					curState.height = Utils.StrSize2PixelSize(value, Defines.defaultFntSize);
					break;
					#endregion
				case (39):
					#region left (39)					
					curState.position.X = 
						Utils.StrSize2PixelSize(value, curState.width);
					break;
					#endregion
				case (46):
					#region margin (46)					
					aList = Utils.ExtractList(value, ' ');
					curState.margin[(Int32)fourSide._top] = 0;
					curState.margin[(Int32)fourSide._right] = 0;
					curState.margin[(Int32)fourSide._bottom] = 0;
					curState.margin[(Int32)fourSide._left]  = 0;					
					
					if (aList.Count >= 1) curState.margin[(Int32)fourSide._top] = Utils.StrSize2PixelSize((string)aList[0],0);
					if (aList.Count >= 2) curState.margin[(Int32)fourSide._right] = Utils.StrSize2PixelSize((string)aList[1],0);
					if (aList.Count >= 3) curState.margin[(Int32)fourSide._bottom] = Utils.StrSize2PixelSize((string)aList[2],0);
					if (aList.Count >= 4) curState.margin[(Int32)fourSide._left] = Utils.StrSize2PixelSize((string)aList[3],0);					
					break;
					#endregion
				case (58):
					#region padding (58)					
					aList = Utils.ExtractList(value, ' ');
					curState.padding[(Int32)fourSide._top] = 0;
					curState.padding[(Int32)fourSide._right] = 0;
					curState.padding[(Int32)fourSide._bottom] = 0;
					curState.padding[(Int32)fourSide._left]  = 0;					
					
					if (aList.Count >= 1) curState.padding[(Int32)fourSide._top] = Utils.StrSize2PixelSize((string)aList[0],0);
					if (aList.Count >= 2) curState.padding[(Int32)fourSide._right] = Utils.StrSize2PixelSize((string)aList[1],0);
					if (aList.Count >= 3) curState.padding[(Int32)fourSide._bottom] = Utils.StrSize2PixelSize((string)aList[2],0);
					if (aList.Count >= 4) curState.padding[(Int32)fourSide._left] = Utils.StrSize2PixelSize((string)aList[3],0);					
					break;
					#endregion
				case (67):
					#region position (67)
					curState.positionStyle = Utils.StrPosition2PositionType(value);
					break;
					#endregion
				case (68):
					#region right (68)					
					curState.position.X = Utils.StrSize2PixelSize(value, curState.width);
					break;
					#endregion
				case (75):
					#region top (75)
					curState.position.Y = Utils.StrSize2PixelSize(value, curState.width);
					break;
					#endregion
				case (80):
					#region width (80)
					curState.width = Utils.StrSize2PixelSize(value, parentHtml.widthLimit);
					curState.endPosition = curState.startPosition + curState.width;
					break;
					#endregion					
				default:
				if ((idx >= 9) && (idx <= 12))					
					#region border-top, right, bottom, left (9-12)
					{
					aList = Utils.ExtractList(value, ' ');
					f = fourSide._default;
					switch (idx)
					{
						case(9):
							f = fourSide._top;
							break;
						case(10):
							f = fourSide._right;
							break;
						case(11):
							f = fourSide._bottom;
							break;
						case(12):
							f = fourSide._left;
							break;
					}
					if (aList.Count >= 1) curState.borderWidth[(Int32)f] = 
							Utils.StrSize2PixelSize((string)aList[0],1);
					if (aList.Count >= 2) curState.borderStyle[(Int32)f] =
							Utils.StrBorder2BorderType((string)aList[1]);
					if (aList.Count >= 3) curState.borderColor[(Int32)f] =
							Utils.WebColor2Color((string)aList[2]);					
					break;
					}
					#endregion	
				if ((idx >= 13) && (idx <= 16))	
					#region border-top, right, bottom, left-color (13-16)
					{
					f = fourSide._default;
					switch (idx)
					{
						case(13):
							f = fourSide._top;
							break;
						case(14):
							f = fourSide._right;
							break;
						case(15):
							f = fourSide._bottom;
							break;
						case(16):
							f = fourSide._left;
							break;
					}
					curState.borderColor[(Int32)f] = Utils.WebColor2Color(value);
					break;
					}
					#endregion
				if ((idx >= 17) && (idx <= 20))	
					#region border-top, right, bottom, left-style (17-20)
					{
					f = fourSide._default;
					switch (idx)
					{
						case(17):
							f = fourSide._top;
							break;
						case(18):
							f = fourSide._right;
							break;
						case(19):
							f = fourSide._bottom;
							break;
						case(20):
							f = fourSide._left;
							break;
					}
					curState.borderStyle[(Int32)f] = Utils.StrBorder2BorderType(value);
					break;
					}
					#endregion							                  
    			if ((idx >= 21) && (idx <= 24))	
					#region border-top, right, bottom, left-width (21-24)
    				{
					f = fourSide._default;
					switch (idx)
					{
						case(21):
							f = fourSide._top;
							break;
						case(22):
							f = fourSide._right;
							break;
						case(23):
							f = fourSide._bottom;
							break;
						case(24):
							f = fourSide._left;
							break;
					}    				
					curState.borderWidth[(Int32)f] = Utils.StrSize2PixelSize(value,1);
					break;
					}
					#endregion
				if ((idx >= 47) && (idx <= 50))	
					#region margin-top, right, bottom, left-width (47-50)
					{
					f = fourSide._default;
					switch (idx)
					{
						case(47):
							f = fourSide._top;
							break;
						case(48):
							f = fourSide._right;
							break;
						case(49):
							f = fourSide._bottom;
							break;
						case(50):
							f = fourSide._left;
							break;
					}
					curState.margin[(Int32)f] = Utils.StrSize2PixelSize(value,0);
					break;
					}
					#endregion	
				if ((idx >= 59) && (idx <= 62))	
					#region padding-top, right, bottom, left-width (59-62)
					{
					f = fourSide._default;
					switch (idx)
					{
						case(59):
							f = fourSide._top;
							break;
						case(60):
							f = fourSide._right;
							break;
						case(61):
							f = fourSide._bottom;
							break;
						case(62):
							f = fourSide._left;
							break;
					}
					curState.padding[(Int32)f] = Utils.StrSize2PixelSize(value,0);
					break;
					}
					#endregion
				break;
				#endregion
				
    		}    		
    	}
    	private void loadStyle(CssStyleType style, ref CurrentStateType curState,
    	                  	   bool loadNonInherited)
    	{
    		for (Int32 i = 0; i < style.styles.Count; i ++)
    		{
    			PropertyItemType item = style.styles[i];
    			loadStyle(item.key, item.value, ref curState, loadNonInherited);
    		}
    	}   
    	/// <summary>
    	/// add a style (e.g. p {color:silver;})
    	/// </summary>    	
    	public void AddCssStyle(string cssStyle)
    	{
    		styleList.AddCssStyle(cssStyle);
    	}
    	/// <summary>
    	/// Clear all styles
    	/// </summary>
    	public void ClearCssStyle()
    	{
    		styleList = new TagCssStyleDictionary();
    	}
    	/// <summary>
    	/// prinst the list of css styles
    	/// </summary>
    	public void PrintItems()
    	{
    		styleList.PrintItems();
    	}
    	
    	public string Css()
    	{
    		return styleList.Css();
    	}
		/// <summary>
		/// return true if applyTag match style
		/// </summary>
    	private bool matchTag(CssStyleType style, HtmlTag applyTag)
    	{
    		if (Utils.MatchCurrentTag(applyTag, style.styleTagName) &&
    		   (Utils.MatchParentTags(applyTag, style.parentTagName)))
    		    return true;
    		return false;    			    		
    	}
    	/// <summary>
    	/// update curState based on loaded cssStyle
    	/// </summary>
    	internal void ApplyCssStyle(HtmlTag applyTag, ref CurrentStateType curState, bool loadNonInherited)
    	{
    		if (applyTag.StartTag())
    		{    			    				
    			CssStyleList sList = styleList.ListAllCssStyle(applyTag.name);

    			foreach (CssStyleType style in sList)
    				if (matchTag(style, applyTag))
    				loadStyle(style, ref curState, loadNonInherited);
    			
    			if (applyTag.variables.Contains("style") && (applyTag.variables["style"].value != ""))
    			{
    				
    				ArrayList aList = Utils.DecodeCssStyle(applyTag.name + " {" +
    				                                       applyTag.variables["style"].value
    				                                      + "}");
    				
    				foreach (CssStyleType style in aList)
    				//if (matchTag(style, applyTag))
    					loadStyle(style, ref curState, loadNonInherited);
    				
//    				if (aList.Count > 0)
//    				{
//    					CssStyleType item = aList[0];
//    					foreach (CssStyleType style in item.styles)
//    						if (matchTag(style, applyTag))
//    							loadStyle(style, ref curState, loadNonInherited);
//    						
//    				}
    			}
    		}
    	}
    	/// <summary>
    	/// constructor
    	/// </summary>    	
    	public MiniCss(QuickZip.MiniHtml.MiniHtml mh) : base()
    	{
    		styleList = new TagCssStyleDictionary();
    		parentHtml = mh;
    	}
    	/// <summary>
    	/// destructor
    	/// </summary>    	
    	~MiniCss()
    	{
    		styleList = null;
    		parentHtml = null;
    	}
    	/// <summary>
    	/// parse css styles
    	/// </summary>    	
    	public void Parse(string css)
        {    		
    		parentHtml.parser.ParseCss(css);
        }
    	
    }
}

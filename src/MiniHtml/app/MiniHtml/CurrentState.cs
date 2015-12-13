using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace QuickZip.MiniHtml
{


    /// <summary>
    /// Represent the state of all tag (e.g. Bold for text) when reaching that location.
    /// </summary>
    public class CurrentStateType
    {
        private float origWidthLimit;

        public MiniHtml parentHtml;
        public bool stateChanged = true;
        public TagList stateTagList = new TagList();
        public tagStatusType state;

        private Font pFont = new Font(Defines.defaultFntName, Defines.defaultFntSize, FontStyle.Regular);
        public Font font { get { return pFont; } set { pFont = value; stateChanged = true; } }
        
        public bool visible;
        
        public bool isHeader, isScript;
        public bool subScript, supScript;
        public Color bkColor, fntColor, linkColor, activeColor, visitedColor, URLColor;
        
        public float startPosition, endPosition = 100;
        public Int32[] margin = new Int32[Enum.GetNames(typeof(fourSide)).Length];
        public Int32[] padding = new Int32[Enum.GetNames(typeof(fourSide)).Length];
        
        public Int32[] borderWidth = new Int32[Enum.GetNames(typeof(fourSide)).Length];
        public borderStyleType[] borderStyle = new borderStyleType[Enum.GetNames(typeof(fourSide)).Length];
        public Color[] borderColor = new Color[Enum.GetNames(typeof(fourSide)).Length];
        public Int32 borderSpacing;

        public Int32 height, width;
        public Point position;
        public positionStyleType positionStyle;
        
        public textTransformType textTransform;
        public Cursor cursor;
        public bulletStyleType bulletType;
        public string title;
        public string targetURL;
        public hAlignType hAlign;
        public vAlignType vAlign;
       
        public string formName, formAction;
        public formMethodType formMethod;
       
        //GetBorderSpace is allocSpace in previous version
        public float GetBorderSpace(fourSide f, bool includePadding)
        {
            Int32 side = (Int32)f;
            if (includePadding)
            { return padding[side] + margin[side] + borderWidth[side]; }
            else
            { return margin[side] + borderWidth[side]; }
        }
        public float GetBorderSpace(fourSide f) { return GetBorderSpace(f, true); }
        /// <summary>
        /// Remove margin and padding (if includePadding) from a rectangle
        /// </summary>
        //    |--------------|
        //    |              |
        //    |  |--------|  |
        //    |  | return |  |
        //    |  |  this  |  |
        //    |  |--------|  |
        //    |              |
        //    |--------------|
        
        public RectangleF DrawableRect(PointF basePos, RectangleF oldRect, bool includePadding)
        {
            return new RectangleF(
                basePos.X + oldRect.Left + GetBorderSpace(fourSide._left, includePadding),
                basePos.Y + oldRect.Top + GetBorderSpace(fourSide._top, includePadding),
                oldRect.Width - GetBorderSpace(fourSide._right, includePadding) - 
                    GetBorderSpace(fourSide._left, includePadding),
                oldRect.Height - GetBorderSpace(fourSide._bottom, includePadding) - 
                    GetBorderSpace(fourSide._top, includePadding));
        }
        /// <summary>
        /// Add margin/padding space to input rect.
        /// </summary>        
        //  |-------------|
        //  |   |---------|---|
        //  |---| Return this |
        //      |-------------|
        
        public RectangleF UpdateRect(RectangleF oldRect, bool includePadding)
        {
            return new RectangleF(
                oldRect.Left + GetBorderSpace(fourSide._left, includePadding),
                oldRect.Top + GetBorderSpace(fourSide._top, includePadding),
                oldRect.Width + GetBorderSpace(fourSide._right, includePadding),
                oldRect.Height  + GetBorderSpace(fourSide._bottom, includePadding));
        }

        /// <summary>
        /// Remove margin and padding (if includePadding) from a rectangle
        /// </summary>
        public RectangleF DrawableRect(RectangleF oldRect, bool includePadding)
        {
            return DrawableRect(new PointF(0, 0), oldRect, includePadding);
        }
        /// <summary>
        /// Remove margin and padding (if includePadding) from a rectangle
        /// </summary>
        public RectangleF DrawableRect(PointF basePos, RectangleF oldRect)
        {
            return DrawableRect(basePos, oldRect, true);
        }        
        /// <summary>
        /// Create a copy of current CurrentStateType object
        /// </summary>        
        public CurrentStateType Clone()
        {            
            CurrentStateType state = new CurrentStateType(parentHtml,origWidthLimit);
            state.origWidthLimit = this.origWidthLimit;
            state.stateChanged = false;
            state.stateTagList = new TagList();
            foreach (HtmlTag aTag in this.stateTagList)
                state.stateTagList.Add(aTag);
            state.state = this.state;

            state.font = (Font)(this.font.Clone());
            state.visible = this.visible;
            state.isHeader = this.isHeader;
            state.isScript = this.isScript;
            state.subScript = this.subScript;
            state.supScript = this.supScript;
            state.bkColor = this.bkColor;
            state.fntColor = this.fntColor;
            state.linkColor = this.linkColor;
            state.activeColor = this.activeColor;
            state.visitedColor = this.visitedColor;
            state.URLColor = this.URLColor;
            state.startPosition = this.startPosition;
            state.endPosition = this.endPosition;
            state.margin = (Int32[])this.margin.Clone();
            state.padding = (Int32[])this.padding.Clone();
            
            state.borderWidth = (Int32[])this.borderWidth.Clone();
            state.borderStyle = (borderStyleType[])this.borderStyle.Clone();
            state.borderColor = (Color[])this.borderColor.Clone();
            state.borderSpacing = this.borderSpacing;

            state.height = this.height;
            state.width = this.width;
            state.position = this.position;
            state.positionStyle = this.positionStyle;

            state.textTransform = this.textTransform;
            state.cursor = this.cursor;
            state.bulletType = this.bulletType;
            state.title = this.title;
            state.targetURL = this.targetURL;
            state.hAlign = this.hAlign;
            state.vAlign = this.vAlign;

            state.formName = this.formName;
            state.formAction = this.formAction;
            state.formMethod = this.formMethod;


            return state;
        }
        /// <summary>
        /// Initialize the current CurrentStateType object, required by UpdateState()
        /// </summary>
        public void InitState()
        {
            stateChanged = true;
            pFont = new Font(Defines.defaultFntName, Defines.defaultFntSize, FontStyle.Regular);
            targetURL = "";
            bkColor = Color.Transparent;
            fntColor = Color.Black;
            linkColor = Color.Red;
            activeColor = Color.DarkRed;
            visitedColor = Color.Purple;
            URLColor = Color.Blue;
            bulletType = bulletStyleType.Decimal;
            hAlign = hAlignType.Left;
            vAlign = vAlignType.Unknown;
            startPosition = 0;
            endPosition = origWidthLimit;
            isHeader = false;
            isScript = false;
            visible = true;
            subScript = false;
            supScript = false;
            formName = "";
            formAction = "";
            formMethod = formMethodType.Default;

            for (Int32 i = 0; i <= 4; i++)
            {
                margin[i] = 0;
                padding[i] = 0;                
                borderColor[i] = Color.Transparent;
                borderStyle[i] = borderStyleType.None;
                borderWidth[i] = 0;
            }
            position = new Point(0, 0);
            height = -1;
            width = -1;
            textTransform = textTransformType.None;
            positionStyle = positionStyleType.Static;
            cursor = Cursors.Default;
        }
        /// <summary>
        /// Add a tag to stateTagList
        /// </summary>        
        internal void AddTag(HtmlTag aTag)
        {
            stateTagList.Add(aTag);
            stateChanged = true;
        }
        /// <summary>
        /// Remove a tag from stateTagList, based on name
        /// </summary>        
        public void RemoveTag(string aTagName)
        {
            for (int i = stateTagList.Count - 1; i >= 0; i--)
            {
                if (stateTagList[i].name.ToLower() == aTagName.ToLower())
                {
                    stateTagList.Remove(stateTagList[i]);
                    stateChanged = true;
                    return;
                }
            }            
            
        }
        /// <summary>
        /// Update the specified state object, will run UpdateState() of every tag in stateTagList
        /// </summary>        
        public static void UpdateState(ref CurrentStateType curState)
        {
            if (!(curState.stateChanged)) { return; }
            curState.InitState();
            foreach (HtmlTag aTag in curState.stateTagList)
            { aTag.UpdateState(ref curState); }
            curState.stateChanged = false;
        }
        /// <summary>
        /// Return the color of text, based on URL and tagStatus
        /// </summary>        
        public Color TextColor()
        {
        	//TODO: URL Color is not affected by Css
            if (targetURL == "")
            { return fntColor; }

            switch (state)
            {
                case tagStatusType.Active:
                    return activeColor;
                case tagStatusType.Focused:
                    return linkColor;
                case tagStatusType.Normal:
                    return URLColor;
            }

            return URLColor;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public CurrentStateType(MiniHtml aMiniHtml, float aWidthLimit)
        {
            parentHtml = aMiniHtml;
            state = tagStatusType.Normal;
            endPosition = startPosition + aWidthLimit;
            origWidthLimit = aWidthLimit;
            stateChanged = false;
            InitState();
        }
        /// <summary>
        /// Return true if targetURL != ""
        /// </summary>        
        public bool isURL()
        {
            return (targetURL != "");
        }        
               
        public bool isTitle()
        {
        	return (isHeader && (stateTagList.IndexOf("title") != -1));
        }
    }
}

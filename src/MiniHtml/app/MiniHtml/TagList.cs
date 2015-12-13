using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace QuickZip.MiniHtml
{
    /// <summary>
    /// A List of HtmlTags
    /// </summary>
    public class TagList : CollectionBase
    {
        private HtmlTag getTag(Int32 aIndex)
        {
            return (HtmlTag)List[aIndex];
        }
        internal void setTag(Int32 aIndex, HtmlTag value)
        {
            List[aIndex] = value;
        }
        public void VerifyType(object value)
        {
            if (!(value is HtmlTag))
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
        ~TagList()
        {
            List.Clear();
        }
        internal Int32 Add(HtmlTag value)
        {
            List.Add(value);
            return List.Count - 1;
        }
        internal void Insert(Int32 index, HtmlTag value)
        {
        	if (index == -1)
        		List.Add(value);
        	else
            	List.Insert(index, value);
        }
        internal void Remove(HtmlTag value)
        {
        	if (Contains(value))
            	List.Remove(value);
        }
        internal bool Contains(HtmlTag value)
        {
            return List.Contains(value);
        }
        internal HtmlTag this[Int32 index]
        {
            get
            {
                return getTag(index);
            }
            set
            {
                setTag(index, value);
            }
        }
        internal Int32 IndexOf(HtmlTag value)
        {
            for (int i = 0; i < Count; i++)
                if (this[i] == value)
                    return i;
            return -1;
        }
        internal Int32 IndexOfType(HtmlTag value)
        {
        	Int32 retVal = 0;
            for (int i = 0; i < Count; i++)
            {
            	if ((this[i].name == value.name) &&
            	    (this[i].EndTag() == value.EndTag()) &&
            	    (this[i].StartTag() == value.StartTag()))
            	   		retVal += 1;
            	 if (this[i] == value)
                    return retVal;
            }                
            return -1;
        }

		public Int32 IndexOf(string value)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].name == value)
                    return i;
            return -1;
        } 
			
	    public Int32 CountName(string value)
		{
			Int32 retVal = 0;
			for (int i = 0; i < Count; i++)
                if (this[i].name == value)
                    retVal++;
			return retVal;
		}
		
        public void PrintItems()
        {
            foreach (HtmlTag item in this)
            {
                HtmlTag aTag = item;
                String k = "";
                while (aTag != null)
                {
                    aTag = aTag.parentTag;
                    k += " ";
                }
                Console.WriteLine(k + '[' + item.name + ']');
                item.childTags.PrintItems();
            }

        }

    }

}

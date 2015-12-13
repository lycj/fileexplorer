using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FileExplorer.WPF
{
    public class VirtualItemGenerator : IItemGeneratorHelper
    {        
        #region fields
        private IOCPanel _panel;        
        

        #endregion

        #region constructors

        public VirtualItemGenerator(IOCPanel panel)
        {
            _panel = panel;            
        }

        #endregion

        #region events

        #endregion

        #region properties

        private IItemContainerGenerator _generator { get { return _panel.ItemContainerGenerator; } }

        public UIElement this[int idx]
        {
            get { return getElement(idx); }
        }

        public int this[UIElement ele]
        {
            get { return getIndex(ele); }
        }

      
        #endregion

        #region methods

        public Dictionary<int, UIElement> Generate(int startIdx, int endIdx)
        {
            Dictionary<int, UIElement> retVal = new Dictionary<int, UIElement>();
            // Get the generator position of the first visible data item
            GeneratorPosition startPos = _generator.GeneratorPositionFromIndex(startIdx);
            
            /* Get index where we'd insert the child for this position. If the item is realized
               (position.Offset == 0), it's just position.Index, otherwise we have to add one to
               insert after the corresponding child
               Offset of Virtualized items start at 1 
               Offset of Realized items start at 0 
             */

            bool isVirtualized = startPos.Offset != 0; 
            int childIndex = isVirtualized ? 
                    startPos.Index + 1 :        //Insert to Position.
                    startPos.Index;             //Position Index

            using (_generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = startIdx; itemIndex <= endIdx; ++itemIndex, ++childIndex)
                {
                    bool newlyRealized;
                    UIElement child = _generator.GenerateNext(out newlyRealized) as UIElement;

                    if (newlyRealized)
                    {                        
                        _panel.addOrInsertInternalChild(childIndex, child);
                        _generator.PrepareItemContainer(child);
                        //(child as FrameworkElement).ApplyTemplate();
                    }
                    else Debug.Assert(child == _panel.getInternalChildren(childIndex), "Wrong child was generated");

                    retVal.Add(itemIndex, child); //To-Do: Measure Child here.
                }
            }

            return retVal;
        }

        public void CleanUp(int minDesiredGenerated, int maxDesiredGenerated)
        {
            int internalChildrenCount = _panel.getInternalChildren().Count;

            for (int i = internalChildrenCount - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = _generator.IndexFromGeneratorPosition(childGeneratorPos);
                if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
                {
                    _generator.Remove(childGeneratorPos, 1);
                    _panel.removeInternalChild(i);
                }
            }
        }

        private UIElement getElement(int idx)
        {
            var uiDic = Generate(idx, idx);
            if (!(uiDic.ContainsKey(idx)))
                throw new Exception("Generate() failed to create the container.");
            return uiDic[idx];            
        }

        private int getIndex(UIElement ele)
        {
            int idx = _panel.Children.IndexOf(ele);
            if (idx == -1)
                return -1;
            return _generator.IndexFromGeneratorPosition(new GeneratorPosition(idx, 0));
        }



        public Size Measure(int idx, Size availableSize)
        {
            UIElement ele = getElement(idx);
            ele.Measure(availableSize);
            return ele.DesiredSize;
        }

        public void Arrange(int idx, Rect finalRect)
        {
            UIElement ele = getElement(idx);
            ele.Arrange(finalRect);
        }

      
        #endregion

       
       

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    public static partial class ExtensionMethods
    {
        public static Point GetMiddlePoint(this IPositionAware model)
        {
            IResizable resizable = model as IResizable;
            if (resizable != null)
                return new Point(model.Left + (resizable.Width / 2), model.Top + (resizable.Height / 2));
            else return new Point(model.Left, model.Top);
        }

        public static void SetPosition(this IPositionAware model, Point position)
        {            
            model.Left = position.X;
            model.Top = position.Y;
        }

        /// <summary>
        /// Update model's Left and Top based on offset.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="offset"></param>
        public static void OffsetPosition(this IPositionAware model, Vector offset)
        {
            model.Left += offset.X;
            model.Top += offset.Y;
        }
    }
}

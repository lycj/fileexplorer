using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.Models;

namespace FileExplorer.Models
{
    public class Metadata : PropertyChangedBase, IMetadata, IEquatable<Metadata>
    {
        #region Cosntructor

        public Metadata(DisplayType displayType, string category, string header, object content, bool isEditable = false)
        {
            DisplayType = displayType;
            Category = category;
            _header = header;
            _content = content;
            _isEditable = isEditable;

            IsVisibleInStatusbar = false;
            IsVisibleInSidebar = false;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return String.Format("Metadata {0}-{1}={2}", Category, HeaderText, Content);
        }

        public bool Equals(Metadata other)
        {
            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            if (String.IsNullOrEmpty(HeaderText))
                return Content.GetHashCode() + Category.GetHashCode();
            return HeaderText.GetHashCode() + Category.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Metadata && Equals(obj as Metadata);
        }


        #endregion

        #region Data

        object _content;
        string _header;
        Func<object, bool> _valueChanged;
        private bool _isEditable;

        #endregion

        #region Public Properties

        public string Category { get; set; }
        public bool IsHeader { get; set; }
        public bool IsEditable { get { return _isEditable; } set { _isEditable = value; NotifyOfPropertyChange(() => IsEditable); } }
        public DisplayType DisplayType { get; set; }
        public bool IsVisibleInStatusbar { get; set; }
        public bool IsVisibleInSidebar { get; set; }
        public string HeaderText { get { return _header; } set { _header = value; NotifyOfPropertyChange(() => HeaderText); } }
        public object Content
        {
            get { return _content; }
            set
            {
                if (_valueChanged(value))
                    _content = value;
                NotifyOfPropertyChange(() => Content);
                
                
            }
        }

        #endregion




    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF.BaseControls
{
    public class NonsealedDataObject : IDataObject
    {
        private DataObject _dataObject = new DataObject();

        public virtual object GetData(string format, bool autoConvert)
        {
            return _dataObject.GetData(format, autoConvert);
        }

        public virtual object GetData(Type format)
        {
            return GetData(format.FullName, true);
        }

        public virtual object GetData(string format)
        {
            return GetData(format, true);
        }

        public virtual bool GetDataPresent(string format, bool autoConvert)
        {
            return _dataObject.GetDataPresent(format, autoConvert);
        }

        public virtual bool GetDataPresent(Type format)
        {
            return GetDataPresent(format.FullName, true);
        }

        public virtual bool GetDataPresent(string format)
        {
            return GetDataPresent(format, true);
        }

        public virtual string[] GetFormats(bool autoConvert)
        {
            return _dataObject.GetFormats(autoConvert);
        }

        public virtual string[] GetFormats()
        {
            return GetFormats(true);
        }

        public virtual void SetData(string format, object data, bool autoConvert)
        {
            _dataObject.SetData(format, data, autoConvert);
        }

        public virtual void SetData(Type format, object data)
        {
            SetData(format.FullName, data, true);
        }

        public virtual void SetData(string format, object data)
        {
            SetData(format, data, true);
        }

        public virtual void SetData(object data)
        {
            SetData(data.GetType(), data); 
        }
    }
}

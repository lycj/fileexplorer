using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.ComponentModel;
using System.IO;

namespace QuickZip.IO.PIDL.UserControls.Model
{

    public class FileModel : ExModel
    {
        #region Data        
        private long _length;
        #endregion

        #region Ctor
        public FileModel(FileInfoEx file)
            : base(file)
        {
            this._length = file.Length;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Length
        /// </summary>
        static PropertyChangedEventArgs lengthChangeArgs =
            ObservableHelper.CreateArgs<FileModel>(x => x.Length);

        public long Length
        {
            get { return _length; }
            set
            {
                _length = value;
                NotifyPropertyChanged(lengthChangeArgs);
            }
        }
        #endregion

    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;

namespace QuickZip.IO.PIDL.UserControls.Model
{

    public class DriveModel : DirectoryModel
    {
        #region Data  
              

        #endregion

        #region Ctor        
        //0.3
        public DriveModel(DirectoryInfoEx dir)
            : base(dir)
        {
            try
            {
                DriveInfo driveInfo = new DriveInfo(dir.FullName.Replace(":\\", ""));
                IsReady = driveInfo.IsReady;
                if (IsReady)
                {
                    VolumeLabel = driveInfo.VolumeLabel;
                    TotalSize = driveInfo.TotalSize;
                    FreeSpace = driveInfo.AvailableFreeSpace;
                    DriveType = driveInfo.DriveType;
                    DriveFormat = driveInfo.DriveFormat;
                    PercentFull = (int)((float)(TotalSize - FreeSpace) / (float)TotalSize * 100);
                }
            }
                //0.5
            catch (ArgumentException) //Drive not found
            {

            }
        }


        #endregion


        #region Public Properties

        public string DriveFormat { get; private set; }
        public string VolumeLabel { get; private set; }
        public int PercentFull { get; private set; }
        public long TotalSize { get; private set; }
        public long FreeSpace { get; private set; }
        public DriveType DriveType { get; private set; }
        public bool IsReady { get; private set; }
        
        #endregion

    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Cinch;
using System.Windows;
using QuickZip.Translation;
using QuickZip.UserControls.Input;

namespace QuickZip.UserControls.MVVM.Command.Model
{
    /// <summary>
    /// Used to toggle View Modes
    /// </summary>
    public class TogglePreviewModel : CommandModel
    {
        protected static string uriRootToolbarResources = uriRootThemeResources + "Toolbar/";

        #region Constructor

        public TogglePreviewModel(BaseToolbarViewModel rootModel)            
        {
            
                         
            _toggleOn = GetResourceBitmap(uriRootToolbarResources, "preview_on_16.png");
            _toggleOff = GetResourceBitmap(uriRootToolbarResources, "preview_off_16.png");

            Header = "";
            IsRightAligned = true;
            IsExecutable = true;
            setToggleBitmap(false);
            _rootModel = rootModel;
        }


        #endregion

        #region Methods

        public override void Execute(object param)
        {            
            _rootModel.IsPreviewerVisible = !_rootModel.IsPreviewerVisible;
            setToggleBitmap(_rootModel.IsPreviewerVisible);
        }

        private void setToggleBitmap(bool isToggled)
        {
            HeaderIcon = isToggled ? _toggleOn : _toggleOff;
        }

        #endregion

        #region Data

        Bitmap _toggleOn, _toggleOff;
        private BaseToolbarViewModel _rootModel;

        #endregion

        #region Public Properties

        #endregion


    }


}

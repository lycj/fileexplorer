using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.Model;

namespace QuickZip.UserControls.MVVM.ViewModel
{
    

    public class MetadataViewModel : Cinch.ViewModelBase

    {
        #region Constructor

        public MetadataViewModel(GenericMetadataModel embeddedModel)
        {
            EmbeddedModel = embeddedModel;
            IsDateTime = embeddedModel.PropertyType.Equals(typeof(DateTime));
            IsPercent = embeddedModel.PropertyType.Equals(typeof(short));
            IsNumber = embeddedModel.PropertyType.Equals(typeof(int)) ||
                embeddedModel.PropertyType.Equals(typeof(uint)) || 
                embeddedModel.PropertyType.Equals(typeof(long)) ||
                embeddedModel.PropertyType.Equals(typeof(ulong)) ||
                embeddedModel.PropertyType.Equals(typeof(float)) ||
                    embeddedModel.PropertyType.Equals(typeof(double))
                ;
            IsString = embeddedModel.PropertyType.Equals(typeof(String));
            IsStringArray = embeddedModel.PropertyType.Equals(typeof(String[]));
        }

        #endregion

        #region Methods

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public bool IsDateTime { get; private set; }
        public bool IsPercent { get; private set; }
        public bool IsNumber { get; private set; }
        public bool IsString { get; private set; }
        public bool IsStringArray { get; private set; }                
        public GenericMetadataModel EmbeddedModel { get; private set; }

        #endregion

    }
}

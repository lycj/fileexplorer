using FileExplorer.WPF.Utils;
using FileExplorer.Defines;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FileExplorer.Models
{

    public class SkyDriveMetadataProvider : MetadataProviderBase
    {
        public static string[] SupportedKeys = new string[]
        {
            "picture", "description", "link", "type", "size", "location", "camera_make", "camera_model",
            "focal_ratio", "focal_length", "exposure_numerator", "exposure_denominator", "when_taken",
            "width", "height"
        };

        public SkyDriveMetadataProvider()
            : base(new BasicMetadataProvider(), new FileBasedMetadataProvider())
        {

        }

        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();
            retList.AddRange(await base.GetMetadataAsync(selectedModels, modelCount, parentModel));

            if (selectedModels.Count() == 1)
            {
                var itemModel = selectedModels.First() as SkyDriveItemModel;
                if (itemModel.Metadata != null)
                {
                    var dictionary = itemModel.Metadata as IDictionary<string, object>;
                    if (dictionary.ContainsKey("width") && dictionary.ContainsKey("height"))
                    {
                        string dimension = String.Format("{0} x {1}", dictionary["width"], dictionary["height"]);
                        retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strImage, MetadataStrings.strDimension,
                                           dimension) { IsVisibleInSidebar = true });
                    }

                    if (dictionary.ContainsKey("picture"))
                    {
                        Uri pictureUrl = new Uri((string)dictionary["picture"]);
                        retList.Add(new Metadata(DisplayType.Image, MetadataStrings.strImage, MetadataStrings.strThumbnail,
                                          new BitmapImage(pictureUrl)) { IsVisibleInSidebar = true });
                    }
                       

                    //foreach (var key in SupportedKeys)
                    //{
                    //    if (dictionary.ContainsKey(key))
                    //    {
                    //        object val = dictionary[key];
                    //        if (val != null)
                    //            switch (key)
                    //            {
                    //                case "picture" : 
                    //                    retList.Add(new Metadata(DisplayType.Image, MetadataStrings.strImage, MetadataStrings.strThumbnail, 
                    //                        new BitmapImage(new Uri((string)val))) { IsVisibleInSidebar = true });
                    //                    break;
                    //                case "when_taken":
                    //                    retList.Add(new Metadata(DisplayType.TimeElapsed, "OneDrive", key, DateTime.Parse(val as string)) { IsVisibleInSidebar = true });
                    //                    break;
                    //                case "link":
                    //                    retList.Add(new Metadata(DisplayType.Link, "OneDrive", key, val) { IsVisibleInSidebar = true });
                    //                    break;
                    //                default:
                    //                    retList.Add(new Metadata(DisplayType.Auto, "OneDrive", key, val) { IsVisibleInSidebar = true });
                    //                    break;
                    //            }

                    //    }
                    //}
                }
            }


            return retList;
        }

    }
}

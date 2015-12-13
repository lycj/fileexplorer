using FileExplorer.WPF.Utils;
using ExifLib;
using FileExplorer.Defines;
using FileExplorer.IO.Defines;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using FileExplorer.IO;

namespace FileExplorer.Models
{
    public class ImageMetadataProvider : MetadataProviderBase
    {
        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();
            if (selectedModels.Count() == 1)
            {
                var diskModel = selectedModels.First() as DiskEntryModelBase;
                if (diskModel.IsFileWithExtension(FileExplorer.IO.Defines.FileExtensions.ImageExtensions))
                {
                    try
                    {
                        using (var stream = await diskModel.DiskProfile.DiskIO.OpenStreamAsync(diskModel,
                            FileExplorer.Defines.FileAccess.Read, CancellationToken.None))
                        using (Bitmap bitmap = new Bitmap(stream))
                        {
                            retList.Add(new Metadata(DisplayType.Image, MetadataStrings.strImage, MetadataStrings.strThumbnail,
                                W32ConverterUtils.ToBitmapImage(stream.ToByteArray())) { IsVisibleInSidebar = true });

                        }
                    }
                    catch { }
                }
            }
            return retList;
        }
    }


    public class ExifMetadataProvider : MetadataProviderBase
    {


        public static ExifTags[] RecognizedExifTags = new[]
            {
                ExifTags.ResolutionUnit, ExifTags.ExposureProgram, ExifTags.ISOSpeedRatings, ExifTags.Flash, 
                ExifTags.Orientation, ExifTags.PixelXDimension, ExifTags.PixelYDimension, 
                
                ExifTags.DateTime, ExifTags.DateTimeDigitized, ExifTags.DateTimeOriginal, 

                ExifTags.ApertureValue, 

                ExifTags.FNumber, ExifTags.FocalLength, ExifTags.XResolution, ExifTags.YResolution,

                ExifTags.ExposureTime
            };

        public override async Task<IEnumerable<IMetadata>> GetMetadataAsync(IEnumerable<IEntryModel> selectedModels, int modelCount, IEntryModel parentModel)
        {
            List<IMetadata> retList = new List<IMetadata>();


            if (selectedModels.Count() == 1)
            {

                #region addExifVal
                Action<ExifReader, ExifTags> addExifVal = (reader, tag) =>
                {
                    object val = null;
                    switch (tag)
                    {
                        case ExifTags.FNumber:
                        case ExifTags.FocalLength:
                        case ExifTags.XResolution:
                        case ExifTags.YResolution:
                            int[] rational;
                            if (reader.GetTagValue(tag, out rational))
                                val = rational[0];
                            break;
                        case ExifTags.DateTime:
                        case ExifTags.DateTimeDigitized:
                        case ExifTags.DateTimeOriginal:
                            if (reader.GetTagValue<object>(tag, out val))
                                val = DateTime.ParseExact((string)val, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                            break;
                        default:
                            reader.GetTagValue<object>(tag, out val);
                            break;
                    }

                    if (val != null)
                    {
                        DisplayType displayType = DisplayType.Auto;
                        switch (val.GetType().Name)
                        {
                            case "DateTime":
                                displayType = DisplayType.TimeElapsed;
                                break;
                            case "Double":
                            case "Float":
                                val = Math.Round(Convert.ToDouble(val), 2).ToString();
                                displayType = DisplayType.Text;
                                break;
                            default:
                                displayType = DisplayType.Text;
                                val = val.ToString();
                                break;
                        }
                        retList.Add(new Metadata(displayType, MetadataStrings.strImage, tag.ToString(),
                                 val) { IsVisibleInSidebar = true });
                    }
                };
                #endregion
                try
                {
                    var diskModel = selectedModels.First() as DiskEntryModelBase;
                    if (diskModel != null)
                        if (diskModel.IsFileWithExtension(FileExtensions.ExifExtensions))
                        {
                            using (var stream = await diskModel.DiskProfile.DiskIO.OpenStreamAsync(diskModel,
                                FileExplorer.Defines.FileAccess.Read, CancellationToken.None))
                            using (ExifReader reader = new ExifReader(stream))
                            {
                                var thumbnailBytes = reader.GetJpegThumbnailBytes();
                                if (thumbnailBytes != null && thumbnailBytes.Length > 0)
                                    retList.Add(new Metadata(DisplayType.Image, MetadataStrings.strImage, MetadataStrings.strThumbnail,
                                        W32ConverterUtils.ToBitmapImage(thumbnailBytes)) { IsVisibleInSidebar = true });
                                else retList.Add(new Metadata(DisplayType.Image, MetadataStrings.strImage, MetadataStrings.strThumbnail,
                                        W32ConverterUtils.ToBitmapImage(stream.ToByteArray())) { IsVisibleInSidebar = true });

                                UInt16 width, height;
                                if (reader.GetTagValue(ExifTags.PixelXDimension, out width) &&
                                    reader.GetTagValue(ExifTags.PixelYDimension, out height))
                                {
                                    string dimension = String.Format("{0} x {1}", width, height);
                                    retList.Add(new Metadata(DisplayType.Text, MetadataStrings.strImage, MetadataStrings.strDimension,
                                        dimension) { IsVisibleInSidebar = true });
                                }

                                //foreach (var tag in RecognizedExifTags)
                                //    addExifVal(reader, tag);
                            }
                        }
                }
                catch
                {
                    return AsyncUtils.RunSync(() => (new ImageMetadataProvider()
                        .GetMetadataAsync(selectedModels, modelCount, parentModel)));
                }
            }



            return retList;
        }

    }

}

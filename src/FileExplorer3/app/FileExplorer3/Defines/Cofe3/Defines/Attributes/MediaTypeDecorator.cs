using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FileExplorer.WPF.Utils;

namespace FileExplorer
{
    //http://blog.riaproject.com/tips/44.html
    public enum MediaType
    {
        [MIMEType("")]
        Unknown,

        [MIMEType("")]
        None,

        /// <summary>
        /// Represents COFE.Web.Search in xml.
        /// </summary>
        //[MIMEType("application/cofews.search+xml")]
        [MIMEType("application/json")]
        Search,

        /// <summary>
        /// Represents COFE.Web.Entry in xml.
        /// </summary>
        //[MIMEType("application/cofews.entries+xml")]
        [MIMEType("application/json")]
        Entry,

        /// <summary>
        /// Represents COFE.Web.EntryType in xml.
        /// </summary>
        [MIMEType("application/cofews.entrytype+xml")]
        EntryType,        

        /// <summary>
        /// Represents COFE.Web.EntryList in xml.
        /// </summary>
        //[MIMEType("application/cofews.entrylist+xml")]
        [MIMEType("application/json")]
        EntryList,

        /// <summary>
        /// Represents COFE.Web.Event in xml.
        /// </summary>
        [MIMEType("application/cofews.eventlist+xml")]
        EventList,

        /// <summary>
        /// Represents COFE.Web.ResourceList in xml, which contain a number of resource links.
        /// </summary>
        [MIMEType("application/cofews.resourcelist+xml")]
        ResourceList,

        /// <summary>
        /// Represents COFE.Web.Metadata in xml, which is data related to an entry or entry list.
        /// </summary>
        [MIMEType("application/cofews.metadata+xml")]
        Meta,        

        //Standard Types        

        [MIMEType("application/json", ".json")]
        Json,
        /// <summary>
        /// Represents Xml document.
        /// </summary>
        [MIMEType("application/xhtml+xml", ".xml")]
        Xml,

        /// <summary>
        /// Represents ms word document.
        /// </summary>
        [MIMEType("application/msword", ".docx", ".doc")]
        Word,
        
        //Image Types
        /// <summary>
        /// Represents a iconBitmap, iconBitmap is converted to png in web services.
        /// </summary>
        [MIMEType("image/bmp", ".bmp")]
        Bmp,

        /// <summary>
        /// Represents a JPEG.
        /// </summary>
        [MIMEType("image/jpeg", ".jpg", ".jpeg", ".jpe")]
        Jpeg,

        /// <summary>
        /// Represent a PNG.
        /// </summary>
        [MIMEType("image/png", ".png")]
        Png,

        /// <summary>
        /// Represents a file stream.
        /// </summary>
        [MIMEType("application/octet-stream")]
        Stream
    }

    public static class MediaTypeExtension
    {
        public static string ToMIMEStr(MediaType mediaType)
        {
            return AttributeUtils<MIMETypeAttribute>.FindAttribute(mediaType).MIMEType;
        }

        public static MediaType ToMediaType(string mimeStr)
        {
            foreach (var mt in Enum.GetValues(typeof(MediaType)))
                if (MediaTypeExtension.ToMIMEStr((MediaType)mt).Equals(mimeStr, StringComparison.CurrentCultureIgnoreCase))
                    return (MediaType)mt;
            return MediaType.Stream;
        }
    }
}

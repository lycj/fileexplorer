using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FileExplorer.WPF.Utils;

namespace FileExplorer
{
    /// <summary>
    /// Used to tag MediaType enum, to add MIME information.
    /// </summary>
    public class MIMETypeAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of the object in .Net.
        /// </summary>
        public Type ObjectType { get; private set; }

        /// <summary>
        /// Gets the MIME type of the MediaType.
        /// </summary>
        public string MIMEType { get; private set; }        

        /// <summary>
        /// Gets a list of extensions of the MediaType.
        /// </summary>
        public List<string> FileExtensions { get; private set; }

        public MIMETypeAttribute(string mimeType)
        {
            MIMEType = mimeType;
            ObjectType = default(Type);
            FileExtensions = new List<string>();
        }

        public MIMETypeAttribute(string mimeType, Type objectType)
            : this(mimeType)
        {            
            ObjectType = objectType;
        }

        public MIMETypeAttribute(string mimeType, params string[] fileExtensions)
            : this(mimeType)
        {
            FileExtensions = new List<string>(fileExtensions);
        }

        public MIMETypeAttribute(string mimeType, Type objectType, params string[] fileExtensions)
            : this(mimeType, objectType)
        {
            FileExtensions = new List<string>(fileExtensions);
        }
    }

}
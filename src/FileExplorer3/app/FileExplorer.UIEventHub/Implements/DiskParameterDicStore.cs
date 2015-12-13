using FileExplorer.Utils;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileExplorer
{
    public class DiskParameterDicStore : SerializableDictionary<string, object>, IParameterDicStore
    {       
        private string _fileName;
        public DiskParameterDicStore(string fileName)
            : base(StringComparer.CurrentCultureIgnoreCase)
        {
            _fileName = fileName;
            AsyncUtils.RunSync(() => LoadAsync());
        }

        ~DiskParameterDicStore()
        {
            AsyncUtils.RunSync(() => SaveAsync());
        }

        public async Task LoadAsync()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                _fileName);
            if (File.Exists(path))
            {
                Clear();
                using (var reader = new XmlTextReader(File.OpenRead(path)))
                {
                    reader.Read();
                    Debug.WriteLine(reader.LocalName);
                    base.ReadXml(reader);
                }
            }
        }

        public async Task SaveAsync()
        {
            string path = Path.Combine(
              Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
              _fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var writer = new XmlTextWriter(File.Create(path), Encoding.Unicode))
            {
                writer.WriteStartElement("Items");
                base.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public IParameterDicStore Clone()
        {
            return this;
        }
    }
}

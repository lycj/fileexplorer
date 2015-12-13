using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace QuickZip.Translation
{
    public class TranslatorInfo
    {
        public string Name { get; set; } 
        public string Email { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Version { get; set; }
        public string WebPage { get; set; }

        public TranslatorInfo()
        {
            Name = "Translator";
            Email = "";
            WebPage = "";
            LastUpdate = DateTime.UtcNow;
            Version = "5.1";
        }
    }

    public class Entry
    {
        public string OriginalValue { get; set; }
        public string CurrentValue { get; set; }
        public bool IsChanged { get { return OriginalValue != CurrentValue; } }

        public Entry(string originalValue)
        {
            OriginalValue = originalValue;
            CurrentValue = originalValue;
        }

        public static implicit operator string(Entry entry)
        {
            return entry.CurrentValue;
        }
    }

    public enum DictionaryMode { Unknown, V2 };

    public class TranslationDictionary
    {
        

        private DictionaryMode _dictionaryMode = DictionaryMode.Unknown;

        private Dictionary<string, Dictionary<string, Entry>> _data =
            new Dictionary<string, Dictionary<string, Entry>>();

        public string EnglishName { get; set; }
        public string CultureName { get; set; }
        public string Culture { get; set; }
        public DictionaryMode DictionaryMode { get { return _dictionaryMode; } private set { _dictionaryMode = value; } }
        public TranslatorInfo TranslatorInfo { get; private set; }

        public Dictionary<string, Dictionary<string, Entry>> Values { get { return _data; } }

        public TranslationDictionary()
        {
            TranslatorInfo = new TranslatorInfo();
        }

        

        #region Load and Save
        public bool Load(string path)
        {
            TranslatorInfo = new TranslatorInfo();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            switch (DictionaryMode = GetDictionaryMode(xmlDocument))
            {
                case DictionaryMode.V2:
                    return LoadV2(xmlDocument);
                default: return false;
            }
        }

        public void Save(Stream stream, DictionaryMode mode)
        {
            switch (mode)
            {
                case DictionaryMode.V2:
                    SaveV2(stream);
                    break;
            }
        }

        public void Save(string path, DictionaryMode mode)
        {
            using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write))
                Save(fs, mode);
        }

        void SaveV2(Stream stream)
        {
            XmlWriterSettings setting = new XmlWriterSettings()
            {
                Encoding = Encoding.Unicode,
                Indent = true,
                IndentChars = "     ",
                NewLineHandling = System.Xml.NewLineHandling.Replace,
                NewLineChars = Environment.NewLine
            };
            using (XmlWriter writer = XmlTextWriter.Create(stream, setting))
            {
                writer.WriteStartElement("Dictionary");
                writer.WriteAttributeString("EnglishName", EnglishName);
                writer.WriteAttributeString("CultureName", CultureName);
                writer.WriteAttributeString("Culture", Culture);

                writer.WriteStartElement("TranslatorInfo");
                writer.WriteElementString("Translator", TranslatorInfo.Name);
                writer.WriteElementString("Email", TranslatorInfo.Email);
                writer.WriteElementString("WebPage", TranslatorInfo.WebPage);
                writer.WriteElementString("LastUpdate", TranslatorInfo.LastUpdate.ToShortDateString());
                writer.WriteElementString("Version", TranslatorInfo.Version);
                writer.WriteEndElement();

                foreach (string id in _data.Keys)
                {
                    Dictionary<string, Entry> dic = _data[id];
                    writer.WriteStartElement("Value");
                    writer.WriteAttributeString("Id", id);

                    foreach (string dicKey in dic.Keys)
                        if (!String.Equals("id", dicKey, StringComparison.CurrentCultureIgnoreCase))
                            writer.WriteAttributeString(dicKey, dic[dicKey]);

                    writer.WriteEndElement();
                }


                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
            }
        }

        public void Clear()
        {
            _data.Clear();
        }

        bool LoadV2(XmlDocument xmlDocument)
        {
            EnglishName = xmlDocument.DocumentElement.GetAttribute("EnglishName");
            CultureName = xmlDocument.DocumentElement.GetAttribute("CultureName");
            Culture = xmlDocument.DocumentElement.GetAttribute("Culture");

            foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement element = node as XmlElement;
                    switch (element.Name)
                    {
                        case "TranslatorInfo":
                            foreach (XmlNode tNode in node.ChildNodes)
                            {
                                switch (tNode.Name)
                                {
                                    case "Translator": TranslatorInfo.Name = tNode.InnerText; break;
                                    case "Email": TranslatorInfo.Email = tNode.InnerText; break;
                                    case "WebPage": TranslatorInfo.WebPage = tNode.InnerText; break;
                                    case "LastUpdate":
                                        DateTime dt = DateTime.Now;
                                        if (DateTime.TryParse(tNode.InnerText, out dt))
                                            TranslatorInfo.LastUpdate = dt;
                                        break;
                                    case "Version": TranslatorInfo.Version = tNode.InnerText; break;
                                }
                            }
                            break;
                        case "Value":
                            string id = element.GetAttribute("Id");
                            if (!String.IsNullOrEmpty(id))
                            {
                                if (!_data.ContainsKey(id))
                                    _data.Add(id, new Dictionary<string, Entry>());

                                Dictionary<string, Entry> dic = _data[id];
                                foreach (XmlAttribute a in element.Attributes)
                                {
                                    if (dic.ContainsKey(a.Name))
                                        dic[a.Name].CurrentValue = a.Value;
                                    else dic.Add(a.Name, new Entry(a.Value));
                                }
                            }
                            break;
                    }
                }
            }
            //Debug.WriteLine(_data);

            return true;
        }
        #endregion


        public DictionaryMode GetDictionaryMode(XmlDocument xmlDocument)
        {
            try
            {
                if (xmlDocument.DocumentElement.Name == "Dictionary")
                    return DictionaryMode.V2;
            }
            catch { }

            return DictionaryMode.Unknown;
        }

        public string Translate(string uid, string vid, string defstring)
        {
            if (_data.ContainsKey(uid))
            {
                Dictionary<string, Entry> dic = _data[uid];
                if (dic.ContainsKey(vid))
                    return dic[vid];
            }
            return defstring;
        }
    }
}

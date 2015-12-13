using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace QuickZip.UserControls
{
    public static class SerializeHelper
    {
        public static bool Read<T>(string fileName, ref T instance, Type[] extraTypes)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            if (File.Exists(fileName))
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T), extraTypes);
                    using (TextReader reader = new StreamReader(fileName))
                    {
                        instance = (T)serializer.Deserialize(reader);
                    }

                    return true;
                }
                catch
                {                   
                }

            return false;
        }


        public static bool Write<T>(T instance, string fileName, Type[] extraTypes)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            string tempPath = fileName + ".tmp";

            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);

                XmlSerializer serializer = new XmlSerializer(typeof(T), extraTypes);
                TextWriter writer = new StreamWriter(tempPath);
                serializer.Serialize(writer, instance);
                writer.Close();

                File.Copy(tempPath, fileName, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool Read<T>(string fileName, ref T instance)
        {
            return Read<T>(fileName, ref instance, new Type[] { });
        }

        public static bool Write<T>(T instance, string fileName)
        {
            return Write<T>(instance, fileName, new Type[] { });
        }

    }
}

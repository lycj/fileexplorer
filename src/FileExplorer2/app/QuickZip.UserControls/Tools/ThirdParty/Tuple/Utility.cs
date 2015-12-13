using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace Visualstudiomagazine
{
    //http://stackoverflow.com/questions/1822687/c-3-0-tuple-equivalents-for-poor-men
    //http://visualstudiomagazine.com/articles/2007/04/01/generics-move-beyond-collections.aspx?sc_lang=en
    public static class Utility
    {
        public static T Min<T>(T first, T second)
            where T : IComparable<T>
        {
            return (first.CompareTo(second) < 0) ?
                first : second;
        }

        public static T Max<T>(T first, T second)
            where T : IComparable<T>
        {
            return (first.CompareTo(second) < 0) ?
                second : first;
        }

        public static void Swap<T>(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }

        public static T CreateFromFile<T>(string pathName)
        where T : new()
        {
            T rVal = new T();
            if (!File.Exists(pathName))
                return rVal;

            FileStream fsStorage = null;
            XmlReader reader = null;

            try
            {
                fsStorage = new FileStream(pathName,
                    FileMode.Open);
                reader = new XmlTextReader(fsStorage);
                XmlSerializer serializer =
                    new XmlSerializer(typeof(T));
                rVal = (T)serializer.Deserialize(reader);
            }
            catch (System.InvalidOperationException)
            {
                // The file was not a valid XML file. 
                // Return a default object.
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (fsStorage != null)
                    fsStorage.Close();
            }
            return rVal;
        }

        public static void SaveToFile<T>(T value,
            string pathName)
        {
            using (TextWriter writer = new
                StreamWriter(pathName))
            {
                XmlSerializer serializer =
                    new XmlSerializer(typeof(T));
                serializer.Serialize(writer, value);
            }
        }

    }

}

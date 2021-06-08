using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

#pragma warning disable 0162, 0414, 0219

namespace SecPlayerPrefs
{
    public class SecureDataManager<T> where T : new()
    {
        private T stats;
        private string key;

        public SecureDataManager(string filename)
        {
            this.key = filename;
            stats = Load();
        }

        public T Get()
        {
            return stats;
        }

        private T Load()
        {
            if (!SecurePlayerPrefs.HasKey(key))
                return new T();

            string data = SecurePlayerPrefs.GetString(key);

            T loadedPO = DeserializeObject(data);

            return loadedPO;
        }

        public void Save(T stats)
        {
            string serializedData = SerializeObject(stats);
            SecurePlayerPrefs.SetString(key, serializedData);
            SecurePlayerPrefs.Save();
        }

        private string SerializeObject(T pObject)
        {
            string XmlizedString = null;
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xs.Serialize(xmlTextWriter, pObject);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());

            return XmlizedString;
        }

        private T DeserializeObject(string pXmlizedString)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            return (T)xs.Deserialize(memoryStream);
        }

        private static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        private static byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

    }
}
#pragma warning restore 0162, 0414, 0219

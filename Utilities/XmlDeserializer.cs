using System.IO;
using System.Xml.Serialization;

namespace Mapper
{
    public static class XmlDeserializer
    {
        public static T LoadXml<T>(string fileName)
        {
            using (var xml = new StreamReader(fileName))
            {
                var serializer = new XmlSerializer(typeof(T));
                var t = (T)serializer.Deserialize(xml);
                return t;
            }
        }
    }
}

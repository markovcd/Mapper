using System.IO;
using System.Xml.Serialization;

namespace Mapper.Utilities
{
    public static class EntitySerializer
    {
    	public static XmlSerializer Create<T>()
    	{
    		return new XmlSerializer(typeof(T));
    	}
    	
    	public static T Deserialize<T>(string fileName)
        {
            using (var xml = new StreamReader(fileName))
            {
            	var serializer = Create<T>();
                var t = (T)serializer.Deserialize(xml);
                return t;
            }
        }
        
        public static void Serialize<T>(T entity, string fileName)
        {
        	using (var xml = new StreamWriter(fileName))
            {
        		var serializer = Create<T>();
        		serializer.Serialize(xml, entity);
        	}
        }
    }
}

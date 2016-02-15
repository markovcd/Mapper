using System.Xml.Serialization;

namespace Mapper.Entities
{
    public class ContentMapping : Mapping
    {
        [XmlAttribute]
        public string Content { get; set; }

        public string GetValue()
        {
            return Content;
        }
    }
}

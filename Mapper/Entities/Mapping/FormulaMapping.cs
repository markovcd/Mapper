using System.Xml.Serialization;

namespace Mapper.Entities
{
	public class FormulaMapping : Mapping
    {
        [XmlAttribute]
        public string Formula { get; set; }

        public string GetValue()
        {
            return Formula;
        }
    }
}

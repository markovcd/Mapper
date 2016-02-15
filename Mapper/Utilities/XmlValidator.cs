using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Mapper.Utilities
{
    public static class XmlValidator
    {
        private static Stream LoadResource(string fileName, string nameSpace = "Mapper.Schemas")
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(nameSpace + "." + fileName);
        }

        private static XmlSchemaSet LoadSchema(Stream xsd)
        {
            var reader = XmlReader.Create(xsd);
            var set = new XmlSchemaSet();
            set.Add("", reader);
            return set;
        }

        private static Stream LoadMappingResource()
        {
            return LoadResource("MappingSchema.xsd");
        }

        private static Stream LoadConfigResource()
        {
            return LoadResource("ConfigSchema.xsd");
        }

        private static void Validate(string xmlPath, XmlSchemaSet schema)
        {
            var xml = XDocument.Load(xmlPath);
            xml.Validate(schema, ValidateEventHandler);
        }

        private static void ValidateEventHandler(object s, ValidationEventArgs e)
        {
            throw e.Exception;
        }

        public static void ValidateMapping(string xmlPath)
        {
            var mappingSchema = LoadSchema(LoadMappingResource());
            Validate(xmlPath, mappingSchema);
        }

        public static void ValidateConfig(string xmlPath)
        {
            var configResource = LoadSchema(LoadConfigResource());
            Validate(xmlPath, configResource);
        }
    }
}

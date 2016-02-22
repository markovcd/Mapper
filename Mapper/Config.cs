using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using Mapper.Entities;
using Mapper.Utilities;

namespace Mapper
{
    /// <summary>
    /// Description of Config.
    /// </summary>  
    public class Config : ICloneable
	{
        [XmlAttribute]
        public string Name { get; set; }

        [XmlIgnore]
		public DateTime? From { get; set; }

        [XmlAttribute(AttributeName = "From", DataType = "date")]
        public DateTime FromSerializable
        {
            get { return From.Value; }
            set { From = value; }
        }

        public bool ShouldSerializeFromSerializable()
        {
            return From.HasValue;
        }

        [XmlIgnore]
        public DateTime? To { get; set; }

        [XmlAttribute(AttributeName = "To", DataType = "date")]
        public DateTime ToSerializable
        {
            get { return To.Value; }
            set { To = value; }
        }

        public bool ShouldSerializeToSerializable()
        {
            return To.HasValue;
        }

        [XmlAttribute]
		public string ConfigPath { get; set; }
		
		[XmlAttribute]
		public string SourcePath { get; set; }

        [XmlAttribute]
        public string TemplatePath { get; set; }
		
		[XmlAttribute]
		public string TargetPath { get; set; }
		
		[XmlAttribute]
		public bool Append { get; set; }
		
		public void Execute()
		{
		    Assert();

            var file = File.LoadXml(ConfigPath);
            if (!string.IsNullOrEmpty(TemplatePath)) file.Name = TemplatePath;

            var mapper = new ExcelMapper(SourcePath, TargetPath, file, Append);
		    mapper.FileAdding += (s, e) => Console.Write(e.FilePath);
		    mapper.FileAdded += (s, e) => Console.WriteLine('.');
            mapper.AddFiles(From.Value, To.Value);
			mapper.Dispose();
		}

        [DebuggerNonUserCode]
        private void Assert()
        {
            if (!From.HasValue) throw new ArgumentNullException("From");
            if (!To.HasValue) throw new ArgumentNullException("To");
            if (string.IsNullOrEmpty(ConfigPath)) throw new ArgumentNullException("ConfigPath");
            if (string.IsNullOrEmpty(SourcePath)) throw new ArgumentNullException("SourcePath");
            if (string.IsNullOrEmpty(TargetPath)) throw new ArgumentNullException("TargetPath");
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Config Clone()
        {
            return (Config)MemberwiseClone();
        }
	}

    [XmlRoot(Namespace = "http://mapper.com/configs", ElementName = "Configs")]
    public class Configs : List<Config>, ICloneable
    {
		public void Execute()
		{
			foreach (var config in this) config.Execute();
		}
		
		static public Configs LoadXml(string filePath)
		{
            XmlValidator.ValidateConfig(filePath);
            return EntitySerializer.Deserialize<Configs>(filePath);
		}
		
		public void SaveXml(string filePath)
		{
			EntitySerializer.Serialize(this, filePath);
		}
		
        public Configs(IEnumerable<Config> configs) : base(configs) { }

        public Configs() { }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Configs Clone()
        {
            return new Configs(this.Select(c => c.Clone()));
        }
    }
}

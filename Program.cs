using System;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using System.Linq;

namespace Mapper
{
	class Program
	{			
		public static void Main(string[] args)
		{
			var cfg = args.Any() ? args[0] : Console.ReadLine();
			DateTime date;
			
			if (cfg.Equals("-month", StringComparison.CurrentCultureIgnoreCase))
				cfg = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
			
			if (DateTime.TryParseExact(cfg, "yyyy-MM", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
				cfg = "Configs\\" + cfg + ".xml";
			
			if (!System.IO.File.Exists(cfg))
				cfg = "Configs\\" + cfg;
			
			if (!System.IO.File.Exists(cfg))
				cfg = cfg + ".xml";
			    
			using(var xml = new StreamReader(cfg))
			{
				var serializer = new XmlSerializer(typeof(ConfigList));
				var config = (ConfigList)serializer.Deserialize(xml);
				config.Execute();
			}
		}
    }	
}
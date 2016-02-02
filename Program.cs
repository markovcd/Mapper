using System;
using System.Collections.Generic;
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
			var cfg = args.Any() ? args[0] : "config.xml";
			DateTime date;
			
			if (cfg.Equals("-month", StringComparison.CurrentCultureIgnoreCase))
				cfg = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
			
			if (DateTime.TryParseExact(cfg, "yyyy-MM", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
				cfg = "Configs\\" + cfg + ".xml";
			    
			using(var xml = new StreamReader(cfg))
			{
				var serializer = new XmlSerializer(typeof(ConfigList));
				var config = (ConfigList)serializer.Deserialize(xml);
				config.Execute();
			}
		}
    }
	
	
}
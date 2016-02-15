using System;
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
			    			
			ConfigList.LoadXml(cfg).Execute();			
		}
    }	
}
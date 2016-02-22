using System;
using System.Globalization;
using System.Linq;
using Mapper;

namespace MapperConsole
{
	class Program
	{			
		public enum Operation
		{
			None, AppendLastDay, AppendLastMonth
		}
		
		public static void Main(string[] args)
		{
			var cfg = args.Any() ? args[0] : Console.ReadLine();
			DateTime date;
			var o = Operation.None;
			
			if (cfg.Equals("-day", StringComparison.CurrentCultureIgnoreCase))
			{
				cfg = "append";
				o = Operation.AppendLastDay;
			}
			
			if (cfg.Equals("-month", StringComparison.CurrentCultureIgnoreCase))
			{
				cfg = "append";
				o = Operation.AppendLastMonth;
			}
			
			if (DateTime.TryParseExact(cfg, "yyyy-MM", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
				cfg = "Configs\\" + cfg + ".xml";
			
			if (!System.IO.File.Exists(cfg))
				cfg = "Configs\\" + cfg;
			
			if (!System.IO.File.Exists(cfg))
				cfg = cfg + ".xml";
			   
			if (o == Operation.AppendLastDay)
				Configs.LoadXml(cfg).LastDay().Execute();
			else if (o == Operation.AppendLastMonth)
				Configs.LoadXml(cfg).LastMonth().Execute();
			else
				Configs.LoadXml(cfg).Execute();		
		}
    }	
}
using System;
using System.Linq;
using Mapper;

namespace MapperConsole
{
	class Program
	{
		public static void Main(string[] args)
		{
			var cfg = args.Any() ? args[0] : Console.ReadLine();	
			
			if (!System.IO.File.Exists(cfg))
				cfg = "Configs\\" + cfg;
			
			if (!System.IO.File.Exists(cfg))
				cfg = cfg + ".xml";
			   
			Configs.LoadXml(cfg).Execute();
		}
    }	
}
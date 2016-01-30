using System;

namespace Mapper
{
	class Program
	{
		public static void Main(string[] args)
		{
			var f = File.LoadXml(@"Mappings\Biała2.xml");

            var m = new FileConstructor(@"C:\Users\Arek\Desktop\mapper\Baza", "biala.xlsx", f);

			var d1 = DateTime.Parse("2015-12-01");
			var d2 = DateTime.Parse("2015-12-31");
			
			m.AddFiles(d1, d2);
			m.Dispose();
		}

        public File GenerateData()
        {
            throw new NotImplementedException();

        }
    }
	
	
}
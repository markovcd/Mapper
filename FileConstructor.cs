using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Mapper
{
	/// <summary>
	/// Description of FileConstructor.
	/// </summary>
	public class FileConstructor : IDisposable
	{
		private readonly ExcelPackage output;
		private readonly File file;
		private readonly Dictionary<string, int> lastRows;
	
		public string SourceDirectory { get; private set; }
		public string TargetPath { get; private set; }	
		
		public FileConstructor(string sourceDir, string targetPath, File file, bool append = false)
		{
			SourceDirectory = sourceDir;
			TargetPath = targetPath;
	
			this.file = file;
		    

			output = new ExcelPackage(new FileInfo(file.Name));

            lastRows = InitSampleRows(append);
		}
		
		private Dictionary<string, int> InitSampleRows(bool append = false)
		{
			var d = new Dictionary<string, int>();
	
			foreach (var card in file.Cards)
			foreach (var sample in card.Samples)
			{	
				var identifier = sample.GetIdentifier();
				if (d.ContainsKey(identifier)) continue;
				var row = append ? FindLastRow(sample) : card.TargetFirstRow;
				d.Add(identifier, row);
			}					
			
			return d;
		}
		
		private void UpdateSampleRows(Card card)
		{
			var max = card.Samples
			              .GroupBy(s => s.Page)
						  .ToDictionary(
						      g => g.Key, 
						  	  g => g.Max(s => lastRows[s.GetIdentifier()]));
			
			foreach (var sample in card.Samples)
				lastRows[sample.GetIdentifier()] = max[sample.Page];
		}
		

		public int FindLastRow(Sample sample)
		{
			var worksheet = sample.GetOutputWorksheet(output.Workbook);

            for (var row = worksheet.Dimension.Rows; row > sample.Card.TargetFirstRow; row--)
                if (!sample.Card.IsTargetRowEmpty(row, worksheet))
                    return row;
			
			return sample.Card.TargetFirstRow;
		}
			
		
		public void AddFiles(DateTime from, DateTime to)
		{		
			foreach (var f in file.InputFileInfo.ConstructPaths(SourceDirectory, from, to))
                AddFile(f.Key, f.Value);
		}
		
		public void AddFile(DateTime date, string filePath)
		{
            var inputFile = new FileInfo(filePath);
            if (!inputFile.Exists) throw new FileNotFoundException(string.Format("Nie znaleziono pliku {0}", filePath));

            using (var input = new ExcelPackage(inputFile))
            {               
                Console.WriteLine(filePath);

                foreach (var card in file.Cards)
                    AddCard(card, input.Workbook, date);
            }
		}
	
		public void AddCard(Card card, ExcelWorkbook inputWorkbook, DateTime date)
		{
			var inputWorksheet = card.GetInputWorksheet(inputWorkbook);

            UpdateSampleRows(card);

            foreach (var sample in card.Samples)
				AddSample(sample, inputWorksheet, date);
		}
	
		public void AddSample(Sample sample, ExcelWorksheet inputWorksheet, DateTime date)
		{
			var outputWorksheet = sample.GetOutputWorksheet(output.Workbook);

		    var from = sample.GetSourceFromNumber();
            var to = sample.GetSourceToNumber();
		    var count = to - from + 1;

            foreach (var i in Enumerable.Range(from, count).Where(i => !sample.IsSourceEmpty(i, inputWorksheet)))
                AddSample(sample, inputWorksheet, outputWorksheet, date, i);
		}

	    public void AddSample(Sample sample, ExcelWorksheet inputWorksheet, ExcelWorksheet outputWorksheet, DateTime date, int index)
	    {
            if (GetLastRow(sample) > sample.Card.TargetFirstRow)
                AddNextRow(sample, outputWorksheet);

            FillDate(date, outputWorksheet, sample);

            foreach (var mapping in sample.Mappings)
                AddMapping(mapping, inputWorksheet, outputWorksheet, index);

	        IncreaseLastRow(sample);
	    }

        public int GetLastRow(Sample sample)
		{
			return lastRows[sample.GetIdentifier()];
		}

	    public void IncreaseLastRow(Sample sample)
	    {
	        lastRows[sample.GetIdentifier()]++;
	    }

	    public void AddNextRow(Sample sample, ExcelWorksheet worksheet)
		{	   
            if (!sample.Card.IsTargetRowEmpty(GetLastRow(sample), worksheet)) return;

            worksheet.InsertRow(GetLastRow(sample), 1, sample.Card.TargetFirstRow);
            ExcelHelper.AddConditionalFormattingRow(worksheet);
        }
	
		public void FillDate(DateTime date, ExcelWorksheet worksheet, Sample sample)
		{
		    sample.Card.GetDateCell(GetLastRow(sample), worksheet).Value = date;
		}
	
		public void AddMapping(Mapping mapping, ExcelWorksheet inputWorksheet, ExcelWorksheet outputWorksheet, int index)
		{
			var target = mapping.GetTargetCell(GetLastRow(mapping.Sample), outputWorksheet);
			
			var contentMapping = mapping as ContentMapping;
			var movableMapping = mapping as MovableMapping;
			var cellMapping = mapping as CellMapping;
			
			if (contentMapping != null)
				target.Value = contentMapping.GetValue();
			else if (movableMapping != null)
				target.Value = movableMapping.GetValue(index, inputWorksheet);
			else if (cellMapping != null)
				target.Value = cellMapping.GetValue(inputWorksheet);
		}
		
		private void Protect()
		{
			var worksheets = file.Cards.SelectMany(c => c.Samples)
				                       .Select(s => s.GetOutputWorksheet(output.Workbook));
			
			foreach (var worksheet in worksheets) 
			{
				worksheet.Protection.SetPassword(file.Password);
				worksheet.Protection.AllowAutoFilter = true;
				worksheet.Protection.IsProtected = true;
			}
		}

        public void Dispose()
		{
        	if (!string.IsNullOrEmpty(file.Password)) Protect();
        	
        	output.SaveAs(new FileInfo(TargetPath));
            output.Dispose();
		}
	}
}

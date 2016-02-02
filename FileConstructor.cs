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
		private readonly Dictionary<Sample, int> lastRows;
	
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
		
		private Dictionary<Sample, int> InitSampleRows(bool append = false)
		{
			var d = new Dictionary<Sample, int>();
	
			foreach (var card in file.Cards)
			foreach (var sample in card.Samples)
			{	
				if (d.ContainsKey(sample)) continue;
				var row = append ? FindLastRow(sample) : card.TargetFirstRow;
				d.Add(sample, row);
			}					
			
			return d;
		}
		
		private void UpdateSampleRows(Card card)
		{
			var max = card.Samples.Max(s => GetLastRow(s));
			              
			foreach (var sample in card.Samples)
				lastRows[sample] = max;
		}
		

		public int FindLastRow(Sample sample)
		{
			var worksheet = sample.Card.GetTargetWorksheet(output.Workbook);

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
            
            if (!inputFile.Exists) 
            	throw new FileNotFoundException(string.Format("Nie znaleziono pliku {0}", filePath));

            using (var source = new ExcelPackage(inputFile))
            {               
                Console.WriteLine(filePath);

                foreach (var card in file.Cards)
                    AddCard(card, source.Workbook, date);
            }
		}
	
		public void AddCard(Card card, ExcelWorkbook sourceWorkbook, DateTime date)
		{
			var targetWorksheet = card.GetTargetWorksheet(output.Workbook);

            UpdateSampleRows(card);

            foreach (var sample in card.Samples)
				AddSample(sample, sample.GetSourceWorksheet(sourceWorkbook), targetWorksheet, date);
		}
	
		public void AddSample(Sample sample, ExcelWorksheet sourceWorksheet, ExcelWorksheet targetWorksheet, DateTime date)
		{
		    var from = sample.GetSourceFromNumber();
            var to = sample.GetSourceToNumber();
		    var count = to - from + 1;

            foreach (var i in Enumerable.Range(from, count).Where(i => !sample.IsSourceEmpty(i, sourceWorksheet)))
                AddSample(sample, sourceWorksheet, targetWorksheet, date, i);
		}

	    public void AddSample(Sample sample, ExcelWorksheet sourceWorksheet, ExcelWorksheet targetWorksheet, DateTime date, int index)
	    {
            if (GetLastRow(sample) > sample.Card.TargetFirstRow)
                AddNextRow(sample, targetWorksheet);

            FillDate(date, targetWorksheet, sample);

            foreach (var mapping in sample.Mappings)
                AddMapping(mapping, sourceWorksheet, targetWorksheet, index);

	        IncreaseLastRow(sample);
	    }

        public int GetLastRow(Sample sample)
		{
			return lastRows[sample];
		}

	    public void IncreaseLastRow(Sample sample)
	    {
	        lastRows[sample]++;
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
	
		public void AddMapping(Mapping mapping, ExcelWorksheet sourceWorksheet, ExcelWorksheet targetWorksheet, int index)
		{
			var target = mapping.GetTargetCell(GetLastRow(mapping.Sample), targetWorksheet);
			
			var contentMapping = mapping as ContentMapping;
			var movableMapping = mapping as MovableMapping;
			var cellMapping = mapping as CellMapping;
			
			if (contentMapping != null)
				target.Value = contentMapping.GetValue();
			else if (movableMapping != null)
				target.Value = movableMapping.GetValue(index, sourceWorksheet);
			else if (cellMapping != null)
				target.Value = cellMapping.GetValue(sourceWorksheet);
		}
		
		private void Protect()
		{
			var worksheets = file.Cards.Select(c => c.GetTargetWorksheet(output.Workbook));
			
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

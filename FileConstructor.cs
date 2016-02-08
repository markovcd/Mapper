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

        private int FindLastRow(Sample sample)
		{
			var worksheet = sample.Card.GetTargetWorksheet(output.Workbook);

            for (var row = worksheet.Dimension.Rows; row > sample.Card.TargetFirstRow; row--)
                if (!sample.Card.IsTargetRowEmpty(row, worksheet))
                    return row;
			
			return sample.Card.TargetFirstRow;
		}

        private int GetLastRow(Sample sample)
        {
            return lastRows[sample];
        }

        private void IncreaseLastRow(Sample sample)
        {
            lastRows[sample]++;
        }

        private void FillDate(DateTime date, ExcelWorksheet worksheet, Sample sample)
        {
            sample.Card.GetDateCell(GetLastRow(sample), worksheet).Value = date;
        }

        private void AddNextRow(Sample sample, ExcelWorksheet worksheet)
        {
            if (!sample.Card.IsTargetRowEmpty(GetLastRow(sample), worksheet)) return;

            worksheet.InsertRow(GetLastRow(sample), 1, sample.Card.TargetFirstRow);
            ExcelHelper.AddConditionalFormattingRow(worksheet);
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

        private void AddCard(Card card, ExcelWorkbook sourceWorkbook, DateTime date)
		{
			var targetWorksheet = card.GetTargetWorksheet(output.Workbook);
        	
            UpdateSampleRows(card);

            foreach (var v in GetCard(card, sourceWorkbook, date))
            	AddSample(v.Key, v.Value, targetWorksheet);
		}
        
        private void AddSample(Sample sample, IEnumerable<SampleEntry> entries, ExcelWorksheet targetWorksheet)
        {
        	foreach (var entry in entries) 
        	{
        		if (GetLastRow(sample) > sample.Card.TargetFirstRow)
                AddNextRow(sample, targetWorksheet);

	            FillDate(entry.Date, targetWorksheet, sample);
	            
	            AddMapping(entry, targetWorksheet);
	
	            IncreaseLastRow(sample);
        	}	
        }
        
        private void AddMapping(SampleEntry entry, ExcelWorksheet targetWorksheet)
        {
        	foreach (var m in entry.Entries)
            {
        		var target = m.Mapping.GetTargetCell(GetLastRow(entry.Sample), targetWorksheet);
                target.Value = m.Value;
            }
        }
        
        private static Dictionary<Sample, IEnumerable<SampleEntry>> GetCard(Card card, ExcelWorkbook sourceWorkbook, DateTime date)
		{
      		Func<IGrouping<Sample, SampleEntry>, IEnumerable<SampleEntry>> sort = 
      			g => card.Order == Order.ByDates ? g.OrderBy(s => s.Date).AsEnumerable() : g.AsEnumerable();
            
            return card.Samples.SelectMany(s => GetSamples(s, sourceWorkbook, date))
            	       .GroupBy(s => s.Sample)
            		   .ToDictionary(g => g.Key, sort);        
		}
    
	    private static IEnumerable<SampleEntry> GetSamples(Sample sample, ExcelWorkbook sourceWorkbook, DateTime date)
	    {
	    	var singleSheetSample = sample as SingleSheetSample;
            var dateSheetSample = sample as DateSheetSample;

            if (singleSheetSample != null) 
            	return GetSingleSheetSamples(singleSheetSample, sourceWorkbook, date);
            if (dateSheetSample != null) 
            	return GetDateSheetSamples(dateSheetSample, sourceWorkbook, date);
            
            throw new InvalidOperationException("Nieznany rodzaj próbki.");
	    }
	    
	    private static IEnumerable<SampleEntry> GetSingleSheetSamples(SingleSheetSample sample, ExcelWorkbook sourceWorkbook, DateTime date)
		{
		    var sourceWorksheet = sample.GetSourceWorksheet(sourceWorkbook);

            var from = sample.GetSourceFromNumber();
            var to = sample.GetSourceToNumber();

			return Enumerable.Range(from, to - from + 1)
				             .Where(i => !sample.IsSourceEmpty(i, sourceWorksheet))
					  		 .Select(i => new SampleEntry(sample, sourceWorksheet, date, i));				
		}

	    private static IEnumerable<SampleEntry> GetDateSheetSamples(DateSheetSample sample, ExcelWorkbook sourceWorkbook, DateTime date)
	    {
	        //if (file.InputFileInfo.Period == Period.Daily)
	            //throw new InvalidOperationException("Typ DateSheetSample nie jest obsługiwany w pliku wejściowym dziennym.");

	        var from = new DateTime(date.Year, date.Month, 1);
            var to = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            return new DateEnumerable(Period.Daily, from, to).Where(d => !sample.IsSourceEmpty(d, sourceWorkbook))
            	                                             .Select(d => new SampleEntry(
            													sample,
            	                       							sample.GetSourceWorksheet(d, sourceWorkbook), 
            	                       							d, -1));
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

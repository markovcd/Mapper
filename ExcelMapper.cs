﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Mapper
{
    public class FileEventArgs : EventArgs
    {
        public string FilePath { get; private set; }

        public FileEventArgs(string filePath)
        {
            FilePath = filePath;
        }
    }

    public class CardEventArgs : EventArgs
    {
        public Card Card { get; private set; }

        public CardEventArgs(Card card)
        {
            Card = card;
        }
    }

    /// <summary>
    /// Description of ExcelMapper.
    /// </summary>
    public class ExcelMapper : IDisposable
	{
		private readonly ExcelPackage output;
		private readonly File file;
		private readonly Dictionary<Sample, int> lastRows;
	
		public string SourceDirectory { get; private set; }
		public string TargetPath { get; private set; }

	    public event EventHandler<FileEventArgs> FileAdding;
        public event EventHandler<FileEventArgs> FileAdded;
        public event EventHandler<CardEventArgs> CardAdding;
        public event EventHandler<CardEventArgs> CardAdded;

        public ExcelMapper(string sourceDir, string targetPath, File file, bool append = false)
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
            var firstRow = sample.Card.TargetFirstRow;
            var lastRow = GetLastRow(sample);

            if (!sample.Card.IsTargetRowEmpty(lastRow, worksheet)) return;

            foreach (var column in sample.Card.TargetColumns)
                worksheet.Cells[column + lastRow].StyleID = worksheet.Cells[column + firstRow].StyleID;

            ExcelHelper.AddConditionalFormattingRow(worksheet);
        }

        public void AddFiles(DateTime from, DateTime to)
		{		
			foreach (var f in file.InputFileInfo.ConstructPaths(SourceDirectory, from, to))
                AddFile(f.Value, f.Key);
		}
		
		public void AddFile(string filePath, DateTime date)
		{
            var inputFile = new FileInfo(filePath);
            
            if (!inputFile.Exists) 
            	throw new FileNotFoundException(string.Format("Nie znaleziono pliku {0}", filePath));

            using (var source = new ExcelPackage(inputFile))
                AddFile(source, date);
		}

        public void AddFile(ExcelPackage source, DateTime date)
	    {
            OnFileAdding(new FileEventArgs(source.File.FullName));

            foreach (var card in file.Cards)
                AddCard(card, source, date);

            OnFileAdded(new FileEventArgs(source.File.FullName));
        }

        public void AddCard(Card card, ExcelPackage source, DateTime date)
		{
            OnCardAdding(new CardEventArgs(card));

            var targetWorksheet = card.GetTargetWorksheet(output.Workbook);
        	
            UpdateSampleRows(card);

            foreach (var v in card.GetCard(source.Workbook, date))
            	AddSamples(v.Key, v.Value, targetWorksheet);

            OnCardAdded(new CardEventArgs(card));
        }
        
        private void AddSamples(Sample sample, IEnumerable<SampleEntry> entries, ExcelWorksheet targetWorksheet)
        {
        	foreach (var entry in entries) 
        	{
        		if (GetLastRow(sample) > sample.Card.TargetFirstRow)
                AddNextRow(sample, targetWorksheet);

	            FillDate(entry.Date, targetWorksheet, sample);
	            
	            AddMappings(entry, targetWorksheet);
	
	            IncreaseLastRow(sample);
        	}	
        }
               
        private void AddMappings(SampleEntry entry, ExcelWorksheet targetWorksheet)
        {
        	foreach (var m in entry.Entries)
        	{
        	    var target = m.Mapping.GetTargetCell(GetLastRow(entry.Sample), targetWorksheet);

        	    if (m.IsFormula)  
        	        target.Formula = m.Value.ToString();     	   
        	    else
        	    	target.Value = m.Mapping.IsDateColumnMapping() ? m.ToDate() : m.Value;
        	}
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

	    protected virtual void OnFileAdding(FileEventArgs e)
	    {
	        if (FileAdding != null)
                FileAdding.Invoke(this, e);
	    }

        protected virtual void OnFileAdded(FileEventArgs e)
        {
            if (FileAdded != null)
                FileAdded.Invoke(this, e);
        }

        protected virtual void OnCardAdding(CardEventArgs e)
        {
            if (CardAdding != null)
                CardAdding.Invoke(this, e);
        }

        protected virtual void OnCardAdded(CardEventArgs e)
        {
            if (CardAdded != null)
                CardAdded.Invoke(this, e);
        }
    }
}
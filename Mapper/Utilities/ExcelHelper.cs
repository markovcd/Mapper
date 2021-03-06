﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;

namespace Mapper.Utilities
{
    public static class ExcelHelper
    {
        private class Address
        {
            public string Column { get; private set; }
            public int Row { get; private set; }

            public Address(string column, int row)
            {
                Column = column;
                Row = row;
            }

            public Address AddRow()
            {
                return new Address(Column, Row + 1);
            }

            public static Address Parse(string address)
            {
                var s = new string(address.Where(c => !char.IsDigit(c)).ToArray());
                var i = int.Parse(new string(address.Where(char.IsDigit).ToArray()));
                return new Address(s, i);
            }

            public override string ToString()
            {
                return Column + Row;
            }
        }

        private class Range
        {
            public Address From { get; private set; }
            public Address To { get; private set; }

            public Range(Address from, Address to)
            {
                From = from;
                To = to;
            }

            public Range AddRow()
            {
                return new Range(From, To.AddRow());
            }

            public static Range Parse(string range)
            {
                var split = range.Split(':').ToList();
                if (split.Count == 1) split.Add(split.First());

                var a1 = Address.Parse(split[0]);
                var a2 = Address.Parse(split[1]);

                return new Range(a1, a2);
            }

            public override string ToString()
            {
                return From + ":" + To;
            }
        }

        public static void AddConditionalFormattingRow(ExcelWorksheet worksheet)
        {
            foreach (var node in GetConditionalFormattingNodes(worksheet))
                AddConditionalFormattingRow(node);
        }

        private static void AddConditionalFormattingRow(XmlNode node)
        {
            var attr = node.Attributes["sqref"];
            attr.Value = AddConditionalFormattingRow(attr.Value);
        }

        private static string AddConditionalFormattingRow(string ranges)
        {
            return ranges.Split(' ')
                         .Select(Range.Parse)
                         .Select(r => r.AddRow().ToString())
                         .Aggregate((s1, s2) => s1 + " " + s2);
        }

        private static IEnumerable<XmlNode> GetConditionalFormattingNodes(ExcelWorksheet worksheet)
        {
            return
                worksheet.WorksheetXml.DocumentElement.ChildNodes.Cast<XmlNode>()
                    .Where(n => n.Name.Equals("conditionalFormatting"));
        }
        
        public static int ColumnLetterToInt(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException();
            if (char.IsDigit(columnName[0])) return int.Parse(columnName);

            columnName = columnName.ToUpperInvariant();

            var sum = 0;

            foreach (var t in columnName)
            {
                sum *= 26;
                sum += (t - 'A' + 1);
            }

            return sum;
        }

        public static bool IsValuePresent(ExcelRange cell)
        {
            if (cell == null || cell.Value == null) return false;
            if ((cell.Value is string) && (string.IsNullOrWhiteSpace(cell.Value as string) || string.IsNullOrEmpty(cell.Value as string)))
                return false;

            return true;
        }
        
        public static bool IsXlsx(string filePath)
        {
        	return Path.GetExtension(filePath).Equals(".xlsx", StringComparison.CurrentCultureIgnoreCase);
        }

        public static string ConvertToXlsx(string filePath, Excel.Application excel = null)
        {
            excel = excel ?? CreateExcel();

            var workbook = excel.Workbooks.Open(filePath, UpdateLinks: false, ReadOnly: true);
        	
        	var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
     
        	workbook.SaveAs(path, FileFormat: Excel.XlFileFormat.xlOpenXMLWorkbook, 
        	                AccessMode: Excel.XlSaveAsAccessMode.xlNoChange);
        	
        	workbook.Close(false);
        	excel.Quit();
        	
        	return path;
        }

        public static Excel.Application CreateExcel()
        {
            return new Excel.Application
            {
                Visible = false,
                AskToUpdateLinks = false,
                DisplayAlerts = false,
                AutomationSecurity = MsoAutomationSecurity.msoAutomationSecurityForceDisable,
                FileValidation = MsoFileValidationMode.msoFileValidationSkip
            };
        }

        public static DateTime ToDate(object value, string[] formats = null)
        {
        	if (value == null) return DateTime.MinValue;
        	if (value is DateTime) return (DateTime)value;
            if (value is double) return DateTime.FromOADate((double)value);

            if (!(value is string)) return (DateTime) Convert.ChangeType(value, TypeCode.DateTime);

            formats = formats ?? new[] { "yyyy-MM-dd" };
            
            DateTime date;

            if (DateTime.TryParseExact((string)value, formats, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out date))
                return date;
           
            throw new FormatException(string.Format("Nie można zinterpretować daty {0}", value));
            

        }
    }
}

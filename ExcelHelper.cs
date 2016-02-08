using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using OfficeOpenXml;

namespace Mapper
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

        public static string ConvertToXlsx(string xlsPath)
        {
            var excel = @"c:\Program Files (x86)\Microsoft Office\Office12\excelcnv.exe";
            var xlsxPath = Path.GetTempFileName() + ".xlsx";
            var process = Process.Start(excel, string.Format(@" -nme -oice {0} {1}", xlsPath, xlsxPath));
            process.WaitForExit();
            return xlsxPath;
        }

        public static DateTime ToDate(object value)
        {
            if (value is DateTime) return (DateTime)value;
            if (value is double) return DateTime.FromOADate((double)value);

            return (DateTime)Convert.ChangeType(value, TypeCode.DateTime);
        }
    }
}

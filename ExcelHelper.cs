using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using OfficeOpenXml;

namespace Mapper
{
    public static class ExcelHelper
    {
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

        private static string AddConditionalFormattingRow(string range)
        {
            var r = range.Split(':').Select(ParseRangeString).ToList();
            if (r.Count == 1) r.Add(r.First());
            r[r.Count - 1] = Tuple.Create(r[r.Count - 1].Item1, r[r.Count - 1].Item2 + 1);
            return r.Select(t => t.Item1 + t.Item2).Aggregate((r1, r2) => r1 + ":" + r2);
        }

        private static Tuple<string, int> ParseRangeString(string range)
        {
            var s = new string(range.Where(c => !char.IsDigit(c)).ToArray());
            var i = Int32.Parse(new string(range.Where(char.IsDigit).ToArray()));
            return Tuple.Create(s, i);
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
            if ((cell.Value is string) && (string.IsNullOrWhiteSpace(cell.Value as string) || String.IsNullOrEmpty(cell.Value as string)))
                return false;

            return true;
        }
    }
}

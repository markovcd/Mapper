using System;
using System.Linq;
using OfficeOpenXml;

namespace Mapper
{
    public class ColumnSample : SingleSheetSample
    {
        public override int GetSourceFromNumber()
        {
            int i;
            if (int.TryParse(SourceFrom, out i)) return i;
            throw new FormatException("Wartość w SourceFrom w ColumnSample musi być liczbą.");
        }

        public override int GetSourceToNumber()
        {
            int i;
            if (int.TryParse(SourceTo, out i)) return i;
            throw new FormatException("Wartość w SourceTo w ColumnSample musi być liczbą.");
        }

        public override bool IsSourceEmpty(int row, ExcelWorksheet worksheet)
        {
            return Mappings.OfType<ColumnMapping>()
                           .Select(m => m.IsSourceValuePresent(row, worksheet))
                           .All(b => !b);
        }
    }
}

using System.Linq;
using Mapper.Utilities;
using OfficeOpenXml;

namespace Mapper.Entities
{
    /// <summary>
    /// Description of RowSample.
    /// </summary>
    public class RowSample : SingleSheetSample
    {
        public override int GetSourceFromNumber()
        {
            return ExcelHelper.ColumnLetterToInt(SourceFrom);
        }

        public override int GetSourceToNumber()
        {
            return ExcelHelper.ColumnLetterToInt(SourceTo);
        }

        public override bool IsSourceEmpty(int column, ExcelWorksheet worksheet)
        {
            return Mappings.OfType<RowMapping>()
                           .Select(m => m.IsSourceValuePresent(column, worksheet))
                           .All(b => !b);
        }
    }
}

using OfficeOpenXml;

namespace Mapper
{
    public abstract class MovableMapping : Mapping
    {
        public abstract ExcelRange GetSourceCell(int index, ExcelWorksheet worksheet);

        public object GetValue(int index, ExcelWorksheet worksheet)
        {
            return GetSourceCell(index, worksheet).Value;
        }

        public bool IsSourceValuePresent(int index, ExcelWorksheet worksheet)
        {
            return !IsIgnorable && ExcelHelper.IsValuePresent(GetSourceCell(index, worksheet));
        }
    }
}

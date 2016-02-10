using System;
using OfficeOpenXml;

namespace Mapper
{
    public class MappingEntry
    {
        public Mapping Mapping { get; private set; }
        public object Value { get; private set; }
        public bool IsFormula { get; private set; }

        public MappingEntry(Mapping mapping, ExcelWorksheet sourceWorksheet, int index)
        {
            var contentMapping = mapping as ContentMapping;
            var movableMapping = mapping as MovableMapping;
            var cellMapping = mapping as CellMapping;
            var formulaMapping = mapping as FormulaMapping;

            if (contentMapping != null)
                Value = contentMapping.GetValue();
            else if (movableMapping != null)
                Value = movableMapping.GetValue(index, sourceWorksheet);
            else if (cellMapping != null)
                Value = cellMapping.GetValue(sourceWorksheet);
            else if (formulaMapping != null)
            {
                IsFormula = true;
                Value = formulaMapping.GetValue();
            }
            else
                throw new InvalidOperationException("Nieznany rodzaj próbki.");

            Mapping = mapping;
        }
    }
}

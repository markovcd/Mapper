using System;
using OfficeOpenXml;

namespace Mapper
{
    public class MappingEntry
    {
        public Mapping Mapping { get; private set; }
        public object Value { get; private set; }

        public MappingEntry(Mapping mapping, ExcelWorksheet sourceWorksheet, int index)
        {
            var contentMapping = mapping as ContentMapping;
            var movableMapping = mapping as MovableMapping;
            var cellMapping = mapping as CellMapping;

            object value;

            if (contentMapping != null)
                value = contentMapping.GetValue();
            else if (movableMapping != null)
                value = movableMapping.GetValue(index, sourceWorksheet);
            else if (cellMapping != null)
                value = cellMapping.GetValue(sourceWorksheet);
            else
                throw new InvalidOperationException("Nieznany rodzaj próbki.");

            Mapping = mapping;
            Value = value;
        }
    }
}

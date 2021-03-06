﻿using System.Xml.Serialization;
using Mapper.Utilities;
using OfficeOpenXml;

namespace Mapper.Entities
{
    /// <summary>
	/// Description of CellMapping.
	/// </summary>
	public class CellMapping : Mapping
    {
        [XmlAttribute]
        public string SourceAddress { get; set; }

        public ExcelRange GetSourceCell(ExcelWorksheet worksheet)
        {
            return worksheet.Cells[SourceAddress];
        }

        public object GetValue(ExcelWorksheet worksheet)
        {
            return GetSourceCell(worksheet).Value;
        }

        public bool IsSourceValuePresent(ExcelWorksheet worksheet)
        {
            return !IsIgnorable && ExcelHelper.IsValuePresent(GetSourceCell(worksheet));
        }
    }
}

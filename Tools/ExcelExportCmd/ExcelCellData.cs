using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExportCmd
{
    public class ExcelCellData
    {
        public int RowNum { get; }

        public int ColNum { get; }

        public string VariableName { get; }

        public string VariableType { get; }

        public string ExportGroup { get; }

        public string Comment { get; }

        public ExcelCell Cell { get; }

        public ExcelCellData(ExcelCell cell,int rowNum, int colNum, string variableName, string variableType, string exportGroup, string comment)
        {
            Cell = cell;
            RowNum = rowNum;
            ColNum = colNum;
            VariableName = variableName;
            VariableType = variableType;
            ExportGroup = exportGroup;
            Comment = comment;
        }
    }
}

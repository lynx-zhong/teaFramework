using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExportCmd
{
    /// <summary>
    /// 单页Excel 数据
    /// </summary>
    public class ExcelSheetData
    {
        ExcelWorksheet excelSheet;

        public Dictionary<int, HeaderData> HeaderDic = new Dictionary<int, HeaderData>();

        public ExcelSheetData(ExcelWorksheet excelSheet)
        {
            this.excelSheet = excelSheet;

            AddHeaderData();
        }

        void AddHeaderData() 
        {
            HeaderDic.Clear();

            ExcelRow excelWorksheet = excelSheet.Rows[1];

            for (int i = 0; i < excelWorksheet.AllocatedCells.Count; i++)
            {
                Console.WriteLine(excelWorksheet.AllocatedCells[i]);
            }
        }

        public void GetRowDataList() 
        {

        }
    }

    public class HeaderData 
    {
        public string variableNameDic;
        public string variableTypeDic;
        public string variableDescriptionDic;
    }

    public class RowData 
    {
        
    }

    public class CellData 
    {
        public string VariableName;
        public string VariableType;
        public string VariableValue;
    }
}
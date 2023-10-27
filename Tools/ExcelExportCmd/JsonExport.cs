using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExportCmd
{
    internal class JsonExport : ExportCtrl
    {
        public static void Export(ExcelSheetData sheetData,string filePath) 
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("[");

            Dictionary<int, Dictionary<int, ExcelCellData>> cellConent = sheetData.GetCellConent();

            foreach (var rowInfo in cellConent)
            {
                Dictionary<int, ExcelCellData> rowData = rowInfo.Value;

                stringBuilder.AppendLine("{");

                foreach (var item in rowData)
                {
                    ExcelCellData cell = item.Value;

                    stringBuilder.AppendLine($"\"{cell.VariableName}\":\"{cell.Cell.Value}\"");
                }

                stringBuilder.AppendLine("},");
            }

            stringBuilder.AppendLine("]");

            string fileName = sheetData.GetFileName();
            string jsonPath = Path.Combine(filePath, fileName + ".json");

            WriteFile(stringBuilder, jsonPath);
        }
    }
}

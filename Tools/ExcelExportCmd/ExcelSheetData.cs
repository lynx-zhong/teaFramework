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
        string fileName;

        Dictionary<int, string> variableNameDic = new Dictionary<int, string>();

        Dictionary<int, string> variableTypeDic = new Dictionary<int, string>();

        Dictionary<int, string> exportGroupDic = new Dictionary<int, string>();

        Dictionary<int, string> commentDic = new Dictionary<int, string>();

        Dictionary<int, Dictionary<int, ExcelCellData>> cellContent = new Dictionary<int, Dictionary<int, ExcelCellData>>();

        public ExcelSheetData(ExcelWorksheet worksheet)
        {
            fileName = worksheet.Name;

            CollectHeaderData(worksheet);

            CollectCellData(worksheet);

            Console.WriteLine();
        }

        void CollectHeaderData(ExcelWorksheet worksheet)
        {
            for (int rowNum = 0; rowNum < worksheet.Rows.Count; rowNum++)
            {
                ExcelRow row = worksheet.Rows[rowNum];
                ExcelCell firstCell = row.AllocatedCells[0];

                if (firstCell.Value == null)
                    continue;

                var fristStr = firstCell.Value.ToString();
                if (string.IsNullOrEmpty(fristStr))
                    continue;

                Dictionary<int, string> targetDic;
                if (fristStr.StartsWith(ExportDefine.ExportVariableNameTag))
                    targetDic = variableNameDic;
                else if (fristStr.StartsWith(ExportDefine.ExportVarialeTypeTag))
                    targetDic = variableTypeDic;
                else if (fristStr.StartsWith(ExportDefine.ExportGroupTag))
                    targetDic = exportGroupDic;
                else if (fristStr.StartsWith(ExportDefine.ExportCommentTag))
                    targetDic = commentDic;
                else
                    continue;

                for (int colNum = 1; colNum < row.AllocatedCells.Count; colNum++)
                {
                    ExcelCell cell = row.AllocatedCells[colNum];

                    if (cell.Value == null)
                        continue;

                    var cellStr = cell.Value.ToString();
                    if (string.IsNullOrEmpty(cellStr))
                        continue;

                    targetDic.Add(colNum, cellStr);
                }
            }
        }

        void CollectCellData(ExcelWorksheet worksheet)
        {
            for (int rowNum = 0; rowNum < worksheet.Rows.Count; rowNum++)
            {
                Dictionary<int, ExcelCellData> rowCelldata = new Dictionary<int, ExcelCellData>();

                ExcelRow row = worksheet.Rows[rowNum];
                ExcelCell firstCell = row.AllocatedCells[0];
                if (firstCell.Value != null)
                {
                    var fristStr = firstCell.Value.ToString();
                    if (string.IsNullOrEmpty(fristStr))
                        continue;

                    if (fristStr.StartsWith(ExportDefine.ExportSpecialTag))
                        continue;
                }

                for (int colNum = 1; colNum < row.AllocatedCells.Count; colNum++)
                {
                    if (variableNameDic.ContainsKey(colNum))
                    {
                        ExcelCell cell = row.AllocatedCells[colNum];

                        string variableTypeStr = variableTypeDic.ContainsKey(colNum) ? variableTypeDic[colNum] : string.Empty;
                        string groupStr = exportGroupDic.ContainsKey(colNum) ? exportGroupDic[colNum] : string.Empty;
                        string commentStr = commentDic.ContainsKey(colNum) ? commentDic[colNum] : string.Empty;

                        ExcelCellData cellData = new ExcelCellData(cell,rowNum, colNum, variableNameDic[colNum], variableTypeStr, groupStr, commentStr);
                        
                        rowCelldata.Add(colNum, cellData);
                    }
                }

                cellContent.Add(rowNum, rowCelldata);
            }
        }

        public Dictionary<int, Dictionary<int, ExcelCellData>> GetCellConent()
        {
            return cellContent;
        }

        public string GetFileName() 
        {
            return fileName;
        }
    }
}
using GemBox.Spreadsheet;
using System;

namespace ExcelExportCmd
{
    internal class Launch
    {
        static List<ExcelSheetData> confSheetList = new List<ExcelSheetData>();

        static string ExcelDirectoryPath = @"..\..\..\..\..\策划目录\Excel";


        static void Main(string[] args)
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            string filePath = "C:\\GITHUB\\teaFramework\\策划目录\\Excel\\道具表.xlsx";

            DisposeExcel(filePath);

            StartExport();
        }

        private static void StartExport()
        {
            foreach (ExcelSheetData sheet in confSheetList) 
            {
                JsonExport.Export(sheet, ExcelDirectoryPath);
            }
        }

        static void DisposeExcel(string filePath)
        {
            ExcelFile excelFile = ExcelFile.Load(filePath);

            foreach (var worksheet in excelFile.Worksheets)
            {
                if (worksheet.Name.StartsWith(ExportDefine.ExportConfTag))
                {
                    ExcelSheetData sheetData = new ExcelSheetData(worksheet);
                    confSheetList.Add(sheetData); 
                }
            }
        }
    }
}
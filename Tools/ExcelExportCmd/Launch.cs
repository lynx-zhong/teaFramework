using GemBox.Spreadsheet;

namespace ExcelExportCmd
{
    internal class Launch
    {
        static string ExcelDirectoryPath = @"..\..\..\..\..\策划目录\Excel";

        static List<ExcelWorksheet> exportExcelWorksheets = new List<ExcelWorksheet>();

        static List<ExcelWorksheet> exportEnumExcelWorksheets = new List<ExcelWorksheet>();

        static void Main(string[] args)
        {
            SpreadsheetInfo.SetLicense("EQU2-1000-0000-000U");

            CollectExportInfo();

            Export();
        }

        static void CollectExportInfo() 
        {
            string[] allExcelFileFullPath = Directory.GetFiles(ExcelDirectoryPath, "*.xlsx", SearchOption.AllDirectories);
            
            foreach (string fileFullPath in allExcelFileFullPath)
            {
                string path = Path.GetFullPath(fileFullPath);
                Console.WriteLine($"目录：   {path}");

                ExcelFile excelFile = ExcelFile.Load(path);
                if (excelFile != null)
                {
                    return;
                }

                foreach (ExcelWorksheet sheet in exportExcelWorksheets)
                {
                    if (sheet.Name.StartsWith(ExportDefine.ExportConfTag))
                        exportExcelWorksheets.Add(sheet);

                    if (sheet.Name.StartsWith(ExportDefine.ExportEnumTag))
                        exportEnumExcelWorksheets.Add(sheet);


                }
            }
        }

        static void Export() 
        {
            Console.WriteLine($" 需要导出的数量：   {exportExcelWorksheets.Count}");

            foreach (ExcelWorksheet sheet in exportExcelWorksheets) 
            {
                ExcelSheetData excelSheetData = new ExcelSheetData(sheet);

            }
        }
    }
}
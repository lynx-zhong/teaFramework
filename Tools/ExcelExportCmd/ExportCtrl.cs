using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExportCmd
{
    public class ExportCtrl
    {
        public static void WriteFile(StringBuilder stringBuilder,string filePath) 
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            StreamWriter codeFile = File.CreateText(filePath);
            codeFile.Write(stringBuilder);
            codeFile.Flush();
            codeFile.Dispose();
            codeFile.Close();
        }
    }
}

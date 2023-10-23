using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CodeGenetate
{
    // TODO 根节点添加导出标记不合适
    // 只能在 Project文件夹下导出
    // 文件存在 改文件代码

    public class CodeGenerateFunc
    {
        #region 命名定义
        static string fieldAutoGenStartMark = "    // file auto generate start";
        static string fieldAutoGenEndMark = "    // file auto generate end";

        static string functionAutoGenStartMark = "    // fucntion auto generate start";
        static string functionAutoGenEndMark = "    // fucntion auto generate end";

        #endregion


        static string CodeFloder = @"Scripts\GameLogic\UI";
        static StringBuilder codeStr;

        public static void Generate(Transform exportGoTransform)
        {
            List<CodeGenerateNodeBind> codeGenerateNodes = GetAllExportInfo(exportGoTransform);
            string goPath = AssetDatabase.GetAssetPath(exportGoTransform);
            if (string.IsNullOrEmpty(goPath))
            {
                Debug.LogError("只能在 project 文件夹下导出");
                return;
            }
            string floderName = Path.GetDirectoryName(goPath);
            floderName = Path.GetFileName(floderName);

            //
            codeStr = new StringBuilder();
            codeStr.AppendLine("using UnityEngine;");
            codeStr.AppendLine();
            codeStr.AppendLine($"public class {exportGoTransform.name}");
            codeStr.AppendLine("{");

            //
            WriteField(codeGenerateNodes);

            //
            WriteFunction(codeGenerateNodes);

            codeStr.AppendLine("}");

            //
            ExportCodeFile(floderName, exportGoTransform.name);
        }

        static List<CodeGenerateNodeBind> GetAllExportInfo(Transform exportTrans)
        {
            List<CodeGenerateNodeBind> codeGenerates = new List<CodeGenerateNodeBind>();

            CodeGenerateNodeBind codeGenerate = exportTrans.GetComponent<CodeGenerateNodeBind>();
            if (codeGenerate)
                codeGenerates.Add(codeGenerate);

            for (int i = 0; i < exportTrans.transform.childCount; i++)
            {
                List<CodeGenerateNodeBind> childInfos = GetAllExportInfo(exportTrans.GetChild(i));
                codeGenerates.AddRange(childInfos);
            }

            return codeGenerates;
        }

        static void WriteField(List<CodeGenerateNodeBind> codeGenerateNodes)
        {
            codeStr.AppendLine(fieldAutoGenStartMark);

            for (int i = 0; i < codeGenerateNodes.Count; i++)
            {
                CodeGenerateNodeBind codeGenerateNode = codeGenerateNodes[i];
                List<ComponentStruct> exportComponents = codeGenerateNode.GetExportComponents();
                for (int j = 0; j < exportComponents.Count; j++)
                {
                    ComponentStruct componentStruct = exportComponents[j];
                    codeStr.AppendLine($"    public {componentStruct.ComponentType.Name} {componentStruct.NodeVariableName}");
                }
            }

            codeStr.AppendLine(fieldAutoGenEndMark);
        }

        static void WriteFunction(List<CodeGenerateNodeBind> allCodeGenerateNodes)
        {
            // codeStr.AppendLine(functionAutoGenStartMark);

            // List<ComponentStruct> needExportComponents = new List<ComponentStruct>();

            // for (int i = 0; i < allCodeGenerateNodes.Count; i++)
            // {
            //     CodeGenerateNodeBind codeGenerateNode = allCodeGenerateNodes[i];
            //     List<ComponentStruct> exportComponents = codeGenerateNode.GetExportComponents();
            //     for (int j = 0; j < exportComponents.Count; j++)
            //     {
            //         if (exportComponents[i].ComponentFullName == typeof(Button).FullName)
            //         {
                        
            //         }
            //     }
            // }

            // codeStr.AppendLine(functionAutoGenEndMark);

            Debug.Log(typeof(Button).FullName);
            Debug.Log(typeof(Button).Name);
        }

        private static void ExportCodeFile(string floderName, string fileName)
        {
            string directoryPath = Path.Combine(Application.dataPath, CodeFloder, floderName);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string fileFullPath = Path.Combine(directoryPath, fileName + ".txt");

            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
            }

            if (!File.Exists(fileFullPath))
            {
                StreamWriter codeFile = File.CreateText(fileFullPath);
                codeFile.Write(codeStr);
                codeFile.Flush();
                codeFile.Dispose();
                codeFile.Close();
            }

            AssetDatabase.Refresh();
        }
    }
}
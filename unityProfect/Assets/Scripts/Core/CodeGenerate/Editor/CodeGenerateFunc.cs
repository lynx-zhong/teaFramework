using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
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

        static string bindFucntionStartMark = "        // bind function start";
        static string bindFucntionEndMark = "        // bind function end";

        static string functionAutoGenStartMark = "    // fucntion auto generate start";
        static string functionAutoGenEndMark = "    // fucntion auto generate end";

        static string unBindFunctionStartMark = "        // unbind function start";
        static string unBindFunctionEndMark = "        // unbind function end";

        #endregion


        static string CodeFloder = @"Scripts\GameLogic\UI";
        static StringBuilder codeStr;
        static GameObject exportGo;

        public static void Generate(Transform exportGoTransform)
        {
            exportGo = exportGoTransform.gameObject;

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
            codeStr.AppendLine("using UnityEngine.UI;");
            codeStr.AppendLine();
            codeStr.AppendLine($"public class {exportGoTransform.name}");
            codeStr.AppendLine("{");

            //
            WriteField(codeGenerateNodes);

            //
            WriteInit(codeGenerateNodes);

            //
            WriteFunction(codeGenerateNodes);

            //
            WriteUnInit(codeGenerateNodes);

            codeStr.AppendLine("}");

            //
            ExportCodeFile(floderName, exportGoTransform.name);
        }

        static List<CodeGenerateNodeBind> GetAllExportInfo(Transform exportTrans)
        {
            List<CodeGenerateNodeBind> codeGenerates = new List<CodeGenerateNodeBind>();

            CodeGenerateNodeBind codeGenerate = exportTrans.GetComponent<CodeGenerateNodeBind>();
            if (codeGenerate)
            {
                for (int i = 0; i < codeGenerate.exportComponents.Count; i++)
                {
                    if (codeGenerate.exportComponents[i].ComponentType == null)
                    {
                        List<Type> types = codeGenerate.GetElementComponents();
                        codeGenerate.exportComponents[i].ComponentType = types[codeGenerate.exportComponents[i].SelectedComponentIndex];
                    }
                }
                codeGenerates.Add(codeGenerate);
            }

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
                    codeStr.AppendLine($"    public {componentStruct.ComponentType.Name} {componentStruct.VariableName};");
                }
            }

            codeStr.AppendLine(fieldAutoGenEndMark);
            codeStr.AppendLine();
        }

        private static void WriteInit(List<CodeGenerateNodeBind> allCodeGenerateNodes)
        {
            codeStr.AppendLine();
            codeStr.AppendLine("    private void Start()");
            codeStr.AppendLine("    {");

            //
            List<ComponentStruct> bindFunctions = GetBindFunctionComponent(allCodeGenerateNodes);
            if (bindFunctions.Count > 0)
            {
                codeStr.AppendLine(bindFucntionStartMark);

                //
                for (int i = 0; i < bindFunctions.Count; i++)
                {
                    if (bindFunctions[i].ComponentType == typeof(Button))
                    {
                        codeStr.AppendLine($"        {bindFunctions[i].VariableName}.onClick.AddListener(On{bindFunctions[i].VariableName}ButtonClick);");
                    }
                }

                codeStr.AppendLine(bindFucntionEndMark);
            }

            //
            codeStr.AppendLine("    }");
        }

        static void WriteFunction(List<CodeGenerateNodeBind> allCodeGenerateNodes)
        {
            //
            List<ComponentStruct> bindFunctions = GetBindFunctionComponent(allCodeGenerateNodes);
            if (bindFunctions.Count == 0)
                return;

            //
            codeStr.AppendLine();
            codeStr.AppendLine(functionAutoGenStartMark);

            for (int i = 0; i < bindFunctions.Count; i++)
            {
                codeStr.AppendLine();
                codeStr.AppendLine($"    private void On{bindFunctions[i].VariableName}ButtonClick()");
                codeStr.AppendLine("    {");
                codeStr.AppendLine();
                codeStr.AppendLine("    }");
            }

            codeStr.AppendLine();
            codeStr.AppendLine(functionAutoGenEndMark);
        }

        private static void WriteUnInit(List<CodeGenerateNodeBind> allCodeGenerateNodes)
        {
            codeStr.AppendLine();
            codeStr.AppendLine("    private void OnDestory()");
            codeStr.AppendLine("    {");

            //
            List<ComponentStruct> bindFunctions = GetBindFunctionComponent(allCodeGenerateNodes);
            if (bindFunctions.Count > 0)
            {
                codeStr.AppendLine(unBindFunctionStartMark);

                //
                for (int i = 0; i < bindFunctions.Count; i++)
                {
                    if (bindFunctions[i].ComponentType == typeof(Button))
                    {
                        codeStr.AppendLine($"        {bindFunctions[i].VariableName}.onClick.AddListener(On{bindFunctions[i].VariableName}ButtonClick);");
                    }
                }

                codeStr.AppendLine(unBindFunctionEndMark);
            }

            //
            codeStr.AppendLine("    }");
        }

        static List<ComponentStruct> GetBindFunctionComponent(List<CodeGenerateNodeBind> allCodeGenerateNodes)
        {
            List<ComponentStruct> needExportComponents = new List<ComponentStruct>();

            for (int i = 0; i < allCodeGenerateNodes.Count; i++)
            {
                CodeGenerateNodeBind codeGenerateNode = allCodeGenerateNodes[i];
                List<ComponentStruct> exportComponents = codeGenerateNode.GetExportComponents();
                for (int j = 0; j < exportComponents.Count; j++)
                {
                    if (exportComponents[j].ComponentType == typeof(Button))
                    {
                        needExportComponents.Add(exportComponents[j]);
                    }
                }
            }

            return needExportComponents;
        }

        private static void ExportCodeFile(string floderName, string fileName)
        {
            string directoryPath = Path.Combine(Application.dataPath, CodeFloder, floderName);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string fileFullPath = Path.Combine(directoryPath, fileName + ".cs");

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

        [DidReloadScripts]
        static void OnScriptsReloaded()
        {
            Debug.Log("-----------.  ");
            Type scriptType = Type.GetType($"CodeGenerate,Assembly-CSharp");
            scriptType = CustomScriptUtility.CreateScript("MyMonoBehaviour", scriptCode);
        }
    }
}
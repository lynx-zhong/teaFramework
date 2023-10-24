using System.Collections.Generic;
using UnityEditor.Callbacks;
using System.Reflection;
using UnityEngine.UI;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace CodeGenetate
{
    // TODO 根节点添加导出标记不合适
    // 只能在 Project文件夹下导出
    // 文件存在 改文件代码、
    // 禁用组件没有变更 

    public class CodeGenerateFunc
    {
        #region 命名定义
        static string fieldAutoGenStartMark = "    // field auto generate start";
        static string fieldAutoGenEndMark = "    // field auto generate end";

        static string bindFucntionStartMark = "        // bind function start";
        static string bindFucntionEndMark = "        // bind function end";

        static string functionAutoGenStartMark = "    // fucntion auto generate start";
        static string functionAutoGenEndMark = "    // fucntion auto generate end";

        static string unBindFunctionStartMark = "        // unbind function start";
        static string unBindFunctionEndMark = "        // unbind function end";

        #endregion


        static string CodeFloder = @"Scripts\GameLogic\UI";
        static StringBuilder codeFullStr;
        static string exportCodeCacheKey = "exportCodeCacheKey";

        public static void Generate(Transform exportTrans)
        {
            //
            string path = AssetDatabase.GetAssetPath(exportTrans);
            PlayerPrefs.SetString(exportCodeCacheKey, path);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("只能在 project 文件夹下导出");
                return;
            }

            //
            string floderName = Path.GetDirectoryName(path);
            floderName = Path.GetFileName(floderName);
            string directoryPath = Path.Combine(Application.dataPath, CodeFloder, floderName);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            //
            string fileFullPath = Path.Combine(directoryPath, exportTrans.name + ".txt");
            if (File.Exists(fileFullPath))
            {
                string fileInfo = File.ReadAllText(fileFullPath);
                codeFullStr = new StringBuilder(fileInfo);
            }
            else
                codeFullStr = new StringBuilder();

            //
            WriteStringBuilder(exportTrans, fileFullPath);

            //
            ExportCodeFile(fileFullPath);
        }

        static void WriteStringBuilder(Transform exportTrans, string fileFullPath)
        {
            List<CodeGenerateNodeBind> codeGenerateNodes = GetAllExportInfo(exportTrans);
            bool fileExists = File.Exists(fileFullPath);

            //
            if (!fileExists)
            {
                codeFullStr = new StringBuilder();
                codeFullStr.AppendLine("using UnityEngine;");
                codeFullStr.AppendLine("using UnityEngine.UI;");
                codeFullStr.AppendLine();
                codeFullStr.AppendLine($"public class {exportTrans.name} : MonoBehaviour");
                codeFullStr.AppendLine("{");
            }

            //
            WriteField(codeGenerateNodes, fileExists);

            //
            WriteInit(codeGenerateNodes, fileExists);

            //
            WriteFunction(codeGenerateNodes, fileExists);

            //
            WriteUnInit(codeGenerateNodes, fileExists);

            //
            // if (!fileExists)
            // {
            codeFullStr.AppendLine("}");
            // }
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

        static void WriteField(List<CodeGenerateNodeBind> codeGenerateNodes, bool fileExists)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(fieldAutoGenStartMark);

            for (int i = 0; i < codeGenerateNodes.Count; i++)
            {
                CodeGenerateNodeBind codeGenerateNode = codeGenerateNodes[i];
                List<ComponentStruct> exportComponents = codeGenerateNode.GetExportComponents();
                for (int j = 0; j < exportComponents.Count; j++)
                {
                    ComponentStruct componentStruct = exportComponents[j];
                    stringBuilder.AppendLine($"    public {componentStruct.ComponentType.Name} {componentStruct.VariableName};");
                }
            }

            stringBuilder.AppendLine(fieldAutoGenEndMark);
            stringBuilder.AppendLine();

            RegexReplace(fileExists, stringBuilder, fieldAutoGenStartMark, fieldAutoGenEndMark);
        }

        private static void WriteInit(List<CodeGenerateNodeBind> allCodeGenerateNodes, bool fileExists)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("    private void Start()");
            stringBuilder.AppendLine("    {");

            //
            List<ComponentStruct> bindFunctions = GetBindFunctionComponent(allCodeGenerateNodes);
            if (bindFunctions.Count > 0)
            {
                stringBuilder.AppendLine(bindFucntionStartMark);

                //
                for (int i = 0; i < bindFunctions.Count; i++)
                {
                    if (bindFunctions[i].ComponentType == typeof(Button))
                    {
                        stringBuilder.AppendLine($"        {bindFunctions[i].VariableName}.onClick.AddListener(On{bindFunctions[i].VariableName}ButtonClick);");
                    }
                }

                stringBuilder.AppendLine(bindFucntionEndMark);
            }

            //
            stringBuilder.AppendLine("    }");

            RegexReplace(fileExists, stringBuilder, bindFucntionStartMark, bindFucntionEndMark);
        }

        static void WriteFunction(List<CodeGenerateNodeBind> allCodeGenerateNodes, bool fileExists)
        {
            //
            List<ComponentStruct> bindFunctions = GetBindFunctionComponent(allCodeGenerateNodes);
            if (bindFunctions.Count == 0)
                return;

            if (!fileExists)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(functionAutoGenStartMark);

                for (int i = 0; i < bindFunctions.Count; i++)
                {
                    stringBuilder.AppendLine($"    private void On{bindFunctions[i].VariableName}ButtonClick()");
                    stringBuilder.AppendLine("    {");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("    }");
                    stringBuilder.AppendLine();
                }
                stringBuilder.AppendLine(functionAutoGenEndMark);
                codeFullStr.Append(stringBuilder);
            }
            else
            {
                for (int i = 0; i < bindFunctions.Count; i++)
                {
                    codeFullStr.Replace(bindFunctions[i].NodeOldVarialeName, bindFunctions[i].VariableName);
                }
            }
        }

        private static void WriteUnInit(List<CodeGenerateNodeBind> allCodeGenerateNodes, bool fileExists)
        {
            //
            List<ComponentStruct> bindFunctions = GetBindFunctionComponent(allCodeGenerateNodes);

            if (!fileExists)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("    private void OnDestroy()");
                stringBuilder.AppendLine("    {");

                if (bindFunctions.Count > 0)
                {
                    stringBuilder.AppendLine(unBindFunctionStartMark);

                    //
                    for (int i = 0; i < bindFunctions.Count; i++)
                    {
                        if (bindFunctions[i].ComponentType == typeof(Button))
                        {
                            stringBuilder.AppendLine($"        {bindFunctions[i].VariableName}.onClick.RemoveListener(On{bindFunctions[i].VariableName}ButtonClick);");
                        }
                    }

                    stringBuilder.AppendLine(unBindFunctionEndMark);
                }

                //
                stringBuilder.AppendLine("    }");
            }
            else
            {
                RegexReplace(fileExists, stringBuilder, bindFucntionStartMark, bindFucntionEndMark);
            }
        }

        static void RegexReplace(bool fileExists, StringBuilder appendStringBuilder, string replaceStartMark, string replaceEndMark)
        {
            if (!fileExists)
                codeFullStr.Append(appendStringBuilder);
            else
            {
                string pattern = @$"{replaceStartMark}([\s\S]*?){replaceEndMark}";
                string modifiedString = Regex.Replace(codeFullStr.ToString(), pattern, appendStringBuilder.ToString());
                codeFullStr = new StringBuilder(modifiedString);
            }
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

        private static void ExportCodeFile(string fileFullPath)
        {
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
            }

            if (!File.Exists(fileFullPath))
            {
                StreamWriter codeFile = File.CreateText(fileFullPath);
                codeFile.Write(codeFullStr);
                codeFile.Flush();
                codeFile.Dispose();
                codeFile.Close();
            }

            AssetDatabase.Refresh();
        }

        [DidReloadScripts]
        static void OnScriptsReloaded()
        {
            return;
            string path = PlayerPrefs.GetString(exportCodeCacheKey);
            PlayerPrefs.SetString(exportCodeCacheKey, string.Empty);
            if (string.IsNullOrEmpty(path))
                return;

            string exportCodeName = Path.GetFileNameWithoutExtension(path);
            GameObject exportGo = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            exportGo = PrefabUtility.InstantiatePrefab(exportGo) as GameObject;

            //
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            Type type = assembly.GetType(exportCodeName);
            Component component = exportGo.GetComponent(type);
            if (component == null)
                component = exportGo.AddComponent(type);

            //
            List<CodeGenerateNodeBind> generateNodeBinds = GetAllExportInfo(exportGo.transform);
            Dictionary<string, TypeGameObject> componentDic = new Dictionary<string, TypeGameObject>();
            foreach (var components in generateNodeBinds)
            {
                foreach (var itemComponent in components.exportComponents)
                {
                    if (!componentDic.ContainsKey(itemComponent.VariableName))
                    {
                        TypeGameObject typeGameObject = new TypeGameObject()
                        {
                            type = itemComponent.ComponentType,
                            go = components.gameObject,
                        };
                        componentDic.Add(itemComponent.VariableName, typeGameObject);
                    }
                }
            }

            //
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                TypeGameObject bindType = componentDic[fieldInfos[i].Name];
                if (bindType.type == typeof(GameObject))
                    fieldInfos[i].SetValue(component, bindType.go);
                else
                    fieldInfos[i].SetValue(component, bindType.go.GetComponent(bindType.type));
            }

            //
            PrefabUtility.SaveAsPrefabAsset(exportGo, path);
            UnityEngine.Object.DestroyImmediate(exportGo);
            AssetDatabase.Refresh();
        }

        struct TypeGameObject
        {
            public Type type;
            public GameObject go;
        }
    }
}
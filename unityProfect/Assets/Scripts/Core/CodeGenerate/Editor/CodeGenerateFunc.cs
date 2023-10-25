using UnityEditor.Callbacks;
using System.Reflection;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;


namespace CodeGenetate
{
    // TODO 根节点添加导出标记不合适
    // 只能在 Project文件夹下导出
    // 文件存在 改文件代码、
    // 增加临时文件去生成覆盖的文件，并加入git的忽略，免得覆盖掉很麻烦的东西 拿不回数据

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
            string fileFullPath = Path.Combine(directoryPath, exportTrans.name + ".cs");

            //
            WriteStringBuilder(exportTrans, fileFullPath);

            //
            ExportCodeFile(fileFullPath);

            //
            SetVarialOldName(exportTrans);
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
            else
            {
                string fileInfo = File.ReadAllText(fileFullPath);
                codeFullStr = new StringBuilder(fileInfo);
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
            if (!fileExists)
            {
                codeFullStr.AppendLine("}");
            }
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
            List<ComponentStruct> allExportComponents = new List<ComponentStruct>();

            StringBuilder canReplaceStringBuilder = new StringBuilder();
            canReplaceStringBuilder.AppendLine(fieldAutoGenStartMark);

            for (int i = 0; i < codeGenerateNodes.Count; i++)
            {
                CodeGenerateNodeBind codeGenerateNode = codeGenerateNodes[i];
                List<ComponentStruct> exportComponents = codeGenerateNode.GetExportComponents();
                for (int j = 0; j < exportComponents.Count; j++)
                {
                    ComponentStruct componentStruct = exportComponents[j];
                    canReplaceStringBuilder.AppendLine($"    public {componentStruct.ComponentType.Name} {componentStruct.VariableName};");

                    //
                    allExportComponents.Add(componentStruct);
                }
            }

            canReplaceStringBuilder.AppendLine(fieldAutoGenEndMark);

            if (!fileExists)
            {
                canReplaceStringBuilder.AppendLine();
                codeFullStr.Append(canReplaceStringBuilder);
            }
            else
            {
                RegexReplace(canReplaceStringBuilder, fieldAutoGenStartMark, fieldAutoGenEndMark);

                foreach (ComponentStruct node in allExportComponents)
                {
                    if (string.IsNullOrEmpty(node.OldVarialeName))
                        continue;

                    codeFullStr.Replace(node.OldVarialeName, node.VariableName);
                }
            }
        }

        private static void WriteInit(List<CodeGenerateNodeBind> allCodeGenerateNodes, bool fileExists)
        {
            if (!fileExists)
            {
                codeFullStr.AppendLine();
                codeFullStr.AppendLine("    private void Start()");
                codeFullStr.AppendLine("    {");
            }

            //
            StringBuilder canReplaceStringBuilder = new StringBuilder();
            List<ComponentStruct> bindFunctions = GetBindFunctionComponent(allCodeGenerateNodes);
            if (bindFunctions.Count > 0)
            {
                canReplaceStringBuilder.AppendLine(bindFucntionStartMark);

                //
                for (int i = 0; i < bindFunctions.Count; i++)
                {
                    if (bindFunctions[i].ComponentType == typeof(Button))
                    {
                        canReplaceStringBuilder.AppendLine($"        {bindFunctions[i].VariableName}.onClick.AddListener(On{bindFunctions[i].VariableName}ButtonClick);");
                    }
                }

                canReplaceStringBuilder.AppendLine(bindFucntionEndMark);
            }

            //
            if (!fileExists)
            {
                codeFullStr.Append(canReplaceStringBuilder);
                codeFullStr.AppendLine("    }");
            }
            else
            {
                RegexReplace(canReplaceStringBuilder, bindFucntionStartMark, bindFucntionEndMark);
            }
        }

        private static void WriteUnInit(List<CodeGenerateNodeBind> allCodeGenerateNodes, bool fileExists)
        {
            if (!fileExists)
            {
                codeFullStr.AppendLine();
                codeFullStr.AppendLine("    private void OnDestroy()");
                codeFullStr.AppendLine("    {");
            }

            //
            StringBuilder canReplaceStringBuilder = new StringBuilder();
            List<ComponentStruct> bindFunctions = GetBindFunctionComponent(allCodeGenerateNodes);
            if (bindFunctions.Count > 0)
            {
                canReplaceStringBuilder.AppendLine(unBindFunctionStartMark);

                //
                for (int i = 0; i < bindFunctions.Count; i++)
                {
                    if (bindFunctions[i].ComponentType == typeof(Button))
                    {
                        canReplaceStringBuilder.AppendLine($"        {bindFunctions[i].VariableName}.onClick.RemoveListener(On{bindFunctions[i].VariableName}ButtonClick);");
                    }
                }

                canReplaceStringBuilder.AppendLine(unBindFunctionEndMark);
            }

            //
            if (!fileExists)
            {
                codeFullStr.Append(canReplaceStringBuilder);
                codeFullStr.AppendLine("    }");
            }
            else
            {
                RegexReplace(canReplaceStringBuilder, unBindFunctionStartMark, unBindFunctionEndMark);
            }
        }

        static void WriteFunction(List<CodeGenerateNodeBind> allCodeGenerateNodes, bool fileExists)
        {
            //
            List<ComponentStruct> bindFunctions = GetBindFunctionComponent(allCodeGenerateNodes);
            if (bindFunctions.Count == 0)
            {
                codeFullStr.AppendLine(functionAutoGenStartMark);
                codeFullStr.AppendLine();
                codeFullStr.AppendLine("    // do not delete this lable");
                codeFullStr.AppendLine();
                codeFullStr.AppendLine(functionAutoGenEndMark);
                return;
            }

            if (!fileExists)
            {
                codeFullStr.AppendLine();
                codeFullStr.AppendLine(functionAutoGenStartMark);

                for (int i = 0; i < bindFunctions.Count; i++)
                {
                    codeFullStr.AppendLine($"    private void On{bindFunctions[i].VariableName}ButtonClick()");
                    codeFullStr.AppendLine("    {");
                    codeFullStr.AppendLine();
                    codeFullStr.AppendLine("    }");
                    codeFullStr.AppendLine();
                }
                codeFullStr.AppendLine(functionAutoGenEndMark);
            }
            else
            {
                // 删除不做，危险。万一函数体有些东西很重要

                // 增加
                string[] codeFullArr = codeFullStr.ToString().Split(functionAutoGenEndMark);
                if (codeFullArr.Length < 2)
                {
                    Debug.LogError($"没有找到函数标记:  {functionAutoGenEndMark}  不继续更新函数内容");
                    return;
                }

                for (int i = 0; i < bindFunctions.Count; i++)
                {
                    if (codeFullArr[0].IndexOf($"private void On{bindFunctions[i].VariableName}ButtonClick()") == -1)
                    {
                        StringBuilder addStringBuilder = new StringBuilder();
                        addStringBuilder.AppendLine($"    private void On{bindFunctions[i].VariableName}ButtonClick()");
                        addStringBuilder.AppendLine("    {");
                        addStringBuilder.AppendLine();
                        addStringBuilder.AppendLine("    }");
                        addStringBuilder.AppendLine();
                        codeFullArr[0] = codeFullArr[0] + addStringBuilder.ToString();
                    }
                }

                codeFullStr = new StringBuilder();
                codeFullStr.Append(codeFullArr[0]);
                codeFullStr.Append(functionAutoGenEndMark);
                codeFullStr.Append(codeFullArr[1]);
            }
        }

        static void RegexReplace(StringBuilder appendStringBuilder, string replaceStartMark, string replaceEndMark)
        {
            string appendString = appendStringBuilder.ToString().TrimEnd(new char[] { '\r', '\n' });
            string pattern = @$"{replaceStartMark}([\s\S]*?){replaceEndMark}";
            string modifiedString = Regex.Replace(codeFullStr.ToString(), pattern, appendString);
            codeFullStr = new StringBuilder(modifiedString);
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

        private static void SetVarialOldName(Transform exportTrans)
        {
            List<CodeGenerateNodeBind> codeGenerateNodes = GetAllExportInfo(exportTrans);
            for (int i = 0; i < codeGenerateNodes.Count; i++)
            {
                CodeGenerateNodeBind codeGenerateNode = codeGenerateNodes[i];
                for (int j = 0; j < codeGenerateNode.exportComponents.Count; j++)
                {
                    ComponentStruct componentStruct = codeGenerateNodes[i].exportComponents[j];
                    componentStruct.OldVarialeName = componentStruct.VariableName;

                    codeGenerateNode.exportComponents[j] = componentStruct;
                }
                codeGenerateNodes[i] = codeGenerateNode;
            }
        }

        [DidReloadScripts]
        static void OnScriptsReloaded()
        {
            string path = PlayerPrefs.GetString(exportCodeCacheKey);
            PlayerPrefs.SetString(exportCodeCacheKey, string.Empty);
            if (string.IsNullOrEmpty(path))
                return;

            if (Selection.activeGameObject == null)
                return;

            string exportCodeName = Path.GetFileNameWithoutExtension(path);
            if (exportCodeName != Selection.activeGameObject.name)
                return;

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
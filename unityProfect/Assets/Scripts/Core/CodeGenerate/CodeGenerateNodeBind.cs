#if UNITY_EDITOR 

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using System;
using UnityEngine;

namespace CodeGenetate
{
    [Serializable]
    [DisallowMultipleComponent]
    public class CodeGenerateNodeBind : MonoBehaviour
    {
        public string TestString = "10";
        const string GameObejctName = "GameObject";

        public List<ComponentStruct> exportComponents;

        [HideInInspector]
        public int selectedLayerIndex = 1;

        [HideInInspector]
        public string[] layerStringArr = new string[]
        {
            "BackgroundLayer","NarmalLayer","InfoLayer","TopLayer"
        };

        [HideInInspector]
        public bool isRootNode = false;

        public List<string> GetElementAllComponentsStringList()
        {
            Component[] allComponent = gameObject.GetComponents<Component>();
            List<string> tempCompontNames = new List<string>
            {
                GameObejctName,
            };

            foreach (Component item in allComponent)
            {
                string compontType = Regex.Replace(item.ToString(), @"(.*\()(.*)(\).*)", "$2");
                string[] temp = compontType.Split('.');

                if (temp.Length > 0)
                    compontType = temp[temp.Length - 1];

                if (compontType != "CodeGenerateNodeBind")
                {
                    tempCompontNames.Add(compontType);
                }
            }

            return tempCompontNames;
        }

        public List<ComponentStruct> GetExportComponents()
        {
            if (exportComponents == null || exportComponents.Count == 0)
            {
                exportComponents = new List<ComponentStruct>();
                AddExportComponent();
            }
            return exportComponents;
        }

        public void AddExportComponent()
        {
            ComponentStruct componentStruct = new ComponentStruct()
            {
                nodeVariableName = string.Format("{0}{1}",gameObject.name,GameObejctName),
                componentStr = GameObejctName,
                selectedComponentIndex = 0,
            };

            exportComponents.Add(componentStruct);
        }

        public void RemoveExportComponent(int index)
        {
            exportComponents.RemoveAt(index);
        }
    }

    [Serializable]
    public class ComponentStruct
    {
        public string nodeVariableName;
        public string nodeOldVarialeName;
        public string componentStr;
        public int selectedComponentIndex;
    }
}

#endif
#if UNITY_EDITOR 

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using System;
using UnityEngine;
using JetBrains.Annotations;

namespace CodeGenetate
{
    [Serializable]
    [DisallowMultipleComponent]
    public class CodeGenerateNodeBind : MonoBehaviour
    {
        [HideInInspector]
        public bool isRootNode = false;

        [HideInInspector]
        public int selectedLayerIndex = 1;

        [HideInInspector]
        public string[] layerStringArr = new string[]
        {
            "BackgroundLayer","NarmalLayer","InfoLayer","TopLayer"
        };


        public List<ComponentStruct> exportComponents;

        public List<Type> GetElementComponents()
        {
            List<Type> allComponents = new List<Type>
            {
                typeof(GameObject),
            };

            Component[] getComponents = gameObject.GetComponents<Component>();
            for (int i = 0; i < getComponents.Length; i++)
            {
                if (getComponents[i].GetType() != typeof(CodeGenerateNodeBind))
                {
                    allComponents.Add(getComponents[i].GetType());
                }
            }

            return allComponents;
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
                VariableName = string.Format("{0}{1}", gameObject.name, typeof(GameObject).Name),
                SelectedComponentIndex = 0,
                ComponentType = typeof(GameObject),
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
        public string VariableName;
        public string NodeOldVarialeName;

        [HideInInspector]
        public int SelectedComponentIndex;

        public Type ComponentType;
    }
}

#endif
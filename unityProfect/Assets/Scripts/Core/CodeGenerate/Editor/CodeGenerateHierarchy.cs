using UnityEngine;
using UnityEditor;
using System;

namespace CodeGenetate
{
    public class CodeGenerateHierarchy : Editor
    {
        [InitializeOnLoadMethod]
        static void CodeGenerateInitializeOnLoadMethod()
        {
            EditorApplication.hierarchyWindowItemOnGUI += CodeGenerateHierarchyOnGui;
            EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
        }

        private static void OnHierarchyWindowChanged()
        {
            // Debug.Log("嫦娥        ----");
            // if (!PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject))
            // {
            //     Debug.Log("------------>  IsPartOfAnyPrefab");
                // 当退出Prefab编辑模式时，Selection.activeGameObject将不再是Prefab的一部分
                // 所以我们可以在这里触发保存Prefab的操作
                // SavePrefab();
            // }
        }

        private static void CodeGenerateHierarchyOnGui(int instanceID, Rect selectionRect)
        {
            if (Application.isPlaying)
                return;

            GameObject targetGo = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (targetGo == null)
                return;

            CodeGenerateNodeBind generateNodeBind = targetGo.GetComponent<CodeGenerateNodeBind>();
            if (generateNodeBind == null)
                return;

            Rect targetRect = new Rect(selectionRect.x - 28, selectionRect.y, 30, selectionRect.height);
            GUI.color = Color.yellow;
            GUI.Label(targetRect, "\u2605");
            GUI.color = Color.white;
        }
    }
}
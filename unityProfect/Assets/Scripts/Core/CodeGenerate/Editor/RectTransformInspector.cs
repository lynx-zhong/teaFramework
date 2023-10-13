using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using CodeGenetate;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;
using UnityEngine.AI;

[CustomEditor(typeof(RectTransform)), CanEditMultipleObjects]
public class RectTransformInspector : Editor
{
    private Editor instance;
    private MethodInfo onSceneGUIMethod;
    private static readonly object[] emptyArray = new object[0];

    private void OnEnable()
    {
        CreateEditorInstance();
    }

    public override void OnInspectorGUI()
    {
        if (instance != null)
            instance.OnInspectorGUI();

        ShowNodeBing();
    }

    private void CreateEditorInstance()
    {
        var editorType = Assembly.GetAssembly(typeof(Editor)).GetTypes().FirstOrDefault(m => m.Name == "RectTransformEditor");

        if (editorType == null)
        {
            Debug.LogError("RectTransformEditor not found. Ensure that you   have the correct Unity version.");
            return;
        }

        instance = Editor.CreateEditor(targets, editorType) as Editor; // Use Editor.CreateEditor
        onSceneGUIMethod = editorType.GetMethod("OnSceneGUI", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private void OnSceneGUI()
    {
        if (instance == null)
            CreateEditorInstance();

        if (instance != null && onSceneGUIMethod != null)
            onSceneGUIMethod.Invoke(instance, emptyArray);
    }

    private void OnDisable()
    {
        if (instance != null)
            DestroyImmediate(instance);
    }


    #region 代码节点绑定

    void ShowNodeBing()
    {
        //
        if (Application.isPlaying)
            return;


        //
        serializedObject.Update();
        GUILayout.Space(15);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("导出"))
            OnExportButtonClick();
        if (GUILayout.Button("禁用导出"))
            OnBanExportButtonClick();
        GUILayout.EndHorizontal();

        //
        CodeGenerateNodeBind generateNodeBind = target.GetComponent<CodeGenerateNodeBind>();
        if (generateNodeBind != null)
        {
            GUILayout.Space(3);
            GUILayout.Label("--------------------------------------------- components ---------------------------------------------", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });

            //
            var exportComponents = generateNodeBind.GetExportComponents();
            var allComponentStrings = generateNodeBind.GetElementAllComponentsStringList();
            for (int i = 0; i < exportComponents.Count; i++)
            {
                ShowComponent(generateNodeBind, exportComponents, allComponentStrings, i);
            }

            // GUI.color = Color.green;
            // GUIStyle gUIStyle = new GUIStyle();
            // gUIStyle.name
            if (GUILayout.Button("添加组件", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter,name = "ShurikenPlus" }))
            {
                generateNodeBind.AddExportComponent();
            }
            // GUI.color = Color.white;

            //
            GUILayout.Space(10);
            // GUI.color = Color.green;
            if (GUILayout.Button("导出", "flow node hex 3", new GUILayoutOption[] { GUILayout.MinHeight(30) }))
            {

            }
            // GUI.color = Color.white;
        }
        serializedObject.ApplyModifiedProperties();
    }

    void OnExportButtonClick()
    {
        target.AddComponent<CodeGenerateNodeBind>();
    }

    private void OnBanExportButtonClick()
    {
        DestroyImmediate(target.GetComponent<CodeGenerateNodeBind>());
    }

    void ShowComponent(CodeGenerateNodeBind generateNodeBind, List<ComponentStruct> exportComponents, List<string> allComponentStrings, int i)
    {
        Transform targetTran = target as Transform;

        //
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();

        //
        ComponentStruct componentStruct = exportComponents[i];
        componentStruct.nodeVariableName = GUILayout.TextField(componentStruct.nodeVariableName);

        int enumIndex = EditorGUILayout.Popup(componentStruct.selectedComponentIndex, allComponentStrings.ToArray(), "DropDown", new GUILayoutOption[] { GUILayout.MaxWidth(150) });
        if (enumIndex != componentStruct.selectedComponentIndex)
        {
            componentStruct.selectedComponentIndex = enumIndex;
            componentStruct.nodeVariableName = string.Format("{0}{1}", targetTran.name, allComponentStrings[enumIndex]);
        }

        if (GUILayout.Button("删除", new GUILayoutOption[] { GUILayout.MaxWidth(60) }))
        {
            generateNodeBind.RemoveExportComponent(i);
            serializedObject.ApplyModifiedProperties();
            return;
        }
        GUILayout.EndHorizontal();

        exportComponents[i] = componentStruct;
    }

    #endregion
}

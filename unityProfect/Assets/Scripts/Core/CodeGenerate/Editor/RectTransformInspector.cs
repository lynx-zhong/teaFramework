using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using CodeGenerate;
using Unity.VisualScripting;
using System;
using System.Collections.Generic;
using UnityEditorInternal;

[CustomEditor(typeof(RectTransform)), CanEditMultipleObjects]
public class RectTransformInspector : Editor
{
    private Editor instance;
    private MethodInfo onSceneGUIMethod;
    private static readonly object[] emptyArray = new object[0];

    private void OnEnable()
    {
        CreateEditorInstance();

        //
        CodeGenerateOnEnable();
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

    ReorderableList reorderableList;

    void CodeGenerateOnEnable()
    {
        InitReorderableList();
    }

    void InitReorderableList()
    {
        CodeGenerateNodeBind generateNodeBind = target.GetComponent<CodeGenerateNodeBind>();
        if (generateNodeBind == null)
            return;

        reorderableList = new ReorderableList(generateNodeBind.GetExportComponents(), typeof(ComponentStruct), true, true, true, true)
        {
            elementHeight = 20,
            drawHeaderCallback = DrawHeader,
            drawElementCallback = DrawElement,
            onAddCallback = DrawOnAddCallBack,
            onRemoveCallback = DrawOnRemoveCallBack
        };
    }

    void ShowNodeBing()
    {
        //
        if (Application.isPlaying)
            return;




        //
        GUILayout.Space(12);

        //
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
            serializedObject.Update();

            if (reorderableList == null)
                InitReorderableList();

            //
            GUILayout.Space(3);
            generateNodeBind.isRootNode = GUILayout.Toggle(generateNodeBind.isRootNode, "根节点");

            //
            if (generateNodeBind.isRootNode)
            {
                GUILayout.Space(3);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Layer Select ：", new GUILayoutOption[] { GUILayout.MaxWidth(100) });
                generateNodeBind.selectedLayerIndex = EditorGUILayout.Popup(generateNodeBind.selectedLayerIndex, generateNodeBind.layerStringArr, new GUILayoutOption[] { GUILayout.MaxWidth(200) });
                GUILayout.EndHorizontal();
            }

            //
            GUILayout.Space(5);
            reorderableList.DoLayoutList();

            //
            GUILayout.Space(10);
            if (generateNodeBind.isRootNode)
            {
                if (GUILayout.Button("导出"))
                    CodeGenerateFunc.Generate(target as Transform);
            }

            //
            CheckAndSavePrefabChanges();


            serializedObject.ApplyModifiedProperties();
        }

    }

    private void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Code Generate Node");
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        CodeGenerateNodeBind generateNodeBind = target.GetComponent<CodeGenerateNodeBind>();
        if (generateNodeBind == null)
            return;

        List<ComponentStruct> componentStructs = generateNodeBind.GetExportComponents();
        List<Type> components = generateNodeBind.GetElementComponents();
        ComponentStruct componentStruct = componentStructs[index];

        //
        componentStruct.VariableName = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width / 2, rect.height - 4), componentStruct.VariableName);
        int enumIndex = EditorGUI.Popup(new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2, rect.height), componentStruct.SelectedComponentIndex, GetComponentsString(components));
        if (enumIndex != componentStruct.SelectedComponentIndex)
        {
            componentStruct.SelectedComponentIndex = enumIndex;
            componentStruct.VariableName = string.Format("{0}{1}", target.name, components[componentStruct.SelectedComponentIndex].Name);
        }

        componentStruct.ComponentType = components[componentStruct.SelectedComponentIndex];

        //
        componentStructs[index] = componentStruct;
    }

    string[] GetComponentsString(List<Type> components)
    {
        List<string> strings = new List<string>();

        for (int i = 0; i < components.Count; i++)
        {
            strings.Add(components[i].Name);
        }

        return strings.ToArray();
    }

    private void DrawOnRemoveCallBack(ReorderableList list)
    {
        CodeGenerateNodeBind generateNodeBind = target.GetComponent<CodeGenerateNodeBind>();
        if (generateNodeBind == null)
            return;

        generateNodeBind.RemoveExportComponent(list.index);
    }

    private void DrawOnAddCallBack(ReorderableList list)
    {
        CodeGenerateNodeBind generateNodeBind = target.GetComponent<CodeGenerateNodeBind>();
        if (generateNodeBind == null)
            return;

        generateNodeBind.AddExportComponent();
    }

    void OnExportButtonClick()
    {
        target.AddComponent<CodeGenerateNodeBind>();
    }

    private void OnBanExportButtonClick()
    {
        DestroyImmediate(target.GetComponent<CodeGenerateNodeBind>());
        GameObject targetObject = (target as Transform).gameObject;
        EditorUtility.SetDirty(targetObject);
    }

    private void CheckAndSavePrefabChanges()
    {
        if (GUI.changed && !Application.isPlaying)
        {
            GameObject targetObject = (target as Transform).gameObject;
            EditorUtility.SetDirty(targetObject);
        }
    }
    #endregion
}

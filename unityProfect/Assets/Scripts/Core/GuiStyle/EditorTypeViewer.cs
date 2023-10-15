using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class EditorTypeViewer : EditorWindow
{
    private int selectionGridIndex;
    private string[] buttonNames = new string[] { "First", "Second", "Third" };

    float sliders = 0;
    int slidera = 0;
    string BeginToggleGroup = "BeginToggleGroup";
    bool ToggleGroup = false;
    string inputField = "";
    string inputLimitField = string.Empty;
    bool fg = false;
    float sliderValue = 0;
    float scrollBarValue = 0;
    int count = 0;
    string tag = "aaa";
    int Layerfield = 0;
    string[] pathname = new string[] { "All", "Asset", "..." };
    float minVal = 1;
    float maxVal = 2;
    float minLimit = -5;
    float maxLimit = 5;
    static Vector3 center = new Vector3(1, 2, 3);
    static Vector3 size1 = new Vector3(1, 2, 3);
    Bounds _bounds = new Bounds(center, size1);
    Color m_color = Color.white;
    AnimationCurve m_curve = AnimationCurve.Linear(0, 0, 10, 10);
    Vector4 rotationComponents;
    int flags = 0;
    string[] options = new string[] { "CanJump", "CanShoot", "CanSwim", "Canabc", "Canacc" };
    GameObject game;
    bool showFoldout;
    Vector2 m_vector2 = new Vector2();
    Vector3 m_vector3 = new Vector3();
    Vector4 m_vector4 = new Vector4();
    bool fold;
    bool fold2;


    [MenuItem("EditorGUITools/GUI 种类展示")]
    static void window()
    {
        EditorTypeViewer mybianyi = GetWindow<EditorTypeViewer>();
        mybianyi.Show();
    }

    Vector2 windowScrollViewPositon = Vector2.zero;

    private void OnGUI()
    {
        windowScrollViewPositon = GUILayout.BeginScrollView(windowScrollViewPositon, true, true);

        //
        ShowLine("文本");
        GUILayout.Label("我的编译器（My compiler）"); //提示语句
        EditorGUILayout.SelectableLabel("这是可以复制的文本");

        //
        ShowLine("按钮");
        if (GUILayout.Button("按钮"))
            Debug.Log("点击了按钮");

        GUILayout.Space(3);
        selectionGridIndex = GUILayout.SelectionGrid(selectionGridIndex, buttonNames, 3);       // 3是指 一排放3个

        GUILayout.Space(3);
        selectionGridIndex = GUILayout.Toolbar(selectionGridIndex, buttonNames);

        //
        ShowLine("输入框");

        GUILayout.BeginHorizontal();
        GUILayout.Label("输入框", GUILayout.MaxWidth(200));
        inputField = GUILayout.TextField(inputField);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("输入框，限制字数：", GUILayout.MaxWidth(200));
        inputLimitField = GUILayout.TextField(inputLimitField, 5);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("多行输入框", GUILayout.MaxWidth(200));
        inputLimitField = EditorGUILayout.TextArea(inputLimitField);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("密码输入框：", GUILayout.MaxWidth(200));
        inputField = GUILayout.PasswordField(inputField, '*');
        GUILayout.EndHorizontal();

        _bounds = EditorGUILayout.BoundsField("BoundsField:", _bounds);
        m_color = EditorGUILayout.ColorField("ColorField:", m_color);
        m_curve = EditorGUILayout.CurveField("CurveField:", m_curve);
        tag = EditorGUILayout.TagField("TagField:", tag);
        Layerfield = EditorGUILayout.LayerField("LayerField:", Layerfield);
        flags = EditorGUILayout.MaskField("MaskField:", flags, options);
        game = (GameObject)EditorGUILayout.ObjectField(game, typeof(GameObject), true);//typeof(类型) 确定好类型系统会自动帮我找到所有的关于这个类型的物体

        m_vector2 = EditorGUILayout.Vector2Field("Vector2:", m_vector2);
        m_vector3 = EditorGUILayout.Vector3Field("Vector3:", m_vector3);
        m_vector4 = EditorGUILayout.Vector4Field("Vector4:", m_vector4);

        //
        ShowLine("开关");
        fg = EditorGUILayout.Toggle("这是一个开关", fg);

        ToggleGroup = EditorGUILayout.BeginToggleGroup(BeginToggleGroup, ToggleGroup);
        inputField = GUILayout.TextField(inputField);
        EditorGUILayout.EndToggleGroup();

        //
        ShowLine("进度条");         // 进度条要设置他的宽高，不然不显示
        sliderValue = GUILayout.HorizontalSlider(sliderValue, 0, 10, GUILayout.Width(300));
        sliderValue = GUILayout.VerticalSlider(sliderValue, 10, 0, GUILayout.Height(100));
        sliders = EditorGUILayout.Slider("Slider:", sliders, 0, 10);
        EditorGUILayout.LabelField("Min Val:", minVal.ToString());
        EditorGUILayout.LabelField("Max Val:", maxVal.ToString());
        EditorGUILayout.MinMaxSlider("MinMaxSlider", ref minVal, ref maxVal, minLimit, maxLimit);
        slidera = EditorGUILayout.IntSlider("IntSlider:", slidera, 1, 10);

        //
        ShowLine("滚动条");
        float scrollbarSize = 10;   // 滑块大小
        scrollBarValue = GUILayout.HorizontalScrollbar(scrollBarValue, scrollbarSize, 0, 10, GUILayout.Width(300));
        scrollBarValue = GUILayout.VerticalScrollbar(scrollBarValue, scrollbarSize, 0, 10, GUILayout.Height(100));

        //
        ShowLine("FlexibleSpace 用法");

        GUILayout.BeginHorizontal();
        GUILayout.Button("B1", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.Button("B2", GUILayout.Width(50));
        GUILayout.EndHorizontal();
        GUILayout.Button("length");

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Button("B1", GUILayout.Width(100));
        GUILayout.Space(200);
        GUILayout.Button("B2", GUILayout.Width(50));
        GUILayout.EndHorizontal();
        GUILayout.Button("length");

        //
        ShowLine("下拉");
        count = EditorGUILayout.Popup("下拉：", count, pathname);

        //
        ShowLine("折叠");
        showFoldout = EditorGUILayout.Foldout(showFoldout, "折叠子物体：");
        if (showFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("折叠块内容1");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("折叠块内容2");
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("折叠块内容3");
        }

        //
        ShowLine("另一种分割线");
        EditorGUILayout.BeginHorizontal();//开始最外层横向布局
        GUILayout.FlexibleSpace();//布局之间左右对齐
        GUILayout.Label("-----------------分割线-----------------");
        GUILayout.FlexibleSpace();//布局之间左右对齐
        EditorGUILayout.EndHorizontal();

        //
        ShowLine("提示");
        EditorGUILayout.HelpBox("HelpBox Error:", MessageType.Error);//红色错误号
        EditorGUILayout.HelpBox("HelpBox Info:", MessageType.Info);//白色提示号
        EditorGUILayout.HelpBox("HelpBox None:", MessageType.None);//解释号
        EditorGUILayout.HelpBox("HelpBox Warning:", MessageType.Warning);//黄色警告号

        
        if (Selection.activeGameObject)
        {
            Transform selectedTransform = Selection.activeGameObject.transform;
            fold = EditorGUILayout.InspectorTitlebar(fold, selectedTransform);
            if (fold)
            {
                selectedTransform.position = EditorGUILayout.Vector3Field("Position", selectedTransform.position);
                EditorGUILayout.Space();
                rotationComponents = EditorGUILayout.Vector4Field("Detailed Rotation", QuaternionToVector4(selectedTransform.localRotation));
                EditorGUILayout.Space();
                selectedTransform.localScale = EditorGUILayout.Vector3Field("Scale", selectedTransform.localScale);
            }
        }


        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject)
        {
            fold2 = EditorGUILayout.InspectorTitlebar(fold2, selectedGameObject);
            if (fold2)
            {
                selectedGameObject.transform.position = EditorGUILayout.Vector3Field("Position", selectedGameObject.transform.position);
                EditorGUILayout.Space();
                rotationComponents = EditorGUILayout.Vector4Field("Detailed Rotation", QuaternionToVector4(selectedGameObject.transform.localRotation));
                EditorGUILayout.Space();
                selectedGameObject.transform.localScale = EditorGUILayout.Vector3Field("Scale", selectedGameObject.transform.localScale);
            }
        }


        GUILayout.EndScrollView();
    }

    Quaternion ConvertToQuaternion(Vector4 v4)
    {
        return new Quaternion(v4.x, v4.y, v4.z, v4.w);
    }

    Vector4 QuaternionToVector4(Quaternion q)
    {
        return new Vector4(q.x, q.y, q.z, q.w);
    }

    void ShowLine(string typeStr = "")
    {
        GUILayout.Space(5);

        GUI.color = Color.green;
        GUILayout.Label($"-------------------------------------------- {typeStr} --------------------------------------------", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
        GUI.color = Color.white;

        GUILayout.Space(5);
    }
}
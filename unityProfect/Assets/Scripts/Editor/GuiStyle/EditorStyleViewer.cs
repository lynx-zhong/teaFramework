using UnityEngine;
using UnityEditor;

public class EditorStyleViewer : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private string search = string.Empty;

    [MenuItem("EditorGUITools/GUI 风格展示")]
    public static void Init()
    {
        GetWindow<EditorStyleViewer>();
    }

    void OnGUI()
    {
        //
        GUILayout.BeginHorizontal("HelpBox");

        GUILayout.Label("单击示例将复制其名到剪贴板", "label");
        GUILayout.FlexibleSpace();
        GUILayout.Label("查找:");
        search = EditorGUILayout.TextField(search);

        GUILayout.EndHorizontal();

        //
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (GUIStyle style in GUI.skin)
        {
            if (style.name.ToLower().Contains(search.ToLower()))
            {
                DisplayGUIStyle(style);
            }
        }

        GUILayout.EndScrollView();
    }

    void DisplayGUIStyle(GUIStyle style)
    {
        GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
        GUILayout.Space(100);

        GUILayout.Label(style.name,style);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("复制样式"))
        {
            EditorGUIUtility.systemCopyBuffer = style.name;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(7);
    }
}

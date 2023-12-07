using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EditorIconViewer : EditorWindow
{
    // 从具有给定名称的 Unity 内置资源中获取 GUIContent
    // EditorGUIUtility.IconContent 用于为 GUI 元素创建 GUIContent。
    // 只会加载图标.通常情况下
    //     第一个参数获取 Assets/Editor Default Resources/Icons 中的图标。只需要图标的名称，而不需要 png 扩展名。
    //     第二个参数为悬停工具提示提供文本。此字符串 需要以竖线“|”字符开头以将其标记为工具提示。
    // 注意：目前无法悬停定位在工具提示上方。


    private static List<GUIContent> icons;

    [MenuItem("EditorGUITools/GUI 展示Icon")]
    private static void GetEditorIcons()
    {
        icons = new List<GUIContent>();

        GetWindow<EditorIconViewer>("图标");

        Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
        foreach (Texture2D texture in textures)
        {
            GUIContent icon = EditorGUIUtility.IconContent(texture.name, $"|{texture.name}");
            if (icon != null && icon.image != null)
                icons.Add(icon);
        }
    }

    Vector2 scrollPostion;

    void OnGUI()
    {
        scrollPostion = GUILayout.BeginScrollView(scrollPostion);

        for (int i = 0; i < icons.Count; i += 35)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 35; j++)
            {
                if (i + j < icons.Count)
                {
                    if (GUILayout.Button(icons[i + j], GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        EditorGUIUtility.systemCopyBuffer = icons[i + j].tooltip;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
}

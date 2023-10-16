using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InspectorMode))]
public class Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        Debug.Log("绘制");

        // base.OnInspectorGUI();

        // SerializedProperty property = serializedObject.FindProperty("testInt");
        // Debug.Log(property.intValue);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;


public class TestEditor : Editor
{
    [MenuItem("Zzz/test")]
    static void Test()
    {
        Assembly assembly = Assembly.Load("Assembly-CSharp");
        Type type = assembly.GetType("CodeGenerate");
        object obj = Activator.CreateInstance(type);
        FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);


        GameObject game = GameObject.Find("Canvas");
        Component component = game.AddComponent(type);

        for (int i = 0; i < fieldInfos.Length; i++)
        {
            Debug.Log(fieldInfos[i].Name);
            fieldInfos[i].SetValue(component,game);
        }

        DestroyImmediate(game);

    }
}

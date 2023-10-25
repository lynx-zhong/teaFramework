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
        string Aa = "hesdasadasd";

        Debug.Log(Aa.IndexOf("public"));
    }
}

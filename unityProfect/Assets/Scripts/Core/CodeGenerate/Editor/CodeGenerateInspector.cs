using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace CodeGenetate
{
    // [CustomEditor(typeof(CodeGenerateNodeBind))]
    public class CodeGenerateInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            target.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;

            CodeGenerateNodeBind codeGenerateNodeBind = target.GetComponent<CodeGenerateNodeBind>();
            codeGenerateNodeBind.TestString = "555";

            // SerializedProperty serializedProperty = target
        }
    }
}
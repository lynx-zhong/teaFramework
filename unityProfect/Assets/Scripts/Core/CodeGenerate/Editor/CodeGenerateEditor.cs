using UnityEngine;
using UnityEditor;

namespace CodeGenerate
{
    public class CodeGenerateHierarchy : Editor
    {
        [InitializeOnLoadMethod]
        static void CodeGenerateInitializeOnLoadMethod()
        {
            EditorApplication.hierarchyWindowItemOnGUI += CodeGenerateHierarchyOnGui;
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

    // [CustomEditor(typeof(CodeGenerateNodeBind))]
    public class CodeGenerateInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            target.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
        }
    }
}
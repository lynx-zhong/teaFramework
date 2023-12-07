using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class AddressableGroupSettingWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;

    [MenuItem("Tools/打包/BuildGroupSetting")]
    public static void ShowExample()
    {
        AddressableGroupSettingWindow wnd = GetWindow<AddressableGroupSettingWindow>();
        wnd.titleContent = new GUIContent("AddressableGroupSettingWindow");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        VisualElement labelFromUXML = visualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        
    }
}

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.Linq;

namespace AddressableGroup
{
    public class BundleGroupSettingEditorWindow : EditorWindow
    {
        private static BundleGroupSettingScriptableObj groupSettingScriptableObj;
        private const string groupDataScriptableObjFullPath = @"Assets\Scripts\Core\Addressable\Editor\BundleGroupSettingConfig.asset";

        [MenuItem("Tools/打包/Bundle Group Setting")]
        public static void ShowWindow()
        {
            var window = GetWindow<BundleGroupSettingEditorWindow>();
            window.titleContent = new GUIContent("Bundle Setting");
            window.Show();
        }
        
        private void OnEnable()
        {
            groupSettingScriptableObj = AssetDatabase.LoadAssetAtPath<BundleGroupSettingScriptableObj>(groupDataScriptableObjFullPath);
            if (groupSettingScriptableObj == null)
            {
                groupSettingScriptableObj = CreateInstance<BundleGroupSettingScriptableObj>();
                AssetDatabase.CreateAsset(groupSettingScriptableObj, groupDataScriptableObjFullPath);
                AssetDatabase.Refresh();
            }
            
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        }

        private void OnDestroy()
        {
            EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGUI;

            if (groupSettingScriptableObj != null)
            {
                EditorUtility.SetDirty(groupSettingScriptableObj);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void OnGUI()
        {
            if (groupSettingScriptableObj == null)
                return;
            
            DrawList();
            
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("创建 Addressable Group",GUILayout.Height(25)))
            {
                DeleteAllGroup();
                CreateGroups();
            }

            if (GUILayout.Button("添加分组",GUILayout.Height(25)))
            {
                groupSettingScriptableObj.AddNewBundleFolder();
            }

            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }
        
        private void DrawList()
        {
            GUILayout.Label("分组控制",GUILayout.Height(20));
            
            List<FolderPackingBundleInfo> groupList = groupSettingScriptableObj.GetBundleFolderList();
            if (groupList == null)
            {
                return;
            }
            for (int i = 0; i < groupList.Count; i++)
            {
                EditorGUILayout.Space(1);
                EditorGUILayout.BeginHorizontal();
                
                FolderPackingBundleInfo folderPackingBundleInfo = groupList[i];

                folderPackingBundleInfo.FolderAsset = EditorGUILayout.ObjectField("", folderPackingBundleInfo.FolderAsset, typeof(DefaultAsset), false,GUILayout.Width(180)) as DefaultAsset;
                if (folderPackingBundleInfo.FolderAsset != null)
                {
                    string path = AssetDatabase.GetAssetPath(folderPackingBundleInfo.FolderAsset);
                    GUILayout.Label(path.Replace("Assets/",""));
                    folderPackingBundleInfo.FolderPath = path;
                }
                
                GUILayout.FlexibleSpace();
                folderPackingBundleInfo.packingBuildMode = (PackingBuildMode)EditorGUILayout.EnumPopup("", folderPackingBundleInfo.packingBuildMode);
                groupList[i] = folderPackingBundleInfo;
                
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    groupList.RemoveAt(i);
                    EditorUtility.SetDirty(groupSettingScriptableObj);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space(1);
        }

        public static void CreateGroups()
        {
            List<FolderPackingBundleInfo> groupList = groupSettingScriptableObj.GetBundleFolderList();
            foreach (var group in groupList)
            {
                if (string.IsNullOrEmpty(group.FolderPath))
                    continue;
                
                var groupName = Path.GetFileName(group.FolderPath);
                if (group.packingBuildMode == PackingBuildMode.PackFolderAndChildPackFolder)
                {
                    string[] allFolderPath = Directory.GetDirectories(group.FolderPath,"*",SearchOption.AllDirectories);
                    foreach (var path in allFolderPath)
                    {
                        var name = $"{groupName}-{Path.GetFileName(path)}";
                        CreateSingleGroup(name,path,BundledAssetGroupSchema.BundlePackingMode.PackTogether);
                    }
                }
                else
                {
                    BundledAssetGroupSchema.BundlePackingMode packingMode = group.packingBuildMode == PackingBuildMode.PackTogether ? BundledAssetGroupSchema.BundlePackingMode.PackTogether : BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                    CreateSingleGroup(groupName,group.FolderPath,packingMode);
                }
            }
            
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void CreateSingleGroup(string groupName,string assetFolderPath,BundledAssetGroupSchema.BundlePackingMode packingMode)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup tempGroupIns = settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
                
            ContentUpdateGroupSchema updateSchema = tempGroupIns.GetSchema<ContentUpdateGroupSchema>();
            updateSchema.StaticContent = true;

            BundledAssetGroupSchema bundledAssetGroupSchema = tempGroupIns.GetSchema<BundledAssetGroupSchema>();
            bundledAssetGroupSchema.BundleMode = packingMode;

            string[] allFiles = Directory.GetFiles(assetFolderPath).Where(file => !file.EndsWith(".meta")).ToArray();
            foreach (var filePath in allFiles)
            {
                var guid = AssetDatabase.AssetPathToGUID(filePath);
                settings.CreateOrMoveEntry(guid, tempGroupIns, false, false);
            }
            
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        static void DeleteAllGroup()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            List<AddressableAssetGroup> groupListCopy = new List<AddressableAssetGroup>(settings.groups);
            foreach (var group in groupListCopy.Where(group => !IsDefaultGroup(group)))
            {
                settings.RemoveGroup(group);
            }

            EditorUtility.SetDirty(settings);
            AssetDatabase.Refresh();
        }
        
        static bool IsDefaultGroup(AddressableAssetGroup group)
        {
            return group != null && group.Name is "Built In Data" or "Default Local Group";
        }

        // void DragCheck()
        // {
        //     GUI.color = Color.green;
        //     var are = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        //     GUIContent des = new GUIContent("\n拖拽组件对象到此可快速绑定");
        //     GUI.Box(are, des);
        //
        //     if (are.Contains(Event.current.mousePosition)) 
        //     {
        //         DragAndDrop.AcceptDrag();
        //         DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        //
        //         switch (Event.current.type)
        //         {
        //             case EventType.DragPerform:
        //                 if (are.Contains(Event.current.mousePosition))
        //                 {
        //                     for (int i = 0; i < DragAndDrop.objectReferences.Length; ++i)
        //                     {
        //                         GameObject reference = DragAndDrop.objectReferences[i] as GameObject;
        //                     }
        //                 }
        //                 break;
        //         }
        //     }
        //    
        //     GUI.color = Color.white;
        // }

        private static void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!groupSettingScriptableObj.IsAddBundleFolder(assetPath))
                return;

            Rect targetRect = new Rect(selectionRect.x - 28, selectionRect.y, 30, selectionRect.height);
            GUI.color = Color.red;
            GUI.Label(targetRect, "\u2714");
            GUI.color = Color.white;
        }
    }
}
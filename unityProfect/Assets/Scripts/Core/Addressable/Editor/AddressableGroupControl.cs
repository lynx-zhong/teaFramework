using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace AddressableGroup
{
    public class AddressableGroupControl : EditorWindow
    {
        // 保存数据的列表
        private List<FolderBundleData> dataList = new List<FolderBundleData>();

        // 示例文件夹选择器的路径
        private string selectedFolderPath = "Assets";

        // 示例枚举选择器的选择
        private BundleMode selectedEnum = BundleMode.PackTogether;

        [MenuItem("Tool/打包/Bundle Group")]
        private static void ShowWindow()
        {
            var window = GetWindow<AddressableGroupControl>();
            window.titleContent = new GUIContent("Bundle 分组控制");
            window.Show();
        }
        
        // 在窗口启动时加载数据
        // private void OnEnable()
        // {
        //     // 示例：从 EditorPrefs 加载数据
        //     selectedFolderPath = EditorPrefs.GetString("SelectedFolderPath", "Assets");
        //     selectedEnum = (BundleMode)EditorPrefs.GetInt("SelectedEnum", (int)BundleMode.PackTogether);
        //
        //     // 加载列表数据
        //     string dataJson = EditorPrefs.GetString("DataList", "");
        //     if (!string.IsNullOrEmpty(dataJson))
        //     {
        //         dataList = JsonUtility.FromJson<List<FolderBundleData>>(dataJson);
        //     }
        // }

        private void OnGUI()
        {
            // 绘制列表
            DrawList();

            // 绘制文件夹选择和枚举选择
            // DrawSelectionControls();

            // 绘制保存按钮
            if (GUILayout.Button("Save"))
            {
                SaveData();
            }
        }
        
        // 绘制列表
        private void DrawList()
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                // 显示文件夹选择字段
                dataList[i].selectedFolder = EditorGUILayout.ObjectField("选择文件夹", dataList[i].selectedFolder, typeof(DefaultAsset), false,GUILayout.Width(300)) as DefaultAsset;

                // 显示文件夹路径
                if (dataList[i].selectedFolder != null)
                {
                    GUILayout.Label(AssetDatabase.GetAssetPath(dataList[i].selectedFolder));
                }

                // 枚举选择
                dataList[i].bundleMode = (BundleMode)EditorGUILayout.EnumPopup("打包方式", dataList[i].bundleMode);

                // 移除按钮
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    dataList.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("添加分组控制"))
            {
                dataList.Add(new FolderBundleData());
            }
        }

        // 绘制文件夹选择和枚举选择
        private void DrawSelectionControls()
        {
            EditorGUILayout.BeginHorizontal();

            // 文件夹选择
            GUILayout.Label("Selected Folder:");
            selectedFolderPath = EditorGUILayout.TextField(selectedFolderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                // 打开文件夹选择面板
                string newFolderPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
                if (!string.IsNullOrEmpty(newFolderPath))
                {
                    selectedFolderPath = newFolderPath;
                }
            }

            // 枚举选择
            GUILayout.Label("Selected Enum:");
            selectedEnum = (BundleMode)EditorGUILayout.EnumPopup(selectedEnum);

            EditorGUILayout.EndHorizontal();
        }

        // 保存数据
        private void SaveData()
        {
            // 示例：将数据保存到 EditorPrefs
            // 实际项目中可能会使用其他方式进行数据持久化
            EditorPrefs.SetString("SelectedFolderPath", selectedFolderPath);
            // EditorPrefs.SetInt("SelectedEnum", (int)selectedEnum);

            // 保存列表数据
            string dataJson = JsonUtility.ToJson(dataList);
            EditorPrefs.SetString("DataList", dataJson);

            Debug.Log("Data saved!");
        }


    }
}
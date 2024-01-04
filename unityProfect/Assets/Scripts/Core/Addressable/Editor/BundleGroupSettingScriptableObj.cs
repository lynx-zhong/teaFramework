using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Serialization;

namespace AddressableGroup
{
    [Serializable]
    public struct FolderPackingBundleInfo
    {
        public DefaultAsset FolderAsset;
        public string FolderPath;
        public PackingBuildMode packingBuildMode;
    }

    public enum PackingBuildMode
    {
        /// <summary>
        /// 文件夹内打包在一起
        /// </summary>
        PackTogether,
        
        /// <summary>
        /// 文件夹内所有的东西分别打包
        /// </summary>
        PackSeparately,
        
        /// <summary>
        /// 文件夹内，子文件夹，子子文件夹的，每个文件夹打一个bundle
        /// </summary>
        PackFolderAndChildPackFolder
    }

    public class BundleGroupSettingScriptableObj : ScriptableObject
    {
        [SerializeField]
        private List<FolderPackingBundleInfo> buildFolderList;

        private void OnEnable()
        {
            if (buildFolderList == null)
                buildFolderList = new List<FolderPackingBundleInfo>();
        }
        
        public List<FolderPackingBundleInfo> GetBundleFolderList()
        {
            return buildFolderList;
        }

        public void AddNewBundleFolder()
        {
            buildFolderList.Add(new FolderPackingBundleInfo());
        }

        public bool IsAddBundleFolder(string assetPath)
        {
            return buildFolderList != null && buildFolderList.Any(item => item.FolderPath == assetPath);
        } 
    }

    [CustomEditor(typeof(BundleGroupSettingScriptableObj))]
    public class BundleGroupSettingScriptableObjInspector : Editor
    {
        public override void OnInspectorGUI()
        {   
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("管理分组",GUILayout.Height(35)))
            {
                BundleGroupSettingEditorWindow.ShowWindow(); 
            }
            
            GUI.backgroundColor = Color.white;
            GUILayout.Space(2);
            base.OnInspectorGUI();
        }
    }
}
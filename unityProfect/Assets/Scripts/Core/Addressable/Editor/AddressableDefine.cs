using System;
using UnityEditor;
using UnityEngine.Serialization;

namespace AddressableGroup
{
    [Serializable]
    public class FolderBundleData
    {
        public string folderPath;
        public BundleMode bundleMode;
        public DefaultAsset selectedFolder;
    }

    // 示例枚举类型
    public enum BundleMode
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
        PackFolder
    }
}
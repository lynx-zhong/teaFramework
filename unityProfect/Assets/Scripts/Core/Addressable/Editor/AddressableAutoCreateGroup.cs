using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AddressableGroupToolEditor : AssetPostprocessor
{
    // 指定的文件夹路径
    private static string targetFolderPath = "Assets/Resources/YourTargetFolder";

    // 指定的 Addressable Group 名称
    private static string groupName = "YourGroupName";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
        // Logger.Log($"导入的资源对象：  ",importedAssets);
        // Logger.Log($"删除的资源对象：  ",deletedAssets);
        // Logger.Log($"移动的资源对象：  ",movedAssets);
        // Logger.Log($"移动到的资源对象：  ",movedFromPath);
        
        // 判断是否有资源变动发生在指定文件夹下
        if (HasAssetChangesInFolder(importedAssets) || HasAssetChangesInFolder(deletedAssets) || HasAssetChangesInFolder(movedAssets) || HasAssetChangesInFolder(movedFromPath))
        {
            // 创建或更新 Addressable Group
            CreateOrUpdateAddressableGroup();
        }
    }

    private static bool HasAssetChangesInFolder(string[] assets)
    {
        foreach (var asset in assets)
        {
            if (asset.StartsWith(targetFolderPath))
            {
                return true;
            }
        }
        return false;
    }

    private static void CreateOrUpdateAddressableGroup()
    {
        // 获取 AddressableSettings
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        // 检查 Group 是否已存在
        AddressableAssetGroup assetGroup = settings.FindGroup(groupName);

        if (assetGroup == null)
        {
            // 如果不存在，则创建新的 Group
            assetGroup = settings.CreateGroup(groupName, false, false, false, null);
            Debug.Log("Addressable Group created: " + groupName);
        }
        else
        {
            // 如果存在，你可以在这里添加更新逻辑
            Debug.Log("Addressable Group already exists: " + groupName);
        }

        // 将指定文件夹下的资源添加到 Group 中
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { targetFolderPath });
        foreach (string assetGuid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            // 添加资源到 Group 中
            settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), assetGroup, false, false);
        }

        // 更新 AddressableSettings
        EditorUtility.SetDirty(settings);
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        AssetDatabase.SaveAssets();
    }
}

using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.IO;

namespace PsdExport
{
    public class PsdExportImage
    {
        public static void Exporte(GameObject go, PsdLayerSerializer layer, PsdRootNodeSerializer rootNodeSerializer, string moduleName)
        {
            Image image = go.AddComponent<Image>();

            //
            string spritePath = Path.Combine(PsdExportConfig.Instance.UnityUIPngFolder, moduleName, layer.NodeName + ".png");
            if (layer.NodeName.StartsWith(rootNodeSerializer.CommonUIMark))
                spritePath = Path.Combine(PsdExportConfig.Instance.UnityCommonPngFolder, layer.NodeName + ".png");
            else if (layer.NodeName.StartsWith(rootNodeSerializer.IconUIMark))
            {
                string[] nameArr = layer.NodeName.Split("_");
                string iconFolderName = nameArr[1];
                spritePath = Path.Combine(PsdExportConfig.Instance.UnityIconFolder,iconFolderName, layer.NodeName + ".png");
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            image.sprite = sprite;
            image.SetNativeSize();

            //
            image.rectTransform.sizeDelta = layer.Size;
            image.rectTransform.anchoredPosition = layer.Position;

            //
            image.type = layer.ImageType;

            //
            Color color = image.color;
            color.a = 100 / layer.Alpha;

            //
            SetParentButtonGraphic(image, layer);
        }

        static void SetParentButtonGraphic(Image image, PsdLayerSerializer layer)
        {
            if (layer.ButtonStatus == PsdButtonStatus.Normal)
            {
                image.raycastTarget = true;
                Button parentButton = image.transform.GetComponentInParent<Button>();
                parentButton.targetGraphic = image;
            }
            else
            {
                image.raycastTarget = false;
            }
        }
    }
}
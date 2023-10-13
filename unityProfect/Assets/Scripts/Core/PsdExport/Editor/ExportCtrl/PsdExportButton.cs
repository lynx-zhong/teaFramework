using UnityEngine.UI;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace PsdExport
{
    public class PsdExportButton
    {
        public static void Exporte(GameObject go, PsdLayerSerializer layer, PsdRootNodeSerializer rootNodeSerializer, string moduleName)
        {
            Button button = go.AddComponent<Button>();

            //
            if (layer.BaseType != PsdBaseType.Null)
            {
                Graphic graphic = go.GetComponent<Graphic>();
                button.targetGraphic = graphic;
            }

            //
            bool isSpriteSwapMode = false;
            foreach (var item in layer.LayerList)
            {
                if (item.ButtonStatus != PsdButtonStatus.Null && item.ButtonStatus != PsdButtonStatus.Normal)
                {
                    isSpriteSwapMode = true;
                    break;
                }
            }
            if (isSpriteSwapMode)
            {
                button.transition = Selectable.Transition.SpriteSwap;
                SpriteState spriteState = button.spriteState;
                foreach (PsdLayerSerializer childLayer in layer.LayerList)
                {
                    if (childLayer.ButtonStatus == PsdButtonStatus.Highlighted)
                    {
                        Sprite sprite = LoadSprite(childLayer, moduleName, rootNodeSerializer.CommonUIMark);
                        spriteState.highlightedSprite = sprite;
                    }
                    else if (childLayer.ButtonStatus == PsdButtonStatus.Disabled)
                    {
                        Sprite sprite = LoadSprite(childLayer, moduleName, rootNodeSerializer.CommonUIMark);
                        spriteState.disabledSprite = sprite;
                    }
                    else if (childLayer.ButtonStatus == PsdButtonStatus.Pressed)
                    {
                        Sprite sprite = LoadSprite(childLayer, moduleName, rootNodeSerializer.CommonUIMark);
                        spriteState.pressedSprite = sprite;
                    }
                }
                button.spriteState = spriteState;
            }
        }

        static Sprite LoadSprite(PsdLayerSerializer layer, string moduleName, string commonUIMark)
        {
            string spritePath = Path.Combine(PsdExportConfig.Instance.UnityUIPngFolder, moduleName, layer.NodeName + ".png");
            if (layer.NodeName.StartsWith(commonUIMark))
                spritePath = Path.Combine(PsdExportConfig.Instance.UnityCommonPngFolder, layer.NodeName + ".png");

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            return sprite;
        }
    }
}
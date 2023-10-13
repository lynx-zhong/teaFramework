using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace PsdExport
{
    //TODO 颜色 描边
    public class PsdExportLable
    {
        public static void Exporte(GameObject go, PsdLayerSerializer layer)
        {
            TextMeshProUGUI textMeshPro = go.AddComponent<TextMeshProUGUI>();
            textMeshPro.rectTransform.anchoredPosition = layer.Position;
            textMeshPro.rectTransform.sizeDelta = new Vector2(layer.Size.x,layer.Size.y);

            //
            string fontPath = PsdExportConfig.Instance.GetUnityFontName(layer.Font);
            TMP_FontAsset tMP_Font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
            textMeshPro.font = tMP_Font;

            //
            textMeshPro.text = layer.Content;
            textMeshPro.fontSize = layer.FontSize;
            textMeshPro.alignment = layer.Alignment;
            textMeshPro.color = layer.Color;

            //
            if (layer.IsBold)
                textMeshPro.fontStyle |= FontStyles.Bold;

            //
            if (layer.IsItalic)
                textMeshPro.fontStyle |= FontStyles.Italic;

            //
            if (layer.IsUnderline)
                textMeshPro.fontStyle |= FontStyles.Underline;

            //
            if (layer.IsStrikethrough)
                textMeshPro.fontStyle |= FontStyles.Strikethrough;

            //
            SetParentButtonGraphic(textMeshPro,layer);
        }

        static void SetParentButtonGraphic(TextMeshProUGUI textMeshPro,PsdLayerSerializer layer)
        {
            if (layer.ButtonStatus == PsdButtonStatus.Normal)
            {
                textMeshPro.raycastTarget = true;
                Button parentButton = textMeshPro.transform.GetComponentInParent<Button>();
                parentButton.targetGraphic = textMeshPro;
            }
            else
            {
                textMeshPro.raycastTarget = false;
            }
        }

    }
}
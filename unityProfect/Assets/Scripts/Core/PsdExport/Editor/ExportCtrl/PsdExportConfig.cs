
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PsdExport
{
    [CreateAssetMenu(fileName = "PsdExportConfig", menuName = "ScriptableObject/PsdExportConfig")]
    public class PsdExportConfig : ScriptableObject
    {
        private static PsdExportConfig psdExportConfig;

        public static PsdExportConfig Instance
        {
            get
            {
                if (psdExportConfig == null)
                    psdExportConfig = AssetDatabase.LoadAssetAtPath<PsdExportConfig>(@"Assets\Scripts\Core\PsdExport\Editor\Config\PsdExportConfig.asset");

                return psdExportConfig;
            }
        }

        public string RootUxmlPath => @"Assets\Scripts\Core\PsdExport\Editor\Window\PsdExport.uxml";
        public string ArtPsdFolderName => "Psd目录";
        public string ArtXmlFolderName => "xmlFolder";


        [Header("屏幕设计分辨率")]
        public Vector2 UIRefrenceResolution = new Vector2(1125, 2436);

        [Header("UI图片目录")]
        public string UnityUIPngFolder = "Assets/AssetsPackage/UIImage";

        [Header("通用UI图片目录")]
        public string UnityCommonPngFolder = "Assets/AssetsPackage/UIImage/UICommon";

        [Header("图集存放目录")]
        public string SpriteAtlasFolder = "Assets/AssetsPackage/UIAtlas";

        [Header("Icon目录")]
        public string UnityIconFolder = "Assets/AssetsPackage/UIIcon";

        [Header("字体目录")]
        public string FontPath = "Assets/Font";

        public string GetUnityFontName(string psdFontName)
        {
            string fontName = "system SDF.asset";
            return Path.Combine(FontPath, fontName);
        }
    }
}
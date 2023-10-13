using System.Xml.Serialization;

namespace PsdExport
{
    [XmlRoot("RootNode")]
    public class PsdRootNodeSerializer
    {
        [XmlAttribute("prefabName")]
        public string PrefabName { get; set; }

        [XmlAttribute("moduleName")]
        public string ModuleName { get; set; }

        [XmlAttribute("pngFolderPath")]
        public string PngFolderPath { get; set; }

        [XmlAttribute("commonUIFolderPath")]
        public string CommonUIFolderPath { get; set; }

        [XmlAttribute("CommonUIMark")]
        public string CommonUIMark { get; set; }

        [XmlAttribute("IconUIMark")]
        public string IconUIMark { get; set; }

        [XmlElement("Layer")]
        public PsdLayerSerializer RootLayer { get; set; }
    }
}


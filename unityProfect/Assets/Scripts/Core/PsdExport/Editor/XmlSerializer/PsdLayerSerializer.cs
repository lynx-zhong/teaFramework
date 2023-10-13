using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace PsdExport
{
    [XmlRoot("LayerSet")]
    public class PsdLayerSerializer
    {
        [XmlAttribute("name")]
        public string NodeName { get; set; }

        [XmlAttribute("ComponentType")]
        public string ComponentTypeString { get; set; }
        public PsdComponentType ComponentType
        {
            get
            {
                if (ComponentTypeString == null)
                    return PsdComponentType.Null;

                return Enum.Parse<PsdComponentType>(ComponentTypeString);
            }
        }

        [XmlAttribute("BaseType")]
        public string BaseTypeString { get; set; }
        public PsdBaseType BaseType
        {
            get
            {
                if (BaseTypeString == null)
                {
                    return PsdBaseType.Null;
                }
                return Enum.Parse<PsdBaseType>(BaseTypeString);
            }
        }

        [XmlAttribute("Color")]
        public string ColorString { get; set; }
        public Color Color
        {
            get
            {
                Color color;
                ColorUtility.TryParseHtmlString(ColorString, out color);
                return color;
            }
        }


        [XmlAttribute("Alpha")]
        public float Alpha { get; set; }

        [XmlAttribute("Rect")]
        public string Rect;
        public Vector2 Position
        {
            get
            {
                if (Rect == null)
                    return Vector2.zero;

                string[] strArr = Rect.Split(",");
                return new Vector2(float.Parse(strArr[0]), float.Parse(strArr[1]));
            }
        }
        public Vector2 Size
        {
            get
            {
                if (Rect == null)
                    return Vector2.zero;

                string[] strArr = Rect.Split(",");

                if (BaseType == PsdBaseType.Lable)
                    return new Vector2(float.Parse(strArr[2]) + 20, float.Parse(strArr[3]) + 20);

                return new Vector2(float.Parse(strArr[2]), float.Parse(strArr[3]));
            }
        }

        [XmlAttribute("IsNotExport")]
        public string IsNotExportString { get; set; }
        public bool IsNotExport
        {
            get
            {
                if (IsNotExportString == null)
                    return false;

                if (IsNotExportString == "false")
                    return false;

                return true;
            }
        }


        [XmlElement("Layer")]
        public List<PsdLayerSerializer> LayerList;

        #region Lable 相关属性

        [XmlAttribute("Font")]
        public string Font { get; set; }

        [XmlAttribute("Content")]
        public string Content { get; set; }

        [XmlAttribute("FontSize")]
        public float FontSize { get; set; }

        [XmlAttribute("Alignment")]
        public string AlignmentString { get; set; }
        public TextAlignmentOptions Alignment
        {
            get
            {
                if (AlignmentString == "Justification.RIGHT")
                    return TextAlignmentOptions.MidlineRight;
                else if (AlignmentString == "Justification.LEFT")
                    return TextAlignmentOptions.MidlineLeft;

                return TextAlignmentOptions.MidlineRight;
            }
        }

        [XmlAttribute("Bold")]
        public string IsBoldString { get; set; }
        public bool IsBold
        {
            get
            {
                if (IsBoldString == "true")
                    return true;

                return false;
            }
        }

        [XmlAttribute("Italic")]
        public string Italictring { get; set; }
        public bool IsItalic
        {
            get
            {
                if (Italictring == "true")
                    return true;

                return false;
            }
        }

        [XmlAttribute("Underline")]
        public string IsUnderlineString { get; set; }
        public bool IsUnderline
        {
            get
            {
                if (IsUnderlineString == "true")
                    return true;

                return false;
            }
        }

        [XmlAttribute("Strikethrough")]
        public string IsStrikethroughString { get; set; }
        public bool IsStrikethrough
        {
            get
            {
                if (IsStrikethroughString == "true")
                    return true;

                return false;
            }
        }

        #endregion

        #region Image 相关属性

        [XmlAttribute("ImageType")]
        public string ImageTypeString { get; set; }
        public Image.Type ImageType
        {
            get
            {
                if (ImageTypeString == "Simple")
                    return Image.Type.Simple;
                else if (ImageTypeString == "Sliced")
                    return Image.Type.Sliced;

                return Image.Type.Simple;
            }
        }

        [XmlAttribute("SpriteBorder")]
        public string SpriteBorderString { get; set; }
        public Vector4 SpriteBorder
        {
            get
            {
                string[] strArr = SpriteBorderString.Split(",");
                return new Vector4(float.Parse(strArr[0]), float.Parse(strArr[1]), float.Parse(strArr[2]), float.Parse(strArr[3]));
            }
        }

        #endregion

        #region  Button 相关属性

        [XmlAttribute("ButtonStatus")]
        public string ButtonStatusString { get; set; }
        public PsdButtonStatus ButtonStatus
        {
            get
            {
                if (ButtonStatusString == null)
                    return PsdButtonStatus.Null;

                string enumStr = ButtonStatusString.Replace("Button_", "");
                return Enum.Parse<PsdButtonStatus>(enumStr);
            }
        }

        #endregion



        #region 辅助函数

        public VertexPoint GetVertexPoint()
        {
            VertexPoint vertexPoint = new VertexPoint
            {
                rigthTop = new Vector2(Position.x + Size.x / 2, Position.y + Size.y / 2),
                rightDown = new Vector2(Position.x + Size.x / 2, Position.y - Size.y / 2),
                leftTop = new Vector2(Position.x - Size.x / 2, Position.y + Size.y / 2),
                leftDown = new Vector2(Position.x - Size.x / 2, Position.y - Size.y / 2)
            };
            return vertexPoint;
        }

        #endregion
    }


}
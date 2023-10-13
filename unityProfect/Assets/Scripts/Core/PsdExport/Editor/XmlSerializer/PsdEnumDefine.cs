using UnityEngine;

namespace PsdExport
{
    public enum PsdComponentType
    {
        Null,
        Button,
    }

    public enum PsdBaseType
    {
        Null,
        Image,
        Lable,
    }

    public enum PsdButtonStatus
    {
        Null,
        Normal,
        Highlighted,        // 鼠标进入
        Pressed,            // 点击
        Selected,           // 点击后
        Disabled,           // 失效
    }

    public class VertexPoint
    {
        public VertexPoint()
        {
            rightDown = new Vector2(-100000,100000);
            rigthTop = new Vector2(-100000,-100000);
            
            leftDown = new Vector2(100000,100000);
            leftTop = new Vector2(100000,-100000);
        }

        public VertexPoint(Vector3[] corners)
        {
            rightDown = corners[3];
            rigthTop = corners[2];
            leftDown = corners[0];
            leftTop = corners[1];
        }

        public Vector2 rigthTop;
        public Vector2 rightDown;
        public Vector2 leftTop;
        public Vector2 leftDown;

        public Vector2 GetSize()
        {
            float hight = rigthTop.y - rightDown.y;
            float width = rigthTop.x - leftTop.x;

            return new Vector2(width,hight);
        }

        public Vector2 GetPosition()
        {
            Vector2 size = GetSize();
            return new Vector2(leftDown.x + size.x/2,leftDown.y + size.y/2);
        }
    }
}
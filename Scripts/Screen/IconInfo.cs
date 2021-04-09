using System;
using MNS.Events;
using UnityEngine;

namespace D2Inventory
{

    public class IconInfo
    {

        public Sprite Sprite;

        public Vector2 ScreenSize;

        public Vector2 ScreenPos;

        public Color Color;

        public IconInfo(Sprite sprite, Vector2 screenSize, Vector2 screenPos, Color color)
        {
            Sprite = sprite;
            ScreenSize = screenSize;
            ScreenPos = screenPos;
            Color = color;
        }
        
    }

}


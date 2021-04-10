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

        public Transform Parent;

        public IconInfo(Sprite sprite, Vector2 screenSize, Vector2 screenPos, Color color, Transform parent)
        {
            Sprite = sprite;
            ScreenSize = screenSize;
            ScreenPos = screenPos;
            Color = color;
            Parent = parent;
        }

        // EMPTY static argument to return instead of null

        public bool IsEmpty { get; private set; }

        private IconInfo() => IsEmpty = true;

        public static readonly IconInfo Empty = new IconInfo();
    }

}


using System;
using MNS.Events;
using UnityEngine;

namespace D2Inventory
{

    public class IconInfo
    {

        public IconMode Mode { get; private set; }

        public Sprite Sprite { get; private set; }

        public Vector2 ScreenSize { get; private set; }

        public Vector2 ScreenPos { get; private set; }

        public Color Color { get; private set; }

        public Transform Parent { get; private set; }

        public bool Hidden { get; private set; }

        private IconInfo(Sprite sprite = null, Vector2 screenSize = new Vector2(), Vector2 screenPos = new Vector2(), Transform parent = null, bool hidden = false, IconMode action = 0)
        {
            Mode = action;
            Sprite = sprite;
            ScreenSize = screenSize;
            ScreenPos = screenPos;
            Parent = parent;
            Color = Color.white;
            Hidden = hidden;
        }

        public static readonly IconInfo Delete = new IconInfo(action: IconMode.Delete);

        public static readonly IconInfo Hide = new IconInfo(action: IconMode.Hide);

        public static readonly IconInfo Reveal = new IconInfo(action: IconMode.Reveal);

        public static readonly IconInfo SetHalfOpacity = new IconInfo(action: IconMode.SetHalfOpacity);

        public static readonly IconInfo SetFullOpacity = new IconInfo(action: IconMode.SetFullOpacity);

        public static IconInfo GetMoveOnly(Vector2 pos) => new IconInfo(screenPos: pos, action: IconMode.Move);

        public static IconInfo GetChangeParent(Transform parent) => new IconInfo(parent: parent, action: IconMode.SetParent);

        public static IconInfo GetAllFieldsUpdated(Sprite sprite, Vector2 screenSize, Vector2 screenPos, Transform parent, bool hidden)
             => new IconInfo(sprite, screenSize, screenPos, parent, hidden, IconMode.UpdateAllFields);

    }

    public enum IconMode
    {
        UpdateAllFields,
        Move,
        SetParent,
        Hide,
        Reveal,
        SetHalfOpacity,
        SetFullOpacity,
        Delete
    }
}


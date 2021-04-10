using System.Collections.Generic;
using MNS.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{
    public class IconDrawer 
    {

        private Dictionary<int, Image> _icons = new Dictionary<int, Image>();

        private ObjectPool<Image> _iconsPool;

        public IconDrawer()
        {
            var poolSample = new GameObject("Icon").AddComponent<Image>();
            _iconsPool = new ObjectPool<Image>(poolSample, 15, "Icons Pool");
            GameObject.Destroy(poolSample);
        }

        public int CreateIcon(IconInfo info)
        {
            var newIcon = _iconsPool.Pop();
            int iD = SimpleID.GetNewID();
            _icons.Add(iD, newIcon);

            UpdateAllFields(info, iD);

            return iD;
        }

        public void UpdateAllFields(IconInfo info, int iD)
        {   
            var icon = GetIcon(iD);
           
            icon.sprite = info.Sprite;
            icon.color = info.Color;
            icon.transform.SetParent(info.Parent);
            icon.transform.SetAsFirstSibling();
            icon.rectTransform.sizeDelta = info.ScreenSize;
            icon.transform.position = info.ScreenPos;
        }

        public void MoveIcon(int iD, Vector2 screenPos)
        {
            var icon = GetIcon(iD);

            icon.transform.position = screenPos;
            icon.transform.SetAsLastSibling();
        }

        public void RemoveIcon(int iD)
        {
            var icon = GetIcon(iD);

            _icons.Remove(iD);
            _iconsPool.ReturnItem(icon);
        }

        private Image GetIcon(int iD)
        {
            if(_icons.TryGetValue(iD, out Image icon))
                return icon;

            Debug.LogError($"Icon with id {iD} has not been found");
            return null;
        }
    }
}
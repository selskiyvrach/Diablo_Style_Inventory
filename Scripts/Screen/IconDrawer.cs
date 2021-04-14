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

        public void ApplyIconChange(int iD, IconInfo changeInfo)
        {
                if(changeInfo.Mode == IconMode.
            Delete) RemoveIcon(iD);

                else if(changeInfo.Mode == IconMode.
            MoveOnly) MoveIcon(iD, changeInfo.ScreenPos);

                else if(changeInfo.Mode == IconMode.
            UpdateAllFields) UpdateAllFields(changeInfo, iD);

                else if(changeInfo.Mode == IconMode.
            Hide) HideIcon(iD); 

                else if(changeInfo.Mode == IconMode.
            Reveal) RevealIcon(iD);

                else if(changeInfo.Mode == IconMode.
            ChangeParent) ChangeParent(iD, changeInfo.Parent);
        }

        public void UpdateAllFields(IconInfo info, int iD)
        {   
            if(TryGetIcon(iD, out Image icon))
            {
                icon.sprite = info.Sprite;
                icon.color = info.Color;
                icon.transform.SetParent(info.Parent);
                icon.transform.SetAsFirstSibling();
                icon.rectTransform.sizeDelta = info.ScreenSize;
                icon.transform.position = info.ScreenPos;
            }
        }

        public void MoveIcon(int iD, Vector2 screenPos)
        {
            if(TryGetIcon(iD, out Image icon))
            {
                icon.transform.position = screenPos;
                icon.transform.SetAsLastSibling();
            }
        }

        public void ChangeParent(int iD, Transform parent)
        {
            if(TryGetIcon(iD, out Image icon))
                icon.transform.SetParent(parent);
        }

        public void HideIcon(int iD)
        {
            if(TryGetIcon(iD, out Image icon))
                icon.gameObject.SetActive(false);
        }

        public void RevealIcon(int iD)
        {
            if(TryGetIcon(iD, out Image icon))
                icon.gameObject.SetActive(true);
        }

        public void RemoveIcon(int iD)
        {
            if(TryGetIcon(iD, out Image icon))
            {
                _icons.Remove(iD);
                _iconsPool.ReturnItem(icon);
            }
        }

        private bool TryGetIcon(int iD, out Image icon)
        {
            icon = null;

            if(_icons.TryGetValue(iD, out Image icon2))
                icon = icon2;
            else
                Debug.LogError($"Icon with id {iD} has not been found");

            return icon != null;
        }
    }
}
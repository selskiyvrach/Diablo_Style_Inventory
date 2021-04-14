using System;
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

        public Color HalfOpacity { get; set; } = new Color(1, 1, 1, .5f);

        public Color FullOpacity { get; set; } = new Color(1, 1, 1, 1);

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
            Move) MoveIcon(iD, changeInfo.ScreenPos);

                else if(changeInfo.Mode == IconMode.
            UpdateAllFields) UpdateAllFields(changeInfo, iD);

                else if(changeInfo.Mode == IconMode.
            Hide) HideIcon(iD, true); 

                else if(changeInfo.Mode == IconMode.
            Reveal) HideIcon(iD, false); 

                else if(changeInfo.Mode == IconMode.
            SetParent) ChangeParent(iD, changeInfo.Parent);
            
                else if(changeInfo.Mode == IconMode.
            SetHalfOpacity) SetHalfOpacity(iD, true);

                else if(changeInfo.Mode == IconMode.
            SetFullOpacity) SetHalfOpacity(iD, false);
        }

        public void UpdateAllFields(IconInfo info, int iD)
            => ApplyAction(iD, (icon) => { 
                icon.sprite = info.Sprite;
                icon.color = info.Color;
                icon.transform.SetParent(info.Parent);
                icon.transform.SetAsFirstSibling();
                icon.rectTransform.sizeDelta = info.ScreenSize;
                icon.transform.position = info.ScreenPos; });

        public void MoveIcon(int iD, Vector2 screenPos)
            => ApplyAction(iD, (icon) => { 
                icon.transform.position = screenPos; 
                icon.transform.SetAsLastSibling();});

        public void ChangeParent(int iD, Transform parent)
            => ApplyAction(iD, (icon) => 
                icon.transform.SetParent(parent));

        public void HideIcon(int iD, bool value)
            => ApplyAction(iD, (icon) => 
                icon.gameObject.SetActive(!value));

        public void RemoveIcon(int iD)
            => ApplyAction(iD, (icon) => { 
                _icons.Remove(iD); 
                _iconsPool.ReturnItem(icon);});

        private void SetHalfOpacity(int iD, bool value)
            => ApplyAction(iD, (icon) => 
                icon.color = value ? HalfOpacity : FullOpacity);

        private void ApplyAction(int iD, Action<Image> toApply)
        {
            if(_icons.TryGetValue(iD, out Image icon))
                toApply(icon);
            else 
                Debug.LogError($"Icon with id {iD} has not been found");
        }
    }
}
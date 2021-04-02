using System;
using UnityEngine;

namespace D2Inventory
{
    [RequireComponent(typeof(ContainerManager))]
    [ExecuteInEditMode]
    public class FixedRatioRectTransforms : MonoBehaviour
    {
        [SerializeField] bool execute = true;
        [SerializeField] Match matchSide;
        [SerializeField] ContainerManager manager;

        private float _unitSize;
        private ContainerBase _firstCont;
        private ContainerBase[] _all;
        
        private void Update()
            => CalculateRect();

        private void CalculateRect()
        {
            _all ??= manager.GetAllContainers();

            if(!execute || manager == null || _all.Length == 0) return;

            _firstCont = _all[0];

            var newUnitSize = matchSide == Match.Width ? 
                _firstCont.ScreenRect.Rect.size.x / _firstCont.SizeData.SizeInt.x : 
                _firstCont.ScreenRect.Rect.size.y / _firstCont.SizeData.SizeInt.y;
            
            if(newUnitSize != _unitSize)
            {
                foreach(var i in _all)
                    i.ScreenRect.SetSizeDelta(new Vector2(_unitSize * i.SizeData.SizeInt.x, _unitSize * i.SizeData.SizeInt.y));

                manager.SetUnitSize(_unitSize);
            }
            _unitSize = newUnitSize;
        }
    }

    public enum Match
    {
        Width,
        Height
    }
}
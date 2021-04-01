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
        private float _lastUnitSize;
        private ContainerBase _firstCont;
        private ContainerBase[] _all;
        
        private void Update()
            => CalculateRect();


        private void CalculateRect()
        {
            _all ??= manager.GetAllContainers();

            if(!execute || manager == null || _all.Length == 0) return;

            _firstCont = _all[0];

            _unitSize = matchSide == Match.Width ? 
                _firstCont.ScreenRect.GetSizeDelta().x / _firstCont.SizeData.SizeInt.x : 
                _firstCont.ScreenRect.GetSizeDelta().y / _firstCont.SizeData.SizeInt.y;
            
            if(_unitSize != _lastUnitSize)
            {
                foreach(var i in _all)
                    i.ScreenRect.SetSizeDelta(new Vector2(_unitSize * i.SizeData.SizeInt.x, _unitSize * i.SizeData.SizeInt.y));

                manager.SetUnitSize(_unitSize);
            }
            _lastUnitSize = _unitSize;
        }
    }

    public enum Match
    {
        Width,
        Height
    }
}
using UnityEngine;
using System.Linq;

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
        
        private void Update()
            => CalculateRect();

        private void CalculateRect()
        {
            if(!execute || manager == null || manager.All.Length == 0) return;

            var containers = manager.All;

            _firstCont = containers[0];

            _unitSize = matchSide == Match.Width ? 
                _firstCont.ScreenRect.GetSizeDelta().x / _firstCont.SizeData.SizeInt.x : 
                _firstCont.ScreenRect.GetSizeDelta().y / _firstCont.SizeData.SizeInt.y;

            foreach(var i in containers)
                i.ScreenRect.SetSizeDelta(new Vector2(_unitSize * i.SizeData.SizeInt.x, _unitSize * i.SizeData.SizeInt.y));
        }
    }

    public enum Match
    {
        Width,
        Height
    }
}
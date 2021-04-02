using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class FixedRatioRectTransforms : MonoBehaviour
{
    [SerializeField] bool execute = true;
    [SerializeField] Match matchSide;
    [SerializeField] ScreenSpaceItemContainer[] itemContainers = new ScreenSpaceItemContainer[0];

    private float _unitSize;
    private ScreenSpaceItemContainer firstCont;
    
    private void Update()
        => CalculateRect();

    private void CalculateRect()
    {
        if(!execute) return;
        itemContainers = itemContainers.Where(n => n != null).ToArray();
        if(itemContainers.Length == 0) return;
        
        firstCont = itemContainers[0];

        _unitSize = matchSide == Match.Width ? 
            firstCont.ScreenRect.Rect.size.x / firstCont.SizeData.SizeInt.x : 
            firstCont.ScreenRect.Rect.size.y / firstCont.SizeData.SizeInt.y;

        foreach(var i in itemContainers)
            i.ScreenRect.SetSizeDelta(new Vector2(_unitSize * i.SizeData.SizeInt.x, _unitSize * i.SizeData.SizeInt.y));
            
    }
}

public enum Match
{
    Width,
    Height
}
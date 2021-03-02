using UnityEngine;

[ExecuteInEditMode]
public class PreservedRatioRectTransform : MonoBehaviour
{
    ///<summary>Panel which unit size will be applied to every other panel</summary>
    [SerializeField] bool masterPanel;
    [SerializeField] RectTransform storePanel;
    [SerializeField] Match matchSide;
    [SerializeField] Vector2IntSpaceData sizeData;

    private static float UnitSize;
    
    private void Update()
        => CalculateRect();

    private void CalculateRect()
    {
        if(masterPanel)
            UnitSize = matchSide == Match.Width ? storePanel.sizeDelta.x / sizeData.SizeInt.x : storePanel.sizeDelta.y / sizeData.SizeInt.y;

        if(masterPanel)
            storePanel.sizeDelta = matchSide == Match.Width ? 
                new Vector2(storePanel.sizeDelta.x, UnitSize * sizeData.SizeInt.y) : 
                new Vector2(UnitSize * sizeData.SizeInt.x, storePanel.sizeDelta.y);
        else 
            storePanel.sizeDelta = new Vector2(UnitSize * sizeData.SizeInt.x, UnitSize * sizeData.SizeInt.y);

    }
}

public enum Match
{
    Height,
    Width
}

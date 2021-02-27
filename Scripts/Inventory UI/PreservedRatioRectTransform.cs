using UnityEngine;

[ExecuteInEditMode]
public class PreservedRatioRectTransform : MonoBehaviour
{
    [SerializeField] RectTransform storePanel;
    [SerializeField] Match matchSide;
    [SerializeField] Vector2IntSpaceData sizeData;
    
    private void Update()
        => CalculateRect();

    private void CalculateRect()
    {
        storePanel.sizeDelta = matchSide == Match.Width ? 
            new Vector2(storePanel.sizeDelta.x, storePanel.sizeDelta.x / sizeData.SizeInt.x * sizeData.SizeInt.y) : 
            new Vector2(storePanel.sizeDelta.y / sizeData.SizeInt.y * sizeData.SizeInt.x, storePanel.sizeDelta.y);
    }
}

public enum Match
{
    Height,
    Width
}

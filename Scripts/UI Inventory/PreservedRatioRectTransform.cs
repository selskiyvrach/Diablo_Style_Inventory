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
            new Vector2(storePanel.sizeDelta.x, storePanel.sizeDelta.x / sizeData.Size.x * sizeData.Size.y) : 
            new Vector2(storePanel.sizeDelta.y / sizeData.Size.y * sizeData.Size.x, storePanel.sizeDelta.y);
    }
}

public enum Match
{
    Height,
    Width
}

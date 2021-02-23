using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PreservedRatioRectTransform))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField] RectTransform storePanel;
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] Vector2IntSpaceData sizeData;
    private Vector3[] _corners; // 0 - leftBottom, 1 - leftTop, 2 - rightTop 3 - rightBottom
    private Vector2IntSpacing _space;

    // trackers
    float _squareSize;
    Image _highlighter;
    bool _highlighting;

    private void Awake() {
        _corners = new Vector3[4];
        storePanel.GetWorldCorners(_corners);
        _squareSize = storePanel.sizeDelta.x / sizeData.SizeInt.x;
        _space = new Vector2IntSpacing(sizeData.SizeInt);
        // highlighter
        _highlighter = new GameObject().AddComponent<Image>();
        _highlighter.rectTransform.sizeDelta = new Vector2(_squareSize, _squareSize);
        _highlighter.transform.SetParent(inventoryCanvas.transform);
        _highlighter.gameObject.SetActive(false);
    }

    public void AddItem(Item newItem)
    {
        var uIItem = new GameObject();
        var i = uIItem.AddComponent<UIItem>();
        i.Init(newItem, _squareSize, storePanel.transform);
        if(_space.TryPlaceItemAuto(newItem, out Vector2Int topLeftCornerPos))
            i.transform.position = CellCenterToScreen(topLeftCornerPos) + new Vector2(i.GetComponent<RectTransform>().sizeDelta.x, - i.GetComponent<RectTransform>().sizeDelta.y) / 2 
                - new Vector2(_squareSize, - _squareSize) / 2;
    } 

    private void Update() {
        if(PosOverlapInventory(Input.mousePosition))
            HighlightCell(CellCenterToScreen(ScreenPosToInventoryCell(Input.mousePosition)));

        if(_highlighting != PosOverlapInventory(Input.mousePosition))
            _highlighter.gameObject.SetActive(_highlighting = !_highlighting);
    }

    public Vector2Int ScreenPosToInventoryCell(Vector2 screenPos)
    {   
        Vector2 relativePos = screenPos - (Vector2)_corners[1]; // screenPos from top lef corner
        Vector2 squarePos = relativePos / _squareSize; // divided by square size to get square number
        Vector2Int cellPos = new Vector2Int((int)squarePos.x, - (int)squarePos.y); // converted to int with negative y since it counts cell top to bottom
        return cellPos;
    }

    public Vector2 CellCenterToScreen(Vector2Int cellPos)
    {
        float x = (_squareSize / 2) + (cellPos.x * _squareSize);
        float y = (_squareSize / 2) + (cellPos.y * _squareSize);
        return new Vector2(x, - y) + (Vector2)_corners[1];
    }

    public void HighlightCell(Vector2 screenPosOfCell)
    {
        if(_highlighting)
            _highlighter.transform.position = screenPosOfCell;
    }

    public bool PosOverlapInventory(Vector3 screenPos)
        => screenPos.x > _corners[0].x && screenPos.x < _corners[2].x && screenPos.y > _corners[0].y && screenPos.y < _corners[2].y;
}

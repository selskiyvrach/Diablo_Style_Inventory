using UnityEngine;

public class UIItemDragger
{
    private UIItem _draggable;
    private Vector2 _posOffset;
    private bool _withOffset;

    public bool Empty { get; private set; } = true;
    
    public void AddMouseFollower(UIItem toDrag, bool withOffset)
    {
        if(_withOffset = withOffset)
            _posOffset = Input.mousePosition - _draggable.transform.position;

        _draggable = toDrag;
        _draggable.gameObject.SetActive(true);
        Empty = false;
    } 

    public void UpdateMe()
    {
        if(_draggable != null)
            _draggable.transform.position = _withOffset ? (Vector2)Input.mousePosition - _posOffset : (Vector2)Input.mousePosition;
    }

    public void ExtractMouseFollower(out UIItem item)
    {
        item = _draggable;
        _draggable = null;
        Empty = true;
    }
}

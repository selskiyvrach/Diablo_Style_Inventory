using UnityEngine;

public abstract class ItemData : ScriptableObject, IVector2IntSize
{
    [SerializeField] Vector2Int size;
    [SerializeField] string itemName;
    public Vector2Int Size => size;
    public string ItemName => itemName;
}

using UnityEngine;

public abstract class ItemData : ScriptableObject, IVector2IntSize
{
    [SerializeField] string itemName;
    [SerializeField] Sprite sprite;
    [SerializeField] Vector2Int size;

    public string Name => itemName;
    public Sprite Sprite => sprite; 
    public Vector2Int SizeInt => size;
}

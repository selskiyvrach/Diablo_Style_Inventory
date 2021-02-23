using UnityEngine;

[CreateAssetMenu(menuName="Scriptable Objects/Inventory/New 2D Space")] 
public class Vector2IntSpaceData : ScriptableObject, IVector2IntSize
{
    [SerializeField] Vector2Int size;
    public Vector2Int SizeInt => size;
}

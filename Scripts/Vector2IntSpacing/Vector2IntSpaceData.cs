using UnityEngine;

[CreateAssetMenu(menuName="Scriptable Objects/Inventory/Int Size")] 
public class Vector2IntSpaceData : ScriptableObject
{
    [SerializeField] Vector2Int size;
    public Vector2Int SizeInt => size;
}

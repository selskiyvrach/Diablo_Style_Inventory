using UnityEngine;

public interface IUIInventoryItem : IVector2IntSize, IVector2IntPos
{
    Vector2[] CornerCellssCenters { get; set; }
}

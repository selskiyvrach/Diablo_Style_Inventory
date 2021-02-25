using UnityEngine;

public interface IVector2IntSize
{
    Vector2Int SizeInt { get; }
}

public interface IVector2IntPos
{
    Vector2Int TopLeftCornerPosInt { get; set; }
}

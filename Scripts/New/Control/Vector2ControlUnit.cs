using System;
using UnityEngine;

namespace D2Inventory.Control
{
    [CreateAssetMenu(menuName="Scriptable Objects/Control/Vector2 Unit")]
    public class Vector2ControlUnit : ScriptableObject
    {
        public string Name;

        public Vector2 Vector2 => Input.mousePosition;
    }
}

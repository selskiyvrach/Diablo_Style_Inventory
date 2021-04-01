using System;
using UnityEngine;

namespace D2Inventory.Control
{
    [CreateAssetMenu(menuName="Scriptable Objects/Control/Control Unit")]
    public class ControlUnit : ScriptableObject
    {
        public string Name;

        public KeyCode KeyCode;

        public bool Pressed => Input.GetKeyDown(KeyCode);
        public bool Held => Input.GetKey(KeyCode);
        public bool Released => Input.GetKeyUp(KeyCode);

    }
}

using System.Collections;
using System.Collections.Generic;
using MNS.Utils.Values;
using UnityEngine;


namespace D2Inventory
{

    public class InventoryController : MonoBehaviour
    {
        [SerializeField] ChainVector2ValueSource cursorPos;

        private void Awake() {
            cursorPos.Getter = () => Input.mousePosition;
        }
    }
    
}

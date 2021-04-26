using System.Collections;
using System.Collections.Generic;
using MNS.Utils.Values;
using UnityEngine;


namespace D2Inventory
{

    public class TempInputManager : MonoBehaviour
    {
        [SerializeField] ChainBoolValueSource inventoryButton;
        [SerializeField] KeyCode inventoryCode;
        [Space]
        [SerializeField] ChainBoolValueSource switchWeaponsButton;
        [SerializeField] KeyCode switchCode;
        [Space]
        [SerializeField] ChainBoolValueSource interactButton;
        [SerializeField] KeyCode interactCode;
        [Space]
        [SerializeField] ChainVector2ValueSource cursorPos;

        private void Awake() {

            inventoryButton.Getter = () => Input.GetKeyDown(inventoryCode);

            switchWeaponsButton.Getter = () => Input.GetKeyDown(switchCode);

            interactButton.Getter = () => Input.GetKeyDown(interactCode);

            cursorPos.Getter = () => Input.mousePosition;

        }

    }
    
}

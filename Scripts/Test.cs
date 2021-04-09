using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using MNS.Utils.Values;
using D2Inventory;

public class Test : MonoBehaviour
{
    [SerializeField] InventoryItemData[] data;
    [SerializeField] InventoryController controller;

    private void Start() {
        controller.PickUpItem(GetRandomData());
    } 

    private InventoryItemData GetRandomData()
        => data[UnityEngine.Random.Range(0, data.Length)];

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Mouse1))
            controller.PickUpItem(GetRandomData());
        
    }

}

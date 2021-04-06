using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using MNS.Utils.Values;
using D2Inventory;

public class Test : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] ChainVector2ValueSource vectorSource; 
    [SerializeField] ChainVector2ValueSource vectorConsumer;

    private void Awake() {
        vectorSource.Getter = () => new Vector2(666, 69);

        Debug.Log(vectorConsumer.Value);
    } 

}

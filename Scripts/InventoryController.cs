using System;
using System.Collections;
using System.Collections.Generic;
using MNS.Utils.Values;
using UnityEngine;


namespace D2Inventory
{

    public class InventoryController : MonoBehaviour
    {
        [Header("Containers")]
        [SerializeField] ContainerArrayHandlerSource containers;
        [Header("Controls")]
        [SerializeField] ChainVector2ValueSource cursorPos;
        [Header("Inventory Events")]
        [SerializeField] BoolHandlerSource openCloseInventory;
        [SerializeField] BoolHandlerSource weaponsSwithcedToFirstOption;
        [Header("Item Manipulations")]
        [SerializeField] ProjectionHandlerSource projectionChanged;
        [SerializeField] ProjectionHandlerSource itemPickedUp;
        [SerializeField] ProjectionHandlerSource itemDropped;
        [SerializeField] ProjectionHandlerSource itemEquipped;
        [SerializeField] ProjectionHandlerSource itemUneqipped;
        [SerializeField] ProjectionHandlerSource cursorItemChanged;

        private ContainerBase[] _containers;

        private InventoryItem _cursorItem;

        private Projection _lastProjection = Projection.EmptyProjection;

        private void Awake() {
            cursorPos.Getter = () => Input.mousePosition;
        }

        private void OnEnable() {
            containers.Value.AddWithInvoke((o, args) => UpdateContainers(args));   
        }

        private void OnDisable() {
            containers.Value.Handler -= (o, args) => UpdateContainers(args);
        }

        private void Update() {
            CheckProjection();
        }

        public void AddItem(InventoryItemData data)
        {
            // itemPickedUp.Value.Invoke(this, new Projection(null, new Rect(), false, ));
        }

        private void CheckProjection()
        {
            if(_containers == null || _containers.Length == 0) return;

            bool overlapsContainer = false;

            foreach(var container in _containers)
            {
                var proj = container.GetProjection(_cursorItem, cursorPos.Value);
                if(!proj.Empty)
                {   
                    if(!proj.Same)
                        projectionChanged.Value.Invoke(this, _lastProjection = proj);
                    overlapsContainer = true;
                }
            }
            if(!_lastProjection.Empty && !overlapsContainer)
                projectionChanged.Value.Invoke(this, _lastProjection = Projection.EmptyProjection);
        }

        private void UpdateContainers(ContainerBase[] newContainers)
            => _containers = newContainers;

    }
    
}

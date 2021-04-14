using System.Collections.Generic;
using UnityEngine;

namespace D2Inventory
{
    public abstract class ContainerBase : MonoBehaviour
    {
        [SerializeField] protected ScreenRect screenRect; 
        [SerializeField] protected Vector2IntSpaceData sizeData; 
        [SerializeField] protected ItemFitRule fitRule; 

        public bool ActiveInInventory { get; private set; } = true;

        public void SetActiveInInventory(bool value)
            => ActiveInInventory = value;

        public void SetActiveOnScreen(bool value) 
            => screenRect?.SetActive(ActiveOnScreen = value);

// TODO: split the below on classes by features

// Screen/Projection

        public Vector2IntSpaceData SizeData => sizeData;

        protected Projection lastProjection = Projection.EmptyProjection;

        public ScreenRect ScreenRect => screenRect;

        public bool ActiveOnScreen { get; private set; }

        public abstract Projection GetProjection(InventoryItem item, Vector2 screenPos); 

// Placing

        public abstract InventoryItem[] GetContent();

        public abstract InventoryItem PlaceItem(InventoryItem item);

        public abstract InventoryItem ExtractItem(InventoryItem item);

        public abstract bool TryPlaceItemAuto(InventoryItem item);

        public abstract bool CanPlaceItemsAuto(InventoryItem[] items);

// Icon change

        protected List<(int, IconInfo)> change = new List<(int, IconInfo)>();

        public virtual (int, IconInfo)[] GetIconChange()
            => change.ToArray();

        public virtual void RefreshIconCnange()
            => change.Clear();

    }
}
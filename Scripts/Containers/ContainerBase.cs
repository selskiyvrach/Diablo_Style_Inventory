using UnityEngine;

namespace D2Inventory
{
    public abstract class ContainerBase : MonoBehaviour
    {
        [SerializeField] protected ScreenRect screenRect; 
        [SerializeField] protected Vector2IntSpaceData sizeData; 
        [SerializeField] protected ItemFitRule fitRule; 

        protected Projection lastProjection = Projection.EmptyProjection;

        public Vector2IntSpaceData SizeData => sizeData;

        public ScreenRect ScreenRect => screenRect;

        public bool ActiveOnScreen => screenRect.Active;

        public abstract InventoryItem[] GetContent();

        public abstract Projection GetProjection(InventoryItem item, Vector2 screenPos); 

        public abstract InventoryItem PlaceItem(InventoryItem item);

        public abstract InventoryItem ExtractItem(InventoryItem item);

        public abstract bool TryPlaceItemAuto(InventoryItem item);

        public abstract bool TryPlaceItemsAuto(InventoryItem[] items);

        public abstract bool CanPlaceItemsAuto(InventoryItem[] items);

        public void SetActive(bool value) 
            => screenRect?.SetActive(value);
    }
}
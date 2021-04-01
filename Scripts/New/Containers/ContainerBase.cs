using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    public abstract class ContainerBase : MonoBehaviour
    {
        [SerializeField] protected ScreenRect screenRect; public ScreenRect ScreenRect => screenRect;
        [SerializeField] protected Vector2IntSpaceData sizeData; public Vector2IntSpaceData SizeData => sizeData;
        [SerializeField] protected ItemFitRule fitRule; 

        protected Projection lastProjection = Projection.EmptyProjection;

        public bool ActiveOnScreen => screenRect.Active;

        public abstract Projection GetProjection(InventoryItem item, Vector2 screenPos); 

        public abstract InventoryItem PlaceItem(InventoryItem item);

        public abstract InventoryItem ExtractItem(InventoryItem item);

        public abstract bool TryPlaceItemAuto(InventoryItem item);

        public void SetActive(bool value) 
            => screenRect?.SetActive(value);
    }
}
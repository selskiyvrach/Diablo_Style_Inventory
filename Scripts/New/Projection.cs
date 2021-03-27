using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    public class Projection
    {
        public bool Empty { get; private set; } = false;
        public Rect ScreenRect { get; private set; } = new Rect();
        public bool CanPlace { get; private set; } = false;
        public InventoryItem Replacement { get; private set; } = null;
        ///<summary>
        ///If projection says CanPlace that means you need to place all refugees somewhere first</summary>
        public IReadOnlyCollection<InventoryItem> Refugees { get; private set; } = new InventoryItem[0];

        ///<summary>
        ///If projection says CanPlace that means you need to place all refugees somewhere first</summary>
        public Projection(Rect screenRect, bool canPlace, InventoryItem replacement, IReadOnlyCollection<InventoryItem> refugees)
        {
            ScreenRect = screenRect;
            CanPlace = canPlace;
            Replacement = replacement;
            Refugees = (refugees == null) ? 
                new InventoryItem[0] : 
                refugees.Where(n => n != null).Distinct().ToArray();            
        }

        // EMPTY

        // being sent as an empty argument, cashed to save resources
        private Projection() { Empty = true; }
        private static Projection _emptyProjection;
        public static Projection EmptyProjection => _emptyProjection ??= new Projection(); 
    }
}
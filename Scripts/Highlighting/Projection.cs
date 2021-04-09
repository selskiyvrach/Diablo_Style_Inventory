using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    ///<summary>
    ///Contains all information about cursor overlapping an item slot<br/>
    ///Screen rect, whether currenty carried by cursor item can be placed at current pos, which item's will be replaced and so on</summary>
    public class Projection
    {
        public ContainerBase Container { get; private set; }
        public Rect ScreenRect { get; private set; }
        public bool CanPlace { get; private set; }
        public Vector2 PotentialPlacedPos { get; private set; }
        public InventoryItem Replacement { get; private set; }
        ///<summary>
        ///If projection says CanPlace that means you need to place all refugees somewhere first</summary>
        public InventoryItem[] Refugees { get; private set; } = new InventoryItem[0];

        ///<summary>
        ///If projection says CanPlace that means you need to place all refugees somewhere first</summary>
        public Projection(ContainerBase container, Rect screenRect, bool canPlace, Vector2 potentialPlacePos, InventoryItem replacement, InventoryItem[] refugees)
        {
            Container = container;
            ScreenRect = screenRect;
            CanPlace = canPlace;
            PotentialPlacedPos = potentialPlacePos;
            Replacement = replacement;
            Refugees = (refugees == null) ? 
                new InventoryItem[0] : 
                refugees.Where(n => n != null).Distinct().ToArray();            
        }

        public bool FieldsEqual(Rect rect, bool canPlace, Vector2 potentialPlacePos, InventoryItem replacement, InventoryItem[] refugees)
        {
            // guard clause for all params exc for refugees 
            if(!(rect == ScreenRect && CanPlace == canPlace && potentialPlacePos == PotentialPlacedPos && Replacement == replacement))
                return false;
            // if refugees are not empty return false if Refugees are empty or whether their sequences are equal
            if(refugees != null && refugees.Length != 0)
                return (Refugees.Length == 0) ? false : Refugees.SequenceEqual(refugees.Where(n => n != null).Distinct().ToArray());
            // if refugees are empty - return whether Refugees are empty too
            return Refugees.Length == 0;
        }

        public Projection SetCanPlace(bool value)
        {
            CanPlace = value;
            return this;
        }
        

        // EMPTY AND SAME
        // first being sent if no projection on sender, second - if projection doesn't differ from the previous one(so caller should use cashed version)

        public bool Empty { get; private set; } = false;
        public bool Same { get; private set; } = false;
        private Projection(bool empty = false, bool same = false) { Empty = empty; Same = same;  }

        public static readonly Projection EmptyProjection = new Projection(empty: true); 
        ///<summary>
        ///Static empty cashed projection with "Same" boolean set to true. <br/> 
        ///Used to tell caller that nothing has changed and caller should use caller-side cashed projection from the previous call</summary>
        public static readonly Projection SameProjection = new Projection(same: true); 

    }
}
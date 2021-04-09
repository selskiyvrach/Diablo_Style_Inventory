// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace D2Inventory
// {

//     public class SwitchableContainer : ContainerBase
//     {
//         [SerializeField] SingleItemContainer firstOption;
        
//         [SerializeField] SingleItemContainer secondOption;

//         private SingleItemContainer _currentContainer => firstOption.ActiveOnScreen ? firstOption : secondOption;

//         private void Awake()
//             => SetFirstOption();

//         public void Switch()
//         {
//             if(firstOption.ActiveOnScreen) SetSecondOption();
//             else if(secondOption.ActiveOnScreen) SetFirstOption();

//         }

//         private void SetFirstOption()
//         {
//             if(!firstOption.ActiveOnScreen) firstOption.SetActive(true);
//             if(secondOption.ActiveOnScreen) secondOption.SetActive(false);
//         }

//         private void SetSecondOption()
//         {
//             if(!secondOption.ActiveOnScreen) secondOption.SetActive(true);
//             if(firstOption.ActiveOnScreen) firstOption.SetActive(false);
//         }

//         public override InventoryItem ExtractItem(InventoryItem item)
//             => _currentContainer.ExtractItem(item);

//         public override Projection GetProjection(InventoryItem item, Vector2 screenPos)
//             => _currentContainer.GetProjection(item, screenPos);

//         public override InventoryItem PlaceItem(InventoryItem item)
//             => _currentContainer.PlaceItem(item);

//         public override bool TryPlaceItemAuto(InventoryItem item)
//             => _currentContainer.TryPlaceItemAuto(item);
//     }
    
// }
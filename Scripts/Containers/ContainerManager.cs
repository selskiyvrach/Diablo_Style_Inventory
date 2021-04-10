using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] InventoryController controller;
        ///<summary>
        ///Main storage should go first in this array</summary>
        [SerializeField] ContainerBase[] allContainers = new ContainerBase[0];
        [SerializeField] ContainerSwitcher[] switchers = new ContainerSwitcher[0];

        // not null and unique cashed (in case serializedField got messy in editor)
        private ContainerBase[] _allFixed;

        private void Awake()
            => controller.SetContainers(_allFixed = GetAllContainers());

        private void OnEnable() 
        {
            controller.OnInventoryOpened.AddWithInvoke((o, args) => SetContainersActive(args));
            controller.OnWeaponsSwitchedToFirstOption.AddWithInvoke((o, args) => SetSwitchesState(args));
        }
            
        private void OnDisable() 
        {
            controller.OnInventoryOpened.RemoveListener((o, args) => SetContainersActive(args));
            controller.OnWeaponsSwitchedToFirstOption.RemoveListener((o, args) => SetSwitchesState(args));
        }

        public ContainerBase[] GetAllContainers()
            => allContainers.Where(n => n != null).Distinct().ToArray();

        private void SetContainersActive(bool value)
            => allContainers.ForEach((ContainerBase c) => c.SetActive(value));

        private void SetSwitchesState(bool value)
        {
            foreach (var item in switchers)
            {
                item.SetSwitchState(value);
                item.GetChange(out ContainerBase[] active, out ContainerBase[] inactive);
                // if anything changed, basically
                if((active != null && active.Length != 0) || (inactive != null && inactive.Length != 0) )
                    controller.HandleSwitchedWeapons(active, inactive);
            }
        }
    }
}


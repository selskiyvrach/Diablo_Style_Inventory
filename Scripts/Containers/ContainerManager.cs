using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] InventoryController controller;
        [Header("First goes main storage!")]
        ///<summary>
        ///Main storage should go first in this array</summary>
        [SerializeField] ContainerBase[] allContainers = new ContainerBase[0];
        [SerializeField] ContainerSwitcher[] switchers = new ContainerSwitcher[0];

        // not null and unique cashed (in case serializedField got messy in editor)
        private ContainerBase[] _allFixed;

        private void Awake() 
            => _allFixed = GetAllContainers();

        private void Start()
            => controller.SetContainers(_allFixed);

        private void OnEnable() 
        {
            controller.OnWeaponsSwitchedToFirstOption.AddWithInvoke((o, args) => SetSwitchesState(args));
            controller.OnInventoryOpened.AddWithInvoke((o, args) => SetContainersActive(args));
        }
            
        private void OnDisable() 
        {
            controller.OnInventoryOpened.RemoveListener((o, args) => SetContainersActive(args));
            controller.OnWeaponsSwitchedToFirstOption.RemoveListener((o, args) => SetSwitchesState(args));
        }

        public ContainerBase[] GetAllContainers()
            => allContainers.Where(n => n != null).Distinct().ToArray();

        private void SetContainersActive(bool value)
        {
            Debug.Log(value);
            _allFixed.ForEach((c) => c.SetActiveOnScreen(value && c.ActiveInInventory));
        }

        private void SetSwitchesState(bool value)
        {
            foreach (var switcher in switchers)
            {
                switcher.SetSwitchState(value);

                switcher.GetChange(out ContainerBase[] active, out ContainerBase[] inactive);
                
                if(controller.OnInventoryOpened.LastArgs) 
                    SetContainersActive(true);
                // if anything has changed, basically
                if((active != null && active.Length != 0) || (inactive != null && inactive.Length != 0) )
                    controller.HandleSwitchedWeapons(active.Where(n => n != null).ToArray(), inactive.Where(n => n != null).ToArray());
            }
        }
    }
}


using System.Linq;
using UnityEngine;
using D2Inventory.Control;
using MNS.Utils.Values;

namespace D2Inventory
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] InventoryController controller;
        [SerializeField] HandlerSource openCloseHandler;
        [SerializeField] ContainerBase mainStorage;
        [SerializeField] ContainerBase[] allContainers = new ContainerBase[0];

        // cashed clean version of the above
        private ContainerBase[] _allNotNullUnique;

        private float _unitSize;

        private void OnEnable() 
            => openCloseHandler.Value.AddWithInvoke((o, args) => SetContainersActive(((BoolEventArgs)args).Value));
            
        private void OnDisable() 
            => openCloseHandler.Value.Handler -= ((o, args) => SetContainersActive(((BoolEventArgs)args).Value));


        private void Awake()
        {
            controller.SetUnitSize(_unitSize);
            controller.SetUpContainers(GetAllContainers(), mainStorage);
        }

        public void SetUnitSize(float value)
            => _unitSize = value;

        public ContainerBase[] GetAllContainers()
            => _allNotNullUnique ??= allContainers.Where(n => n != null).Distinct().ToArray();

        private void SetContainersActive(bool value)
            => allContainers.ForEach((ContainerBase c) => c.SetActive(value));
    }
}


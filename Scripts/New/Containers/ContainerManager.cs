using System.Linq;
using UnityEngine;
using D2Inventory.Control;
using MNS.Utils.Values;

namespace D2Inventory
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] InventoryController controller;
        [SerializeField] BoolHandlerSource openCloseHandler;
        [SerializeField] ContainerBase mainStorage;
        [SerializeField] ContainerBase[] allContainers = new ContainerBase[0];

        private ContainerBase[] _allFixed;

        private void Awake()
            => _allFixed = GetAllContainers();

        private void OnEnable() 
            => openCloseHandler.Value.AddWithInvoke((o, args) => SetContainersActive(args));
            
        private void OnDisable() 
            => openCloseHandler.Value.Handler -= ((o, args) => SetContainersActive(args));

        public ContainerBase[] GetAllContainers()
            => allContainers.Concat(new ContainerBase[]{ mainStorage }).Where(n => n != null).Distinct().ToArray();

        private void SetContainersActive(bool value)
            => allContainers.ForEach((ContainerBase c) => c.SetActive(value));
    }
}


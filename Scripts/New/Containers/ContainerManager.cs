using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] InventoryController controller;
        [SerializeField] ContainerBase mainStorage;
        [SerializeField] ContainerBase[] allContainers = new ContainerBase[0];

        private ContainerBase[] _allNotNullUnique;

        private float _unitSize;

        private void Awake()
            => controller.SetUpContainers(allContainers, mainStorage);

        public void SetUnitSize(float value)
            => _unitSize = value;

        public ContainerBase[] GetAllContainers()
            => _allNotNullUnique ??= allContainers.Where(n => n != null).Distinct().ToArray();

        public void SetContainersActive(bool value)
            => allContainers.ForEach((ContainerBase c) => c.SetActive(value));
    }
}


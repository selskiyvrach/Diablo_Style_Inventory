using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace D2Inventory
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] ContainerBase[] allContainers = new ContainerBase[0];

        public ContainerBase[] All => allContainers.Where(n => n != null).Distinct().ToArray();
    }    
}


using System;
using System.Linq;
using MNS.Utils.Values;
using UnityEngine;

namespace D2Inventory
{
    [RequireComponent(typeof(ContainerManager))]
    [ExecuteInEditMode]
    public class FixedRatioRectTransforms : MonoBehaviour
    {
        [SerializeField] bool execute = true;
        [SerializeField] float unitSize = 100;
        [SerializeField] ContainerManager manager;
        [Header("Output")]
        [SerializeField] FloatHandlerSource unitSizeHandler;

        #if UNITY_EDITOR

        private void Update()
        {
            if(!execute) return;

            // getting new array each update so no exceptions thrown when editing serialized array on manager
            var _all = manager.GetAllContainers();
                
            foreach(var i in _all)
                i.ScreenRect.SetSizeDelta((Vector2)i.SizeData.SizeInt * unitSize);

            unitSizeHandler.Value.Invoke(this, unitSize);
        }

        #endif
    }
}
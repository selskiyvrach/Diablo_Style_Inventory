using System;
using System.Linq;
using MNS.Utils.Values;
using UnityEngine;

namespace D2Inventory
{
    [RequireComponent(typeof(ContainerManager))]
    [ExecuteInEditMode]
    ///<summary>
    /// Editor featue. Scales all the containers accordingly to their size in cells while maintaining common cell size across them</summary>
    public class FixedRatioForScreenRects : MonoBehaviour
    {
        [SerializeField] bool execute = true;
        ///<summary>
        ///Size of a side of an inventory cell</summary>
        [SerializeField] float unitSize = 100;
        // takes all containers from there
        [SerializeField] ContainerManager manager;
        [Header("Output")]
        // sets unit size in a nadler so other systems can use it
        [SerializeField] FloatHandlerSource unitSizeHandler;

        private void Awake() 
            // setting eventual result of rescaling in editor so other classes can use common unitSize when the game starts
            => unitSizeHandler.Value.Invoke(this, unitSize);

        #if UNITY_EDITOR

        private void Update()
        {
            if(!execute) return;

            // getting new array each update so no exceptions thrown when editing serialized array on the manager
            var _all = manager.GetAllContainers();
                
            foreach(var i in _all)
                i.ScreenRect.SetSizeDelta((Vector2)i.SizeData.SizeInt * unitSize);

            // setting result of rescaling so other classes can use common unitSize during editor stage
            unitSizeHandler.Value.Invoke(this, unitSize);
        }

        #endif
    }
}
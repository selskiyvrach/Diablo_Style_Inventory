using MNS.Utils.Values;
using UnityEngine;


namespace D2Inventory
{

    public class UnitSizeSetter : MonoBehaviour
    {
        [SerializeField] ContainerBase sampleContainer;
        [SerializeField] FloatHandlerSource unitSize;
        [SerializeField] RectTransform inventoryCanvas;

        // TODO: clean this up when figured out what's going on with WebGL build's scaling when uploaded via unity WebGlPublisher
        // and a proper way of dealing with that found


        // The problem: Canvas' size attribute shows ~8000x4000 in webgl build when published via unity web gl publisher. 
        // And all the screen scales mess up. No way to get correct screen coords nor via GetWorldCorners nor using lossyScale.
        // However when published on itch.io it works correctly after some tweaks not needed in standalone build nor in editor playmode

        public static Vector3 LocalScale { get; private set; }

        private void Update() 
        {
            LocalScale = inventoryCanvas.localScale;

            // screen rect might still be zero on start!! since Rect calculations rely on unity rectTransform that's being set on Start()
            unitSize.Value.Invoke(this, sampleContainer.ScreenRect.Rect.size.x / sampleContainer.SizeData.SizeInt.x);
            Destroy(gameObject);
        }
    }
    
}

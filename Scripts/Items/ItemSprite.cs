using UnityEngine;


namespace D2Inventory
{
    [CreateAssetMenu(menuName="Scriptable Objects/Items/Item Visuals")]
    public class ItemVisuals : ScriptableObject
    {
        public Sprite Sprite;

        public Vector2IntSpaceData SizeData;

        [Range(.3f, 2f)]
        public float Scale = .85f;

    }

}


using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{

    public interface IIconed : IVisibleOnScreen 
    {

        Image Icon { get; set; }

        Sprite Sprite { get; }

        Vector2Int SizeInt { get; }

        float Scale { get; }

    }
    
}


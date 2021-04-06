using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{

    public interface IIconed : IVisibleOnScreen 
    {

        Image Icon { get; }

        Sprite Sprite { get; }

        Vector2Int SizeInt { get; }

    }
    
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIItem 
{
    Vector2 ScreenSize { get; set; }
    Vector2 ScreenPos { get; set; }
}

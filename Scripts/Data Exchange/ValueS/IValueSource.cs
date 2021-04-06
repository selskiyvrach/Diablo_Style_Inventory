using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IValueSource<T> 
{
    T Value { get; }
}

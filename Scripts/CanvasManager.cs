using System;
using System.Collections.Generic;
using UnityEngine;


namespace MNS.UI
{

    public class CanvasManager : MonoBehaviour
    {
        private Dictionary<string, Canvas> _canvases = new Dictionary<string, Canvas>();

        public void AddCanvas(string name, Canvas canvas)
        {
            try 
            {
                if(canvas == null)
                    throw new ArgumentNullException();
                _canvases.Add(name, canvas);
            }
            catch(ArgumentNullException)
            {
                Debug.LogError($"Failed to add canvas. Neither name nor canvas can be null");
            }
            catch(ArgumentException)
            {
                Debug.LogError($"Failed to add canvas. Element with name {name} already exists");
            }
        }
    }

}

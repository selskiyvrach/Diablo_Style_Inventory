using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour 
{
    [SerializeField] int gUIDepth = 1;
    [SerializeField] float scanFreq = 1;
    [SerializeField] int fontSize = 50;
    [SerializeField] Color textColor = Color.white;

    string _label = "";
	float _count;
	
	IEnumerator Start ()
	{
		GUI.depth = 2;
		while (true) 
        {
			if (Time.timeScale > 0) 
            {
				yield return new WaitForSeconds (1 / scanFreq);
				_count = (Time.timeScale / Time.deltaTime);
				_label = "FPS : " + (Mathf.Round (_count));
			} else 
				_label = "Pause";
		}
	}
	
	void OnGUI ()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
		GUI.Label (new Rect (50, 50, 100, 100), _label, style);
    }
}
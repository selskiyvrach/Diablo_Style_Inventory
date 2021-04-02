using UnityEngine;

namespace MNS.Utils.Values
{
    [CreateAssetMenu(menuName="Scriptable Objects/Values/Key Pressed Value")]
    public class KeyPressedBoolSource : BoolValueSource
    {
        public string buttonName;

        public KeyCode keyCode;

        [SerializeField] KeyState state;

        public override bool Value { 
            get => state == KeyState.Pressed ? 
                Input.GetKeyDown(keyCode) : 
                state == KeyState.Held ? 
                    Input.GetKey(keyCode) : 
                    Input.GetKeyUp(keyCode);
            protected set{} }
    }

    public enum KeyState 
    {
        Pressed,
        Held,
        Released
    }

}

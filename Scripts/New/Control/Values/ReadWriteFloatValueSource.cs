using UnityEngine;


namespace MNS.Utils.Values
{
    
    [CreateAssetMenu(menuName="Scriptable Objects/Values/Read Write Float")]
    public class ReadWriteFloatValueSource : FloatValueSource
    {
        public override float Value { get; protected set; }
        public void Set(float value) => Value = value;
    }

}

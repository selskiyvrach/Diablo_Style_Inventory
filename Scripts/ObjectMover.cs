using UnityEngine;

namespace MNS.Utils
{

    public class ObjectMover : MonoBehaviour
    {

        [SerializeField] Vector3 moveSpeed;

        private void Update() 
            => transform.position += moveSpeed * Time.deltaTime;

    }
    
}


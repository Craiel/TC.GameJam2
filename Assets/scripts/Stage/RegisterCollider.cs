using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class RegisterCollider : MonoBehaviour
    {
        public void Start() 
        {
            var typedCollider = this.collider as CapsuleCollider;
            if (typedCollider != null)
            {
                Camera.main.GetComponent<StageManager>().CollidingGeometry.Add(this.gameObject);
            }
        }
    }
}

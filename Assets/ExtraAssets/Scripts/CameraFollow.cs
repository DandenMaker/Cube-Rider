using UnityEngine;

namespace TZ_24Play 
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform follow;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 rotation;
        [SerializeField] private float smooth;
        [SerializeField] private bool ignoreY;

        private Vector3 _currentVelocity;


        #region Unity Lifecycle
        private void Start() 
        {
            transform.position = follow.position + offset;
            transform.rotation = Quaternion.Euler(rotation);
        }

        private void LateUpdate() 
        {
            var targetPosition = ignoreY 
                ? new Vector3(follow.transform.position.x + offset.x, transform.position.y, follow.transform.position.z + offset.z)
                : follow.transform.position + offset;
            
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smooth);
        }
        #endregion
    }
}


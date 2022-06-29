using UnityEngine;

namespace TZ_24Play 
{
    [RequireComponent(typeof(Animator))]
    public class Ragdoll : MonoBehaviour
    {
        private Animator _animator;
        private Rigidbody[] _childrenRigidbody;
        private Collider[] _childrenColliders;


        #region Unity Lifecycle
        private void Awake() 
        {
            _animator = GetComponent<Animator>();
            _childrenRigidbody = GetComponentsInChildren<Rigidbody>();
            _childrenColliders = GetComponentsInChildren<Collider>();
        }
        #endregion

        #region Private Functions
        private void SetChildrenRigidbodiesKinematic(bool isKinematic) 
        {
            for(int i = 0; i < _childrenRigidbody.Length; i++) 
            {
                _childrenRigidbody[i].isKinematic = isKinematic;
            }
        }

        private void SetChildrenCollidersEnable(bool isEnable) 
        {
            for(int i = 0; i < _childrenColliders.Length; i++) 
            {
                _childrenColliders[i].enabled = isEnable;
            }
        }
        #endregion

        #region Public Functions
        public void EnableRagdoll() 
        {
            SetChildrenRigidbodiesKinematic(false);
            SetChildrenCollidersEnable(true);
            _animator.enabled = false;
        }

        public void DisableRagdoll() 
        {
            SetChildrenRigidbodiesKinematic(true);
            SetChildrenCollidersEnable(false);
        }

        #region Animation
        public void JumpAnimation() 
        {
            _animator.SetTrigger("Jump");
        }
        #endregion
        #endregion
    }
}


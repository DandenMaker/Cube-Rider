using System.Collections;
using UnityEngine;

namespace TZ_24Play 
{
    public class CubeBehaviour : MonoBehaviour
    {
        [SerializeField] private BlockManager container;

        [Header("Text Effect")]
        [SerializeField] private float textSpeed;
        [SerializeField] private float effectDuration;


        private Camera _main;

        private Rigidbody _rb;
        private Canvas _collectCanvas;
        private TrailRenderer _trail;
        private ParticleSystem _cubeStackEffect;


        #region Unity Lifecycle
        private void Awake() 
        {
            if(container == null) 
            {
                container = transform.parent.GetComponent<BlockManager>();
            }

            _trail = GetComponentInChildren<TrailRenderer>();
            _cubeStackEffect = GetComponentInChildren<ParticleSystem>();
            _main = Camera.main;

            _collectCanvas = GetComponentInChildren<Canvas>();
            _collectCanvas.worldCamera = _main;
            _collectCanvas.enabled = false;
        }

        private void Start() 
        {
            SetTrail(false);
        }

        private void Update() 
        {
            if(transform.position.z < _main.transform.position.z) 
            {
                this.gameObject.SetActive(false);
                this.gameObject.transform.position = Vector3.zero;
                container.SetSingleBlock(this);
            }

           
            if(transform.parent == null && _rb == null) 
            {
                _rb = this.gameObject.AddComponent<Rigidbody>();
                _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }

        private void OnCollisionEnter(Collision other) 
        {
            if(other.gameObject.CompareTag("Player")) 
            {
                if(_rb != null) 
                {
                    Destroy(_rb);
                }

            }
        }

        private IEnumerator DisplayCanvasText() 
        {
            yield return null;
            _collectCanvas.enabled = true;

            float delta = 0f;
            while(delta < effectDuration) 
            {
                delta += Time.deltaTime;

                _collectCanvas.transform.localPosition += new Vector3(0f, textSpeed * Time.deltaTime, -textSpeed * Time.deltaTime);
                yield return null;
            }

            _collectCanvas.enabled = false;
            _collectCanvas.transform.localPosition = Vector3.zero;
        }
        #endregion

        #region  Private Functions
        #endregion

        #region Public Functions
        public void SetTrail(bool state) 
        {
            _trail.enabled = state;
        }

        public void EnableText() 
        {
            StartCoroutine(DisplayCanvasText());
        }

        public void StackEffectPlay() 
        {
            _cubeStackEffect.Play();
        }
        #endregion
    }
}

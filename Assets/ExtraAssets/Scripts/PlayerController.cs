using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

namespace TZ_24Play 
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed;
        [SerializeField] private float sideSpeed;
        [SerializeField] private float minSwipeForce;

        [Header("Collision Settings")]
        [SerializeField] private LayerMask cubeWallMask;
        [SerializeField] private LayerMask groundMask;

        [Header("Dependency")]
        [SerializeField] private Ragdoll ragdoll;
        [SerializeField] private ParticleSystem warpEffect;
        [SerializeField] private Transform cubeHolder;
        [SerializeField] private BlockManager blockContainer;
        [SerializeField] private UIControl gameUi;


        private CubeBehaviour _topBlock;
        private CubeBehaviour _bottomBlock;
        private Rigidbody _rb;
        private List<CubeBehaviour> _cubes;
        private Camera _camera;

        private Vector3 _moveVector;
        private bool _isPlay;



        #region Unity Lifecycle
        private void Awake() 
        {
            _rb = GetComponent<Rigidbody>();
            _cubes = new List<CubeBehaviour>();
            _camera = Camera.main;
            _topBlock = cubeHolder.GetComponentInChildren<CubeBehaviour>();
            _bottomBlock = _topBlock;

            blockContainer.SetSingleBlock(_topBlock);
           _cubes.Add(_topBlock);
        }

        private void OnEnable() 
        {
            EnhancedTouchSupport.Enable();
        }

        private void Start() 
        {
            Init();
        }

        private void OnDisable() 
        {
            EnhancedTouchSupport.Disable();
        }

        private void Update() 
        { 
            _moveVector = Vector3.zero;
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            
            for(int i = 0; i < activeTouches.Count; i++) 
            {
                var activeTouch = activeTouches[i];
                if(activeTouch.phase == UnityEngine.InputSystem.TouchPhase.Moved) 
                {
                    if(Mathf.Abs(activeTouch.delta.x) > minSwipeForce) 
                    {
                        _moveVector = activeTouch.delta.x > 0 ? Vector3.right : Vector3.left; 
                    }
                }
            }

            ragdoll.transform.localPosition = _topBlock.transform.localPosition + new Vector3(0f, 0.5f, 0f);
        }

        private void FixedUpdate() 
        {
            if(_isPlay) 
            {
                if(_rb != null) 
                {
                    _rb.velocity = new Vector3(_moveVector.x * sideSpeed, _rb.velocity.y, speed);
                    // _rb.MovePosition(transform.position + new Vector3(_moveVector.x * sideSpeed * Time.deltaTime, 0, speed * Time.deltaTime));
                }
            }
        }

        private void OnCollisionEnter(Collision other) 
        {
            if(other.gameObject.CompareTag("CubeObject")) 
            {
                var cubeBehaviour = blockContainer.GetSingleBlockByObject(other.gameObject);
                AddBlock(cubeBehaviour);
            }

            if(other.gameObject.CompareTag("CubeWall")) 
            {
                StartCoroutine(MakeOffset());
                Handheld.Vibrate();
            }
        }
        #endregion

        #region Private Functions
        private void Init() 
        {
            _isPlay = false;
            _bottomBlock.SetTrail(true);

            ragdoll.DisableRagdoll();
            gameUi.SetFailPanelState(false);
            gameUi.SetPlayPanelState(true);
            warpEffect.Stop();
        }

        private void Fail() 
        {
            _isPlay = false;

            ragdoll.EnableRagdoll();
            gameUi.SetFailPanelState(true);
            warpEffect.Stop();
        }

        private void AddBlock(CubeBehaviour block) 
        {
            if(_cubes.Contains(block)) return;

            block.transform.SetParent(cubeHolder);

            var lastCube = _cubes[_cubes.Count - 1];
            block.transform.localPosition = 
                lastCube.transform.localPosition + new Vector3(0f, lastCube.transform.localScale.y, 0f);

            ragdoll.JumpAnimation();
            block.EnableText();
            block.StackEffectPlay();
            ragdoll.transform.localPosition += new Vector3(0f, 1f, 0f);
            
            _topBlock = block;
            _cubes.Add(block);
        }

        private IEnumerator MakeOffset() 
        {
            _bottomBlock.SetTrail(false);

            var collisions = CheckCollision();
            for(int i = 0; i < collisions.Length; i++) 
            {
                var currentBlock = collisions[i];
                if(currentBlock == _topBlock) 
                {
                    Fail();
                    yield break;
                }

                currentBlock.transform.SetParent(null);    
            }

            yield return new WaitForSeconds(0.5f);

            for(int i = 0; i < collisions.Length; i++) 
            {
                var currentBlock = collisions[i];
                _cubes.Remove(currentBlock);    
            }

            _bottomBlock = _cubes[0];
            StartCoroutine(SetTrail());
            FixHoles();
        }

        private void FixHoles() 
        {
            for(int i = 1; i < _cubes.Count; i++) 
            {
                var nextElement = _cubes[i];
                var to = _bottomBlock.transform.localPosition + new Vector3(0f, nextElement.transform.localScale.y * i, 0f);

                StartCoroutine(BoxFallLerp(nextElement.transform.localPosition, to, nextElement.gameObject));
            }
        }

        private IEnumerator SetTrail() 
        {
            bool isHit = Physics.Raycast(_bottomBlock.transform.position, Vector3.down, 0.5f, groundMask);
            while(!isHit) 
            {
                isHit = Physics.Raycast(_bottomBlock.transform.position, Vector3.down, 0.5f, groundMask); 
                yield return null;
            }

            _bottomBlock.SetTrail(true);
        }

        private IEnumerator BoxFallLerp(Vector3 from, Vector3 to, GameObject go) 
        {
            float delta = 0f;
            while(delta < 1) 
            {
                delta += Time.deltaTime * 5;
                delta = Mathf.Clamp01(delta);

                go.transform.localPosition = Vector3.Lerp(from, to, delta);

                yield return null;
            }
        }

        private CubeBehaviour[] CheckCollision() 
        {
            var collision = new List<CubeBehaviour>();
            for(int i = 0; i < _cubes.Count; i++) 
            {
                var currentBlock = _cubes[i];

                // A Problem with Physics.Boxcast
                if(Physics.Raycast(currentBlock.transform.position, transform.forward, 0.5f, cubeWallMask)) 
                {
                    collision.Add(currentBlock);
                }
                else if(Physics.Raycast(currentBlock.transform.position - new Vector3(currentBlock.transform.localScale.x / 2, 0f, 0f), transform.forward, 0.5f, cubeWallMask)) 
                {
                    collision.Add(currentBlock);
                }
                else if(Physics.Raycast(currentBlock.transform.position + new Vector3(currentBlock.transform.localScale.x / 2, 0f, 0f), transform.forward, 0.5f, cubeWallMask)) 
                {
                    collision.Add(currentBlock);
                }
            }

            return collision.ToArray();
        }
        #endregion

        #region Public Functions
        public void StartGame() 
        {
            _isPlay = true;
            gameUi.SetPlayPanelState(false);
            warpEffect.Play();
        }

        public void TryAgain() 
        {
            SceneManager.LoadScene(0);
        }
        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TZ_24Play 
{
    public class BlockManager : MonoBehaviour
    {
        [SerializeField] private CubeBehaviour blockPrefab;
        [SerializeField] private int initialCount;

        private List<CubeBehaviour> _blocks;
        private CubeBehaviour[] _allBlocks;

        #region Unity Lifecycle
        private void Awake() 
        {
            _blocks = new List<CubeBehaviour>();
            FillContainer();
        }

        private void Start() 
        {
            _allBlocks = _blocks.ToArray();
        }
        #endregion

        #region Private Functions
        private void FillContainer() 
        {
            for(int i = 0; i < initialCount; i++) 
            {
                var go = Instantiate(blockPrefab, Vector3.zero, blockPrefab.transform.rotation, transform);
                go.gameObject.SetActive(false);

                _blocks.Add(go);
            }
        }
        #endregion

        #region Public Functions
        public CubeBehaviour GetSingleBlock() 
        {
            if(_blocks.Count > 0) 
            {
                var singleBlock = _blocks[0];
                _blocks.Remove(singleBlock);
                
                return singleBlock;
            }

            return null;
        }

        public void SetSingleBlock(CubeBehaviour block) 
        {
            if(!_blocks.Contains(block)) 
            {
                _blocks.Add(block);
            }
        }

        public CubeBehaviour GetSingleBlockByObject(GameObject go) 
        {
            return _allBlocks.First(cube => cube.gameObject == go);
        }
        #endregion
    }
}


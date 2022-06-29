using UnityEngine;

namespace TZ_24Play
{
    public class TrackBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;


        #region Unity Licecycle
        #endregion 

        #region Public Functions
        public void SetBlocks(BlockManager container) 
        {
            for(int i = 0; i < spawnPoints.Length; i++) 
            {
                var point = spawnPoints[i];
                var block = container.GetSingleBlock();

                if(block == null) return;

                block.transform.position = point.position;
                block.gameObject.SetActive(true);
            }
        }
        #endregion
    }
} 


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TZ_24Play 
{
    public class TrackManager : MonoBehaviour
    {
        [SerializeField] private TrackVariant[] variants;
        [SerializeField] private int initialCount;

        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private float minDistance;

        [Header("Dependency")]
        [SerializeField] private BlockManager blockContainer;


        private Queue<TrackBehaviour> _tracks;
        private List<TrackBehaviour> _usedTracks;

        private GameObject _lastUsed;


        #region Unity Lifecycle 
        private void Awake() 
        {
            _tracks = new Queue<TrackBehaviour>();
            _usedTracks = new List<TrackBehaviour>();
        }

        private void Start() 
        {
            InitTracks();
            LocateInitTracks();
        }

        private void Update() 
        {
            if(Mathf.Abs((_lastUsed.transform.position - target.position).z) < minDistance) 
            {
                if(_tracks.Count <= initialCount) 
                {
                    _usedTracks = Utils.Shuffle(_usedTracks).ToList();
                    for(int i = 0; i < _usedTracks.Count; i++) 
                    {
                        var usedTrack = _usedTracks[i];
                        _tracks.Enqueue(usedTrack);
                        _usedTracks.Remove(usedTrack);
                    }
                }

                SetNext();
            }
        }
        #endregion

        #region  Private Functions
        private void InitTracks() 
        {
            var initTrack = GetComponentInChildren<TrackBehaviour>();

            initTrack.SetBlocks(blockContainer);
            _lastUsed = initTrack.gameObject;
            _usedTracks.Add(initTrack);

            var tempList = new List<TrackBehaviour>();
            foreach(var variant in variants) 
            {
                for(int i = 0; i < variant.count; i++) 
                {
                    var track = Instantiate(variant.prefab, Vector3.zero, Quaternion.identity, transform);
                    track.gameObject.SetActive(false);

                    // _tracks.Enqueue(track);
                    tempList.Add(track);
                }
            }

            tempList = Utils.Shuffle(tempList).ToList();
            foreach(var track in tempList) 
            {
                _tracks.Enqueue(track);
            }
        }

        private void LocateInitTracks() 
        {
            for(int i = 0; i < initialCount; i++) 
            {
                SetNext();
            }
        }

        private void SetNext() 
        {
            var track = _tracks.Dequeue();

            var lastScale = _lastUsed.transform.localScale.z / 2;
            var currentScale = track.transform.localScale.z / 2;
            var nextPosition = new Vector3(0, 0, _lastUsed.transform.position.z + lastScale + currentScale);

            track.transform.position = nextPosition;
            track.SetBlocks(blockContainer);
            track.gameObject.SetActive(true);

            _lastUsed = track.gameObject;
            _usedTracks.Add(track);
        }
        #endregion
    }

    [System.Serializable]
    public class TrackVariant 
    {
        public TrackBehaviour prefab;
        public int count;
    }
}
using UnityEngine;

namespace TZ_24Play
{
    public class UIControl : MonoBehaviour
    {
        [SerializeField] private GameObject playPanel;
        [SerializeField] private GameObject failPanel;

        #region Public Functions
        public void SetPlayPanelState(bool state) 
        {
            playPanel.SetActive(state);
        }

        public void SetFailPanelState(bool state) 
        {
            failPanel.SetActive(state);
        }
        #endregion
    }
}


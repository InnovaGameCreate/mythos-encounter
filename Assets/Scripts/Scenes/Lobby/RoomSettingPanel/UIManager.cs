using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _roomSettingPanels = new List<GameObject>();
        [SerializeField] private LookDisplayObject _lookDisplayObjectCs;

        private int _panelNo = 0;

        private void OnEnable()
        {
            ChangePanel(0);
        }

        public void OnCloseButton()
        {
            _lookDisplayObjectCs.OnDisableDisplay();
        }

        /// <summary>
        /// ƒpƒlƒ‹‚Ì•\¦‚ğØ‚è‘Ö‚¦‚é
        /// </summary>
        /// <param name="no"></param>
        public void ChangePanel(int no)
        {
            if (no < _roomSettingPanels.Count)
            {
                _roomSettingPanels[_panelNo].SetActive(false);
                _roomSettingPanels[no].SetActive(true);
                _panelNo = no;
            }
        }
    }
}

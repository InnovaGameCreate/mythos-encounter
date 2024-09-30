using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _roomSettingPanels = new List<GameObject>();

        private int _panelNo = 0;

        private void OnEnable()
        {
            _roomSettingPanels[0].SetActive(true);
            _panelNo = 0;
        }

        private void OnDisable()
        {
            _roomSettingPanels[_panelNo].SetActive(false);
            _panelNo = 0;
        }

        /// <summary>
        /// パネルの表示を切り替える
        /// </summary>
        /// <param name="no"></param>
        /// <param name="state"></param>
        public void SetActive(int no, bool state)
        {
            if (no < _roomSettingPanels.Count) //インデックスの存在判定
            {
                _roomSettingPanels[no].SetActive(state);
                _panelNo = no;
            }
        }
    }
}

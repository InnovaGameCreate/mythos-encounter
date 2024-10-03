using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Lobby.RoomSettingPanel
{
    public class RoomSetting_1 : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManagerCs;

        public void ToRoomSetting_2()
        {
            _uiManagerCs.ChangePanel(1);
        }

        public void ToRoomSetting_5()
        {
            _uiManagerCs.ChangePanel(4);
        }
    }
}

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
            _uiManagerCs.ChangePanel(1); //セッション作成
        }

        public void ToRoomSetting_3()
        {
            _uiManagerCs.ChangePanel(2); //セッション参加
        }
    }
}

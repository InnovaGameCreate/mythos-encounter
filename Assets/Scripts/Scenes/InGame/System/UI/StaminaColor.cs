using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Player;

namespace Scenes.Ingame.InGameSystem.UI
{
    public class StaminaColor : MonoBehaviour
    {
        void Start()
        {
            PlayerStatus playerstatus = PlayerStatus.Instance;
            Renderer renderer = GetComponent<Renderer>();
            playerstatus.OnPlayerStaminaChange.Subscribe(x => {
                if (0 <= x && x <= 10)
                    renderer.material.color = Color.red;
                else if (10 < x && x <= 50)
                    renderer.material.color = new Color(1.0f, 0.5f, 0.0f);
                else
                    renderer.material.color = Color.white;
            }).AddTo(this); //スタミナによって色を変更
        }
    }
}

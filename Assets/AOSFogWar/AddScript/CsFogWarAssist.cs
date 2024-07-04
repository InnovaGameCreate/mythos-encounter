using FischlWorks_FogWar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scenes.Ingame.Manager;

/// <summary>
/// ��邱��
/// 1.Player��fogRevealers�ɒǉ�������
/// </summary>
public class CsFogWarAssist : MonoBehaviour
{
    private csFogWar _fogwar;
    // Start is called before the first frame update
    void Start()
    {
        _fogwar = GetComponent<csFogWar>();
        IngameManager.Instance.OnPlayerSpawnEvent
            .FirstOrDefault()
            .Subscribe(_ =>
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject Player in players)
                {
                    _fogwar.AddFogRevealer(new csFogWar.FogRevealer(Player.transform, 1, false));
                }
            }).AddTo(this);
    }

    
}

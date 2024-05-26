using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 発狂の種類の1つ
    /// 1.スタミナの消費速度と出血で受けるダメージの減少速度が2倍になる.
    /// 2.発症するとプレイヤーが1秒に1度のペースで白い息を吐くようになる.心拍数の増加
    /// </summary>
    public class IncreasePulsation : MonoBehaviour,IInsanity
    {
        private PlayerStatus _myPlayerStatus;
        private TempPlayerMove _myPlayerMove;

        private bool _isFirst = true;//初めて呼び出されたか


        public void Setup()
        {
            _myPlayerStatus = GetComponent<PlayerStatus>();
            _myPlayerMove = GetComponent<TempPlayerMove>();
        }

        public void Active()
        {
            if (_isFirst)
            {
                Setup();
                _isFirst = false;
            }

            //スタミナの消費速度が2倍に
            _myPlayerMove.Pulsation(true);

            //出血で受けるダメージが2倍に
            _myPlayerStatus.PulsationBleeding(true);

            //白い息をはかせる
            //Todo：今後実装する

            Debug.Log("心拍数増加");
        }

        public void Hide()
        {
            _myPlayerMove.Pulsation(false);
            _myPlayerStatus.PulsationBleeding(false);
        }
    }

}
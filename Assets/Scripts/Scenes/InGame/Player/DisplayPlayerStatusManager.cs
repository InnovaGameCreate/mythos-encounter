using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using System;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// PlayerStatusの情報をもとにゲーム内UIにプレイヤー全員の情報を表示させる為のスクリプト
    /// 後ほどネットワークでの、複数人の情報の表示に対応できる形を心がける
    /// MV(R)PにおけるViewの役割を想定
    /// </summary>
    public class DisplayPlayerStatusManager : MonoBehaviour
    {
        [SerializeField] private Slider[] _healthSliders;
        [SerializeField] private Slider[] _sanValueSliders;
        [SerializeField] private TMP_Text[] _healthText;
        [SerializeField] private TMP_Text[] _sanValueText;

        public int playerNum { get { return _players.Length; } }//ゲームに参加しているプレイヤーの人数
        [SerializeField] private GameObject[] _players;//プレイヤーのGameObjectを格納

        public PlayerStatus[] PlayerStatuses { get { return _playerStatuses; } }
        [SerializeField] private PlayerStatus[] _playerStatuses;

        private Subject<Unit> _completeSort = new Subject<Unit>();
        public IObservable<Unit> OnCompleteSort { get { return _completeSort; } }//_playerStatuses配列を作成・ソートを終えた際にイベントが発行 

        private void Start()
        {
            //プレイヤーの数を数える。人数をもとに配列を作成。
            _players = GameObject.FindGameObjectsWithTag("Player");
            _playerStatuses = new PlayerStatus[_players.Length];

            //PlayerIDの昇順にソートを行う。
            foreach (var player in _players)
            {
                int myID = player.GetComponent<PlayerStatus>().playerID;
                _playerStatuses[myID] = player.GetComponent<PlayerStatus>();

                //配列のソート終了を通知する
                _completeSort.OnNext(Unit.Default);
                _completeSort.OnCompleted();
            }
        }
        /// <summary>
        /// Sliderの値を変える為の関数
        /// </summary>
        /// <param name="value">Slinder.Valueに代入する値</param>
        /// <param name="ID">プレイヤーID</param>
        /// <param name="mode">Health(体力), SanValue(SAN値)どちらを変更するのかを決定</param>
        public void ChangeSliderValue(int value , int ID, string mode)
        {
            Debug.Log(ID);
            if (mode == "Health")
            {
                _healthSliders[ID].value = value;
                _healthText[ID].text = value.ToString();
            }

            else if (mode == "SanValue")
            { 
                _sanValueSliders[ID].value = value;
                _sanValueText[ID].text = value.ToString();
            }
                
        }
    }
}


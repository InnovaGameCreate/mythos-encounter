using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using System;
using Scenes.Ingame.Manager;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// PlayerStatusの情報をもとにゲーム内UIにプレイヤー全員の情報を表示させる為のスクリプト（データ管理を担う）
    /// 後ほどネットワークでの、複数人の情報の表示に対応できる形を心がける
    /// </summary>
    public class DisplayPlayerStatusManager : MonoBehaviour
    {

        public int playerNum { get { return _players.Length; } }//ゲームに参加しているプレイヤーの人数
        [SerializeField] private GameObject[] _players;//プレイヤーのGameObjectを格納

        public PlayerStatus[] PlayerStatuses { get { return _playerStatuses; } }
        [SerializeField] private PlayerStatus[] _playerStatuses;

        private Subject<Unit> _completeSort = new Subject<Unit>();
        public IObservable<Unit> OnCompleteSort { get { return _completeSort; } }//_playerStatuses配列を作成・ソートを終えた際にイベントが発行 

        private void Start()
        {
            IngameManager.Instance.OnPlayerSpawnEvent
                .Subscribe(_ =>
                {
                    //プレイヤーの数を数える。人数をもとに配列を作成。
                    //今後はここにある処理全てを「Playerのスポーンが完了した」イベントが発行されたら行うようにする。(PlayerSpawnerから参加人数を取得できるようにする)
                    _players = GameObject.FindGameObjectsWithTag("Player");
                    _playerStatuses = new PlayerStatus[_players.Length];

                    //PlayerIDの昇順にソートを行う。
                    //今後IDは8桁の数字になるかも。その時は数の大きさで昇順にソートするようにせよ
                    foreach (var player in _players)
                    {
                        int myID = player.GetComponent<PlayerStatus>().playerID;
                        _playerStatuses[myID] = player.GetComponent<PlayerStatus>();
                    }

                    //配列のソート終了を通知する
                    _completeSort.OnNext(Unit.Default);
                    _completeSort.OnCompleted();
                }).AddTo(this);
        }
    }
}
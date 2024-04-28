using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scenes.Ingame.Player;
using UniRx;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// パトロールする。プレイヤーの痕跡を探す。巡回状態と索敵状態の動きを決定し、追跡と攻撃状態への移行を行う。
    /// </summary>
    public class EnemySearch : MonoBehaviour
    {
        private EnemyVisibilityMap _myVisivilityMap;
        [SerializeField]
        private float _checkRate;//何秒ごとに視界の状態をチェックするか
        private float _checkTimeCount;//前回チェックしてからの時間を計測
        [SerializeField]
        private bool _debugMode;
        [SerializeField]
        private EnemyMove _myEneyMove;

        [SerializeField]
        private float _visivilityRange;//仕様上視界範囲は全て同一？じゃなかったらこれはEnemyStatusに送り込むよ
        [SerializeField]
        private EnemyStatus _enemyStatus;

        private GameObject _player;
        private PlayerStatus _playerStatus;
        private float _audiomaterPower;

        //索敵行動のクラスです
        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// 外部からこのスクリプトの初期設定をするために呼び出す
        /// </summary>
        public void Init(){
            _player = GameObject.Find("Player");
            if (_player == null) { Debug.LogWarning("プレイヤーが認識できません"); }
            _playerStatus = _player.GetComponent<PlayerStatus>();
            if (_playerStatus == null) { Debug.LogWarning("プレイヤーステータスが認識できません"); }
            //スペックの初期設定
            _audiomaterPower = _enemyStatus.ReturnAudiomaterPower;
            


            //スペックの変更を受け取る
            _enemyStatus.OnAudiometerPowerChange.Subscribe(x => { _audiomaterPower = x; }).AddTo(this);


            _playerStatus.OnPlayerActionStateChange
                .Where(state => state == PlayerActionState.Walk || state == PlayerActionState.Dash || state == PlayerActionState.Sneak)
                .Subscribe(state =>
                {
                    //プレイヤーの騒音を聞く
                    if (_enemyStatus.ReturnEnemyState == EnemyState.Patorolling || _enemyStatus.ReturnEnemyState == EnemyState.Searching)
                    {
                        if (_playerStatus.nowPlayerActionState == PlayerActionState.Sneak)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerSneakVolume * _audiomaterPower * 0.01f),2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x,2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2)))  > 0) 
                            { //音が聞こえるかどうか
                                _myVisivilityMap.HearingSound(_player.transform.position,15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        }
                        else if(_playerStatus.nowPlayerActionState == PlayerActionState.Walk)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerWalkVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //音が聞こえるかどうか
                                _myVisivilityMap.HearingSound(_player.transform.position, 15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        } else if (_playerStatus.nowPlayerActionState == PlayerActionState.Dash)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerRunVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //音が聞こえるかどうか
                                _myVisivilityMap.HearingSound(_player.transform.position, 15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        }
                    }
                }).AddTo(this);

        }


        protected virtual void FixedUpdate()
        {
            if (_myVisivilityMap != null)//索敵の準備ができていない場合
            {
                Debug.LogError("マップ情報がありません、_myVisivilityMapを作成してください");
                return;
            }

            if (_enemyStatus.ReturnEnemyState == EnemyState.Patorolling || _enemyStatus.ReturnEnemyState == EnemyState.Searching)
            { //巡回状態または捜索状態の場合







                //定期的に視界情報を調べる
                _checkTimeCount += Time.deltaTime;
                if (_checkTimeCount > _checkRate)
                {
                    _checkTimeCount = 0;
                    //いろんなものを調べる。これは決定的なものほど優先して認識する
                    if (false)
                    { //プレイヤーの姿が見えるか調べる

                    }
                    else if (false)
                    { //光が見えるか調べる

                    }
                    else if (_enemyStatus.ReturnEnemyState == EnemyState.Patorolling || _enemyStatus.ReturnEnemyState == EnemyState.Searching)//プレイヤーが音を立てているかどうか
                    {
                        if (_playerStatus.nowPlayerActionState == PlayerActionState.Sneak)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerSneakVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //音が聞こえるかどうか
                                if (_debugMode) Debug.Log("忍ぶ音が聞こえる");
                                _myVisivilityMap.HearingSound(_player.transform.position,  15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        }
                        else if (_playerStatus.nowPlayerActionState == PlayerActionState.Walk)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerWalkVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //音が聞こえるかどうか
                                if (_debugMode) Debug.Log("歩く音が聞こえる");
                                _myVisivilityMap.HearingSound(_player.transform.position,  15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        }
                        else if (_playerStatus.nowPlayerActionState == PlayerActionState.Dash)
                        {
                            if (Mathf.Pow((float)(_playerStatus.nowPlayerRunVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //音が聞こえるかどうか
                                if (_debugMode) Debug.Log("走る音が聞こえる");
                                _myVisivilityMap.HearingSound(_player.transform.position, 15);
                                _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            }
                        }
                        else if (_debugMode) Debug.Log("音は聞こえない");
                    }
                    else {
                        //なんの痕跡も見つからなかった場合普通に巡回する
                        _myVisivilityMap.CheckVisivility(this.transform.position, _visivilityRange);
                        if (_myEneyMove.endMove)//移動が終わっている場合
                        { 
                            //痕跡のあった場所まで来たが何もいなかった場合ここが実行されるのでStatusを書き換える
                            _enemyStatus.ChangeEnemyState(EnemyState.Patorolling);
                            //あらたな移動先を取得するメソッドを書き込む
                            _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                        }
                    }
                }


            }
        }


        public void SetVisivilityMap(EnemyVisibilityMap setVisivilityMap) 
        { 
            _myVisivilityMap = setVisivilityMap;
        }


    }
}

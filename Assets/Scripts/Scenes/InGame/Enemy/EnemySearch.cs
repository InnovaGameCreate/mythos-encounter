using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using static Scenes.Ingame.Enemy.EnemyVisibilityMap;
using Cysharp.Threading.Tasks.Triggers;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// パトロールする。プレイヤーの痕跡を探す。巡回状態と索敵状態の動きを決定し、追跡と攻撃状態への移行を行う。
    /// </summary>
    public class EnemySearch : MonoBehaviour
    {
        protected EnemyVisibilityMap _myVisivilityMap;
        [SerializeField]
        protected float _checkRate;//何秒ごとに視界の状態をチェックするか
        protected float _checkTimeCount;//前回チェックしてからの時間を計測
        [SerializeField]
        protected bool _debugMode;
        [SerializeField]
        protected EnemyMove _myEneyMove;

        [SerializeField]
        protected float _visivilityRange;//仕様上視界範囲は全て同一？じゃなかったらこれはEnemyStatusに送り込むよ
        [SerializeField]
        protected EnemyStatus _enemyStatus;

        protected GameObject _player;
        protected PlayerStatus _playerStatus;
        protected float _audiomaterPower;
        protected Vector3 nextPositionCandidate = Vector3.zero;
        //索敵行動のクラスです
        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// 外部からこのスクリプトの初期設定をするために呼び出す
        /// </summary>
        public void Init(EnemyVisibilityMap setVisivilityMap)
        {
            _myVisivilityMap = setVisivilityMap;
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
                        float valume = 0;

                        switch (state)
                        {
                            case PlayerActionState.Sneak:
                                valume = _playerStatus.nowPlayerSneakVolume;
                                break;
                            case PlayerActionState.Walk:
                                valume = _playerStatus.nowPlayerWalkVolume;
                                break;
                            case PlayerActionState.Dash:
                                valume = _playerStatus.nowPlayerRunVolume;
                                break;
                        }

                        if (Mathf.Pow((float)(valume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                        {//音が聞こえるかどうか
                            _myVisivilityMap.HearingSound(_player.transform.position, 15, false);
                            _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
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
                    float valume = 0;//プレイヤーの騒音を記録
                    switch (_playerStatus.nowPlayerActionState)
                    {
                        case PlayerActionState.Sneak:
                            valume = _playerStatus.nowPlayerSneakVolume;
                            if (_debugMode) Debug.Log("忍ぶ音が聞こえる");
                            break;
                        case PlayerActionState.Walk:
                            valume = _playerStatus.nowPlayerWalkVolume;
                            if (_debugMode) Debug.Log("歩く音が聞こえる");
                            break;
                        case PlayerActionState.Dash:
                            valume = _playerStatus.nowPlayerRunVolume;
                            if (_debugMode) Debug.Log("走る音が聞こえる");
                            break;
                    }
                    _checkTimeCount = 0;
                    //いろんなものを調べる。これは決定的なものほど優先して認識する
                    if (CheckCanPlayerVisivlleCheck())
                    { //プレイヤーの姿が見えるか調べる
                      _myEneyMove.SetMovePosition(_player.transform.position);
                      _enemyStatus.SetEnemyState(EnemyState.Chese);
                    }
                    
                    else if (_enemyStatus.ReturnReactToLight&& _myVisivilityMap.RightCheck(this.transform.position,_player.transform.position,_visivilityRange,_playerStatus.nowPlayerLightRange, ref nextPositionCandidate))//&&は左から評価される事に注意
                    { //光が見えるか調べる
                        if (_debugMode) Debug.Log("光が見えた");
                        _enemyStatus.SetEnemyState(EnemyState.Searching);
                        _myEneyMove.SetMovePosition(nextPositionCandidate);
                    }
                    
                    else if (Mathf.Pow((float)(valume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0) 
                    { //プレイヤーの騒音が聞こえるか調べる
 
                        _enemyStatus.SetEnemyState(EnemyState.Searching);
                        _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                        _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                    }
                    
                    else
                    {
                        //なんの痕跡も見つからなかった場合普通に巡回する
                        _myVisivilityMap.CheckVisivility(this.transform.position, _visivilityRange);
                        
                        if (_myEneyMove.endMove)//移動が終わっている場合
                        {
                            
                            //痕跡のあった場所まで来たが何もいなかった場合ここが実行されるのでStatusを書き換える
                            _enemyStatus.SetEnemyState(EnemyState.Patorolling);
                            //あらたな移動先を取得するメソッドを書き込む
                            _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                            
                        }
                            
                        
                    }
                    
                    
                    
                }
            }
        }

        protected bool CheckCanPlayerVisivlleCheck() {
            float range = Vector3.Magnitude(this.transform.position - _player.transform.position);//平方根を求めるのはすごくコストが重いらしいので確実に計算が必要になってからしてます
                                             //視界が通るか＝Rayが通るか
            bool hit;
            Ray ray = new Ray(this.transform.position, _player.transform.position - this.transform.position);
            hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, -1, QueryTriggerInteraction.Collide);
            if (!hit)
            { //何にもあたっていなかった場合
                if (_debugMode) { Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 3); Debug.Log("プレイヤー発見"); }
                return true;
            }
            else { 
                return false;
            }
        }


    }
}

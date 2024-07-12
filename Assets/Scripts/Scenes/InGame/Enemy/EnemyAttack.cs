using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.AI;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵キャラの攻撃を管理する。追跡状態と攻撃状態はこのスクリプトが作動する
    /// </summary>
    public class EnemyAttack : MonoBehaviour
    {
        [Header("このスクリプトを制御する変数")]
        [SerializeField][Tooltip("何秒ごとに視界の状態、攻撃可能性、SANをチェックするか")] private float _checkRate;
        [SerializeField][Tooltip("戦闘時の視界の広さ")] private float _visivilityRange;//仕様上視界範囲は全て同一？じゃなかったらこれはEnemyStatusに送り込むよ
        [SerializeField] [Tooltip("見失ったとしてもどれだけの時間頑張って探そうとするかどうか")]private float _blindChaseTime;
        [SerializeField][Tooltip("デバッグするかどうか")] private bool _debugMode;



        private int _horror;
        private int _attackPower;

        [Header("Horror,AttackPowerを除く攻撃性能")]
        [SerializeField][Tooltip("攻撃の間隔")] private float _attackTime;
        [SerializeField][Tooltip("近接攻撃の射程")] private float _atackRange;
        [SerializeField][Tooltip("遠隔攻撃が可能かどうか")] private bool canShot;
        //[SerializeField][Tooltip("遠隔攻撃の攻撃力")] private int _shotPower;
        [SerializeField][Tooltip("遠隔攻撃の間隔")] private float _shotTime;
        [SerializeField][Tooltip("遠隔攻撃の射程")] private float _shotRange;
        [SerializeField][Tooltip("弾丸")] private GameObject _ballet;







        [Header("自身についているメソッド")]
        [SerializeField] private EnemyStatus _enemyStatus;
        [SerializeField] private EnemySearch _enemySearch;
        [SerializeField] private EnemyMove _enemyMove;
        private NavMeshAgent _myAgent;

        //##########内部で使う変数##########
        private GameObject _player;
        private PlayerStatus _playerStatus;
        private float _attackTimeCount;
        private float _shotTimeCount;
        private float _audiomaterPower;//聞く力
        private float _checkTimeCount;//前回チェックしてからの時間を計測
         float _blindChaseTimeCount;
        private EnemyVisibilityMap _myVisivilityMap;
        private EnemyState _lastEnemyState = EnemyState.None;
        Vector3 nextPositionCandidate = new Vector3(0, 0, 0);
        private Camera _camera;

        /// <summary>
        /// 初期化処理、外部からアクセスする
        /// </summary>
        public void Init(EnemyVisibilityMap setVisivilityMap) {
            _myVisivilityMap = setVisivilityMap;
            _horror = _enemyStatus.ReturnHorror;
            _attackPower = _enemyStatus.ReturnAtackPower;
            _audiomaterPower = _enemyStatus.ReturnAudiomaterPower;

            _player = GameObject.FindWithTag("Player");
            if (_player == null) { Debug.LogWarning("プレイヤーが認識できません"); }
            _playerStatus = _player.GetComponent<PlayerStatus>();
            if (_playerStatus == null) { Debug.LogWarning("プレイヤーステータスが認識できません"); }

            _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            _myAgent = GetComponent<NavMeshAgent>();

            _enemyStatus.OnEnemyStateChange.Subscribe(state => 
            {
                if ((state == EnemyState.Chase || state == EnemyState.Attack) && !((_lastEnemyState == EnemyState.Chase || _lastEnemyState == EnemyState.Attack))) 
                { 
                    _lastEnemyState = state;
                    _myVisivilityMap.SetEveryGridWatchNum(50);
                } 
            }).AddTo(this);

            if(_debugMode)_playerStatus.OnEnemyAttackedMe.Subscribe(_ => Debug.Log("攻撃された"));

        }

        // Update is called once per frame
        protected virtual void FixedUpdate()
        {
            float _playerDistance;
            if (_enemyStatus.ReturnEnemyState == EnemyState.Chase || _enemyStatus.ReturnEnemyState == EnemyState.Attack)
            { //追跡状態または攻撃状態の場合

                //いろいろ数える
                if (_attackTimeCount < _attackTime) { _attackTimeCount += Time.deltaTime; }
                if (_shotTimeCount < _shotTime) { _shotTimeCount += Time.deltaTime; }

                //定期的に状態を変更
                _checkTimeCount += Time.deltaTime;
                if (_checkTimeCount > _checkRate)
                {
                    _playerDistance = Vector3.Magnitude(this.transform.position - _player.transform.position);
                    _checkTimeCount = 0;
                    if (CheckCanPlayerVisivlleCheck()) //敵が見えるルートがあるかかどうかを確認する
                    {
                        //こちらが深淵を除くときry
                        SanCheck();
                        if (_playerDistance < _visivilityRange)//見える距離にいるかどうか
                        {
                            _myVisivilityMap.ChangeEveryGridWatchNum(1, true);
                            _myVisivilityMap.SetGridWatchNum(_player.transform.position, 0);
                            _blindChaseTimeCount = 0;//見えたのであきらめるまでのカウントはリセット
                                                     //移動目標をプレイヤーの座標にする
                            _enemyMove.SetMovePosition(_player.transform.position);
                            if (_playerDistance < _atackRange)//近接攻撃の射程内か確認する
                            { //近接攻撃をする
                                _enemyStatus.SetEnemyState(EnemyState.Attack);
                                if (_attackTimeCount > _attackTime)
                                {
                                    _attackTimeCount = 0;
                                    _shotTimeCount = 0;//遠隔から近接の距離に入った瞬間2連続で攻撃が行われないために両方のカウントを0にしている。
                                    if (_debugMode) Debug.Log("ここで近接攻撃！");
                                    _playerStatus.ChangeHealth(_attackPower, "Damage");
                                    _playerStatus.OnEnemyAttackedMeEvent.OnNext(Unit.Default);

                                }


                            }
                            else if (_playerDistance < _shotRange) //遠隔攻撃の射程内か確認する
                            { //遠隔攻撃をする
                                _enemyStatus.SetEnemyState(EnemyState.Attack);
                                if (_shotTimeCount > _shotTime)
                                {
                                    _attackTimeCount = 0;
                                    _shotTimeCount = 0;
                                    if (_debugMode) Debug.Log("ここで遠隔攻撃！");
                                    GameObject.Instantiate(_ballet, this.transform.position + new Vector3(0,2,0) + this.transform.forward, Quaternion.identity);
                                }


                            }
                            else
                            {//攻撃できないなら追いかける
                                _enemyStatus.SetEnemyState(EnemyState.Chase);
                            }
                        }
                    }
                    else
                    { //敵が見えないならせめてなんとかいそうなエリアへ行こうとする
                        _blindChaseTimeCount += _checkRate;
                        if (_blindChaseTimeCount > _blindChaseTime)
                        { //あきらめるかどうかの判定
                            _enemyStatus.SetEnemyState(EnemyState.Searching);//追っかけるのあきらめた
                        }
                        else
                        { //まだあきらめない場合、近距離に特化したのSearchを行う
                            _enemyStatus.SetEnemyState(EnemyState.Chase);

                            if (_enemyStatus.ReturnReactToLight && _myVisivilityMap.RightCheck(this.transform.position, _player.transform.position, _visivilityRange, _playerStatus.nowPlayerLightRange, ref nextPositionCandidate))//&&は左から評価される事に注意
                            { //光が見えるか調べる
                                if (_debugMode) Debug.Log("追跡中光が見えた");
                                _enemyMove.SetMovePosition(nextPositionCandidate);
                            }
                            else if (_enemyMove.endMove)
                            { //移動が終了している場合
                                if (_playerStatus.nowPlayerActionState == PlayerActionState.Sneak && Mathf.Pow((float)(_playerStatus.nowPlayerSneakVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) - (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)//忍音が聞こえるかどうか
                                {

                                    if (_debugMode) Debug.Log("追跡中忍ぶ音が聞こえる");
                                    _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                                    _enemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));

                                }
                                else if (_playerStatus.nowPlayerActionState == PlayerActionState.Walk && Mathf.Pow((float)(_playerStatus.nowPlayerWalkVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) - (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)//歩く音が聞こえるかどうか
                                {
                                    if (_debugMode) Debug.Log("追跡中歩く音が聞こえる");
                                    _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                                    _enemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));

                                }
                                else if (_playerStatus.nowPlayerActionState == PlayerActionState.Dash && Mathf.Pow((float)(_playerStatus.nowPlayerRunVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) - (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)//走る音が聞こえるかどうか
                                {

                                    if (_debugMode) Debug.Log("追跡中走る音が聞こえる");
                                    _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                                    _enemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));

                                }
                                else
                                {
                                    //なんの痕跡も見つからなかった場合普通に巡回する
                                    _myVisivilityMap.CheckVisivility(this.transform.position, _visivilityRange);
                                    if (_enemyMove.endMove)//移動が終わっている場合
                                    {
                                        _myVisivilityMap.ChangeGridWatchNum(_myAgent.path.corners[_myAgent.path.corners.Length],1,true);
                                        //あらたな移動先を取得するメソッドを書き込む
                                        _enemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else 
            {//追跡攻撃状態でなくても定期的にSANダメージを与えているかを確認する
                _checkTimeCount += Time.deltaTime;
                if (_checkTimeCount > _checkRate)
                {
                    _checkTimeCount = 0;
                    //Debug.LogWarning("San判定");現状視界が360なので視界が通らないここに要は無し
                }
            }
        }

        protected virtual bool CheckCanPlayerVisivlleCheck()
        {//キャラクターによって視界の角度判定つける？ミゴは360°、深き者どもは270°、一般的な人間の狂信者とかなら180°とか...
            float range = Vector3.Magnitude(this.transform.position - _player.transform.position) - 0.2f;//プレイヤー本体のコライダーに当たるため減らしてる
                                                                                                  //視界が通るか＝Rayが通るか
            bool hit;
            Ray ray = new Ray(this.transform.position, _player.transform.position - this.transform.position);
            hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, -1, QueryTriggerInteraction.Collide);
            if (!hit)
            { //何にもあたっていなかった場合
                if (_debugMode) { Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 3); Debug.Log("プレイヤー発見"); }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void SanCheck() {
            Vector3 ScreenPosition = _camera.WorldToScreenPoint(this.transform.position);
            //Debug.Log(ScreenPosition);
            if (ScreenPosition.x > 0 && ScreenPosition.x < 1920) {
                if (ScreenPosition.y > 0 && ScreenPosition.y < 1080) {
                    if (ScreenPosition.z > 0)
                    {
                        _playerStatus.ChangeSanValue(_horror, "Damage");




                    }
                }
            }
        }
    }
}

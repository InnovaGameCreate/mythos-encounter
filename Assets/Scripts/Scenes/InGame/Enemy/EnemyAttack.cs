using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using Fusion;

namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵キャラの攻撃を管理する。追跡状態と攻撃状態はこのスクリプトが作動する
    /// </summary>
    public class EnemyAttack : NetworkBehaviour
    {
        [Header("このスクリプトを制御する変数")]
        [SerializeField][Tooltip("何秒ごとに視界の状態、攻撃可能性、SANをチェックするか")] private float _checkRate;
        [Tooltip("戦闘時の視界の広さ、マップ端から端まで見える値で固定中")] private float _visivilityRange = 500;
       
        [SerializeField][Tooltip("デバッグするかどうか")] private bool _debugMode;



        private int _horror;



        [Header("攻撃動作")]
        [SerializeField][Tooltip("攻撃スクリプトたち、射程の短いものから長いものの順番で入れてください")] private List<EnemyAttackBehaviour> _enemyAttackBehaviours;
        //private float _massSUM;
        private float _atackRange;//最も射程の長い攻撃
        private float _massSUM;//使用可能な攻撃の重み付け
        



        [Header("自身についているメソッド")]
        [SerializeField] private EnemyStatus _enemyStatus;
        [SerializeField] private EnemySearch _enemySearch;
        [SerializeField] private EnemyMove _myEnemyMove;

        //##########内部で使う変数##########
        private GameObject _player;
        private PlayerStatus _playerStatus;
        private float _attackTimeCount;
        private float _shotTimeCount;
        private float _audiomaterPower;//聞く力
        private float _checkTimeCount;//前回チェックしてからの時間を計測
        
        private EnemyVisibilityMap _myVisivilityMap;
        Vector3 nextPositionCandidate = new Vector3(0, 0, 0);
        private Camera _camera;
        private float _blindChaseTime;

        [Networked] private float _blindChaseTimeCount { get; set; }

        public List<EnemyAttackBehaviour> GetEnemyAtackBehaviours (){
            return _enemyAttackBehaviours;
        }

        public void SetEnemyAtackBehaviours(List<EnemyAttackBehaviour> setList) { 
            _enemyAttackBehaviours = setList;
        }


        /// <summary>
        /// 初期化処理、外部からアクセスする
        /// </summary>
        public void Init(EnemyVisibilityMap setVisivilityMap) {
            _myVisivilityMap = setVisivilityMap;
            _horror = _enemyStatus.Horror;
            _audiomaterPower = _enemyStatus.AudiometerPower;
            _blindChaseTime = _enemyStatus.BrindCheseTime;

            _player = GameObject.FindWithTag("Player");
            if (_player == null) { Debug.LogWarning("プレイヤーが認識できません"); }
            _playerStatus = _player.GetComponent<PlayerStatus>();
            if (_playerStatus == null) { Debug.LogWarning("プレイヤーステータスが認識できません"); }

            _camera = GameObject.Find("Main Camera").GetComponent<Camera>();

            _enemyStatus.OnEnemyStateChange.Subscribe(state => 
            {
                if (state ==　EnemyState.Discover) 
                { 
                    ReturnignForDiscover(this.GetCancellationTokenOnDestroy()).Forget();
                    _myVisivilityMap.SetEveryGridWatchNum(50);//リセット
                } 
            }).AddTo(this);


            /*
             #####################射程順に並び変える
             */
            _atackRange = _enemyAttackBehaviours[_enemyAttackBehaviours.Count -1].GetRange();


            if(_debugMode)_playerStatus.OnEnemyAttackedMe.Subscribe(_ => Debug.Log("攻撃された"));

        }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            float _playerDistance;

            if (_enemyStatus.State == EnemyState.Chase || _enemyStatus.State == EnemyState.Attack || _enemyStatus.State == EnemyState.Discover)//メモ、Discover中は移動先の変更などはするが、Stateの変更や攻撃はしない。移動速度（Discover中は移動しない）についてはEnemyMoveが行ってくれる
            { //追跡状態または攻撃状態の場合               
                //定期的に状態を変更
                _checkTimeCount += Runner.DeltaTime;
                if (_checkTimeCount > _checkRate)
                {
                    _playerDistance = Vector3.Magnitude(this.transform.position - _player.transform.position);
                    _checkTimeCount = 0;
                    if (CheckCanSeeThePlayer()) //敵が見えるルートがあるかかどうかを確認する
                    {
                        //こちらが深淵を除くときry
                        SanCheck();
                        if (_playerDistance < _visivilityRange)//見える距離にいるかどうか
                        {
                            _myVisivilityMap.ChangeEveryGridWatchNum(1, true);
                            _myVisivilityMap.SetGridWatchNum(_player.transform.position, 0);
                            _blindChaseTimeCount = 0;//見えたのであきらめるまでのカウントはリセット
                                                     //移動目標をプレイヤーの座標にする
                            _myEnemyMove.SetMovePosition(_player.transform.position);

                            if (_enemyStatus.State != EnemyState.Discover) {//発見動作中は攻撃したりしない
                                if (_atackRange > _playerDistance && _enemyStatus.StiffnessTime <= 0)
                                { //攻撃可能であれば
                                    _enemyStatus.SetEnemyState(EnemyState.Attack);
                                    if (HasStateAuthority) 
                                    {//攻撃のスクリプトを叩くのはホストのみ
                                        _massSUM = 0;
                                        for (int i = 0; i < _enemyAttackBehaviours.Count; i++)
                                        {
                                            if (_enemyAttackBehaviours[i].GetRange() > _playerDistance)
                                            {
                                                _massSUM += _enemyAttackBehaviours[i].GetMass();
                                            }
                                        }
                                        float _pickNum = UnityEngine.Random.RandomRange(0f, _massSUM);
                                        for (int i = 0; i < _enemyAttackBehaviours.Count; i++)
                                        {
                                            _massSUM -= _enemyAttackBehaviours[i].GetMass();
                                            if (_massSUM <= _pickNum)
                                            {
                                                _enemyAttackBehaviours[i].Behaviour(_playerStatus);
                                                _enemyStatus.ChangeStiffnessTime(_enemyAttackBehaviours[i].GetStiffness());
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {//攻撃できないなら追いかける
                                    _enemyStatus.SetEnemyState(EnemyState.Chase);
                                    _massSUM = 0;

                                }
                            }                            
                        }
                    }
                    else
                    { //敵が見えないならせめてなんとかいそうなエリアへ行こうとする
                        _blindChaseTimeCount += _checkRate;
                        if (_blindChaseTimeCount > _blindChaseTime)

                        { //あきらめるかどうかの判定
                            if (_enemyStatus.State != EnemyState.Discover) 
                            { 
                                _enemyStatus.SetEnemyState(EnemyState.Searching);//追っかけるのあきらめた
                            }                                
                        }
                        else
                        { //まだあきらめない場合、近距離に特化したのSearchを行う
                            if (_enemyStatus.State != EnemyState.Discover)
                            {
                                _enemyStatus.SetEnemyState(EnemyState.Chase);
                            }
                            if (_enemyStatus.ReactToLight && _myVisivilityMap.RightCheck(this.transform.position, _player.transform.position, _visivilityRange, _playerStatus.nowPlayerLightRange, ref nextPositionCandidate))//&&は左から評価される事に注意
                            { //光が見えるか調べる
                                if (_debugMode) Debug.Log("追跡中光が見えた");
                                _myEnemyMove.SetMovePosition(nextPositionCandidate);
                            }
                            else if (_myEnemyMove._endMove)
                            { //移動が終了している場合
                                if (_playerStatus.nowPlayerActionState == PlayerActionState.Sneak && Mathf.Pow((float)(_playerStatus.nowPlayerSneakVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) - (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)//忍音が聞こえるかどうか
                                {

                                    if (_debugMode) Debug.Log("追跡中忍ぶ音が聞こえる");
                                    _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                                    _myEnemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));

                                }
                                else if (_playerStatus.nowPlayerActionState == PlayerActionState.Walk && Mathf.Pow((float)(_playerStatus.nowPlayerWalkVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) - (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)//歩く音が聞こえるかどうか
                                {
                                    if (_debugMode) Debug.Log("追跡中歩く音が聞こえる");
                                    _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                                    _myEnemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));

                                }
                                else if (_playerStatus.nowPlayerActionState == PlayerActionState.Dash && Mathf.Pow((float)(_playerStatus.nowPlayerRunVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) - (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)//走る音が聞こえるかどうか
                                {

                                    if (_debugMode) Debug.Log("追跡中走る音が聞こえる");
                                    _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                                    _myEnemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));

                                }
                                else
                                {
                                    //なんの痕跡も見つからなかった場合普通に巡回する
                                    _myVisivilityMap.CheckVisivility(this.transform.position, _visivilityRange);
                                    if (_myEnemyMove._endMove)//移動が終わっている場合
                                    {
                                        _myVisivilityMap.ChangeGridWatchNum(_myEnemyMove.GetMovePosition(), 1, true);
                                        //あらたな移動先を取得するメソッドを書き込む
                                        _myEnemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
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

        protected virtual bool CheckCanSeeThePlayer()
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
                        _playerStatus.ChangeSanValue(_horror, ChangeValueMode.Damage);




                    }
                }
            }
        }

        /// <summary>
        /// Discoverにかかる時間だけ待って、チェイスに移行する
        /// </summary>
        protected virtual async Cysharp.Threading.Tasks.UniTaskVoid ReturnignForDiscover(CancellationToken ct) {
            await Task.Delay(_enemyStatus.DiscoverTime, ct);
            _enemyStatus.SetEnemyState(EnemyState.Chase);
        }
    }
}

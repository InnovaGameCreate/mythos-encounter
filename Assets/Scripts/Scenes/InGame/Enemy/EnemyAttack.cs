using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// 敵キャラの攻撃を管理する。追跡状態と攻撃状態はこのスクリプトが作動する
    /// </summary>
    public class EnemyAttack : MonoBehaviour
    {
        [Header("このスクリプトを制御する変数")]
        [SerializeField][Tooltip("何秒ごとに視界の状態をチェックするか")] private float _checkRate;
        [SerializeField][Tooltip("戦闘時の視界の広さ")] private float _visivilityRange;//仕様上視界範囲は全て同一？じゃなかったらこれはEnemyStatusに送り込むよ
        [SerializeField] [Tooltip("見失ったとしてもどれだけの時間頑張って探そうとするかどうか")]private float _brindCheseTime;
        [SerializeField][Tooltip("デバッグするかどうか")] private bool _debugMode;



        private int _horro;
        private int _atackPower;

        [Header("Horror,AtackPowerを除く攻撃性能")]
        [SerializeField][Tooltip("攻撃の間隔")] private float _attackTime;
        [SerializeField][Tooltip("近接攻撃の射程")] private float _atackRange;
        [SerializeField][Tooltip("遠隔攻撃が可能かどうか")] private bool canShot;
        [SerializeField][Tooltip("遠隔攻撃の攻撃力")] private int _shotPower;
        [SerializeField][Tooltip("遠隔攻撃の間隔")] private float _shotTime;
        [SerializeField][Tooltip("遠隔攻撃の射程")] private float _shotRange;
        [SerializeField][Tooltip("弾丸")] private GameObject _ballet;







        [Header("自身についているメソッド")]
        [SerializeField] private EnemyStatus _enemyStatus;
        [SerializeField] private EnemySearch _enemySearch;
        [SerializeField] private EnemyMove _enemyMove;

        //##########内部で使う変数##########
        private GameObject _player;
        private PlayerStatus _playerStatus;
        private float _atackTimeCount;
        private float _shotTimeCount;
        private float _audiomaterPower;//聞く力
        private float _checkTimeCount;//前回チェックしてからの時間を計測
        private float _brindCheseTimeCount;
        private EnemyVisibilityMap _myVisivilityMap;
        private EnemyState _lastEnemyState = EnemyState.None;
        Vector3 nextPositionCandidate = new Vector3(0, 0, 0);

        /// <summary>
        /// 初期あk処理外部からアクセスすⓈる
        /// </summary>
        public void Init(EnemyVisibilityMap setVisivilityMap) {
            _myVisivilityMap = setVisivilityMap;
            _horro = _enemyStatus.ReturnAtackPower;
            _atackPower = _enemyStatus.ReturnAtackPower;
            _audiomaterPower = _enemyStatus.ReturnAudiomaterPower;

            _player = GameObject.Find("Player");
            if (_player == null) { Debug.LogWarning("プレイヤーが認識できません"); }
            _playerStatus = _player.GetComponent<PlayerStatus>();
            if (_playerStatus == null) { Debug.LogWarning("プレイヤーステータスが認識できません"); }

            _enemyStatus.OnEnemyStateChange.Subscribe(state => 
            {
                if ((state == EnemyState.Chese || state == EnemyState.Atack) && !((_lastEnemyState == EnemyState.Chese || _lastEnemyState == EnemyState.Atack))) 
                { 
                    _lastEnemyState = state;
                    _myVisivilityMap.SetEveryGridWatchNum(50);
                } 
            }).AddTo(this);

        }

        // Update is called once per frame
        protected virtual void FixedUpdate()
        {
            float _playerDistance;
            if (_enemyStatus.ReturnEnemyState == EnemyState.Chese || _enemyStatus.ReturnEnemyState == EnemyState.Atack)
            { //追跡状態または攻撃状態の場合

                //いろいろ数える
                if (_atackTimeCount < _attackTime) { _atackTimeCount += Time.deltaTime; }
                if (_shotTimeCount < _shotTime) { _shotTimeCount += Time.deltaTime; }

                //定期的に状態を変更
                _checkTimeCount += Time.deltaTime;
                if (_checkTimeCount > _checkRate)
                {
                    _playerDistance = Vector3.Magnitude(this.transform.position - _player.transform.position);
                    _checkTimeCount = 0;
                    if (CheckCanPlayerVisivlleCheck()) //敵が見えるかどうかを確認する
                    {
                        _myVisivilityMap.ChangeEveryGridWatchNum(1,true);
                        _myVisivilityMap.SetGridWatchNum(_player.transform.position,0);
                        _brindCheseTimeCount = 0;//見えたのであきらめるまでのカウントはリセット
                        //移動目標をプレイヤーの座標にする
                        _enemyMove.SetMovePosition(_player.transform.position);
                        if (_playerDistance < _atackRange)//近接攻撃の射程内か確認する
                        { //近接攻撃をする
                            _enemyStatus.SetEnemyState(EnemyState.Atack);
                            if (_atackTimeCount > _attackTime) {
                                _atackTimeCount = 0;
                                _shotTimeCount = 0;//遠隔から近接の距離に入った瞬間2連続で攻撃が行われないために両方のカウントを0にしている。
                                if(_debugMode)Debug.LogWarning("ここで近接攻撃！");
                            }
                            

                        }
                        else if (_playerDistance < _shotRange) //遠隔攻撃の射程内か確認する
                        { //遠隔攻撃をする
                            _enemyStatus.SetEnemyState(EnemyState.Atack);
                            if (_shotTimeCount > _shotTime) {
                                _atackTimeCount = 0;
                                _shotTimeCount = 0;
                                if (_debugMode) Debug.LogWarning("ここで遠隔攻撃！");
                            }


                        }
                        else
                        {//攻撃できないなら追いかける
                            _enemyStatus.SetEnemyState(EnemyState.Chese);
                        }
                    }
                    else
                    { //敵が見えないならせめてなんとかいそうなエリアへ行こうとする
                        _brindCheseTimeCount += _checkRate;
                        if (_brindCheseTimeCount > _brindCheseTime) { //あきらめるかどうかの判定
                            _enemyStatus.SetEnemyState(EnemyState.Searching);//追っかけるのあきらめた
                        }
                        else{ //まだあきらめない場合、近距離に特化したのSearchを行う
                            _enemyStatus.SetEnemyState(EnemyState.Chese);

                            if (_enemyStatus.ReturnReactToLight && _myVisivilityMap.RightCheck(this.transform.position, _player.transform.position, _visivilityRange, _playerStatus.nowPlayerLightRange, ref nextPositionCandidate))//&&は左から評価される事に注意
                            { //光が見えるか調べる
                                if (_debugMode) Debug.Log("追跡中光が見えた");
                                _enemyStatus.SetEnemyState(EnemyState.Searching);
                                _enemyMove.SetMovePosition(nextPositionCandidate);
                            }
                            else if (_enemyMove.endMove)
                            { //移動が終了している場合
                                if (_playerStatus.nowPlayerActionState == PlayerActionState.Sneak && Mathf.Pow((float)(_playerStatus.nowPlayerSneakVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) - (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)//忍音が聞こえるかどうか
                                {

                                    if (_debugMode) Debug.Log("追跡中忍ぶ音が聞こえる");
                                    _enemyStatus.SetEnemyState(EnemyState.Searching);
                                    _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                                    _enemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));

                                }
                                else if (_playerStatus.nowPlayerActionState == PlayerActionState.Walk && Mathf.Pow((float)(_playerStatus.nowPlayerWalkVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) - (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)//歩く音が聞こえるかどうか
                                {
                                    if (_debugMode) Debug.Log("追跡中歩く音が聞こえる");
                                    _enemyStatus.SetEnemyState(EnemyState.Searching);
                                    _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                                    _enemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));

                                }
                                else if (_playerStatus.nowPlayerActionState == PlayerActionState.Dash && Mathf.Pow((float)(_playerStatus.nowPlayerRunVolume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) - (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)//走る音が聞こえるかどうか
                                {

                                    if (_debugMode) Debug.Log("追跡中走る音が聞こえる");
                                    _enemyStatus.SetEnemyState(EnemyState.Searching);
                                    _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                                    _enemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));

                                }
                                else
                                {
                                    //なんの痕跡も見つからなかった場合普通に巡回する
                                    _myVisivilityMap.CheckVisivility(this.transform.position, _visivilityRange);
                                    if (_enemyMove.endMove)//移動が終わっている場合
                                    {
                                        //痕跡のあった場所まで来たが何もいなかった場合ここが実行されるのでStatusを書き換える
                                        _enemyStatus.SetEnemyState(EnemyState.Patorolling);
                                        //あらたな移動先を取得するメソッドを書き込む
                                        _enemyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                                    }
                                }
                            }
                        }
                    }
                }           
            }
        }

        protected bool CheckCanPlayerVisivlleCheck()
        {
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
            else
            {
                return false;
            }
        }
    }
}

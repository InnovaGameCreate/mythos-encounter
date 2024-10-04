using Fusion;
using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public class MiGoSearch : EnemySearch
    {
        [Networked] private bool _uniqueChase { get; set; }//特殊なチェイスをするかどうか

        public override void FixedUpdateNetwork()
        {
            if (_uniqueChase) {
                if (_myVisivilityMap != null)//索敵の準備ができていない場合
                {
                    Debug.LogError("マップ情報がありません、_myVisivilityMapを作成してください");
                    return;
                }
                if (_enemyStatus.State == EnemyState.Patrolling || _enemyStatus.State == EnemyState.Searching)
                { //巡回状態または捜索状態の場合
                    //定期的に視界情報を調べる
                    _checkTimeCount += Runner.DeltaTime;
                    if (_checkTimeCount > _checkRate)
                    {
                        _myEneyMove.SetMovePosition(_player.transform.position);
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
                        if (CheckCanSeeThePlayer())
                        { //プレイヤーの姿が見えるか調べる
                            _myEneyMove.SetMovePosition(_player.transform.position);
                            _enemyStatus.SetEnemyState(EnemyState.Chase);
                        }
                        else
                        {
                            if (_enemyStatus.ReactToLight && _myVisivilityMap.RightCheck(this.transform.position, _player.transform.position, _visivilityRange, _playerStatus.nowPlayerLightRange, ref nextPositionCandidate))//&&は左から評価される事に注意
                            { //光が見えるか調べる
                                if (_debugMode) Debug.Log("光が見えた");
                                _enemyStatus.SetEnemyState(EnemyState.Searching);
                            }
                            else if (Mathf.Pow((float)(valume * _audiomaterPower * 0.01f), 2f) - (Mathf.Pow(transform.position.x - _player.transform.position.x, 2) + (Mathf.Pow(transform.position.y - _player.transform.position.y, 2))) > 0)
                            { //プレイヤーの騒音が聞こえるか調べる

                                _enemyStatus.SetEnemyState(EnemyState.Searching);
                                _myVisivilityMap.HearingSound(_player.transform.position, 15, true);
                            }
                            else
                            {
                               
                                //なんの痕跡も見つからなかった場合普通に巡回する
                                _myVisivilityMap.CheckVisivility(this.transform.position, _visivilityRange);
                                if (_myEneyMove._endMove)//移動が終わっている場合
                                {
                                    _myVisivilityMap.ChangeGridWatchNum(_myEneyMove.GetMovePosition(), 1, true);
                                    //痕跡のあった場所まで来たが何もいなかった場合ここが実行されるのでStatusを書き換える
                                    _enemyStatus.SetEnemyState(EnemyState.Patrolling);
                                }
                            }
                        }
                    }
                }
            }
            else {
                base.FixedUpdateNetwork();
            }

        }

        public void ChangeUniqueChase(bool uniqueChase) { 
            _uniqueChase = uniqueChase;
        }

    }
}

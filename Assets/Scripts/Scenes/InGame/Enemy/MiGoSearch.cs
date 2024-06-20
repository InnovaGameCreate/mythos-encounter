using Scenes.Ingame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public class MiGoSearch : EnemySearch
    {
        [SerializeField]private bool _uniqueChase;//特殊なチェイスをするかどうか

        protected override void FixedUpdate()
        {
            if (_uniqueChase) {
                if (_myVisivilityMap != null)//索敵の準備ができていない場合
                {
                    Debug.LogError("マップ情報がありません、_myVisivilityMapを作成してください");
                    return;
                }
                if (_enemyStatus.ReturnEnemyState == EnemyState.Patrolling || _enemyStatus.ReturnEnemyState == EnemyState.Searching)
                { //巡回状態または捜索状態の場合
                    //定期的に視界情報を調べる
                    _checkTimeCount += Time.deltaTime;
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
                        if (CheckCanPlayerVisivlleCheck())
                        { //プレイヤーの姿が見えるか調べる
                            _myEneyMove.SetMovePosition(_player.transform.position);
                            _enemyStatus.SetEnemyState(EnemyState.Chase);
                        }
                        else
                        {
                            if (_enemyStatus.ReturnReactToLight && _myVisivilityMap.RightCheck(this.transform.position, _player.transform.position, _visivilityRange, _playerStatus.nowPlayerLightRange, ref nextPositionCandidate))//&&は左から評価される事に注意
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
                                if (_myEneyMove.endMove)//移動が終わっている場合
                                {
                                    //痕跡のあった場所まで来たが何もいなかった場合ここが実行されるのでStatusを書き換える
                                    _enemyStatus.SetEnemyState(EnemyState.Patrolling);
                                }
                            }
                        }
                    }
                }
            }
            else {
                base.FixedUpdate();
            }

        }

        public void ChangeUniqueChase(bool uniqueChase) { 
            _uniqueChase = uniqueChase;
        }

    }
}

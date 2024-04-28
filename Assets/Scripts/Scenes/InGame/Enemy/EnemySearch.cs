using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Enemy
{
    public class EnemySearch : MonoBehaviour//パトロールする。プレイヤーの痕跡を探す。巡回状態と索敵状態の動きを決定し、追跡と攻撃状態への移行を行う。
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
        private float _visivilityRange;//あとでこれはステータスに持たせるよ

        //索敵行動のクラスです
        // Start is called before the first frame update
        void Start()
        {
        }

        void FixedUpdate()
        {
            if (_myVisivilityMap != null) {
                Debug.LogError("マップ情報がありません、_myVisivilityMapを作成してください");
                return; }//索敵の準備ができていない場合
            if (true) { //巡回状態の場合
                if (_myEneyMove.endMove)
                { //移動が終わっている場合新たな移動先を取得する

                    //移動先を取得するメソッドを書き込む
                    _myEneyMove.SetMovePosition(_myVisivilityMap.GetNextNearWatchPosition(this.transform.position));
                }
                //見てきたエリアを記録してゆく
                _checkTimeCount += Time.deltaTime;
                if (_checkTimeCount > _checkRate) {
                    _checkTimeCount = 0;
                    //ここに現在位置と見た情報の書き込みを依頼するメソッドを書く
                    _myVisivilityMap.CheckVisivility(this.transform.position,_visivilityRange);

                }
            
            
            
            
            } 
        }

        public void SetVisivilityMap(EnemyVisibilityMap setVisivilityMap) 
        { 
            _myVisivilityMap = setVisivilityMap;
        }
    }
}

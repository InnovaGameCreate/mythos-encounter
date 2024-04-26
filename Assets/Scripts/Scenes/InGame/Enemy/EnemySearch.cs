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
        [SerializeField]
        private bool _debugMode;

        //索敵行動のクラスです
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_myVisivilityMap != null) {
                Debug.LogError("マップ情報がありません、_myVisivilityMapを作成してください");
                return; }//索敵の準備ができていない場合


        }

        public void SetVisivilityMap(EnemyVisibilityMap setVisivilityMap) 
        { 
            _myVisivilityMap = setVisivilityMap;
        }
    }
}

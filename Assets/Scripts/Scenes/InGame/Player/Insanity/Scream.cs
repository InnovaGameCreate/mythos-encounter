using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.Ingame.Player
{
    /// <summary>
    /// 発狂の種類の1つ
    /// 1.プレイヤーは5秒間声を上げて発狂する。このとき頭を抱えるアニメーションに移行
    /// 2.一切の行動が不可
    /// </summary>
    public class Scream : MonoBehaviour, IInsanity
    {
        private PlayerSoundManager _myPlayerSoundManager;
        private PlayerMove _myPlayerMove;
        private PlayerItem _myPlayerItem;

        private bool _isSafetyBool = false;//もし叫んでいるときに手違いでSAN値が回復し、このスクリプトが破壊されたときに詰まない為のBool
        private bool _isFirst = true;//初めて呼び出されたか

        public void Setup()
        {
            _myPlayerSoundManager = GetComponent<PlayerSoundManager>();
            _myPlayerMove = GetComponent<PlayerMove>();
            _myPlayerItem = GetComponent<PlayerItem>();
        }

        public void Active()
        {
            if (_isFirst)
            {
                Setup();
                _isFirst = false;
            }

            //発狂中は行動不能になる
            _myPlayerMove.MoveControl(false);
            _myPlayerItem.ChangeCanUseItem(false);
            _myPlayerItem.ChangeCanChangeBringItem(false);
            _isSafetyBool = true;

            //叫び声を上げる
            _myPlayerSoundManager.SetScreamClip("Male");

            //頭を抱えるアニメーションに遷移
            //今後実装

            StartCoroutine(FinishScream());
        }

        public void Hide()
        {
            if (_isSafetyBool)
            {
                _myPlayerMove.MoveControl(true);
                _myPlayerItem.ChangeCanUseItem(true);
                _myPlayerItem.ChangeCanChangeBringItem(true);
            }

        }

        /// <summary>
        /// 叫ぶことが終われば行動不能を解除
        /// </summary>
        /// <returns></returns>
        private IEnumerator FinishScream()
        {
            yield return new WaitForSeconds(5.0f);
            _myPlayerMove.MoveControl(true);
            _myPlayerItem.ChangeCanUseItem(true);
            _myPlayerItem.ChangeCanChangeBringItem(true);

            _isSafetyBool = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;


namespace Scenes.Ingame.Player
{
    /// <summary>
    /// カメラ（プレイヤーの視点）を変更するクラス
    /// playerについているカメラにアタッチする
    /// </summary>
    public class CameraMove : MonoBehaviour
    {
        //カメラの揺れに関する設定
        [SerializeField] private float _offSet;//カメラを揺らす幅
        private Vector3 _cameraPositionDefault;
        private Sequence sequence;
        //コンポーネントの取得
        //[SerializeField] private PlayerStatus _myPlayerState; 

        private void Start()
        {
            //視線の初期の場所を記録
            _cameraPositionDefault = this.transform.position;
        }

        /// <summary>
        /// クリップの時間の長さを基に、カメラ移動の周期を決定する。
        /// 注意：足音のクリップは基本足音２回のセットになっている。１回分にするにはclipTime / 2を使うこと
        /// </summary>
        /// <param name="clipTime"></param>
        public void ChangeViewPoint(float clipTime)
        {
            //動かない状態の時は引数に0を入れている
            //待機状態ではほんの少しだけゆっくりと視点が変わる
            if (clipTime == 0)
            {
                var cycle = 2.0f;//視点移動の周期

                sequence.Kill();
                sequence = DOTween.Sequence();
                sequence.Append(this.transform.DOLocalMove(new Vector3(0, _cameraPositionDefault.y - _offSet / 2, 0), cycle / 2));
                sequence.Append(this.transform.DOLocalMove(new Vector3(0, _cameraPositionDefault.y, 0), cycle / 2));
                sequence.Play().SetLoops(-1, LoopType.Yoyo);

                return;
            }
            

            //今回設定する挙動を作成
            sequence.Kill();
            sequence = DOTween.Sequence();
            sequence.Append(this.transform.DOLocalMove(new Vector3(0, _cameraPositionDefault.y - _offSet, 0), clipTime / 2));
            sequence.Append(this.transform.DOLocalMove(new Vector3(0, _cameraPositionDefault.y, 0), clipTime / 2));
            sequence.Play().SetLoops(-1, LoopType.Yoyo);     
        }
    }
}
    

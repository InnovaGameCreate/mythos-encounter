using Scenes.Ingame.Player;
using UnityEngine;
using DG.Tweening;

namespace Scenes.Ingame.Stage
{
    public class StageRack : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private DrawType drawType;
        private enum DrawType
        {
            RightOpen, //右向きにあける
            LeftOpen, //左向きにあける
            DrawOpen, // 引いて開ける
        }

        [SerializeField]
        private bool _initialStateOpen = true;
        private bool _isOpen = false;
        private bool _isAnimation = false;
        private Vector3 R_OPENVALUE = new Vector3(0, -110, 0);
        private Vector3 L_OPENVALUE = new Vector3(0, 110, 0);
        private Vector3 D_OPENVALUE = new Vector3(1, 0, 0);
        private BoxCollider _rackCollider;
        public void Intract(PlayerStatus status)
        {
            if (Input.GetMouseButtonDown(1) && _isAnimation == false)
            {
                _rackCollider.isTrigger = true;
                _isAnimation = true;
                switch (_isOpen)
                {
                    case true:
                        RackClose();
                        _isOpen = false;
                        break;
                    case false:
                        RackOpen();
                        _isOpen = true;
                        break;
                }
            }
        }

        void Awake()
        {
            _rackCollider = GetComponent<BoxCollider>();
            if (_initialStateOpen)
            {
                _rackCollider.isTrigger = false;
                RackOpen();
                _isOpen = true;
            }
        }

        private void AnimationcComplete()
        {
            _rackCollider.isTrigger = false;
            _isAnimation = false;
        }

        private void RackOpen()
        {
            if (drawType == DrawType.RightOpen)
            {
                transform.DORotate(R_OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine);
            }
            else if (drawType == DrawType.LeftOpen)
            {
                transform.DORotate(L_OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine);
            }
            else if (drawType == DrawType.DrawOpen)
            {
                transform.DOMove(0.3f * D_OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine);
            }
            AnimationcComplete();
        }
        private void RackClose()
        {
            if (drawType == DrawType.RightOpen)
            {
                transform.DORotate(-R_OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine);
            }
            if (drawType == DrawType.LeftOpen)
            {
                transform.DORotate(-L_OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine);
            }
            if (drawType == DrawType.DrawOpen)
            {
                transform.DOMove(0.3f * -D_OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine);
            }
            AnimationcComplete();
        }
    }
}


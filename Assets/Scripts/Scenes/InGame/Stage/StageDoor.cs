using Scenes.Ingame.Player;
using UnityEngine;
using DG.Tweening;

namespace Scenes.Ingame.Stage
{
    public class StageDoor : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private bool _initialStateOpen = true;
        private bool _isOpen = false;
        private bool _isAnimation = false;
        private Vector3 OPENVALUE = new Vector3(0, 90, 0);
        private BoxCollider _doorCollider;

        public bool ReturnIsOpen { get { return _isOpen; } }
        public bool ReturnIsAnimation { get { return _isAnimation; } }

        public void Intract(PlayerStatus status)
        {
            if (Input.GetMouseButtonDown(1) && _isAnimation == false)
            {
                _doorCollider.isTrigger = true;
                _isAnimation = true;
                switch (_isOpen)
                {
                    case true:
                        DoorClose();
                        _isOpen = false;
                        break;
                    case false:
                        DoorOpen();
                        _isOpen = true;
                        break;
                }
            }
        }

        void Awake()
        {
            _doorCollider = GetComponent<BoxCollider>();
            if (_initialStateOpen)
            {
                _doorCollider.isTrigger = false;
                QuickDoorOpen();
                _isOpen = true;
            }
        }
        private void DoorOpen()
        {
            transform.DORotate(OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _doorCollider.isTrigger = false;
                _isAnimation = false;
            });
        }
        private void DoorClose()
        {
            transform.DORotate(-OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _doorCollider.isTrigger = false;
                _isAnimation = false;
            });
        }

        private void QuickDoorOpen()
        {
            transform.DORotate(OPENVALUE, 0).SetRelative(true).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _doorCollider.isTrigger = false;
                _isAnimation = false;
            });
        }

        private void QuickDoorClose()
        {
            transform.DORotate(-OPENVALUE, 0).SetRelative(true).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _doorCollider.isTrigger = false;
                _isAnimation = false;
            });
        }

        public void ChangeDoorOpen (bool open){
            if (_isAnimation) {
                Debug.LogWarning("アニメーション中です");
                if (open)
                {
                    if (!_isOpen) {
                        QuickDoorOpen();
                        _isOpen = false;
                    }
                }
                else { 
                    if (_isOpen)
                    {
                        QuickDoorClose();
                        _isOpen = false;
                    }
                }
            }
        }

        /// <summary>
        /// ドアを設定されていた初期状態に戻す
        /// </summary>
        public void ChangeDoorInitial() {
            if (_isAnimation)
            {
                Debug.LogWarning("アニメーション中です");
                if (_initialStateOpen)
                {
                    if (!_isOpen)
                    {
                        QuickDoorOpen();
                        _isOpen = false;
                    }
                }
                else
                {
                    if (_isOpen)
                    {
                        QuickDoorClose();
                        _isOpen = false;
                    }
                }
            }
        }
    }
}
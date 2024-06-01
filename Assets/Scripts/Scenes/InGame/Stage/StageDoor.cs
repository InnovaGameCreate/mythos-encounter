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
                DoorOpen();
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
    }
}
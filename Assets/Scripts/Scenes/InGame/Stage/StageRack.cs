using Scenes.Ingame.Player;
using UnityEngine;
using DG.Tweening;

namespace Scenes.Ingame.Stage
{
    public class StageRack : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private bool _initialStateOpen = true;
        private bool _isOpen = false;
        private bool _isAnimation = false;
        private Vector3 OPENVALUE = new Vector3(0, 110, 0);
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
        private void RackOpen()
        {
            transform.DORotate(OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _rackCollider.isTrigger = false;
                _isAnimation = false;
            });
        }
        private void RackClose()
        {
            transform.DORotate(-OPENVALUE, 1).SetRelative(true).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                _rackCollider.isTrigger = false;
                _isAnimation = false;
            });
        }
    }
}


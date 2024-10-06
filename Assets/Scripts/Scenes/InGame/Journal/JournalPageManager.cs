using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

namespace Scenes.Ingame.Journal
{
    public class JournalPageManager : MonoBehaviour
    {
        PlayerAction _inputs;


        [SerializeField]
        PlayerJournalTest _player;
        
        [SerializeField]
        List<GameObject> _pages = new List<GameObject>();
        int _nowPage = 0;

        void Awake()
        {
            _inputs = new PlayerAction();
            _inputs.UI.ChangePage.performed += OnChangePage;  //�y�[�W�ύX�{�^���������ꂽ�Ƃ��̃R�[���o�b�N��o�^
        }

        void Start()
        {
            _player.OnChangeJournalState
                .Subscribe(x =>
                {
                    if (x == true)
                        OpenJournal();
                    else
                        CloseJournal();
                }).AddTo(this);

            foreach (var element in _pages)
            {
                element.SetActive(false);
            }

            this.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            _inputs.Enable();   //InputAction��L����
        }

        //�W���[�i�����J��
        void OpenJournal()
        {
            this.gameObject.SetActive(true);
            _pages[_nowPage].SetActive(true);   //���݂̃y�[�W��\��
        }

        //�y�[�W�ύX
        void OnChangePage(InputAction.CallbackContext context)
        {
            //if (!this.gameObject.activeSelf) return;

            _pages[_nowPage].SetActive(false);

            if (_nowPage < _pages.Count - 1)    //�ŏI�y�[�W�̏ꍇ�A�ŏ��̃y�[�W�ɖ߂�
                _nowPage++;
            else if (_nowPage == _pages.Count - 1)
                _nowPage = 0;

            _pages[_nowPage].SetActive(true);
        }

        void OnDestroy()
        {
            _inputs?.Dispose();    //IDisposable�Ȃ��߁A�I�u�W�F�N�g�j�󎞂�Dispose
        }

        //�W���[�i�������
        void CloseJournal()
        {
            _pages[_nowPage].SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}


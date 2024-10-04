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
            _inputs.UI.ChangePage.performed += OnChangePage;  //ページ変更ボタンが押されたときのコールバックを登録
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
            _inputs.Enable();   //InputActionを有効化
        }

        //ジャーナルを開く
        void OpenJournal()
        {
            this.gameObject.SetActive(true);
            _pages[_nowPage].SetActive(true);   //現在のページを表示
        }

        //ページ変更
        void OnChangePage(InputAction.CallbackContext context)
        {
            //if (!this.gameObject.activeSelf) return;

            _pages[_nowPage].SetActive(false);

            if (_nowPage < _pages.Count - 1)    //最終ページの場合、最初のページに戻る
                _nowPage++;
            else if (_nowPage == _pages.Count - 1)
                _nowPage = 0;

            _pages[_nowPage].SetActive(true);
        }

        void OnDestroy()
        {
            _inputs?.Dispose();    //IDisposableなため、オブジェクト破壊時にDispose
        }

        //ジャーナルを閉じる
        void CloseJournal()
        {
            _pages[_nowPage].SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}


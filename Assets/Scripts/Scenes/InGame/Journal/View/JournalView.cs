using Scenes.Ingame.Journal;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;

public class JournalView : ViewBase
{
    [SerializeField] private Animator _animator;

    [SerializeField] private ViewBase _progressView;
    [SerializeField] private ViewBase _featureView;
    [SerializeField] private ViewBase _compatibilityView;
    [SerializeField] private ViewBase _enemyView;
    [SerializeField] private ViewBase _itemView;
    [SerializeField] private ViewBase _spellView;

    private Subject<PageType> _jornalTagClick = new Subject<PageType>();
    public IObservable<PageType> OnJornalTagClick { get { return _jornalTagClick; } }
    private ViewBase _pastView;

    public override void Init()
    {
        _progressView.Init();
        _featureView.Init();
        _compatibilityView.Init();
        _enemyView.Init();
        _itemView.Init();
        _spellView.Init();
    }
    public void PageChange(PageType pageType)
    {
        _animator.SetTrigger("isNext");
        _pastView?.Close();
        switch (pageType)
        {
            case PageType.Progress:
                _progressView.Open();
                _pastView = _progressView;
                break;
            case PageType.Feature:
                _featureView.Open();
                _pastView = _featureView;
                break;
            case PageType.Compatibility:
                _compatibilityView.Open();
                _pastView = _compatibilityView;
                break;
                case PageType.Enemy:
                _enemyView.Open();
                _pastView = _enemyView;
                break;
            case PageType.Item:
                _itemView.Open();
                _pastView = _itemView;
                break;
            case PageType.Spell:
                _spellView.Open();
                _pastView = _spellView;
                break;
        }
    }
    public override void Open()
    {
        _animator.SetTrigger("isOpen");
    }
    public override void Close()
    {
        _animator.SetTrigger("isClose");
    }
}

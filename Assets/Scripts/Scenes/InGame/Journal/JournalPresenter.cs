using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JurnalState
{
    Feature,
    Spell,
    EnemyInfo,
    Item,
    Progress,
    Setting
}
public class JournalPresenter : MonoBehaviour
{
    [SerializeField]
    List<GameObject> _pages = new List<GameObject>();

    int _nowPageNumber = 0;

    void Start()
    {
        for(int i = 0; i < _pages.Count; i++)   //�ŏ��̃y�[�W�ȊO��\��
        {
            if (i != _nowPageNumber)
                _pages[i].SetActive(false);
        }


    }
}

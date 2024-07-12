using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scenes.Ingame.Manager;

using UniRx;
public class GameContentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI contentFlee;
    [SerializeField] private TextMeshProUGUI contentColl;
    [SerializeField] private TextMeshProUGUI contentFind;
    [SerializeField] private TextMeshProUGUI contentEsc;
    IngameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = IngameManager.Instance;
        contentFlee.text = "�G�_�b�������瓦���悤";
        contentColl.text = "�E�o�A�C�e�����W�߂悤";
        contentFind.text = "�E�o�n�_��T����";
        contentEsc.text = "�E�o�n�_����E�o���悤";

        //�E�o�A�C�e�����W�܂������������
        manager.OnOpenEscapePointEvent.Subscribe(_ =>
        {
            contentColl.text = "<s>�E�o�A�C�e�����W�߂悤</s>";
        }).AddTo(this);

    }
}

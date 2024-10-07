using Scenes.Ingame.Enemy.Trace;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellsView : MonoBehaviour
{
    [SerializeField] private Image[] _cells;
    [SerializeField] private Sprite _checkCell;
    [SerializeField] private TextMeshProUGUI _name;
    public void Init(TraceType[] traces,string name)
    {
        Debug.Log($"CellsView.size {traces.Length}");
        _name.text = name;
        for (int i = 0; i < _cells.Length; i++)
        {
            if (traces.Any(t => (int)t == i))
            {
                _cells[i].color = Color.white;
            }
            else
            {
                _cells[i].sprite = _checkCell;
            }
        }
    }
}

using Scenes.Ingame.Enemy.Trace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridView : MonoBehaviour
{
    [SerializeField] private CellsView cell;
    private List<CellsView> celss = new List<CellsView>();
    public void Init(List<EnemyDataStruct> enemyData)
    {
        foreach (EnemyDataStruct data in enemyData)
        {
            var gridCell = Instantiate(cell,this.transform);
            gridCell.Init(data.Feature, data.Name);
            celss.Add(gridCell);
        }
    }
}

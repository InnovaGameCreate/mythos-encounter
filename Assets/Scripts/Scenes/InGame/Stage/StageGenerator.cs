using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("intでステージの縦横のサイズ")]
    private Vector2 _stageSize;
    private List<Vector2> candidatePosition = new List<Vector2>();
    private int[,] _stageGenerateData;
    private int roomId = 0;
    void Start()
    {
        _stageGenerateData = new int[(int)_stageSize.x, (int)_stageSize.y];
        Debug.Log($"StageSize => x = {_stageGenerateData.GetLength(0)},y = {_stageGenerateData.GetLength(1)}, total = {_stageGenerateData.Length}");
        InitialSet();
        //DebugStageData();
        TEST();
    }
    private void TEST()
    {
        RandomFullSpaceRoomPlot(5, 3, 2);
        DebugStageData();
    }
    private void InitialSet()
    {
        for (int y = 0; y < _stageGenerateData.GetLength(1); y++)
        {
            for (int x = 0; x < _stageGenerateData.GetLength(0); x++)
            {
                _stageGenerateData[x, y] = 0;
            }
        }
    }
    private void DebugStageData()
    {
        string printData = "\n";
        for (int y = 0; y < _stageGenerateData.GetLength(1); y++)
        {
            for (int x = 0; x < _stageGenerateData.GetLength(0); x++)
            {
                printData += $"[{_stageGenerateData[x, y]}]";
            }
            printData += "\n";
        }
        Debug.Log(printData);
    }
    private void RandomFullSpaceRoomPlot(int smallRoom = 0, int mediumRoom = 0, int largeRoom = 0)
    {
        candidatePositionSet(3);
        Vector2 roomPosition = Vector2.zero;
        Vector2 smallRoomSize = new Vector2(2, 2);
        Vector2 mediumRoomSize = new Vector2(3, 3);
        Vector2 largeRoomSize = new Vector2(4, 4);
        while (true)
        {
            int roomPositionIndex = Random.Range(0, candidatePosition.Count);
            roomPosition = candidatePosition[roomPositionIndex];
            if (largeRoom > 0)
            {
                RoomPlotId(largeRoomSize, roomPosition);
                largeRoom--;
                if (largeRoom > 0)
                {
                    candidatePositionSet(3);
                    if (candidatePosition.Count <= 0)
                    {
                        largeRoom = 0;
                        candidatePositionSet(2);
                    }
                }
                else
                {
                    candidatePositionSet(2);
                }
            }
            else if (mediumRoom > 0)
            {
                RoomPlotId(mediumRoomSize, roomPosition);
                mediumRoom--;
                if (mediumRoom > 0)
                {
                    candidatePositionSet(2);
                    if (candidatePosition.Count <= 0)
                    {
                        mediumRoom = 0;
                        candidatePositionSet(1);
                    }
                }
                else
                {
                    candidatePositionSet(1);
                }
            }
            else if (smallRoom > 0)
            {
                RoomPlotId(smallRoomSize, roomPosition);
                smallRoom--;
                candidatePositionSet(1);
                if (candidatePosition.Count <= 0)
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// データ上に部屋のデータを登録する
    /// </summary>
    /// <param name="plotRoomSize">ルームの大きさ</param>
    /// <param name="plotPosition">ルームの設定位置</param>
    private void RoomPlotId(Vector2 plotRoomSize, Vector2 plotPosition)
    {
        roomId++;
        int plotX = (int)plotPosition.x;
        int plotY = (int)plotPosition.y;
        for (int y = 0; y < plotRoomSize.y; y++)
        {
            for (int x = 0; x < plotRoomSize.x; x++)
            {
                _stageGenerateData[plotX + x, plotY + y] = roomId;
            }
        }
    }

    /// <summary>
    /// ルームを配置可能な座標のリストを作成する
    /// </summary>
    private void candidatePositionSet(int offsetValue = 1)
    {
        candidatePosition.Clear();
        Vector2 setPosition = Vector2.zero;
        for (int y = 0; y < _stageSize.y - offsetValue; y++)
        {
            for (int x = 0; x < _stageSize.x - offsetValue; x++)
            {
                if (_stageGenerateData[x, y] == 0 &&
                    _stageGenerateData[x, y + offsetValue] == 0 &&
                    _stageGenerateData[x + offsetValue, y] == 0 &&
                    _stageGenerateData[x + offsetValue, y + offsetValue] == 0)
                {
                    setPosition.x = x;
                    setPosition.y = y;
                    candidatePosition.Add(setPosition);
                }
            }
        }
        //if (candidatePosition.Count > 0) Debug.Log($"candidatePosition.Count = {candidatePosition.Count} : offset value is {offsetValue}: farst value is {candidatePosition[0]}");
    }
}

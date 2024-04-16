using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scenes.Ingame.Stage
{
    /// <summary>
    /// 動作説明
    /// １._stageSizeに設定されたサイズのステージデータを生成（配列の_stageGenerateDataで管理）
    /// ２.RandomFullSpaceRoomPlot関数を使い大きい部屋から順に_stageGenerateDat内に部屋のデータを生成。この時生成する部屋は4x4,3x3,2x2の大きさ
    /// ３.RommShaping関数を使い、孤立して空いている隙間を埋めるように部屋を拡張。
    /// ４.GenerateAisle関数を使い、通路の作成。現在は縦横１つずつ作成している
    /// </summary>
    public class StageGenerator : MonoBehaviour
    {
        [SerializeField, Tooltip("intでステージの縦横のサイズ")]
        private Vector2 _stageSize;
        private List<Vector2> candidatePosition = new List<Vector2>();
        private RoomData[,] _stageGenerateData;
        private int roomId = 0;
        const float tileSize = 5.8f;
        private bool playerSpawnRoom = false;
        private bool viewDebugLog = false;
        [Header("Prefabs")]
        [SerializeField]
        private GameObject tilePrefab;
        [SerializeField]
        private GameObject wallXPrefab;
        [SerializeField]
        private GameObject wallYPrefab;
        [SerializeField]
        private GameObject playerSpawnRoomPrefab;
        [SerializeField]
        private GameObject _2x2RoomPrefab;
        [SerializeField]
        private GameObject _3x2RoomPrefab;
        [SerializeField]
        private GameObject _2x3RoomPrefab;
        [SerializeField]
        private GameObject _3x3RoomPrefab;
        [SerializeField]
        private GameObject _4x3RoomPrefab;
        [SerializeField]
        private GameObject _3x4RoomPrefab;
        [SerializeField]
        private GameObject largeRoomPrefab;
        void Start()
        {
            _stageGenerateData = new RoomData[(int)_stageSize.x, (int)_stageSize.y];
            if (viewDebugLog) Debug.Log($"StageSize => x = {_stageGenerateData.GetLength(0)},y = {_stageGenerateData.GetLength(1)}, total = {_stageGenerateData.Length}");
            InitialSet();
            Generate();
        }
        private void Generate()
        {
            RandomFullSpaceRoomPlot(20, 12, 8);
            if (viewDebugLog) DebugStageData();
            RommShaping();
            GenerateAisle();
            if (viewDebugLog) DebugStageData();
            GenerateStage();
        }
        private void InitialSet()
        {
            RoomData initialData = new RoomData();
            initialData.RoomDataSet(RoomType.none, 0);
            for (int y = 0; y < _stageGenerateData.GetLength(1); y++)
            {
                for (int x = 0; x < _stageGenerateData.GetLength(0); x++)
                {
                    _stageGenerateData[x, y] = initialData;
                }
            }
        }
        private void GenerateStage()
        {
            Vector3 instantiatePosition = Vector3.zero;
            Vector3 tileXoffset = new Vector3(tileSize, 0, 0);
            Vector3 tileZoffset = new Vector3(0, 0, tileSize);
            bool[] roomFlag = new bool[roomId + 1];
            for (int i = 0; i <= roomId; i++)
            {
                roomFlag[i] = true;
            }
            //tileの生成

            for (int y = 0; y < _stageSize.y + 1; y++)
            {
                for (int x = 0; x < _stageSize.x + 1; x++)
                {
                    instantiatePosition.x = x * tileSize;
                    instantiatePosition.z = y * tileSize;
                    Instantiate(tilePrefab, instantiatePosition, Quaternion.identity, transform);
                    if (x == 0)
                    {
                        Instantiate(wallXPrefab, instantiatePosition, Quaternion.identity, transform);
                    }
                    else if (x == _stageSize.x)
                    {
                        Instantiate(wallXPrefab, instantiatePosition + tileXoffset, Quaternion.identity, transform);
                    }
                    if (y == 0)
                    {
                        Instantiate(wallYPrefab, instantiatePosition, Quaternion.identity * new Quaternion(0, 90, 0, 0), transform);
                    }
                    else if (y == _stageSize.y)
                    {
                        Instantiate(wallYPrefab, instantiatePosition + tileZoffset, Quaternion.identity * new Quaternion(0, 90, 0, 0), transform);
                    }
                    int roomId = _stageGenerateData[x, y].RoomId;
                    if (roomFlag[roomId])
                    {
                        roomFlag[roomId] = false;
                        switch (_stageGenerateData[x, y].RoomType)
                        {
                            case RoomType.room2x2:
                                if (!playerSpawnRoom)
                                {
                                    Instantiate(playerSpawnRoomPrefab, instantiatePosition, Quaternion.identity, transform);
                                    playerSpawnRoom = true;
                                }
                                else
                                {
                                    Instantiate(_2x2RoomPrefab, instantiatePosition, Quaternion.identity, transform);
                                }
                                break;
                            case RoomType.room3x2:
                                Instantiate(_3x2RoomPrefab, instantiatePosition, Quaternion.identity, transform);
                                break;
                            case RoomType.room2x3:
                                Instantiate(_2x3RoomPrefab, instantiatePosition, Quaternion.identity, transform);
                                break;
                            case RoomType.room3x3:
                                Instantiate(_3x3RoomPrefab, instantiatePosition, Quaternion.identity, transform);
                                break;
                            case RoomType.room4x3:
                                Instantiate(_4x3RoomPrefab, instantiatePosition, Quaternion.identity, transform);
                                break;
                            case RoomType.room3x4:
                                Instantiate(_3x4RoomPrefab, instantiatePosition, Quaternion.identity, transform);
                                break;
                            case RoomType.room4x4:
                                Instantiate(largeRoomPrefab, instantiatePosition, Quaternion.identity, transform);
                                break;
                            default:
                                break;
                        }
                    }
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
                    printData += $"[{_stageGenerateData[x, y].RoomId}]";
                }
                printData += "\n";
            }
            Debug.Log(printData);
        }
        /// <summary>
        /// マップに大きい順にランダムに部屋を割り当てる
        /// </summary>
        /// <param name="smallRoom">2x2のサイズの部屋を生成する数</param>
        /// <param name="mediumRoom">3x3のサイズの部屋を生成する数</param>
        /// <param name="largeRoom">4z4のサイズの部屋を生成する数</param>
        private void RandomFullSpaceRoomPlot(int smallRoom = 0, int mediumRoom = 0, int largeRoom = 0)
        {
            candidatePosition = candidatePositionSet(3, 3);
            Vector2 roomPosition = Vector2.zero;
            while (true)
            {
                int roomPositionIndex = Random.Range(0, candidatePosition.Count);
                roomPosition = candidatePosition[roomPositionIndex];
                if (largeRoom > 0)
                {
                    RoomPlotId(RoomType.room4x4, roomPosition);
                    largeRoom--;
                    if (largeRoom > 0)
                    {
                        candidatePosition = candidatePositionSet(3, 3);
                        if (candidatePosition.Count <= 0)
                        {
                            largeRoom = 0;
                            candidatePosition = candidatePositionSet(2, 2);
                        }
                    }
                    else
                    {
                        candidatePosition = candidatePositionSet(2, 2);
                    }
                }
                else if (mediumRoom > 0)
                {
                    RoomPlotId(RoomType.room3x3, roomPosition);
                    mediumRoom--;
                    if (mediumRoom > 0)
                    {
                        candidatePosition = candidatePositionSet(2, 2);
                        if (candidatePosition.Count <= 0)
                        {
                            mediumRoom = 0;
                            candidatePosition = candidatePositionSet(1, 1);
                        }
                    }
                    else
                    {
                        candidatePosition = candidatePositionSet(1, 1);
                    }
                }
                else if (smallRoom > 0)
                {
                    RoomPlotId(RoomType.room2x2, roomPosition);
                    smallRoom--;
                    candidatePosition = candidatePositionSet(1, 1);
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
        private void RoomPlotId(RoomType plotRoomType, Vector2 plotPosition)
        {
            roomId++;
            Vector2 plotRoomSize = Vector2.zero;
            int plotX = (int)plotPosition.x;
            int plotY = (int)plotPosition.y;
            switch (plotRoomType)
            {
                case RoomType.room2x2:
                    plotRoomSize = ToVector2(2, 2);
                    break;
                case RoomType.room2x3:
                    plotRoomSize = ToVector2(2, 3);
                    break;
                case RoomType.room3x2:
                    plotRoomSize = ToVector2(3, 2);
                    break;
                case RoomType.room3x3:
                    plotRoomSize = ToVector2(3, 3);
                    break;
                case RoomType.room3x4:
                    plotRoomSize = ToVector2(3, 4);
                    break;
                case RoomType.room4x3:
                    plotRoomSize = ToVector2(4, 3);
                    break;
                case RoomType.room4x4:
                    plotRoomSize = ToVector2(4, 4);
                    break;
                default:
                    break;
            }
            for (int y = 0; y < plotRoomSize.y; y++)
            {
                for (int x = 0; x < plotRoomSize.x; x++)
                {
                    _stageGenerateData[plotX + x, plotY + y].RoomDataSet(plotRoomType, roomId);
                }
            }
        }

        /// <summary>
        /// ルームを配置可能な座標のリストを作成する
        /// </summary>
        private List<Vector2> candidatePositionSet(int offsetX = 1, int offsetY = 1)
        {
            List<Vector2> candidatePositions = new List<Vector2>();
            Vector2 setPosition = Vector2.zero;
            for (int y = 0; y < _stageSize.y - offsetY; y++)
            {
                for (int x = 0; x < _stageSize.x - offsetX; x++)
                {
                    if (_stageGenerateData[x, y].RoomId == 0 &&
                        _stageGenerateData[x, y + offsetY].RoomId == 0 &&
                        _stageGenerateData[x + offsetX, y].RoomId == 0 &&
                        _stageGenerateData[x + offsetX, y + offsetY].RoomId == 0)
                    {
                        setPosition.x = x;
                        setPosition.y = y;
                        candidatePositions.Add(setPosition);
                    }
                }
            }
            return candidatePositions;
        }
        /// <summary>
        /// 孤立した部屋を検索するための関数
        /// </summary>
        private List<Vector2> candidateAislePosition(int offsetX = 0, int offsetY = 0)
        {
            List<Vector2> candidatePositions = new List<Vector2>();
            Vector2 setPosition = Vector2.zero;
            for (int y = 0; y < _stageSize.y - offsetY; y++)
            {
                for (int x = 0; x < _stageSize.x - offsetX; x++)
                {
                    if (_stageGenerateData[x, y].RoomId == 0 &&
                        _stageGenerateData[x, y + offsetY].RoomId == 0 &&
                        _stageGenerateData[x + offsetX, y].RoomId == 0)
                    {
                        if (offsetX != 0)
                        {
                            if (x >= 1)
                            {
                                if (_stageGenerateData[x - 1, y].RoomId == 0) continue;
                            }
                            if (y >= 1)
                            {
                                if (_stageGenerateData[x, y - 1].RoomId == 0) continue;
                            }
                            if (y < _stageSize.y - 1)
                            {
                                if (_stageGenerateData[x, y + 1].RoomId == 0) continue;
                            }

                        }
                        if (offsetY != 0)
                        {
                            if (y >= 1 && offsetY != 0)
                            {
                                if (_stageGenerateData[x, y - 1].RoomId == 0) continue;
                            }
                            if (x < _stageSize.x - 1)
                            {
                                if (_stageGenerateData[x + 1, y].RoomId == 0) continue;
                            }
                            if (x >= 1)
                            {
                                if (_stageGenerateData[x - 1, y].RoomId == 0) continue;
                            }
                        }
                        setPosition.x = x;
                        setPosition.y = y;
                        candidatePositions.Add(setPosition);
                    }
                }
            }
            return candidatePositions;
        }
        /// <summary>
        /// 通路の作成
        /// </summary>
        private void GenerateAisle()
        {
            int xAisleNumber = GenerateXAisle((int)_stageSize.x - 4, 4);
            int yAisleNumber = GenerateYAisle((int)_stageSize.y - 4, 4);
            int xValue = 0;
            int yValue = 0;

            RoomData initialData = new RoomData();
            initialData.RoomDataSet(RoomType.none, 0);
            var newStageGenerateData = new RoomData[_stageGenerateData.GetLength(0) + 1, _stageGenerateData.GetLength(1) + 1];
            for (int i = 0; i < (int)_stageSize.y + 1; i++)
            {
                for (int j = 0; j < (int)_stageSize.x; j++)
                {
                    newStageGenerateData[i, j] = initialData;
                }
            }
            for (int y = 0; y < _stageGenerateData.GetLength(1); y++)
            {
                if (y == yAisleNumber)
                {
                    yValue++;
                }
                xValue = 0;
                for (int x = 0; x < _stageGenerateData.GetLength(0); x++)
                {
                    if (x == xAisleNumber)
                    {
                        xValue++;
                    }
                    newStageGenerateData[xValue, yValue] = _stageGenerateData[x, y];
                    xValue++;
                }
                yValue++;
            }
            _stageGenerateData = newStageGenerateData;
        }
        /// <summary>
        ///　ランダムでX軸の通録を作る場所を検索
        /// </summary>
        private int GenerateXAisle(int max, int min = 0)
        {
            int value = Random.Range(min, max);

            var _onlyXAisle = candidateAislePosition(offsetX: 4);
            if (_onlyXAisle.Any(e => e.y == value))
            {
                value = GenerateXAisle(min, max);
            }
            return value;
        }
        private int GenerateYAisle(int max, int min = 0)
        {
            int value = Random.Range(min, max);

            var _onlyYAisle = candidateAislePosition(offsetY: 4);
            if (_onlyYAisle.Any(e => e.x == value))
            {
                value = GenerateYAisle(min, max);
            }
            return value;
        }
        /// <summary>
        /// 孤立した部屋を埋めるように部屋を拡張する関数
        /// </summary>
        private void RommShaping()
        {
            var _only1x4Aisle = candidateAislePosition(offsetY: 3);
            var _only4x1Aisle = candidateAislePosition(offsetX: 3);
            var _only1x3Aisle = candidateAislePosition(offsetY: 2);
            var _only3x1Aisle = candidateAislePosition(offsetX: 2);
            var _only1x2Aisle = candidateAislePosition(offsetY: 1);
            var _only2x1Aisle = candidateAislePosition(offsetX: 1);
            _only1x2Aisle = _only1x2Aisle.Except(_only1x3Aisle).ToList();
            _only2x1Aisle = _only2x1Aisle.Except(_only3x1Aisle).ToList();
            _only1x3Aisle = _only1x3Aisle.Except(_only1x4Aisle).ToList();
            _only3x1Aisle = _only3x1Aisle.Except(_only4x1Aisle).ToList();
            if (viewDebugLog) Debug.Log($"Aisle count  1x3 only = {_only1x3Aisle.Count},3x1 = {_only3x1Aisle.Count}, 1x2 only = {_only1x2Aisle.Count},2x1 = {_only2x1Aisle.Count},");
            foreach (var item in _only1x3Aisle)
            {
                if (item.x > 2)//ブロックの左にroom3x3がある場合
                {
                    if (_stageGenerateData[(int)item.x - 1, (int)item.y].RoomType == RoomType.room3x3)
                    {
                        int roomId = _stageGenerateData[(int)item.x - 1, (int)item.y].RoomId;
                        if (_stageGenerateData[(int)item.x - 1, (int)item.y + 1].RoomId == roomId &&
                           _stageGenerateData[(int)item.x - 1, (int)item.y + 2].RoomId == roomId)
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                for (int x = 0; x < 4; x++)
                                {
                                    _stageGenerateData[(int)item.x - x, (int)item.y + y].RoomDataSet(RoomType.room4x3, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
                if (item.x < _stageGenerateData.GetLength(0) - 2)//ブロックの右にroom3x3がある場合
                {
                    if (_stageGenerateData[(int)item.x + 1, (int)item.y].RoomType == RoomType.room3x3)
                    {
                        int roomId = _stageGenerateData[(int)item.x + 1, (int)item.y].RoomId;
                        if (_stageGenerateData[(int)item.x + 1, (int)item.y + 1].RoomId == roomId ||
                           _stageGenerateData[(int)item.x + 1, (int)item.y + 2].RoomId == roomId)
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                for (int x = 0; x < 4; x++)
                                {
                                    _stageGenerateData[(int)item.x + x, (int)item.y + y].RoomDataSet(RoomType.room4x3, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
            foreach (var item in _only3x1Aisle)
            {
                if (item.y < _stageGenerateData.GetLength(1) - 2)//ブロックの下にroom3x3がある場合
                {
                    if (_stageGenerateData[(int)item.x, (int)item.y + 1].RoomType == RoomType.room3x3)
                    {
                        int roomId = _stageGenerateData[(int)item.x, (int)item.y + 1].RoomId;
                        if (_stageGenerateData[(int)item.x + 1, (int)item.y + 1].RoomId == roomId ||
                           _stageGenerateData[(int)item.x + 2, (int)item.y + 1].RoomId == roomId)
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    _stageGenerateData[(int)item.x + x, (int)item.y + y].RoomDataSet(RoomType.room3x4, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
                if (item.y > 2)//ブロックの上にroom3x3がある場合
                {
                    if (_stageGenerateData[(int)item.x, (int)item.y - 1].RoomType == RoomType.room3x3)
                    {
                        int roomId = _stageGenerateData[(int)item.x, (int)item.y - 1].RoomId;
                        if (_stageGenerateData[(int)item.x + 1, (int)item.y - 1].RoomId == roomId &&
                           _stageGenerateData[(int)item.x + 2, (int)item.y - 1].RoomId == roomId)
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    _stageGenerateData[(int)item.x + x, (int)item.y - y].RoomDataSet(RoomType.room3x4, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
            foreach (var item in _only1x2Aisle)
            {
                if (item.x > 1)//ブロックの左にroom2x2がある場合
                {
                    if (_stageGenerateData[(int)item.x - 1, (int)item.y].RoomType == RoomType.room2x2)
                    {
                        int roomId = _stageGenerateData[(int)item.x - 1, (int)item.y].RoomId;
                        if (_stageGenerateData[(int)item.x - 1, (int)item.y + 1].RoomId == roomId)
                        {
                            for (int y = 0; y < 2; y++)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    _stageGenerateData[(int)item.x - x, (int)item.y + y].RoomDataSet(RoomType.room3x2, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
                if (item.x < _stageGenerateData.GetLength(0) - 1)//ブロックの右にroom2x2がある場合
                {
                    if (_stageGenerateData[(int)item.x + 1, (int)item.y].RoomType == RoomType.room2x2)
                    {
                        int roomId = _stageGenerateData[(int)item.x + 1, (int)item.y].RoomId;
                        if (_stageGenerateData[(int)item.x + 1, (int)item.y + 1].RoomId == roomId)
                        {
                            for (int y = 0; y < 2; y++)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    _stageGenerateData[(int)item.x + x, (int)item.y + y].RoomDataSet(RoomType.room3x2, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
            foreach (var item in _only2x1Aisle)
            {
                if (item.y < _stageGenerateData.GetLength(1) - 1)//ブロックより下にroom2x2がある
                {
                    if (_stageGenerateData[(int)item.x, (int)item.y + 1].RoomType == RoomType.room2x2)
                    {
                        int roomId = _stageGenerateData[(int)item.x, (int)item.y + 1].RoomId;
                        if (_stageGenerateData[(int)item.x + 1, (int)item.y + 1].RoomId == roomId)
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                for (int x = 0; x < 2; x++)
                                {
                                    _stageGenerateData[(int)item.x + x, (int)item.y + y].RoomDataSet(RoomType.room2x3, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
                if (item.y > 1)//ブロックより上にroom2x2がある
                {
                    if (_stageGenerateData[(int)item.x, (int)item.y - 1].RoomType == RoomType.room2x2)
                    {
                        int roomId = _stageGenerateData[(int)item.x, (int)item.y - 1].RoomId;
                        if (_stageGenerateData[(int)item.x + 1, (int)item.y - 1].RoomId == roomId)
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                for (int x = 0; x < 2; x++)
                                {
                                    _stageGenerateData[(int)item.x + x, (int)item.y - y].RoomDataSet(RoomType.room2x3, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
        }

        Vector2 translation = Vector2.zero;
        private Vector2 ToVector2(float x, float y)
        {
            translation.x = x;
            translation.y = y;
            return translation;
        }
    }
}
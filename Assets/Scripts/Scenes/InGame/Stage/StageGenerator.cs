using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using Scenes.Ingame.Manager;
using Scenes.Ingame.InGameSystem;
using Unity.AI;
using Unity.AI.Navigation;
using UnityEditor.SceneManagement;

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
        private RoomData[,] _firsrFloorData;
        private RoomData[,] _secondFloorData;
        private int roomId = 0;
        const float tileSize = 5.85f;
        private bool playerSpawnRoom = false;
        private bool viewDebugLog = false;//確認用のデバックログを表示する
        private CancellationTokenSource source = new CancellationTokenSource();
        [Header("Parent")]
        [SerializeField]
        private GameObject floorObject;
        [SerializeField]
        private GameObject secondFloorObject;
        [SerializeField]
        private GameObject outSideWallObject;
        [SerializeField]
        private GameObject inSideWallObject;
        [SerializeField]
        private GameObject roomObject;
        [Header("Prefabs")]
        [SerializeField]
        private GameObject tilePrefab;
        [SerializeField]
        private GameObject outSideWallXPrefab;
        [SerializeField]
        private GameObject outSideWallYPrefab;
        [SerializeField]
        private GameObject inSideWallXPrefab;
        [SerializeField]
        private GameObject inSideWallYPrefab;
        [SerializeField]
        private GameObject wallXDoorPrefab;
        [SerializeField]
        private GameObject wallYDoorPrefab;
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
            CancellationToken token = source.Token;
            _firsrFloorData = new RoomData[(int)_stageSize.x, (int)_stageSize.y];
            _secondFloorData = new RoomData[(int)_stageSize.x, (int)_stageSize.y];
            if (viewDebugLog) Debug.Log($"StageSize => x = {_firsrFloorData.GetLength(0)},y = {_firsrFloorData.GetLength(1)}, total = {_firsrFloorData.Length}");
            InitialSet();
            Generate(token).Forget();
        }
        private async UniTaskVoid Generate(CancellationToken token)
        {
            for (int floor = 1; floor <= 2; floor++)
            {
                RoomData[,] targetFloor = new RoomData[(int)_stageSize.x, (int)_stageSize.y];
                await RandomFullSpaceRoomPlot(token, targetFloor, 20, 12, 8);
                if (viewDebugLog) DebugStageData(targetFloor);
                await RommShaping(token, targetFloor);
                targetFloor = GenerateAisle(token, targetFloor);
                if (viewDebugLog) Debug.Log("通路生成処理後のデータ");
                if (viewDebugLog) DebugStageData(targetFloor);
                await GenerateStage(token, targetFloor, floor - 1);
                await GenerateWall(token, targetFloor, floor - 1);
                switch (floor)
                {
                    case 1:
                        _firsrFloorData = targetFloor;
                        break;
                    case 2:
                        _secondFloorData = targetFloor;
                        break;
                    default:
                        break;
                }
            }
            floorObject.GetComponent<NavMeshSurface>().BuildNavMesh();
            IngameManager.Instance.SetReady(ReadyEnum.StageReady);//ステージ生成完了を通知
        }
        private void InitialSet()
        {
            RoomData initialData = new RoomData();
            initialData.RoomDataSet(RoomType.none, 0);
            for (int y = 0; y < _firsrFloorData.GetLength(1); y++)
            {
                for (int x = 0; x < _firsrFloorData.GetLength(0); x++)
                {
                    _firsrFloorData[x, y] = initialData;
                    _secondFloorData[x, y] = initialData;
                }
            }
        }
        private async UniTask GenerateStage(CancellationToken token, RoomData[,] stage,int floor)
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
                    instantiatePosition = ToVector3(x * tileSize, (floor + 1) * 5.8f, y * tileSize);
                    if (floor == 0)
                    {
                        Instantiate(tilePrefab, instantiatePosition, Quaternion.identity, floorObject.transform);
                        instantiatePosition = ToVector3(x * tileSize, 0, y * tileSize);
                        Instantiate(tilePrefab, instantiatePosition, Quaternion.identity, floorObject.transform);
                    }
                    else if(floor == 1)
                    {
                        Instantiate(tilePrefab, instantiatePosition, Quaternion.identity, secondFloorObject.transform);
                    }

                    instantiatePosition = ToVector3(x * tileSize, floor * 5.8f, y * tileSize);
                    if (x == 0)
                    {
                        Instantiate(outSideWallXPrefab, instantiatePosition, Quaternion.identity, outSideWallObject.transform);
                    }
                    else if (x == _stageSize.x)
                    {
                        Instantiate(outSideWallXPrefab, instantiatePosition + tileXoffset, Quaternion.identity, outSideWallObject.transform);
                    }
                    if (y == 0)
                    {
                        Instantiate(outSideWallYPrefab, instantiatePosition, Quaternion.identity * new Quaternion(0, 90, 0, 0), outSideWallObject.transform);
                    }
                    else if (y == _stageSize.y)
                    {
                        Instantiate(outSideWallYPrefab, instantiatePosition + tileZoffset, Quaternion.identity * new Quaternion(0, 90, 0, 0), outSideWallObject.transform);

                    }
                    instantiatePosition = ToVector3(x * tileSize, floor * 5.8f, y * tileSize);
                    int roomId = stage[x, y].RoomId;
                    if (roomFlag[roomId])
                    {
                        roomFlag[roomId] = false;
                        switch (stage[x, y].RoomType)
                        {
                            case RoomType.room2x2:
                                if (!playerSpawnRoom)
                                {
                                    Instantiate(playerSpawnRoomPrefab, instantiatePosition, Quaternion.identity, roomObject.transform);
                                    playerSpawnRoom = true;
                                }
                                else
                                {
                                    Instantiate(_2x2RoomPrefab, instantiatePosition, Quaternion.identity, roomObject.transform);
                                }
                                break;
                            case RoomType.room3x2:
                                Instantiate(_3x2RoomPrefab, instantiatePosition, Quaternion.identity, roomObject.transform);
                                break;
                            case RoomType.room2x3:
                                Instantiate(_2x3RoomPrefab, instantiatePosition, Quaternion.identity, roomObject.transform);
                                break;
                            case RoomType.room3x3:
                                Instantiate(_3x3RoomPrefab, instantiatePosition, Quaternion.identity, roomObject.transform);
                                break;
                            case RoomType.room4x3:
                                Instantiate(_4x3RoomPrefab, instantiatePosition, Quaternion.identity, roomObject.transform);
                                break;
                            case RoomType.room3x4:
                                Instantiate(_3x4RoomPrefab, instantiatePosition, Quaternion.identity, roomObject.transform);
                                break;
                            case RoomType.room4x4:
                                Instantiate(largeRoomPrefab, instantiatePosition, Quaternion.identity, roomObject.transform);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// マップに大きい順にランダムに部屋を割り当てる
        /// </summary>
        /// <param name="smallRoom">2x2のサイズの部屋を生成する数</param>
        /// <param name="mediumRoom">3x3のサイズの部屋を生成する数</param>
        /// <param name="largeRoom">4z4のサイズの部屋を生成する数</param>
        private async UniTask RandomFullSpaceRoomPlot(CancellationToken token, RoomData[,] stage, int smallRoom = 0, int mediumRoom = 0, int largeRoom = 0)
        {
            int roomSize = 3;//部屋の大きさ
            Vector2 roomPosition = Vector2.zero;
            while (roomSize > 0)
            {

                candidatePosition = candidatePositionSet(stage,roomSize, roomSize);
                int roomPositionIndex = Random.Range(0, candidatePosition.Count);
                if (candidatePosition.Count <= 0)
                {
                    roomSize--;
                    continue;
                }
                roomPosition = candidatePosition[roomPositionIndex];
                if (largeRoom > 0 && roomSize == 3)
                {
                    RoomPlotId(RoomType.room4x4, roomPosition, stage);
                    largeRoom--;
                }
                else if (mediumRoom > 0 && roomSize == 2)
                {
                    RoomPlotId(RoomType.room3x3, roomPosition, stage);
                    mediumRoom--;
                }
                else if (smallRoom > 0 && roomSize == 1)
                {
                    RoomPlotId(RoomType.room2x2, roomPosition, stage);
                    smallRoom--;
                }
                else
                {
                    roomSize--;
                }
            }
        }

        /// <summary>
        /// データ上に部屋のデータを登録する
        /// </summary>
        /// <param name="plotRoomSize">ルームの大きさ</param>
        /// <param name="plotPosition">ルームの設定位置</param>
        private void RoomPlotId(RoomType plotRoomType, Vector2 plotPosition, RoomData[,] stage)
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
                    stage[plotX + x, plotY + y].RoomDataSet(plotRoomType, roomId);
                }
            }
        }

        /// <summary>
        /// ルームを配置可能な座標のリストを作成する
        /// </summary>
        private List<Vector2> candidatePositionSet(RoomData[,] stage,int offsetX = 1, int offsetY = 1)
        {
            List<Vector2> candidatePositions = new List<Vector2>();
            Vector2 setPosition = Vector2.zero;
            for (int y = 0; y < _stageSize.y - offsetY; y++)
            {
                for (int x = 0; x < _stageSize.x - offsetX; x++)
                {
                    if (RoomIdEqual(ToVector2(x, y), Vector2.zero, 0, stage) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(0, offsetY), 0, stage) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(offsetX, 0), 0, stage) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(offsetX, offsetY), 0, stage))
                    {
                        setPosition = ToVector2(x, y);
                        candidatePositions.Add(setPosition);
                    }
                }
            }
            return candidatePositions;
        }
        /// <summary>
        /// 孤立した部屋を検索するための関数
        /// </summary>
        private List<Vector2> candidateAislePosition(RoomData[,] stage,int offsetX = 0, int offsetY = 0)
        {
            if (offsetX == 0 && offsetY == 0) Debug.LogError("offsetの値が両方とも0です");
            List<Vector2> candidatePositions = new List<Vector2>();
            Vector2 setPosition = Vector2.zero;
            for (int y = 0; y < _stageSize.y - offsetY; y++)
            {
                for (int x = 0; x < _stageSize.x - offsetX; x++)
                {
                    if (RoomIdEqual(ToVector2(x, y), Vector2.zero, 0, stage) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(0, offsetY), 0, stage) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(offsetX, 0), 0, stage))
                    {
                        if (x >= 1)
                        {
                            if (RoomIdEqual(ToVector2(x, y), ToVector2(-1, 0), 0, stage)) continue;
                        }
                        if (y >= 1)
                        {
                            if (RoomIdEqual(ToVector2(x, y), ToVector2(0, -1), 0, stage)) continue;
                        }
                        if (offsetX != 0)
                        {
                            if (y < _stageSize.y - 1)
                            {
                                if (RoomIdEqual(ToVector2(x, y), ToVector2(0, 1), 0, stage)) continue;
                            }

                        }
                        if (offsetY != 0)
                        {
                            if (x < _stageSize.x - 1)
                            {
                                if (RoomIdEqual(ToVector2(x, y), ToVector2(1, 0), 0, stage)) continue;
                            }
                        }
                        setPosition = ToVector2(x, y);
                        candidatePositions.Add(setPosition);
                    }
                }
            }
            return candidatePositions;
        }
        /// <summary>
        /// 次の場所は壁のタイルを検索するための関数
        /// </summary>
        private List<Vector2> candidateNextWallPosition(RoomData[,] stage,int offsetX = 0, int offsetY = 0)
        {
            if (offsetX != 0 && offsetY != 0) { Debug.LogError("無効な引数です。どちらかを0にしてください"); }
            List<Vector2> candidatePositions = new List<Vector2>();
            Vector2 setPosition = Vector2.zero;
            int xLength = stage.GetLength(0);
            int yLength = stage.GetLength(1);
            for (int y = 0; y < yLength - offsetY; y++)
            {
                for (int x = 0; x < xLength - offsetX; x++)
                {
                    if (stage[x, y].RoomId == 0)
                    {
                        if (offsetX != 0)
                        {
                            if (x < xLength - 1 && offsetX > 0)
                            {
                                if (stage[x + offsetX, y].RoomId == 0) continue;
                            }
                            else if (x + offsetX >= 0)
                            {
                                if (stage[x + offsetX, y].RoomId == 0) continue;
                            }
                        }
                        else if (offsetY != 0)
                        {
                            if (y < yLength - 1 && offsetY > 0)
                            {
                                if (stage[x, y + offsetY].RoomId == 0) continue;
                            }
                            else if (y - offsetY >= 0)
                            {
                                if (stage[x, y + offsetY].RoomId == 0) continue;
                            }
                        }
                        setPosition = ToVector2(x + offsetX, y);
                        candidatePositions.Add(setPosition);
                    }
                }
            }
            return candidatePositions;
        }
        /// <summary>
        /// x軸とy軸に１つずつ通路の作成
        /// </summary>
        private RoomData[,] GenerateAisle(CancellationToken token, RoomData[,] stage)
        {
            const int OFFSET = 2;//通路を作らない範囲
            int xAisleNumber = GenerateXAisle(stage,(int)_stageSize.x - OFFSET, OFFSET);
            int yAisleNumber = GenerateYAisle(stage, (int)_stageSize.y - OFFSET, OFFSET);
            bool xSlide = false;
            bool ySlide = false;

            if (viewDebugLog) Debug.Log($"IslePosition => x = {xAisleNumber},y = {yAisleNumber}");
            RoomData initialData = new RoomData();
            initialData.RoomDataSet(RoomType.none, 0);
            var newStageGenerateData = new RoomData[stage.GetLength(0) + 1, stage.GetLength(1) + 1];
            for (int i = 0; i < (int)_stageSize.y + 1; i++)
            {
                for (int j = 0; j < (int)_stageSize.x; j++)
                {
                    newStageGenerateData[i, j] = initialData;
                }
            }
            //X軸を通路分ずらす処理
            for (int y = 0; y < stage.GetLength(1); y++)
            {
                xSlide = false;
                for (int x = 0; x < stage.GetLength(0); x++)
                {
                    if (xSlide == false)
                    {
                        if (x >= xAisleNumber && stage[x, y].RoomId != stage[x - 1, y].RoomId)
                        {
                            xSlide = true;
                        }
                    }
                    if (xSlide)
                    {
                        newStageGenerateData[x + 1, y] = stage[x, y];
                    }
                    else
                    {
                        newStageGenerateData[x, y] = stage[x, y];
                    }
                }
            }
            //y軸を通路分ずらす処理
            var tempXPlotData = new RoomData[newStageGenerateData.GetLength(0), newStageGenerateData.GetLength(1)];
            for (int i = 0; i < (int)_stageSize.y + 1; i++)
            {
                for (int j = 0; j < (int)_stageSize.x + 1; j++)
                {
                    tempXPlotData[i, j] = initialData;
                }
            }
            for (int x = 0; x < newStageGenerateData.GetLength(0); x++)
            {
                ySlide = false;
                for (int y = 0; y < newStageGenerateData.GetLength(1) - 1; y++)
                {
                    if (y >= yAisleNumber && newStageGenerateData[x, y].RoomId != newStageGenerateData[x, y - 1].RoomId)
                    {
                        ySlide = true;
                    }
                    if (ySlide)
                    {
                        tempXPlotData[x, y + 1] = newStageGenerateData[x, y];
                    }
                    else
                    {
                        tempXPlotData[x, y] = newStageGenerateData[x, y];
                    }
                }
            }
            return tempXPlotData;
        }
        /// <summary>
        ///　ランダムでX軸の通録を作る場所を検索
        /// </summary>
        private int GenerateXAisle(RoomData[,] stage,int max, int min = 0)
        {
            int value = Random.Range(min, max);

            var _onlyXAisle = candidateAislePosition(stage,offsetX: 4);
            if (_onlyXAisle.Any(e => e.y == value))
            {
                value = GenerateXAisle(stage,min, max);
            }
            return value;
        }
        private int GenerateYAisle(RoomData[,] stage,int max, int min = 0)
        {
            int value = Random.Range(min, max);

            var _onlyYAisle = candidateAislePosition(stage, offsetY: 4);
            if (_onlyYAisle.Any(e => e.x == value))
            {
                value = GenerateYAisle(stage,min, max);
            }
            return value;
        }
        /// <summary>
        /// 孤立した部屋を埋めるように部屋を拡張する関数
        /// </summary>
        private async UniTask RommShaping(CancellationToken token, RoomData[,]stage)
        {
            var _only1x4Aisle = candidateAislePosition(stage,offsetY: 3);
            var _only4x1Aisle = candidateAislePosition(stage, offsetX: 3);
            var _only1x3Aisle = candidateAislePosition(stage, offsetY: 2);
            var _only3x1Aisle = candidateAislePosition(stage, offsetX: 2);
            var _only1x2Aisle = candidateAislePosition(stage, offsetY: 1);
            var _only2x1Aisle = candidateAislePosition(stage, offsetX: 1);
            _only1x2Aisle = _only1x2Aisle.Except(_only1x3Aisle).ToList();
            _only2x1Aisle = _only2x1Aisle.Except(_only3x1Aisle).ToList();
            _only1x3Aisle = _only1x3Aisle.Except(_only1x4Aisle).ToList();
            _only3x1Aisle = _only3x1Aisle.Except(_only4x1Aisle).ToList();
            if (viewDebugLog) Debug.Log($"Aisle count  1x3 only = {_only1x3Aisle.Count},3x1 = {_only3x1Aisle.Count}, 1x2 only = {_only1x2Aisle.Count},2x1 = {_only2x1Aisle.Count},");
            foreach (var item in _only1x3Aisle)
            {
                if (item.x > 2)//ブロックの左にroom3x3がある場合
                {
                    if (stage[(int)item.x - 1, (int)item.y].RoomType == RoomType.room3x3)
                    {
                        int roomId = stage[(int)item.x - 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, ToVector2(-1, 1), roomId, stage) &&
                           RoomIdEqual(item, ToVector2(-1, 2), roomId, stage))
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                for (int x = 0; x < 4; x++)
                                {
                                    stage[(int)item.x - x, (int)item.y + y].RoomDataSet(RoomType.room4x3, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
                if (item.x < stage.GetLength(0) - 2)//ブロックの右にroom3x3がある場合
                {
                    if (stage[(int)item.x + 1, (int)item.y].RoomType == RoomType.room3x3)
                    {
                        int roomId = stage[(int)item.x + 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, 1), roomId, stage) &&
                           RoomIdEqual(item, ToVector2(1, 2), roomId, stage))
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                for (int x = 0; x < 4; x++)
                                {
                                    stage[(int)item.x + x, (int)item.y + y].RoomDataSet(RoomType.room4x3, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
            foreach (var item in _only3x1Aisle)
            {
                if (item.y < stage.GetLength(1) - 2)//ブロックの下にroom3x3がある場合
                {
                    if (stage[(int)item.x, (int)item.y + 1].RoomType == RoomType.room3x3)
                    {
                        int roomId = stage[(int)item.x, (int)item.y + 1].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, 1), roomId, stage) &&
                           RoomIdEqual(item, ToVector2(2, 1), roomId, stage))
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    stage[(int)item.x + x, (int)item.y + y].RoomDataSet(RoomType.room3x4, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
                if (item.y > 2)//ブロックの上にroom3x3がある場合
                {
                    if (stage[(int)item.x, (int)item.y - 1].RoomType == RoomType.room3x3)
                    {
                        int roomId = stage[(int)item.x, (int)item.y - 1].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, -1), roomId, stage) &&
                           RoomIdEqual(item, ToVector2(2, -1), roomId, stage))
                        {
                            for (int y = 0; y < 4; y++)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    stage[(int)item.x + x, (int)item.y - y].RoomDataSet(RoomType.room3x4, roomId);
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
                    if (stage[(int)item.x - 1, (int)item.y].RoomType == RoomType.room2x2)
                    {
                        int roomId = stage[(int)item.x - 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, ToVector2(-1, 1), roomId, stage))
                        {
                            for (int y = 0; y < 2; y++)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    stage[(int)item.x - x, (int)item.y + y].RoomDataSet(RoomType.room3x2, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
                if (item.x < stage.GetLength(0) - 1)//ブロックの右にroom2x2がある場合
                {
                    if (stage[(int)item.x + 1, (int)item.y].RoomType == RoomType.room2x2)
                    {
                        int roomId = stage[(int)item.x + 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, 1), roomId, stage))
                        {
                            for (int y = 0; y < 2; y++)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    stage[(int)item.x + x, (int)item.y + y].RoomDataSet(RoomType.room3x2, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
            foreach (var item in _only2x1Aisle)
            {
                if (item.y < stage.GetLength(1) - 1)//ブロックより下にroom2x2がある
                {
                    if (stage[(int)item.x, (int)item.y + 1].RoomType == RoomType.room2x2)
                    {
                        int roomId = stage[(int)item.x, (int)item.y + 1].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, 1), roomId, stage))
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                for (int x = 0; x < 2; x++)
                                {
                                    stage[(int)item.x + x, (int)item.y + y].RoomDataSet(RoomType.room2x3, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
                if (item.y > 1)//ブロックより上にroom2x2がある
                {
                    if (stage[(int)item.x, (int)item.y - 1].RoomType == RoomType.room2x2)
                    {
                        int roomId = stage[(int)item.x, (int)item.y - 1].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, -1), roomId, stage))
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                for (int x = 0; x < 2; x++)
                                {
                                    stage[(int)item.x + x, (int)item.y - y].RoomDataSet(RoomType.room2x3, roomId);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 壁を設置するスクリプト
        /// </summary>
        private async UniTask GenerateWall(CancellationToken token, RoomData[,] stage,int floor)
        {
            Vector3 instantiatePosition = Vector3.zero;
            var _xWallPos = candidateNextWallPosition(stage,offsetX: 1);
            var _yWallPos = candidateNextWallPosition(stage, offsetY: 1);
            foreach (var xWall in _xWallPos)
            {
                instantiatePosition = ToVector3(xWall.x * tileSize, floor * 5.8f, xWall.y * tileSize);
                if ((xWall.x + xWall.y) % 4 == 0)
                {
                    Instantiate(wallXDoorPrefab, instantiatePosition, Quaternion.identity, inSideWallObject.transform);
                }
                else
                {
                    Instantiate(inSideWallXPrefab, instantiatePosition, Quaternion.identity, inSideWallObject.transform);
                }
            }
            foreach (var yWall in _yWallPos)
            {
                instantiatePosition = ToVector3(yWall.x * tileSize, floor * 5.8f, yWall.y * tileSize);
                if ((yWall.x + yWall.y) % 4 == 0)
                {
                    Instantiate(wallYDoorPrefab, instantiatePosition, Quaternion.identity, inSideWallObject.transform);
                }
                else
                {
                    Instantiate(inSideWallYPrefab, instantiatePosition, Quaternion.identity, inSideWallObject.transform);
                }
            }
        }

        Vector2 translation2 = Vector2.zero;
        Vector3 translation3 = Vector3.zero;
        private Vector2 ToVector2(float x, float y)
        {
            translation2.x = x;
            translation2.y = y;
            return translation2;
        }
        private Vector3 ToVector3(float x, float y, float z)
        {
            translation3.x = x;
            translation3.y = y;
            translation3.z = z;
            return translation3;
        }
        private bool RoomIdEqual(Vector2 basePosition, Vector2 position1, int roomId, RoomData[,] stage)
        {
            if (stage[(int)(basePosition.x + position1.x), (int)(basePosition.y + position1.y)].RoomId == roomId)
            {
                return true;
            }
            return false;
        }
        private void DebugStageData(RoomData[,] target)
        {
            string printData = "\n";
            for (int y = 0; y < target.GetLength(1); y++)
            {
                for (int x = 0; x < target.GetLength(0); x++)
                {
                    if (target[x, y].RoomId < 10) printData += $"[ {target[x, y].RoomId}]";
                    else printData += $"[{target[x, y].RoomId}]";
                }
                printData += "\n";
            }
            Debug.Log(printData);
        }
        private void OnDestroy()
        {
            source.Cancel();
            source.Dispose();
        }
    }
}
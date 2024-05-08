using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using Scenes.Ingame.Manager;
using Scenes.Ingame.InGameSystem;
using Unity.AI;
using Unity.AI.Navigation;

namespace Scenes.Ingame.Stage
{
    /// <summary>
    /// �������
    /// �P._stageSize�ɐݒ肳�ꂽ�T�C�Y�̃X�e�[�W�f�[�^�𐶐��i�z���_stageGenerateData�ŊǗ��j
    /// �Q.RandomFullSpaceRoomPlot�֐����g���傫���������珇��_stageGenerateDat���ɕ����̃f�[�^�𐶐��B���̎��������镔����4x4,3x3,2x2�̑傫��
    /// �R.RommShaping�֐����g���A�Ǘ����ċ󂢂Ă��錄�Ԃ𖄂߂�悤�ɕ������g���B
    /// �S.GenerateAisle�֐����g���A�ʘH�̍쐬�B���݂͏c���P���쐬���Ă���
    /// </summary>
    public class StageGenerator : MonoBehaviour
    {
        [SerializeField, Tooltip("int�ŃX�e�[�W�̏c���̃T�C�Y")]
        private Vector2 _stageSize;
        private List<Vector2> candidatePosition = new List<Vector2>();
        private RoomData[,] _stageGenerateData;
        private int roomId = 0;
        const float tileSize = 5.85f;
        private bool playerSpawnRoom = false;
        private bool viewDebugLog = false;//�m�F�p�̃f�o�b�N���O��\������
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
            _stageGenerateData = new RoomData[(int)_stageSize.x, (int)_stageSize.y];
            if (viewDebugLog) Debug.Log($"StageSize => x = {_stageGenerateData.GetLength(0)},y = {_stageGenerateData.GetLength(1)}, total = {_stageGenerateData.Length}");
            InitialSet();
            Generate(token).Forget();
        }
        private async UniTaskVoid Generate(CancellationToken token)
        {
            await RandomFullSpaceRoomPlot(token, 20, 12, 8);
            if (viewDebugLog) DebugStageData(_stageGenerateData);
            await RommShaping(token);
            await GenerateAisle(token);
            if (viewDebugLog) Debug.Log("�ʘH����������̃f�[�^");
            if (viewDebugLog) DebugStageData(_stageGenerateData);
            await GenerateStage(token);
            await GenerateWall(token);
            IngameManager.Instance.SetReady(ReadyEnum.StageReady);//�X�e�[�W����������ʒm
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
        private async UniTask GenerateStage(CancellationToken token)
        {
            Vector3 instantiatePosition = Vector3.zero;
            Vector3 tileXoffset = new Vector3(tileSize, 0, 0);
            Vector3 tileZoffset = new Vector3(0, 0, tileSize);
            bool[] roomFlag = new bool[roomId + 1];
            for (int i = 0; i <= roomId; i++)
            {
                roomFlag[i] = true;
            }
            //tile�̐���

            for (int y = 0; y < _stageSize.y + 1; y++)
            {
                for (int x = 0; x < _stageSize.x + 1; x++)
                {
                    instantiatePosition = ToVector3(x * tileSize, 5.8f, y * tileSize);
                    Instantiate(tilePrefab, instantiatePosition, Quaternion.identity, secondFloorObject.transform);
                    instantiatePosition = ToVector3(x * tileSize, 0, y * tileSize);
                    Instantiate(tilePrefab, instantiatePosition, Quaternion.identity, floorObject.transform);
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
                    int roomId = _stageGenerateData[x, y].RoomId;
                    if (roomFlag[roomId])
                    {
                        roomFlag[roomId] = false;
                        switch (_stageGenerateData[x, y].RoomType)
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
        /// �}�b�v�ɑ傫�����Ƀ����_���ɕ��������蓖�Ă�
        /// </summary>
        /// <param name="smallRoom">2x2�̃T�C�Y�̕����𐶐����鐔</param>
        /// <param name="mediumRoom">3x3�̃T�C�Y�̕����𐶐����鐔</param>
        /// <param name="largeRoom">4z4�̃T�C�Y�̕����𐶐����鐔</param>
        private async UniTask RandomFullSpaceRoomPlot(CancellationToken token, int smallRoom = 0, int mediumRoom = 0, int largeRoom = 0)
        {
            int roomSize = 3;//�����̑傫��
            Vector2 roomPosition = Vector2.zero;
            while (roomSize > 0)
            {

                candidatePosition = candidatePositionSet(roomSize, roomSize);
                int roomPositionIndex = Random.Range(0, candidatePosition.Count);
                if (candidatePosition.Count <= 0)
                {
                    roomSize--;
                    continue;
                }
                roomPosition = candidatePosition[roomPositionIndex];
                if (largeRoom > 0 && roomSize == 3)
                {
                    RoomPlotId(RoomType.room4x4, roomPosition);
                    largeRoom--;
                }
                else if (mediumRoom > 0 && roomSize == 2)
                {
                    RoomPlotId(RoomType.room3x3, roomPosition);
                    mediumRoom--;
                }
                else if (smallRoom > 0 && roomSize == 1)
                {
                    RoomPlotId(RoomType.room2x2, roomPosition);
                    smallRoom--;
                }
                else
                {
                    roomSize--;
                }
            }
        }

        /// <summary>
        /// �f�[�^��ɕ����̃f�[�^��o�^����
        /// </summary>
        /// <param name="plotRoomSize">���[���̑傫��</param>
        /// <param name="plotPosition">���[���̐ݒ�ʒu</param>
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
        /// ���[����z�u�\�ȍ��W�̃��X�g���쐬����
        /// </summary>
        private List<Vector2> candidatePositionSet(int offsetX = 1, int offsetY = 1)
        {
            List<Vector2> candidatePositions = new List<Vector2>();
            Vector2 setPosition = Vector2.zero;
            for (int y = 0; y < _stageSize.y - offsetY; y++)
            {
                for (int x = 0; x < _stageSize.x - offsetX; x++)
                {
                    if (RoomIdEqual(ToVector2(x, y), Vector2.zero, 0) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(0, offsetY), 0) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(offsetX, 0), 0) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(offsetX, offsetY), 0))
                    {
                        setPosition = ToVector2(x, y);
                        candidatePositions.Add(setPosition);
                    }
                }
            }
            return candidatePositions;
        }
        /// <summary>
        /// �Ǘ������������������邽�߂̊֐�
        /// </summary>
        private List<Vector2> candidateAislePosition(int offsetX = 0, int offsetY = 0)
        {
            if (offsetX == 0 && offsetY == 0) Debug.LogError("offset�̒l�������Ƃ�0�ł�");
            List<Vector2> candidatePositions = new List<Vector2>();
            Vector2 setPosition = Vector2.zero;
            for (int y = 0; y < _stageSize.y - offsetY; y++)
            {
                for (int x = 0; x < _stageSize.x - offsetX; x++)
                {
                    if (RoomIdEqual(ToVector2(x, y), Vector2.zero, 0) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(0, offsetY), 0) &&
                        RoomIdEqual(ToVector2(x, y), ToVector2(offsetX, 0), 0))
                    {
                        if (x >= 1)
                        {
                            if (RoomIdEqual(ToVector2(x, y), ToVector2(-1, 0), 0)) continue;
                        }
                        if (y >= 1)
                        {
                            if (RoomIdEqual(ToVector2(x, y), ToVector2(0, -1), 0)) continue;
                        }
                        if (offsetX != 0)
                        {
                            if (y < _stageSize.y - 1)
                            {
                                if (RoomIdEqual(ToVector2(x, y), ToVector2(0, 1), 0)) continue;
                            }

                        }
                        if (offsetY != 0)
                        {
                            if (x < _stageSize.x - 1)
                            {
                                if (RoomIdEqual(ToVector2(x, y), ToVector2(1, 0), 0)) continue;
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
        /// ���̏ꏊ�͕ǂ̃^�C�����������邽�߂̊֐�
        /// </summary>
        private List<Vector2> candidateNextWallPosition(int offsetX = 0, int offsetY = 0)
        {
            if (offsetX != 0 && offsetY != 0) { Debug.LogError("�����Ȉ����ł��B�ǂ��炩��0�ɂ��Ă�������"); }
            List<Vector2> candidatePositions = new List<Vector2>();
            Vector2 setPosition = Vector2.zero;
            int xLength = _stageGenerateData.GetLength(0);
            int yLength = _stageGenerateData.GetLength(1);
            for (int y = 0; y < yLength - offsetY; y++)
            {
                for (int x = 0; x < xLength - offsetX; x++)
                {
                    if (_stageGenerateData[x, y].RoomId == 0)
                    {
                        if (offsetX != 0)
                        {
                            if (x < xLength - 1 && offsetX > 0)
                            {
                                if (_stageGenerateData[x + offsetX, y].RoomId == 0) continue;
                            }
                            else if (x + offsetX >= 0)
                            {
                                if (_stageGenerateData[x + offsetX, y].RoomId == 0) continue;
                            }
                        }
                        else if (offsetY != 0)
                        {
                            if (y < yLength - 1 && offsetY > 0)
                            {
                                if (_stageGenerateData[x, y + offsetY].RoomId == 0) continue;
                            }
                            else if (y - offsetY >= 0)
                            {
                                if (_stageGenerateData[x, y + offsetY].RoomId == 0) continue;
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
        /// x����y���ɂP���ʘH�̍쐬
        /// </summary>
        private async UniTask GenerateAisle(CancellationToken token)
        {
            const int OFFSET = 2;//�ʘH�����Ȃ��͈�
            int xAisleNumber = GenerateXAisle((int)_stageSize.x - OFFSET, OFFSET);
            int yAisleNumber = GenerateYAisle((int)_stageSize.y - OFFSET, OFFSET);
            bool xSlide = false;
            bool ySlide = false;

            if (viewDebugLog) Debug.Log($"IslePosition => x = {xAisleNumber},y = {yAisleNumber}");
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
            //X����ʘH�����炷����
            for (int y = 0; y < _stageGenerateData.GetLength(1); y++)
            {
                xSlide = false;
                for (int x = 0; x < _stageGenerateData.GetLength(0); x++)
                {
                    if (xSlide == false)
                    {
                        if (x >= xAisleNumber && _stageGenerateData[x, y].RoomId != _stageGenerateData[x - 1, y].RoomId)
                        {
                            xSlide = true;
                        }
                    }
                    if (xSlide)
                    {
                        newStageGenerateData[x + 1, y] = _stageGenerateData[x, y];
                    }
                    else
                    {
                        newStageGenerateData[x, y] = _stageGenerateData[x, y];
                    }
                }
            }
            //y����ʘH�����炷����
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
            _stageGenerateData = tempXPlotData;
        }
        /// <summary>
        ///�@�����_����X���̒ʘ^�����ꏊ������
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
        /// �Ǘ����������𖄂߂�悤�ɕ������g������֐�
        /// </summary>
        private async UniTask RommShaping(CancellationToken token)
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
                if (item.x > 2)//�u���b�N�̍���room3x3������ꍇ
                {
                    if (_stageGenerateData[(int)item.x - 1, (int)item.y].RoomType == RoomType.room3x3)
                    {
                        int roomId = _stageGenerateData[(int)item.x - 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, ToVector2(-1, 1), roomId) &&
                           RoomIdEqual(item, ToVector2(-1, 2), roomId))
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
                if (item.x < _stageGenerateData.GetLength(0) - 2)//�u���b�N�̉E��room3x3������ꍇ
                {
                    if (_stageGenerateData[(int)item.x + 1, (int)item.y].RoomType == RoomType.room3x3)
                    {
                        int roomId = _stageGenerateData[(int)item.x + 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, 1), roomId) &&
                           RoomIdEqual(item, ToVector2(1, 2), roomId))
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
                if (item.y < _stageGenerateData.GetLength(1) - 2)//�u���b�N�̉���room3x3������ꍇ
                {
                    if (_stageGenerateData[(int)item.x, (int)item.y + 1].RoomType == RoomType.room3x3)
                    {
                        int roomId = _stageGenerateData[(int)item.x, (int)item.y + 1].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, 1), roomId) &&
                           RoomIdEqual(item, ToVector2(2, 1), roomId))
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
                if (item.y > 2)//�u���b�N�̏��room3x3������ꍇ
                {
                    if (_stageGenerateData[(int)item.x, (int)item.y - 1].RoomType == RoomType.room3x3)
                    {
                        int roomId = _stageGenerateData[(int)item.x, (int)item.y - 1].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, -1), roomId) &&
                           RoomIdEqual(item, ToVector2(2, -1), roomId))
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
                if (item.x > 1)//�u���b�N�̍���room2x2������ꍇ
                {
                    if (_stageGenerateData[(int)item.x - 1, (int)item.y].RoomType == RoomType.room2x2)
                    {
                        int roomId = _stageGenerateData[(int)item.x - 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, ToVector2(-1, 1), roomId))
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
                if (item.x < _stageGenerateData.GetLength(0) - 1)//�u���b�N�̉E��room2x2������ꍇ
                {
                    if (_stageGenerateData[(int)item.x + 1, (int)item.y].RoomType == RoomType.room2x2)
                    {
                        int roomId = _stageGenerateData[(int)item.x + 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, 1), roomId))
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
                if (item.y < _stageGenerateData.GetLength(1) - 1)//�u���b�N��艺��room2x2������
                {
                    if (_stageGenerateData[(int)item.x, (int)item.y + 1].RoomType == RoomType.room2x2)
                    {
                        int roomId = _stageGenerateData[(int)item.x, (int)item.y + 1].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, 1), roomId))
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
                if (item.y > 1)//�u���b�N�����room2x2������
                {
                    if (_stageGenerateData[(int)item.x, (int)item.y - 1].RoomType == RoomType.room2x2)
                    {
                        int roomId = _stageGenerateData[(int)item.x, (int)item.y - 1].RoomId;
                        if (RoomIdEqual(item, ToVector2(1, -1), roomId))
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
        /// <summary>
        /// �ǂ�ݒu����X�N���v�g
        /// </summary>
        private async UniTask GenerateWall(CancellationToken token)
        {
            Vector3 instantiatePosition = Vector3.zero;
            var _xWallPos = candidateNextWallPosition(offsetX: 1);
            var _yWallPos = candidateNextWallPosition(offsetY: 1);
            foreach (var xWall in _xWallPos)
            {
                instantiatePosition = ToVector3(xWall.x * tileSize, 0, xWall.y * tileSize);
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
                instantiatePosition = ToVector3(yWall.x * tileSize, 0, yWall.y * tileSize);
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
        private bool RoomIdEqual(Vector2 basePosition, Vector2 position1, int roomId)
        {
            if (_stageGenerateData[(int)(basePosition.x + position1.x), (int)(basePosition.y + position1.y)].RoomId == roomId)
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
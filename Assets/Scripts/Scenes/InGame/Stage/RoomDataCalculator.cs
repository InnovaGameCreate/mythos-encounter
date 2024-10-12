using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Scenes.Ingame.Stage
{
    public class RoomSizeDictionary : Dictionary<RoomType, Vector2>
    {
        //�����`�̃X�e�[�W�̏c���̑傫����Ԃ�
        public int GetSquareRoomSize(RoomType type)
        {
            Vector2 RoomSize = this[type];
            if (RoomSize.x != RoomSize.y)
                Debug.LogError("�����`�̕����ł͂���܂���.");

            return (int)RoomSize.x;
        }
    }
    public class RoomDataCalculator
    {
        /*readonly RoomSizeDictionary RoomTypeOfSize = new RoomSizeDictionary
        {
            {RoomType.room2x2, new Vector2(2, 2)},
            {RoomType.room2x2Stair, new Vector2(2, 2)},
            {RoomType.room2x3, new Vector2(2, 3)},
            {RoomType.room3x2, new Vector2(3, 2)},
            {RoomType.room3x3, new Vector2(3, 3)},
            {RoomType.room3x3Stair, new Vector2(3, 3)},
            {RoomType.room3x4, new Vector2(3, 4)},
            {RoomType.room4x3, new Vector2(4, 3)},
            {RoomType.room4x4, new Vector2(4, 4)}
        };


        readonly Vector2 _stageSize;
        RoomData[,] targetFloor;
        int _roomId = 0;
        List<Vector2> candidatePosition = new List<Vector2>();  //������ݒu�\�ȏꏊ��ۑ����Ă���(�����̍�����W)

        readonly int LARGEROOM = 20;
        readonly int MEDIUMROOM = 12;
        readonly int SMALLROOM = 8;
        readonly int OFFSET = 2;//�ʘH�����Ȃ��͈�
        public RoomDataCalculator(Vector2 stageSize)
        {
            _stageSize = stageSize;
            targetFloor = new RoomData[(int)_stageSize.x, (int)_stageSize.y];   //�X�e�[�W�T�C�Y���̔z��m��
        }

        public void CalcRoomData()
        {
            RoomPlotId(RoomType.room3x3Stair, new Vector2((int)_stageSize.x - 3, (int)_stageSize.y - 3), targetFloor);  //�m��̊K�i�����f�[�^�̓���
            RandomFullSpaceRoomPlot(targetFloor, LARGEROOM, MEDIUMROOM, SMALLROOM);

            RoomShaping(targetFloor);                                          //��Ԃ𖄂߂�悤�ɕ����̑傫���𒲐�
            targetFloor = GenerateAisle(targetFloor);

            switch (floor)
            {
                case 1:
                    _stairPosition.Add(new Vector2((int)_stageSize.x, (int)_stageSize.y));
                    _firsrFloorData = targetFloor;
                    break;
                case 2:
                    _secondFloorData = targetFloor;
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// �f�[�^��ɕ����̃f�[�^��o�^����
        /// </summary>
        /// <param name="plotRoomType">���[���̑傫��</param>
        /// <param name="plotPosition">���[���̐ݒ�ʒu</param>
        /// <param name="updateRoomId">RoomId���X�V���邩�ǂ���</param>
        void RoomPlotId(RoomType plotRoomType, Vector2 plotPosition, RoomData[,] stage, bool updateRoomId = true)
        {
            int inputRoomId = updateRoomId ? 1 + _roomId++ : stage[(int)plotPosition.x, (int)plotPosition.y].RoomId;

            Vector2 plotRoomSize = RoomTypeOfSize[plotRoomType];    //�����̃^�C�v���̃T�C�Y���擾

            for (int y = 0; y < plotRoomSize.y; y++)
            {
                for (int x = 0; x < plotRoomSize.x; x++)
                {
                    stage[(int)plotPosition.x + x, (int)plotPosition.y + y].RoomDataSet(plotRoomType, inputRoomId);
                }
            }
        }

        /// <summary>
        /// �}�b�v�ɑ傫�����Ƀ����_���ɕ��������蓖�Ă�
        /// </summary>
        /// <param name="smallRoom">2x2�̃T�C�Y�̕����𐶐����鐔</param>
        /// <param name="mediumRoom">3x3�̃T�C�Y�̕����𐶐����鐔</param>
        /// <param name="largeRoom">4x4�̃T�C�Y�̕����𐶐����鐔</param>
        void RandomFullSpaceRoomPlot(RoomData[,] stage, int smallRoom = 0, int mediumRoom = 0, int largeRoom = 0)
        {
            int roomSize = 3;
            Vector2 roomPosition = Vector2.zero;
            while (roomSize > 0)
            {
                candidatePosition = candidatePositionSet(stage, roomSize);
                int roomPositionIndex = UnityEngine.Random.Range(0, candidatePosition.Count);
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
        /// ���[����z�u�\�ȍ��W�̃��X�g���쐬����
        /// </summary>
        private List<Vector2> candidatePositionSet(RoomData[,] stage, int size)
        {
            List<Vector2> candidatePositions = new List<Vector2>();
            Vector2 RoomSize = new Vector2(size, size);
            for (int y = 0; y < _stageSize.y - size; y++)
            {
                for (int x = 0; x < _stageSize.x - size; x++)
                {
                    if (IsCanSetRoom(stage, new Vector2(x, y), RoomSize))
                    {
                        candidatePositions.Add(new Vector2(x, y));
                    }
                }
            }
            return candidatePositions;
        }

        /// <summary>
        /// �Ǘ������������������邽�߂̊֐�
        /// </summary>
        List<Vector2> candidateAislePosition(RoomData[,] stage, int offsetX = 0, int offsetY = 0)
        {
            if (offsetX == 0 && offsetY == 0) Debug.LogError("offset�̒l�������Ƃ�0�ł�");
            List<Vector2> candidatePositions = new List<Vector2>();
            for (int y = 0; y < _stageSize.y - offsetY; y++)
            {
                for (int x = 0; x < _stageSize.x - offsetX; x++)
                {
                    if (RoomIdEqual(new Vector2(x, y), Vector2.zero, 0, stage) && //
                        RoomIdEqual(new Vector2(x, y), new Vector2(0, offsetY), 0, stage) &&
                        RoomIdEqual(new Vector2(x, y), new Vector2(offsetX, 0), 0, stage))
                    {
                        if (x >= 1)
                        {
                            if (RoomIdEqual(new Vector2(x, y), new Vector2(-1, 0), 0, stage)) continue;
                        }
                        if (y >= 1)
                        {
                            if (RoomIdEqual(new Vector2(x, y), new Vector2(0, -1), 0, stage)) continue;
                        }
                        if (offsetX != 0)
                        {
                            if (y < _stageSize.y - 1)
                            {
                                if (RoomIdEqual(new Vector2(x, y), new Vector2(0, 1), 0, stage)) continue;
                            }
                        }
                        if (offsetY != 0)
                        {
                            if (x < _stageSize.x - 1)
                            {
                                if (RoomIdEqual(new Vector2(x, y), new Vector2(1, 0), 0, stage)) continue;
                            }
                        }
                        candidatePositions.Add(new Vector2(x, y));
                    }
                }
            }
            return candidatePositions;
        }

        //����̑傫���ŃZ�b�g�\�Ȉʒu��T��
        bool IsCanSetRoom(RoomData[,] stage, Vector2 basePosition, Vector2 roomSize)
        {
            int x = (int)basePosition.x;
            int y = (int)basePosition.y;

            //�����̎l�����ǂ̕����ɂ�����Ă��Ȃ����true��Ԃ�
            if (RoomIdEqual(new Vector2(x, y), Vector2.zero, 0, stage) &&   
               RoomIdEqual(new Vector2(x, y), new Vector2(0, roomSize.y), 0, stage) &&
               RoomIdEqual(new Vector2(x, y), new Vector2(roomSize.x, 0), 0, stage) &&
               RoomIdEqual(new Vector2(x, y), new Vector2(roomSize.x, roomSize.y), 0, stage))
                return true;

            return false;
        }

        bool RoomIdEqual(Vector2 basePosition, Vector2 addPosition, int roomId, RoomData[,] stage)
        {
            if (stage[(int)(basePosition.x + addPosition.x), (int)(basePosition.y + addPosition.y)].RoomId == roomId)
            {
                return true;
            }
            return false;
        }

        void RoomShaping(RoomData[,] stage)
        {
            var _only1x4Aisle = candidateAislePosition(stage, offsetY: 3);
            var _only4x1Aisle = candidateAislePosition(stage, offsetX: 3);
            var _only1x3Aisle = candidateAislePosition(stage, offsetY: 2);
            var _only3x1Aisle = candidateAislePosition(stage, offsetX: 2);
            var _only1x2Aisle = candidateAislePosition(stage, offsetY: 1);
            var _only2x1Aisle = candidateAislePosition(stage, offsetX: 1);
            _only1x2Aisle = _only1x2Aisle.Except(_only1x3Aisle).ToList();
            _only2x1Aisle = _only2x1Aisle.Except(_only3x1Aisle).ToList();
            _only1x3Aisle = _only1x3Aisle.Except(_only1x4Aisle).ToList();
            _only3x1Aisle = _only3x1Aisle.Except(_only4x1Aisle).ToList();
            foreach (var item in _only1x3Aisle)
            {
                if (item.x > 2)//�u���b�N�̍���room3x3������ꍇ
                {
                    if (stage[(int)item.x - 1, (int)item.y].RoomType == RoomType.room3x3)
                    {
                        int roomId = stage[(int)item.x - 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, new Vector2(-1, 1), roomId, stage) &&
                           RoomIdEqual(item, new Vector2(-1, 2), roomId, stage))
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
                if (item.x < stage.GetLength(0) - 2)//�u���b�N�̉E��room3x3������ꍇ
                {
                    if (stage[(int)item.x + 1, (int)item.y].RoomType == RoomType.room3x3)
                    {
                        int roomId = stage[(int)item.x + 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, new Vector2(1, 1), roomId, stage) &&
                           RoomIdEqual(item, new Vector2(1, 2), roomId, stage))
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
                if (item.y < stage.GetLength(1) - 2)//�u���b�N�̉���room3x3������ꍇ
                {
                    if (stage[(int)item.x, (int)item.y + 1].RoomType == RoomType.room3x3)
                    {
                        int roomId = stage[(int)item.x, (int)item.y + 1].RoomId;
                        if (RoomIdEqual(item, new Vector2(1, 1), roomId, stage) &&
                           RoomIdEqual(item, new Vector2(2, 1), roomId, stage))
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
                if (item.y > 2)//�u���b�N�̏��room3x3������ꍇ
                {
                    if (stage[(int)item.x, (int)item.y - 1].RoomType == RoomType.room3x3)
                    {
                        int roomId = stage[(int)item.x, (int)item.y - 1].RoomId;
                        if (RoomIdEqual(item, new Vector2(1, -1), roomId, stage) &&
                           RoomIdEqual(item, new Vector2(2, -1), roomId, stage))
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
                if (item.x > 1)//�u���b�N�̍���room2x2������ꍇ
                {
                    if (stage[(int)item.x - 1, (int)item.y].RoomType == RoomType.room2x2)
                    {
                        int roomId = stage[(int)item.x - 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, new Vector2(-1, 1), roomId, stage))
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
                if (item.x < stage.GetLength(0) - 1)//�u���b�N�̉E��room2x2������ꍇ
                {
                    if (stage[(int)item.x + 1, (int)item.y].RoomType == RoomType.room2x2)
                    {
                        int roomId = stage[(int)item.x + 1, (int)item.y].RoomId;
                        if (RoomIdEqual(item, new Vector2(1, 1), roomId, stage))
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
                if (item.y < stage.GetLength(1) - 1)//�u���b�N��艺��room2x2������
                {
                    if (stage[(int)item.x, (int)item.y + 1].RoomType == RoomType.room2x2)
                    {
                        int roomId = stage[(int)item.x, (int)item.y + 1].RoomId;
                        if (RoomIdEqual(item, new Vector2(1, 1), roomId, stage))
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
                if (item.y > 1)//�u���b�N�����room2x2������
                {
                    if (stage[(int)item.x, (int)item.y - 1].RoomType == RoomType.room2x2)
                    {
                        int roomId = stage[(int)item.x, (int)item.y - 1].RoomId;
                        if (RoomIdEqual(item, new Vector2(1, -1), roomId, stage))
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

        RoomData[,] GenerateAisle(RoomData[,] stage)
        {
            int xAisleNumber = GenerateXAisle(stage, (int)_stageSize.x - OFFSET, OFFSET);
            int yAisleNumber = GenerateYAisle(stage, (int)_stageSize.y - OFFSET, OFFSET);
            bool xSlide = false;
            bool ySlide = false;

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
            //X����ʘH�����炷����
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
            var tempXPlotData = new RoomData[newStageGenerateData.GetLength(0), newStageGenerateData.GetLength(1)];
            for (int i = 0; i < (int)_stageSize.y + 1; i++)
            {
                for (int j = 0; j < (int)_stageSize.x + 1; j++)
                {
                    tempXPlotData[i, j] = initialData;
                }
            }
            //y����ʘH�����炷����
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
        ///�@�����_����X���̒ʘ^�����ꏊ������
        /// </summary>
        int GenerateXAisle(RoomData[,] stage, int max, int min = 0)
        {
            int value = UnityEngine.Random.Range(min, max);

            var _onlyXAisle = candidateAislePosition(stage, offsetX: 4);
            if (_onlyXAisle.Any(e => e.y == value))
            {
                value = GenerateXAisle(stage, min, max);
            }
            return value;
        }

        int GenerateYAisle(RoomData[,] stage, int max, int min = 0)
        {
            int value = UnityEngine.Random.Range(min, max);

            var _onlyYAisle = candidateAislePosition(stage, offsetY: 4);
            if (_onlyYAisle.Any(e => e.x == value))
            {
                value = GenerateYAisle(stage, min, max);
            }
            return value;
        }*/
    }


}


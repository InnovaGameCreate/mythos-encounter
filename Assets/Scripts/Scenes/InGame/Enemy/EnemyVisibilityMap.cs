using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Scenes.Ingame.Stage;
using UniRx;
using UnityEngine.UIElements;


namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// �}�b�v�̎����̒ʂ���ƃ}�b�v�̂ǂ̂�������ǂ̂��炢�m�F���������L�^���Ă䂭�N���X�B�G�L�������}�b�v��F������̂Ɏg�p�����N���X�B
    /// </summary>
    public class EnemyVisibilityMap : MonoBehaviour
    {
        public List<List<VisivilityArea>> visivilityAreaGrid;//Unity�̍��W�n��D��A��ڂ�x����ڂ�y���̃C���[�W������[0][0]���オ[0][max]
        public List<List<byte>> areaWatchNumGrid;
        public float maxVisivilityRange;//���̋����𒴂��Ă���G���A�͌����邱�Ƃ͂Ȃ����̂Ƃ���
        public bool debugMode;
        public float gridRange;
        public Vector3 centerPosition;//�����΂񍶉��̃O���b�h�̒���

        private List<StageDoor> _stageDoors;//�X�e�[�W

        private readonly CompositeDisposable _compositeDisposable = new();

        /// <summary>�}�X�ڂ̈ʒu��2��byte�ŕ\���a�̃}�X�ڂ܂ł̋�����foat�ł���킵�Ă���</summary>
        public struct DoubleByteAndMonoFloat
        {//�ʒu�Ƌ���
            public byte x;
            public byte z;
            public float range;
            
            public DoubleByteAndMonoFloat(byte sX, byte sZ, float sRange)
            {
                x = sX;
                z = sZ;
                range = sRange;
            }
        }
        

        //#############################
        ///������������B
        ///�܂����Ɍ����G���A�̃f�[�^�̃��X�g��2�ɂ���B�Е��͔񓯊��^�Ŏ��E�f�[�^���Čv�Z���Ă��`�F���W����̂ɗ��p����iforeach���Ă��Ȃ��ꏊ�ōČv�Z�̃f�[�^�����ۂɎg���f�[�^�ւƕ��荞�ށj
        ///���Ɍ����񐔂�ʂ̏ꏊ�ɕۑ�����B����͂��̃��X�g���V�����[�R�s�[�ŕʕʂ̓G�ŋ��L���ď�肭�g�����߂ł���B
        ///�܂�Ƃ���h�A�̕ω��ɂ��鎋�E�̕ω��͂�����̃X�N���v�g�̎��̂ɂ����Ă̂݉��Z����āA���܂���������̂ł���B
        ///
        //#############################

        /// <summary>
        /// ���̃}�X�ڂ��王���̒ʂ�}�X�ڂ�List�ŋL�^���Ă���
        /// </summary>
        public struct VisivilityArea
        {
            //public byte watchNum;//���̃G���A��������
            public List<DoubleByteAndMonoFloat> canVisivleAreaPosition;//���̃G���A���猩�邱�Ƃ̂ł���G���A
            public List<DoubleByteAndMonoFloat> defaultCanVisivilityAreaPosition;

            public VisivilityArea(List<DoubleByteAndMonoFloat> setCanVisivleAreaPosition, List<DoubleByteAndMonoFloat> setDefaultCanVisivilityAreaPosition) { 
                canVisivleAreaPosition = setCanVisivleAreaPosition;
                defaultCanVisivilityAreaPosition = setDefaultCanVisivilityAreaPosition;
            }
        }

        private void Start()
        {
            /*
            GridMake(9,9,5.8f,new Vector3(-46.4f,1,-43.7f));
            MapScan();
            */
        }

        /// <summary>
        /// �}�X�ڂ̏W���ł���񎟌�List���쐬����B
        /// </summary>
        /// <param name="x">x���W�����Ƀ}�X�ڂ��������ׂ邩</param>
        /// <param name="z">z���W�����Ƀ}�X�ڂ��������ׂ邩</param>
        /// <param name="range">���̋����ȏ�̎����͒ʂ�Ȃ����̂ƍl���ăV�~�����[�g����鋗��</param>
        /// <param name="setCenterPosition">�����̃}�X�ڂ̒��S�ʒu</param>
        public void GridMake(byte x, byte z, float range, Vector3 setCenterPosition)
        { //�}�b�v���쐬�Bx��z�̓O���b�h�̔z�u���Brange�̓O���b�h�̋����BcenterPosition�͍����̈ʒu
            if (debugMode) Debug.Log("�O���b�h�쐬�J�n");
            visivilityAreaGrid = new List<List<VisivilityArea>>();
            gridRange = range;
            centerPosition = setCenterPosition;
            for (byte i = 0; i < x; i++)
            { //�z��̗v�f���쐬
                List<VisivilityArea> item = new List<VisivilityArea>();
                for (byte j = 0; j < z; j++)
                {
                    VisivilityArea addArea = new VisivilityArea();
                    addArea.canVisivleAreaPosition = new List<DoubleByteAndMonoFloat>();
                    //addArea.defaultCanVisivilityAreaPosition = new List<DoubleByteAndMonoFloat>();
                    item.Add(addArea);

                    if (debugMode) Debug.DrawLine(setCenterPosition + new Vector3(i, 0, j) * range, setCenterPosition + new Vector3(i, 0, j) * range + new Vector3(0, 10, 0), Color.yellow, 10);//�O���b�h�̈ʒu��\��
                }
                visivilityAreaGrid.Add(item);
            }

            areaWatchNumGrid = new List<List<byte>>();
            //�����񐔂Ɋւ���z��̗v�f���쐬
            for (byte i = 0; i < x; i++)
            { //�z��̗v�f���쐬
                List<byte> item = new List<byte>();
                for (byte j = 0; j < z; j++)
                {
                    item.Add(0);
                }
                areaWatchNumGrid.Add(item);
            }


            if (debugMode) Debug.Log("firstSize(x)" + visivilityAreaGrid.Count());
            if (debugMode) Debug.Log("SecondSize(z)" + visivilityAreaGrid[0].Count());

            if (debugMode) Debug.Log("watchNumFirstSize(x)" + areaWatchNumGrid.Count());
            if (debugMode) Debug.Log("watchNumSecondSize(z)" + areaWatchNumGrid[0].Count());
        }

        /// <summary>
        /// �}�b�v���X�L�������ă}�X�ړ��m�ł̎��E�̒ʂ��Ă���������肷��
        /// </summary>
        public void MapScan()
        {//�}�b�v���X�L�������Ď��ۂ̎��E���ǂ̂悤�ɒʂ��Ă��邩��ݒ�
            if (debugMode) Debug.Log("�}�b�v�X�L�����J�n");
            //�e�}�X�ڂւƃA�N�Z�X
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    //�Ώۂ̃}�X���瑼�̃}�X�ڂ������邩���m�F
                    for (byte vX = 0; vX < visivilityAreaGrid.Count(); vX++)
                    {
                        for (byte vZ = 0; vZ < visivilityAreaGrid.Count(); vZ++)
                        {
                            if ((x != vX) || (z != vZ))
                            { //�������g�ł͂Ȃ��ꍇ                               
                                float range2 = Mathf.Pow((x - vX) * 5.8f, 2) + Mathf.Pow((z - vZ) * 5.8f, 2);
                                if (range2 <= Mathf.Pow(maxVisivilityRange, 2))
                                { //���E���ʂ�Ƃ���鋗���łȂ��ꍇ
                                    float range = Mathf.Sqrt(range2);//�����������߂�̂͂������R�X�g���d���炵���̂Ŋm���Ɍv�Z���K�v�ɂȂ��Ă��炵�Ă܂�
                                    //���E���ʂ邩��Ray���ʂ邩
                                    bool hit;
                                    Ray ray = new Ray(centerPosition + new Vector3(x * gridRange, 1, z * gridRange), new Vector3(vX - x, 0, vZ - z));
                                    hit = Physics.Raycast(ray, out RaycastHit hitInfo, range,  2048, QueryTriggerInteraction.Collide);
                                    if (!hit)
                                    { //���ɂ��������Ă��Ȃ������ꍇ
                                        if (debugMode) Debug.DrawRay(ray.origin, ray.direction * range, Color.green, 10);
                                        visivilityAreaGrid[x][z].canVisivleAreaPosition.Add(new DoubleByteAndMonoFloat(vX, vZ, range));
                                    }

                                }
                            }

                        }
                    }
                }
            }
            //�����܂ŗ��ă}�b�v�X�L�������I���
            if (debugMode) Debug.Log("�}�b�v�̃X�L�������������܂���");
        }



        /// <summary>
        /// ���g�̌����񐔂݂̂�Ɨ��������R�s�[���쐬���ĕԂ�
        /// </summary>
        /// <returns>���g�̃f�B�[�v�R�s�[</returns>
        public EnemyVisibilityMap Copy()
        {
            if (debugMode) Debug.Log("�R�s�[�J�n");
            EnemyVisibilityMap copy;
            copy = new EnemyVisibilityMap();
            copy.visivilityAreaGrid = visivilityAreaGrid;

            copy.gridRange = gridRange;
            copy.maxVisivilityRange = maxVisivilityRange;
            copy.debugMode = debugMode;
            copy.centerPosition = centerPosition;

            copy.areaWatchNumGrid = new List<List<byte>>();
            //�����񐔂Ɋւ���z��̗v�f���쐬
            for (byte i = 0; i < copy.visivilityAreaGrid.Count; i++)
            { //�z��̗v�f���쐬
                List<byte> item = new List<byte>();
                for (byte j = 0; j < copy.visivilityAreaGrid[0].Count; j++)
                {
                    item.Add(0);
                }
                copy.areaWatchNumGrid.Add(item);
            }


            if (debugMode)
            { //�}�X�ڂ̏�񂪐���ɃR�s�[�ł��Ă��邩��\������
                for (byte x = 0; x < copy.visivilityAreaGrid.Count(); x++)
                {
                    for (byte z = 0; z < copy.visivilityAreaGrid[0].Count(); z++)
                    {
                        Debug.DrawLine(copy.centerPosition + new Vector3(x, 0, z) * copy.gridRange, copy.centerPosition + new Vector3(x, 0, z) * copy.gridRange + new Vector3(0, 10, 0), Color.green, 10);
                    }
                }
            }
            return copy;
        }



        /// <summary>
        /// �����񐔂̃��X�g�����S�ɓƗ��������̂ɂ��܂��B
        /// </summary>
        public void OriginWatchNum() {
            areaWatchNumGrid = new List<List<byte>>();
            //�����񐔂Ɋւ���z��̗v�f���쐬
            for (byte i = 0; i < visivilityAreaGrid.Count; i++)
            { //�z��̗v�f���쐬
                List<byte> item = new List<byte>();
                for (byte j = 0; j < visivilityAreaGrid[0].Count; j++)
                {
                    item.Add(0);
                }
                areaWatchNumGrid.Add(item);
            }
        }

        /// <summary>
        /// �h�A���X�L��������
        /// </summary>
        public void BeScanner()
        {
            Debug.Log("�X�L�����J�n");
            List<GameObject> doors = new List<GameObject>();
            _stageDoors = new List<StageDoor>();
            doors = GameObject.FindGameObjectsWithTag("Door").ToList();

            foreach (GameObject door in doors)
            {
                Debug.LogWarning(door);
                if (door.TryGetComponent<StageDoor>(out StageDoor sd))
                {
                    _stageDoors.Add(sd);
                }
                else {
                    Debug.Log(door.name +  "��Tg��Door�ł���StageDoor���Ȃ�");
                }
               
            }

            //�f�t�H���g�ɏ����R�s�[
            for (byte i = 0; i < visivilityAreaGrid[0].Count;i++) {
                for (byte j = 0; j < visivilityAreaGrid[i].Count; j++)
                {
                    visivilityAreaGrid[i][j] = new VisivilityArea(visivilityAreaGrid[i][j].canVisivleAreaPosition,new List<DoubleByteAndMonoFloat>());
                    for (byte a = 0; a < visivilityAreaGrid[i][j].canVisivleAreaPosition.Count; a++) {
                        visivilityAreaGrid[i][j].defaultCanVisivilityAreaPosition.Add(visivilityAreaGrid[i][j].canVisivleAreaPosition[a]);//�������ւ�
                    }

                }
            }
            Debug.LogWarning(_stageDoors.Count);
            foreach (StageDoor stageDoorCs in _stageDoors)
            {
                Debug.LogWarning("�T�u�X�N���C�u�����O");
                Debug.LogWarning(_stageDoors.Count);
                stageDoorCs.OnChangeDoorOpen.Subscribe(_ =>
                {
                    Debug.LogWarning("�h�A�̕ύX�����m����");
                    //�����Ȃ��Ƃ���������Ȃ��悤�ɂ���
                    for (byte x = 0; x < visivilityAreaGrid.Count; x++)
                    {
                        for (byte z = 0; z < visivilityAreaGrid[x].Count; z++)
                        {
                            //��U�����镔�������Z�b�g
                            visivilityAreaGrid[x][z] = new VisivilityArea(new List<DoubleByteAndMonoFloat>(), visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition);

                            for (byte a = 0; a < visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition.Count; a++)//���ۂɌ����镔���̂݌�����悤�ɕύX���Ă䂭
                            {
                                float range = Mathf.Sqrt(Mathf.Pow((x - visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a].x) * gridRange, 2) + Mathf.Pow((z - visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a].z) * gridRange, 2));
                                Ray ray = new Ray(centerPosition + new Vector3(x * gridRange, 1, z * gridRange), new Vector3(visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a].x - x, 0, visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a].z - z) * range);
                                bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, 4098, QueryTriggerInteraction.Collide);
                                if (hit)
                                {
                                    if (debugMode)
                                    {
                                        Debug.DrawRay(ray.origin,ray.direction * range,Color.red,2);
                                    }
                                }
                                else
                                {
                                    visivilityAreaGrid[x][z].canVisivleAreaPosition.Add(visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a]);
                                }
                            }
                        }
                    }
                }).AddTo(_compositeDisposable);
            }
            //�����Ȃ��Ƃ���������Ȃ��悤�ɂ���
            for (byte x = 0; x < visivilityAreaGrid.Count; x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[x].Count; z++)
                {
                    //��U�����镔�������Z�b�g
                    visivilityAreaGrid[x][z] = new VisivilityArea(new List<DoubleByteAndMonoFloat>(), visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition);

                    for (byte a = 0; a < visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition.Count; a++)//���ۂɌ����镔���̂݌�����悤�ɕύX���Ă䂭
                    {
                        float range = Mathf.Sqrt(Mathf.Pow((x - visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a].x) * gridRange, 2) + Mathf.Pow((z - visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a].z) * gridRange, 2));
                        Ray ray = new Ray(centerPosition + new Vector3(x * gridRange, 1, z * gridRange), new Vector3(visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a].x - x, 0, visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a].z - z) * range);
                        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, 4098, QueryTriggerInteraction.Collide);
                        if (hit)
                        {
                            if (debugMode)
                            {
                                Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 2);
                            }
                        }
                        else
                        {
                            visivilityAreaGrid[x][z].canVisivleAreaPosition.Add(visivilityAreaGrid[x][z].defaultCanVisivilityAreaPosition[a]);
                        }
                    }
                }
            }


        }

        /// <summary>
        /// ���Ɋm�F���ׂ��ł����Ă��炸�ł��߂��ʒu���擾�B
        /// </summary>
        /// <param name="nowPosition">���݂�character�̍��W</param>
        /// <returns>���ɍs���ׂ����W</returns>
        public Vector3 GetNextNearWatchPosition(Vector3 nowPosition)
        {
            if (debugMode) Debug.Log("���̈ړ�����擾");
            List<byte> nextPositionX = new List<byte>();
            List<byte> nextPositionZ = new List<byte>();
            byte smallestWatchNum = byte.MaxValue;
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    if (smallestWatchNum > areaWatchNumGrid[x][z]) { smallestWatchNum = areaWatchNumGrid[x][z]; }
                }
            }


            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    if (areaWatchNumGrid[x][z] == smallestWatchNum)
                    { //�ł��������ꍇ
                        nextPositionX.Add(x);
                        nextPositionZ.Add(z);
                    }
                    areaWatchNumGrid[x][z] = (byte)(areaWatchNumGrid[x][z] - smallestWatchNum);
                }
            }
            //�ł��߂��v�f���l����
            float nearDistance = float.MaxValue;
            byte nearPositionX = 0; byte nearPositionZ = 0;
            for (short i = 0; i < nextPositionX.Count; i++)
            {
                if (nearDistance > Vector3.Magnitude(nowPosition - (centerPosition + new Vector3(nextPositionX[i], 0, nextPositionZ[i]) * gridRange)))
                {
                    nearDistance = Vector3.Magnitude(nowPosition - (centerPosition + new Vector3(nextPositionX[i], 0, nextPositionZ[i]) * gridRange));
                    nearPositionX = nextPositionX[i];
                    nearPositionZ = nextPositionZ[i];
                }
            }

            //���ۂɎ����ɍs���ׂ����W������
            Vector3 nextPosition = (new Vector3(nearPositionX, 0, nearPositionZ) * gridRange) + centerPosition;
            if (debugMode)
            {//���ɍs���ׂ��ʒu��`��
                Debug.DrawLine(nextPosition, nextPosition + new Vector3(0, 20, 0), Color.magenta, 3);
            }
            return nextPosition;
        }

        /// <summary>
        /// ������ꏊ���猩���}�X�ڂ̌����񐔂̃J�E���g�𑝉�������
        /// </summary>
        /// <param name="nowPosition">���݂̍��W</param>
        /// <param name="visivilityRange">���E�̒���</param>
        public void CheckVisivility(Vector3 nowPosition, float visivilityRange)
        {
            if (debugMode) Debug.Log("���E�̒ʂ���`�F�b�N");
            VisivilityArea newVisivilityArea;
            if ((nowPosition.x < centerPosition.x + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < nowPosition.x))
            {//x���W���}�b�v�͈͓̔��ł��邩�ǂ���
                if ((nowPosition.z < centerPosition.z + (visivilityAreaGrid[0].Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < nowPosition.z)) //z���W���}�b�v�͈͓̔��ł��邩�ǂ���
                {
                    if (debugMode) Debug.Log("�}�b�v�͈͓̔��ł�");
                    byte myPositionx, myPositionz;//�������ǂ��̃O���b�h�ɂ��邩���m�F����
                    myPositionx = (byte)Mathf.FloorToInt((float)(nowPosition.x - centerPosition.x + 0.5 * gridRange) / gridRange);
                    myPositionz = (byte)Mathf.FloorToInt((float)(nowPosition.z - centerPosition.z + 0.5 * gridRange) / gridRange);
                    foreach (DoubleByteAndMonoFloat item in visivilityAreaGrid[myPositionx][myPositionz].canVisivleAreaPosition)
                    {
                        if (item.range < visivilityRange)
                        { //�����鋗��

                            //�����񐔂𑫂��B�������\���̂�List��For���̒��ł�����Ȃ��̂ŃR�s�[���Ă������ď���������B�I�[�o�[�t���[���Ȃ��ꍇ
                            if (areaWatchNumGrid[item.x][item.z]< byte.MaxValue)
                            {
                                areaWatchNumGrid[item.x][item.z] = (byte)(areaWatchNumGrid[item.x][item.z] + 1);
                            }

                            if (debugMode)
                            {//�����G���A����ŕ\��
                                Debug.DrawLine(centerPosition + new Vector3(myPositionx, 0, myPositionz) * gridRange, centerPosition + new Vector3(item.x, 0, item.z) * gridRange, Color.green, 1f);
                            }
                        }
                    }
                    //������������ꏊ�Ɍ����񐔂𑫂��B�������\���̂�List��For���̒��ł�����Ȃ��̂ŃR�s�[���Ă������ď���������
                    if (areaWatchNumGrid[myPositionx][myPositionz] < byte.MaxValue)
                    {
                        areaWatchNumGrid[myPositionx][myPositionz] = (byte)(areaWatchNumGrid[myPositionx][myPositionz] + 1);
                    }
                }
                else
                {
                    if (debugMode) Debug.Log("z���W���}�b�v����͂ݏo�Ă��܂�");
                }

            }
            else
            {
                if (debugMode) Debug.Log("x���W���}�b�v����͂ݏo�Ă��܂�");
            }

            if (debugMode)
            { //�e�}�X�ڂ��ǂꂾ�������Ă��邩���m�F����
                for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                {
                    for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                    {
                        Color drawColor;
                        if (areaWatchNumGrid[x][z] < 25) { drawColor = new Color32((byte)(10 * areaWatchNumGrid[x][z]), 0, (byte)(byte.MaxValue - (10 * areaWatchNumGrid[x][z])), byte.MaxValue); }
                        else
                        {
                            drawColor = Color.red;
                        }

                        Debug.DrawLine(centerPosition + new Vector3(x, 0, z) * gridRange, centerPosition + new Vector3(x, 0, z) * gridRange + new Vector3(0, 10, 0), drawColor, 1f);
                    }
                }
            }
        }

        /// <summary>
        /// ����̈ʒu���特���������Ă����ꍇ�̏���
        /// </summary>
        /// <param name="position">�����̍��W</param>
        /// <param name="resetRange">���������݂���ł��낤�Ƃ������őΏۂƂ���͈�</param>
        /// <param name="periodic">����I�ȃ`�F�b�N�ɂ���ČĂяo���ꂽ�̂��ǂ���</param>
        public void HearingSound(Vector3 position, float resetRange, bool periodic)
        {
            if (debugMode) Debug.Log("����ʒu���畷�����Ă������ɂ��đΏ�");
            VisivilityArea newVisivilityArea;
            if ((position.x < centerPosition.x + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < position.x))
            {//x���W���}�b�v�͈͓̔��ł��邩�ǂ���
                if ((position.z < centerPosition.z + (visivilityAreaGrid[0].Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < position.z)) //z���W���}�b�v�͈͓̔��ł��邩�ǂ���
                {
                    for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                    {
                        for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                        {
                            //�}�X���Ώ۔͈͂����ׂ�                          
                            if (resetRange > Vector3.Magnitude(position - (centerPosition + new Vector3(x, 0, z) * gridRange)))
                            {
                                //�Ώۓ��̏ꍇ�����񐔂�0�Ƃ���
                                areaWatchNumGrid[x][z] = 0;
                                if (debugMode) { DrawCross((centerPosition + new Vector3(x, 0, z) * gridRange), 5, Color.magenta, 2f); }

                            }
                            else
                            {
                                //�ΏۂłȂ��ꍇ�����񐔂�1�ǉ�����(���x�����𕷂����ꍇ�ɍł��V��������ΏۂƂ��邽��)
                                if (periodic)
                                {//�ׂ�������܂��邱�Ƃŉ��̂��Ă��Ȃ��G���A���ɒ[�ɑ{����ɂȂ�Ȃ��悤�ɂ���O���b�`�̑΍�
                                    areaWatchNumGrid[x][z] = (byte)(areaWatchNumGrid[x][z] + 1);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (debugMode) Debug.Log("z���W���}�b�v����͂ݏo�Ă��܂�");
                }
                if (debugMode) Debug.Log("x���W���}�b�v����͂ݏo�Ă��܂�");
            }
        }

        /// <summary>
        /// �v���C���[�̌��������Ă��邩�ǂ��������o����
        /// </summary>
        /// <param name="enemyPosition">�G�̋��ꏊ</param>
        /// <param name="playerPosition">�v���C���[�̋��ꏊ</param>
        /// <param name="visivilityRange">�G�̎��E�̋���</param>
        /// <param name="lightRange">�v���C���[�̎��E�̋���</param>
        /// <param name="NextPosition">�Q�Ɠn���ōł��������̌������ʒu��Ԃ����</param>
        /// <returns>���͌��������ǂ���</returns>
        public bool RightCheck(Vector3 enemyPosition, Vector3 playerPosition, float visivilityRange, float lightRange, ref Vector3 NextPosition)
        {
            if (!(enemyPosition.x < centerPosition.x + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < enemyPosition.x))
            {
                Debug.LogError("EnemyPosition.x���͈͊O�ł�");
                return false;
            }
            if (!(enemyPosition.z < centerPosition.z + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < enemyPosition.z))
            {
                Debug.LogError("EnemyPosition.z���͈͊O�ł�");
                return false;
            }
            if (!(playerPosition.x < centerPosition.x + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < playerPosition.x))
            {
                Debug.LogError("PlayerPosition.x���͈͊O�ł�");
                return false;
            }
            if (!(playerPosition.z < centerPosition.z + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < playerPosition.z))
            {
                Debug.LogError("EPlayerPosition.z���͈͊O�ł�");
                return false;
            }
            //Enemy���猩���\���̂���}�X���擾
            byte enemyGridPositionX, enemyGridPositionZ;
            enemyGridPositionX = (byte)Mathf.FloorToInt((float)(enemyPosition.x - centerPosition.x + 0.5 * gridRange) / gridRange);
            enemyGridPositionZ = (byte)Mathf.FloorToInt((float)(enemyPosition.z - centerPosition.z + 0.5 * gridRange) / gridRange);
            List<DoubleByteAndMonoFloat> enemyVisivilityGridPosition = visivilityAreaGrid[enemyGridPositionX][enemyGridPositionZ].canVisivleAreaPosition;
            if (debugMode)
            {
                for (byte e = 0; e < enemyVisivilityGridPosition.Count; e++)
                {
                    if (enemyVisivilityGridPosition[e].range < visivilityRange) Debug.DrawLine((new Vector3(enemyGridPositionX, 0, enemyGridPositionZ) * gridRange) + centerPosition, (new Vector3(enemyVisivilityGridPosition[e].x, 0, enemyVisivilityGridPosition[e].z) * gridRange) + centerPosition, Color.green, 1f);
                }
            }

            //�����͂��\���̂���}�X���擾
            byte rightGridPositionX, rightGridPositionZ;
            rightGridPositionX = (byte)Mathf.FloorToInt((float)(playerPosition.x - centerPosition.x + 0.5 * gridRange) / gridRange);
            rightGridPositionZ = (byte)Mathf.FloorToInt((float)(playerPosition.z - centerPosition.z + 0.5 * gridRange) / gridRange);
            List<DoubleByteAndMonoFloat> rightingGridPosition = visivilityAreaGrid[rightGridPositionX][rightGridPositionZ].canVisivleAreaPosition;
            if (debugMode)
            {
                for (byte r = 0; r < rightingGridPosition.Count; r++)
                {
                    if (rightingGridPosition[r].range < lightRange) { }
                    Debug.DrawLine((new Vector3(rightGridPositionX, 0, rightGridPositionZ) * gridRange) + centerPosition, (new Vector3(rightingGridPosition[r].x, 0, rightingGridPosition[r].z) * gridRange) + centerPosition, Color.yellow, 1f);
                }
            }

            //���邱�Ƃ̂ł���ł����邢�}�X������
            bool canLookLight = false;
            byte mostShiningGridPositionX = 0, mostShiningGridPositionZ = 0;
            float shining = 0;
            for (byte e = 0; e < enemyVisivilityGridPosition.Count; e++)
            {
                for (byte r = 0; r < rightingGridPosition.Count; r++)
                {
                    if (enemyVisivilityGridPosition[e].x == rightingGridPosition[r].x && enemyVisivilityGridPosition[e].z == rightingGridPosition[r].z)
                    {//�����͂��\�������茩���Ă���}�X���擾
                        if (enemyVisivilityGridPosition[e].range < visivilityRange && rightingGridPosition[r].range < lightRange)
                        { //�������Ɍ����͂�
                            if (debugMode) { DrawCross((new Vector3(rightingGridPosition[r].x, 0, rightingGridPosition[r].z) * gridRange) + centerPosition, 2, Color.yellow, 1); }
                            if (shining < lightRange - rightingGridPosition[r].range)//�ł����邢�}�X�ł���
                            {
                                mostShiningGridPositionX = rightingGridPosition[r].x;
                                mostShiningGridPositionZ = rightingGridPosition[r].z;
                                shining = lightRange - rightingGridPosition[r].range;
                                canLookLight = true;
                            }
                        }
                    }
                }
            }

            //����Ԃ�
            if (canLookLight)
            {
                NextPosition = (new Vector3(mostShiningGridPositionX, 0, mostShiningGridPositionZ) * gridRange) + centerPosition;
                if (debugMode) { DrawCross(NextPosition, 5, Color.yellow, 1); Debug.Log("�����������I"); Debug.DrawLine(NextPosition, NextPosition + new Vector3(0, 20, 0), Color.magenta, 3); }
                return true;
            }
            else
            {
                if (debugMode) { Debug.Log("���͌����Ȃ�����"); }
                return false;
            }
        }

        /// <summary>
        /// �S�Ẵ}�X�ڂ̌����񐔂��K��񐔕ύX����
        /// </summary>
        /// <param name="change">�ω������鐔</param>
        /// /// <param name="plus">�����Ȃ�true�A�����Ȃ�false</param>
        public void ChangeEveryGridWatchNum(byte change, bool plus)
        {
            VisivilityArea newVisivilityArea;
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    if (plus)
                    {
                        if ((byte)(areaWatchNumGrid[x][z]) < byte.MaxValue - change)
                        {
                            areaWatchNumGrid[x][z] = (byte)(areaWatchNumGrid[x][z] + change);
                        }
                        else
                        {
                            areaWatchNumGrid[x][z] = byte.MaxValue;
                        }
                    }
                    else
                    {
                        if ((byte)(areaWatchNumGrid[x][z]) < byte.MinValue + change)
                        {
                            areaWatchNumGrid[x][z] = (byte)(areaWatchNumGrid[x][z] - change);
                        }
                        else
                        {
                            areaWatchNumGrid[x][z] = byte.MinValue;
                        }

                    }


                }
            }
        }

        /// <summary>
        /// �S�Ẵ}�X�ڂ̌����񐔂��Z�b�g����
        /// </summary>
        /// <param name="num"></param>
        public void SetEveryGridWatchNum(byte num)
        {
            VisivilityArea newVisivilityArea;
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    areaWatchNumGrid[x][z] = num;
                }
            }
        }

        /// <summary>
        /// ����̃O���b�h�̌����񐔂��Z�b�g����
        /// </summary>
        /// <param name="position">�}�X�̂���ʒu</param>
        /// <param name="num">�Z�b�g���鐔</param>
        public void SetGridWatchNum(Vector3 position, byte num)
        {
            VisivilityArea newVisivilityArea;
            if (!(position.x < centerPosition.x + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < position.x))
            {
                Debug.LogError("Position.x���͈͊O�ł�");
            }
            if (!(position.z < centerPosition.z + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < position.z))
            {
                Debug.LogError("Position.z���͈͊O�ł�");
            }
            byte gridPositionX, gridPositionZ;
            gridPositionX = (byte)Mathf.FloorToInt((float)(position.x - centerPosition.x + 0.5 * gridRange) / gridRange);
            gridPositionZ = (byte)Mathf.FloorToInt((float)(position.z - centerPosition.z + 0.5 * gridRange) / gridRange);
            areaWatchNumGrid[gridPositionX][gridPositionZ] = num;
        }



        /// <summary>
        /// �v���C���[�̎��ӂɍŏ��߂Â��Ȃ��悤�ɂ��邽�߂Ɏg�p
        /// </summary>
        public void DontApproachPlayer()
        {
            Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
            if (debugMode) Debug.Log("�v���C���[�ɃX�|�[������ڋ߂��Ȃ��悤�ɑΏ�");
            VisivilityArea newVisivilityArea;
            if ((playerPosition.x < centerPosition.x + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < playerPosition.x))
            {//x���W���}�b�v�͈͓̔��ł��邩�ǂ���
                if ((playerPosition.z < centerPosition.z + (visivilityAreaGrid[0].Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < playerPosition.z)) //z���W���}�b�v�͈͓̔��ł��邩�ǂ���
                {
                    for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                    {
                        for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                        {
                            //�}�X���Ώ۔͈�(�n�[�h�R�[�h��50�ɂ��Ă���)�����ׂ�                          
                            if (50 > Vector3.Magnitude(playerPosition - (centerPosition + new Vector3(x, 0, z) * gridRange)))
                            {

                                //�Ώۓ��̏ꍇ�����񐔂�0�Ƃ���
                                areaWatchNumGrid[x][z] = (byte)(areaWatchNumGrid[x][z] + 1); 
                                if (debugMode) { DrawCross((centerPosition + new Vector3(x, 0, z) * gridRange), 5, Color.magenta, 2f); }

                            }
                            else
                            {
                            }
                        }
                    }
                }
                else
                {
                    if (debugMode) Debug.Log("z���W���}�b�v����͂ݏo�Ă��܂�");
                }
                if (debugMode) Debug.Log("x���W���}�b�v����͂ݏo�Ă��܂�");


            }

        }


        private void DrawCross(Vector3 position, float size, Color color, float time)
        {
            Debug.DrawLine(position + new Vector3(size / 2, 0, size / 2), position + new Vector3(-size, 0, -size), color, time);
            Debug.DrawLine(position + new Vector3(-size / 2, 0, size / 2), position + new Vector3(size / 2, 0, -size / 2), color, time);
        }


        /// <summary>
        /// UniRx�̍w�ǂ��I������
        /// </summary>
        public void Dispose() { 
            _compositeDisposable.Dispose();
        }

    }
}
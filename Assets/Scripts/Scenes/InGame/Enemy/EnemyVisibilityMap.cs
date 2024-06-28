using Scenes.Ingame.Stage;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;


namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// マップの視線の通り方とマップのどのあたりをどのくらい確認したかを記録してゆくクラス。敵キャラがマップを認識するのに使用されるクラス。
    /// </summary>
    public class EnemyVisibilityMap : MonoBehaviour
    {

        public List<List<List<VisivilityArea>>> visivilityAreaGrid;//Unity�̍��W�n��D��A��ڂ�x����ڂ�y����ڂ�z���̃C���[�W������[0][0]���オ[0][max]
        public float maxVisivilityRange;//���̋����𒴂��Ă���G���A�͌����邱�Ƃ͂Ȃ����̂Ƃ���

        public bool debugMode;
        public float gridRange;
        public Vector3 centerPosition;//いちばん左下のグリッドの中央


        [StructLayout(LayoutKind.Auto)]
        public struct TripleByteAndMonoFloat
        {//�ʒu�Ƌ���

            public byte x;
            public byte y;
            public byte z;
            public float range;

            public List<StageDoor> needOpenDoor;
            public List<StageDoor> needCloseDoor;

            public TripleByteAndMonoFloat(byte sX, byte sY,byte sZ, float sRange)
            {
                x = sX;
                y = sY;
                z = sZ;
                range = sRange;
                needOpenDoor = new List<StageDoor>();
                needCloseDoor = new List<StageDoor>();
            }

            public TripleByteAndMonoFloat(byte sX, byte sY,byte sZ, float sRange,List<StageDoor> sNeedOpenDoor, List<StageDoor> sNeedCloseDoor)
            {
                x = sX;
                y = sY;
                z = sZ;
                range = sRange;
                needOpenDoor = sNeedOpenDoor;
                needCloseDoor = sNeedCloseDoor;
            }
        }

        /// <summary>
        /// マス目が何度見られたかをbyteで記録し、このマス目から視線の通るマス目をListで記録している
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        public struct VisivilityArea
        {

            public byte watchNum;//���̃G���A��������
            public List<TripleByteAndMonoFloat> canVisivleAreaPosition;


            public VisivilityArea(byte sWatchNum)
            {
                watchNum = sWatchNum;
                canVisivleAreaPosition = new List<TripleByteAndMonoFloat>();
            }
            public VisivilityArea(byte sWatchNum, List<TripleByteAndMonoFloat> sDoubleByteAndFloat)
            {
                watchNum = sWatchNum;
                canVisivleAreaPosition = new List<TripleByteAndMonoFloat>(sDoubleByteAndFloat);
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
        /// マス目の集合である二次元Listを作成する。
        /// </summary>

        /// <param name="x">x���W�����Ƀ}�X�ڂ��������ׂ邩</param>
        /// <param name="z">z���W�����Ƀ}�X�ڂ��������ׂ邩</param>
        /// <param name="range">���̋����ȏ�̎����͒ʂ�Ȃ����̂ƍl���ăV�~�����[�g����鋗��</param>
        /// <param name="setCenterPosition">�����̃}�X�ڂ̒��S�ʒu</param>
        public void GridMake(byte x,byte y, byte z, float range, Vector3 setCenterPosition)
        { //�}�b�v���쐬�Bx��z�̓O���b�h�̔z�u���Brange�̓O���b�h�̋����BcenterPosition�͍����̈ʒu
            if (debugMode) Debug.Log("�O���b�h�쐬�J�n");
            visivilityAreaGrid = new List<List<List<VisivilityArea>>>();
            gridRange = range;
            centerPosition = setCenterPosition;
            for (byte i = 0; i < x; i++)
            { //�z��̗v�f���쐬
               
                List<List<VisivilityArea>> itemy = new List<List<VisivilityArea>>();
                for (byte j = 0; j < y; j++)

                {
                    List<VisivilityArea> itemz = new List<VisivilityArea>();
                    for (byte k=0;k < z;k++) {


                        if (debugMode) Debug.DrawLine(setCenterPosition + ToVector3(i, j, k) * range, setCenterPosition + ToVector3(i, j, k) * range + ToVector3(0, 10, 0), Color.yellow, 10);//�O���b�h�̈ʒu��\��
                        itemz.Add(new VisivilityArea(0));
                    }
                    itemy.Add(itemz);

                }
                visivilityAreaGrid.Add(itemy);
            }
            if (debugMode) Debug.Log("firstSize(x)" + visivilityAreaGrid.Count());
            if (debugMode) Debug.Log("SecondSize(y)" + visivilityAreaGrid[0].Count());
            if (debugMode) Debug.Log("SecondSize(z)" + visivilityAreaGrid[0][0].Count());
        }

        /// <summary>
        /// マップをスキャンしてマス目同士での視界の通っている情報を決定する
        /// </summary>
        public void MapScan()
        {//マップをスキャンして実際の視界がどのように通っているかを設定
            if (debugMode) Debug.Log("マップスキャン開始");
            //各マス目へとアクセス
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {

                for (byte y =0;y < visivilityAreaGrid[0].Count(); y++) {

                    for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)

                    {

                        //�Ώۂ̃}�X���瑼�̃}�X�ڂ������邩���m�F
                        for (byte vX = 0; vX < visivilityAreaGrid.Count(); vX++)
                        {

                            for (byte vY = 0; vY < visivilityAreaGrid[0].Count(); vY++)
                            {
                                for (byte vZ = 0; vZ < visivilityAreaGrid[0][0].Count(); vZ++)
                                {
                                    if ((x != vX) || (y != vY)||(z != vZ))
                                    { //�������g�ł͂Ȃ��ꍇ                               
                                        float range2 = Mathf.Pow((x - vX) * gridRange, 2) + Mathf.Pow((y - vY) * gridRange, 2) + Mathf.Pow((z - vZ) * gridRange, 2);
                                        if (range2 <= Mathf.Pow(maxVisivilityRange, 2))
                                        { //���E���ʂ�Ƃ���鋗���łȂ��ꍇ
                                            float range = Mathf.Sqrt(range2);//�����������߂�̂͂������R�X�g���d���炵���̂Ŋm���Ɍv�Z���K�v�ɂȂ��Ă��炵�Ă܂�
                                                                             //���E���ʂ邩��Ray���ʂ邩
                                            bool hit;
                                            Ray ray = new Ray(centerPosition + ToVector3(x * gridRange, y * gridRange, z * gridRange), ToVector3(vX - x, vY - y, vZ - z));
                                            hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, 2048, QueryTriggerInteraction.Collide);
                                            if (!hit)
                                            { //���ɂ��������Ă��Ȃ������ꍇ
                                                if (debugMode) Debug.DrawRay(ray.origin, ray.direction * range, Color.green, 10);
                                                visivilityAreaGrid[x][y][z].canVisivleAreaPosition.Add(new TripleByteAndMonoFloat(vX,vY, vZ, range));
                                            }

                                        }

                                    }

                                }
                            }
                        }
                    }
                }
                
            }
            //ここまで来てマップスキャンが終わる
            if (debugMode) Debug.Log("マップのスキャンが完了しました");
        }

        /// <summary>

        /// �h�A���X�L�������ĉ����ԂłȂ��Ǝ��E�̒ʂ�Ȃ���������
        /// </summary>
        public void NeedOpenDoorScan() {
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte y =0;y < visivilityAreaGrid[0].Count();y++) {
                    for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)
                    {

                        foreach (TripleByteAndMonoFloat visivilityAreaPosition in visivilityAreaGrid[x][y][z].canVisivleAreaPosition)//�e�}�X�ڂ��Ƃ̌�����ł��낤�}�X�ɃA�N�Z�X
                        {

                            float range = Mathf.Sqrt(Mathf.Pow((x - visivilityAreaPosition.x) * gridRange, 2) + Mathf.Pow((y - visivilityAreaPosition.y) * gridRange, 2) + Mathf.Pow((z - visivilityAreaPosition.z) * gridRange, 2));


                            //�e�G���A�̃O���b�h�ɃA�N�Z�X
                            Ray ray = new Ray(centerPosition + ToVector3(x * gridRange, y * gridRange, z * gridRange), ToVector3(visivilityAreaPosition.x - x, visivilityAreaPosition.y - y, visivilityAreaPosition.z - z));


                            foreach (RaycastHit doorHit in Physics.RaycastAll(ray.origin, ray.direction, range, 4096, QueryTriggerInteraction.Collide).ToArray<RaycastHit>())
                            {//�����������ׂẴh�A�ɃA�N�Z�X
                                if (doorHit.collider.gameObject.TryGetComponent<StageDoor>(out StageDoor stageDoorCs))
                                {
                                    if (debugMode) Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 10);
                                    visivilityAreaPosition.needOpenDoor.Add(stageDoorCs);

                                }
                                else { Debug.LogWarning("�h�A�̃^�O���t���Ă���̂�StageDoor.cs���t���Ă��Ȃ��I�u�W�F�N�g������"); }
                            }
                        }
                    }
                }


            }
            
        }

        /// <summary>
        /// �h�A���X�L�������ĕ���ԂłȂ��Ǝ��E�̒ʂ�Ȃ���������
        /// </summary>
        public void NeedCloseDoorScan()
        {
            Debug.Log("���Ă��Ȃ��Ă͂Ȃ�Ȃ��h�A���X�L�������܂�");
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte y = 0; y < visivilityAreaGrid[0].Count(); y++)
                {


                    for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)
                    {
                        foreach (TripleByteAndMonoFloat visivilityAreaPosition in visivilityAreaGrid[x][y][z].canVisivleAreaPosition)//�e�}�X�ڂ��Ƃ̌�����ł��낤�}�X�ɃA�N�Z�X
                        {

                            float range = Mathf.Sqrt(Mathf.Pow((x - visivilityAreaPosition.x) * gridRange, 2) + Mathf.Pow((y - visivilityAreaPosition.y) * gridRange, 2) + Mathf.Pow((z - visivilityAreaPosition.z) * gridRange, 2));


                            //�e�G���A�̃O���b�h�ɃA�N�Z�X
                            Ray ray = new Ray(centerPosition + ToVector3(x * gridRange, y*gridRange, z * gridRange), ToVector3(visivilityAreaPosition.x - x, visivilityAreaPosition.y - y, visivilityAreaPosition.z - z));


                            foreach (RaycastHit doorHit in Physics.RaycastAll(ray.origin, ray.direction, range, 4096, QueryTriggerInteraction.Collide).ToArray<RaycastHit>())
                            {//�����������ׂẴh�A�ɃA�N�Z�X
                                if (doorHit.collider.gameObject.TryGetComponent<StageDoor>(out StageDoor stageDoorCs))
                                {
                                    if (debugMode) Debug.DrawRay(ray.origin, ray.direction * range, Color.blue, 10);
                                    visivilityAreaPosition.needCloseDoor.Add(stageDoorCs);

                                }
                                else { Debug.LogWarning("�h�A�̃^�O���t���Ă���̂�StageDoor.cs���t���Ă��Ȃ��I�u�W�F�N�g������"); }
                            }
                        }
                    }
                }
            }
            Debug.Log("���Ă��Ȃ��Ă͂Ȃ�Ȃ��h�A���X�L�������������܂���");
        }


        /// <summary>
        /// ���g�̃f�B�[�v�R�s�[���쐬���ĕԂ�

        /// </summary>
        /// <returns>自身のディープコピー</returns>
        public EnemyVisibilityMap DeepCopy()
        {
            if (debugMode) Debug.Log("ディープコピー開始");
            EnemyVisibilityMap copy;
            copy = new EnemyVisibilityMap();

            copy.visivilityAreaGrid = new List<List<List<VisivilityArea>>>();

            foreach (List<List<VisivilityArea>> item in visivilityAreaGrid)//3�������X�g���R�s�[

            {
                List<List<VisivilityArea>> secondVisivilityArea = new List<List<VisivilityArea>>();//�񎟌��z��

                foreach (List<VisivilityArea> item2 in item) {

                    List<VisivilityArea> therdVisivilityarea = new List<VisivilityArea>();

                    foreach (VisivilityArea item3 in item2) {

                        List<TripleByteAndMonoFloat> addCanVisivilityAndMonoFloat = new List<TripleByteAndMonoFloat>();

                        foreach (TripleByteAndMonoFloat value in item3.canVisivleAreaPosition)
                        {


                            addCanVisivilityAndMonoFloat.Add(new TripleByteAndMonoFloat(value.x, value.y,value.z, value.range, new List<StageDoor>(value.needOpenDoor), new List<StageDoor>(value.needCloseDoor)));
                        }
                        therdVisivilityarea.Add(new VisivilityArea(item3.watchNum, addCanVisivilityAndMonoFloat));
                    }
                    secondVisivilityArea.Add(therdVisivilityarea);
                }
                copy.visivilityAreaGrid.Add(secondVisivilityArea);//�񎟌�List���O������Add����
            }




            copy.gridRange = gridRange;
            copy.maxVisivilityRange = maxVisivilityRange;
            copy.debugMode = debugMode;
            copy.centerPosition = centerPosition;
            
            if (debugMode)
            { //マス目の情報が正常にコピーできているかを表示する
                for (byte x = 0; x < copy.visivilityAreaGrid.Count(); x++)
                {
                    for (byte y = 0; y < copy.visivilityAreaGrid[0].Count(); y++) {
                        for (byte z = 0; z < copy.visivilityAreaGrid[0][0].Count(); z++)
                        {
                            Debug.DrawLine(copy.centerPosition + ToVector3(x, y, z) * copy.gridRange, copy.centerPosition + ToVector3(x, y, z) * copy.gridRange + ToVector3(0, 10, 0), Color.green, 10);
                        }
                    }

                }
            }

            if (visivilityAreaGrid.Count() != copy.visivilityAreaGrid.Count()) { Debug.LogWarning("�����Ⴄ1"); } else { Debug.Log("���͓���"); }
            if (visivilityAreaGrid[0].Count() != copy.visivilityAreaGrid[0].Count()) { Debug.LogWarning("�����Ⴄ2"); }
            if (visivilityAreaGrid[0][0].Count() != copy.visivilityAreaGrid[0][0].Count()) { Debug.LogWarning("�����Ⴄ3"); }
            return copy;
           
        }


        /// <summary>
        /// 次に確認すべき最も見ておらず最も近い位置を取得。
        /// </summary>
        /// <param name="nowPosition">現在のcharacterの座標</param>
        /// <returns>次に行くべき座標</returns>
        public Vector3 GetNextNearWatchPosition(Vector3 nowPosition)
        {
            if (debugMode) Debug.Log("次の移動先を取得");
            List<byte> nextPositionX = new List<byte>();
            List<byte> nextPositionY = new List<byte>();
            List<byte> nextPositionZ = new List<byte>();
            byte smallestWatchNum = byte.MaxValue;
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte y =0;y < visivilityAreaGrid[0].Count();y++) {
                    for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)
                    {
                        if (smallestWatchNum > visivilityAreaGrid[x][y][z].watchNum) { smallestWatchNum = visivilityAreaGrid[x][y][z].watchNum; }
                    }
                }

            }


            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {

                for (byte y = 0; y < visivilityAreaGrid[0].Count();y++) {
                    for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)
                    {
                        if (visivilityAreaGrid[x][y][z].watchNum == smallestWatchNum)
                        { //�ł��������ꍇ
                            nextPositionX.Add(x);
                            nextPositionY.Add(y);
                            nextPositionZ.Add(z);
                        }
                        VisivilityArea newVisivilityArea = ToVisivilityArea((byte)(visivilityAreaGrid[x][y][z].watchNum - smallestWatchNum), visivilityAreaGrid[x][y][z].canVisivleAreaPosition); ;
                        visivilityAreaGrid[x][y][z] = newVisivilityArea;

                    }
                }

            }
            //最も近い要素を考える
            float nearDistance = float.MaxValue;
            byte nearPositionX = 0; byte nearPositionY = 0; byte nearPositionZ = 0;//Y�����̕]���l��10�{���Ă���
            for (int i = 0; i < nextPositionX.Count; i++)
            {
                if (nearDistance > Vector3.Magnitude(nowPosition - (centerPosition + ToVector3(nextPositionX[i], nextPositionY[i] * 10, nextPositionZ[i]) * gridRange)))
                {
                    nearDistance = Vector3.Magnitude(nowPosition - (centerPosition + ToVector3(nextPositionX[i], nextPositionY[i] * 10, nextPositionZ[i]) * gridRange));
                    nearPositionX = nextPositionX[i];
                    nearPositionY = nextPositionY[i];
                    nearPositionZ = nextPositionZ[i];
                }
            }


            //���ۂɎ����ɍs���ׂ����W������
            Vector3 nextPosition = (ToVector3(nearPositionX, nearPositionY, nearPositionZ) * gridRange) + centerPosition;
            if (debugMode)
            {//���ɍs���ׂ��ʒu��`��
                Debug.DrawLine(nextPosition, nextPosition + ToVector3(0, 20, 0), Color.magenta, 3);

            }
            return nextPosition;
        }

        /// <summary>
        /// 今いる場所から見れるマス目の見た回数のカウントを増加させる
        /// </summary>
        /// <param name="nowPosition">現在の座標</param>
        /// <param name="visivilityRange">視界の長さ</param>
        public void CheckVisivility(Vector3 nowPosition, float visivilityRange)
        {
            if (debugMode) Debug.Log("視界の通りをチェック");
            VisivilityArea newVisivilityArea;
            if ((nowPosition.x < centerPosition.x + (visivilityAreaGrid.Count - 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < nowPosition.x))

            {//x���W���}�b�v�͈͓̔��ł��邩�ǂ���

                if ((nowPosition.y < centerPosition.y + (visivilityAreaGrid[0].Count -0.5) * gridRange) && (centerPosition.y - 0.5 * gridRange < nowPosition.y))
                {//y���W
                    if ((nowPosition.z < centerPosition.z + (visivilityAreaGrid[0][0].Count - 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < nowPosition.z)) //z���W���}�b�v�͈͓̔��ł��邩�ǂ���
                    {
                        if (debugMode) Debug.Log("�}�b�v�͈͓̔��ł�");
                        byte myPositionX,myPositionY, myPositionZ;//�������ǂ��̃O���b�h�ɂ��邩���m�F����
                        myPositionX = (byte)Mathf.FloorToInt((float)(nowPosition.x - centerPosition.x + 0.5 * gridRange) / gridRange);
                        myPositionY=  (byte)Mathf.FloorToInt((float)(nowPosition.y - centerPosition.y + 0.5 * gridRange) / gridRange);
                        myPositionZ = (byte)Mathf.FloorToInt((float)(nowPosition.z - centerPosition.z + 0.5 * gridRange) / gridRange);
                        foreach (TripleByteAndMonoFloat item in visivilityAreaGrid[myPositionX][myPositionY][myPositionZ].canVisivleAreaPosition)
                        {
                            if (item.range < visivilityRange)
                            { //�����鋗��
                              //�h�A�Ɋ֘A���Č���������ɂ��邩���ׂ�
                                bool noDoor = true;
                                foreach (StageDoor needOpen in item.needOpenDoor) //�J���Ă��Ȃ���΂Ȃ�Ȃ��h�A�͊J���Ă��邩�`�F�b�N
                                {
                                    if (needOpen.ReturnIsOpen == false)
                                    {
                                        noDoor = false; break;
                                    }
                                }
                                if (noDoor)
                                {
                                    foreach (StageDoor needClose in item.needCloseDoor)//���Ă��Ȃ���΂Ȃ�Ȃ��h�A�͕��Ă��邩�`�F�b�N
                                    {
                                        if (needClose.ReturnIsOpen == true)
                                        {
                                            noDoor = false;
                                            break;
                                        }
                                    }
                                }

                                if (noDoor)
                                {
                                    //�����񐔂𑫂��B�������\���̂�List��For���̒��ł�����Ȃ��̂ŃR�s�[���Ă������ď���������B�I�[�o�[�t���[���Ȃ��ꍇ
                                    if ((byte)(visivilityAreaGrid[item.x][item.y][item.z].watchNum) < byte.MaxValue)
                                    {
                                        newVisivilityArea = ToVisivilityArea((byte)(visivilityAreaGrid[item.x][item.y][item.z].watchNum + 1), visivilityAreaGrid[item.x][item.y][item.z].canVisivleAreaPosition);
                                        visivilityAreaGrid[item.x][item.y][item.z] = newVisivilityArea;
                                    }
                                    if (debugMode)
                                    {//�����G���A����ŕ\��
                                        Debug.DrawLine(centerPosition + ToVector3(myPositionX, myPositionY, myPositionZ) * gridRange, centerPosition + ToVector3(item.x, item.y, item.z) * gridRange, Color.green, 1f);
                                    }
                                }


                            }
                        }
                        //������������ꏊ�Ɍ����񐔂𑫂��B�������\���̂�List��For���̒��ł�����Ȃ��̂ŃR�s�[���Ă������ď���������
                        if ((byte)(visivilityAreaGrid[myPositionX][myPositionY][myPositionZ].watchNum) < byte.MaxValue)
                        {
                            newVisivilityArea = ToVisivilityArea((byte)(visivilityAreaGrid[myPositionX][myPositionY][myPositionZ].watchNum + 1), visivilityAreaGrid[myPositionX][myPositionY][myPositionZ].canVisivleAreaPosition);
                            visivilityAreaGrid[myPositionX][myPositionY][myPositionZ] = newVisivilityArea;
                        }
                    }

                    else

                    {
                         Debug.LogError("z���W���}�b�v����͂ݏo�Ă��܂�");
                    }
                }

                else {
                    Debug.LogError("z���W���}�b�v����͂ݏo�Ă��܂�");

                }
            }
            else
            {

                Debug.LogError("x���W���}�b�v����͂ݏo�Ă��܂�");

            }

            if (debugMode)
            { //各マス目がどれだけ見られているかを確認する
                for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                {
                    for (byte y =0;y < visivilityAreaGrid[0].Count();y++) {
                        for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)
                        {
                            Color drawColor;
                            if (visivilityAreaGrid[x][y][z].watchNum < 25) { drawColor = new Color32((byte)(10 * visivilityAreaGrid[x][y][z].watchNum), 0, (byte)(byte.MaxValue - (10 * visivilityAreaGrid[x][y][z].watchNum)), byte.MaxValue); }
                            else
                            {
                                drawColor = Color.red;
                            }

                            Debug.DrawLine(centerPosition + ToVector3(x, y, z) * gridRange, centerPosition + ToVector3(x, y, z) * gridRange + ToVector3(0, 10, 0), drawColor, 1f);
                        }

                    }

                }
            }
        }

        /// <summary>
        /// 特定の位置から音が聞こえてきた場合の処理
        /// </summary>
        /// <param name="position">音源の座標</param>
        /// <param name="resetRange">音源が存在するであろうという事で対象とする範囲</param>
        /// <param name="periodic">定期的なチェックによって呼び出されたのかどうか</param>
        public void HearingSound(Vector3 position, float resetRange, bool periodic)
        {
            if (debugMode) Debug.Log("特定位置から聞こえてきた音について対処");
            VisivilityArea newVisivilityArea;
            if ((position.x < centerPosition.x + (visivilityAreaGrid.Count - 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < position.x))

            {//x���W���}�b�v�͈͓̔��ł��邩�ǂ���
                if ((position.y < centerPosition.y + (visivilityAreaGrid[0].Count - 0.5) * gridRange) && (centerPosition.y - 0.5 * gridRange < position.y))

                {
                    if ((position.z < centerPosition.z + (visivilityAreaGrid[0][0].Count - 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < position.z)) //z���W���}�b�v�͈͓̔��ł��邩�ǂ���
                    {
                        for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                        {

                            for (byte y = 0; y < visivilityAreaGrid[0].Count(); y++)
                            {
                                for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)
                                {
                                    //�}�X���Ώ۔͈͂����ׂ�                          
                                    if (resetRange > Vector3.Magnitude(position - (centerPosition + ToVector3(x, y, z) * gridRange)))
                                    {
                                        //�Ώۓ��̏ꍇ�����񐔂�0�Ƃ���
                                        newVisivilityArea = ToVisivilityArea((byte)(0), visivilityAreaGrid[x][y][z].canVisivleAreaPosition);
                                        visivilityAreaGrid[x][y][z] = newVisivilityArea;
                                        if (debugMode) { DrawCross((centerPosition + ToVector3(x, y, z) * gridRange), 5, Color.magenta, 2f); }

                                    }
                                    else
                                    {
                                        //�ΏۂłȂ��ꍇ�����񐔂�1�ǉ�����(���x�����𕷂����ꍇ�ɍł��V��������ΏۂƂ��邽��)
                                        if (periodic)
                                        {//�ׂ�������܂��邱�Ƃŉ��̂��Ă��Ȃ��G���A���ɒ[�ɑ{����ɂȂ�Ȃ��悤�ɂ���O���b�`�̑΍�
                                            newVisivilityArea = ToVisivilityArea((byte)(visivilityAreaGrid[x][y][z].watchNum + 1), visivilityAreaGrid[x][y][z].canVisivleAreaPosition);
                                            visivilityAreaGrid[x][y][z] = newVisivilityArea;
                                        }
                                    }

                                }


                            }

                        }
                    }
                    else
                    {
                        Debug.LogError("z���W���}�b�v����͂ݏo�Ă��܂�");
                    }
                }

                else { Debug.LogError("y���W���}�b�v����͂ݏo�Ă��܂�"); }
                
                
            }
            else {
                Debug.LogError("x���W���}�b�v����͂ݏo�Ă��܂�");

            }
        }

        /// <summary>
        /// プレイヤーの光が見えているかどうかを検出する
        /// </summary>
        /// <param name="enemyPosition">敵の居場所</param>
        /// <param name="playerPosition">プレイヤーの居場所</param>
        /// <param name="visivilityRange">敵の視界の距離</param>
        /// <param name="lightRange">プレイヤーの視界の距離</param>
        /// <param name="NextPosition">参照渡しで最も強い光の見えた位置を返される</param>
        /// <returns>光は見えたかどうか</returns>
        public bool RightCheck(Vector3 enemyPosition, Vector3 playerPosition, float visivilityRange, float lightRange, ref Vector3 NextPosition)
        {
            if (!((enemyPosition.x < centerPosition.x + (visivilityAreaGrid.Count - 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < enemyPosition.x)))
            {
                Debug.LogError("EnemyPosition.xが範囲外です");
                return false;
            }

            if (!((enemyPosition.y < centerPosition.y + (visivilityAreaGrid[0].Count - 0.5) * gridRange) && (centerPosition.y - 0.5 * gridRange < enemyPosition.y)))
            {
                Debug.LogError("EnemyPosition.y���͈͊O�ł�");
                return false;
            }
            if (!((enemyPosition.z < centerPosition.z + (visivilityAreaGrid[0][0].Count - 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < enemyPosition.z)))

            {
                Debug.LogError("EnemyPosition.zが範囲外です");
                return false;
            }
            if (!((playerPosition.x < centerPosition.x + (visivilityAreaGrid.Count - 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < playerPosition.x)))
            {
                Debug.LogError("PlayerPosition.xが範囲外です");
                return false;
            }

            if (!((playerPosition.y < centerPosition.y + (visivilityAreaGrid[0].Count - 0.5) * gridRange) && (centerPosition.y - 0.5 * gridRange < playerPosition.y)))
            {
                Debug.LogError("PlayerPosition.y���͈͊O�ł�");
                return false;
            }
            if (!((playerPosition.z < centerPosition.z + (visivilityAreaGrid[0][0].Count - 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < playerPosition.z)))

            {
                Debug.LogError("EPlayerPosition.zが範囲外です");
                return false;
            }

            //Enemy���猩���\���̂���}�X���擾
            byte enemyGridPositionX,enemyGridPositionY, enemyGridPositionZ;

            enemyGridPositionX = (byte)Mathf.FloorToInt((float)(enemyPosition.x - centerPosition.x + 0.5 * gridRange) / gridRange);
            enemyGridPositionY = (byte)Mathf.FloorToInt((float)(enemyPosition.y - centerPosition.y + 0.5 * gridRange) / gridRange);
            enemyGridPositionZ = (byte)Mathf.FloorToInt((float)(enemyPosition.z - centerPosition.z + 0.5 * gridRange) / gridRange);

            List<TripleByteAndMonoFloat> enemyVisivilityGridPosition = new List<TripleByteAndMonoFloat> ();//�����Ă邱�Ƃ͂��̐�ɂ����ēG����h�A�̖��Ȃ�������}�X�����𒊏o���邱��

            foreach (TripleByteAndMonoFloat item    in visivilityAreaGrid[enemyGridPositionX][enemyGridPositionY][enemyGridPositionZ].canVisivleAreaPosition) {
                //�h�A�Ɋ֘A���Č���������ɂ��邩���ׂ�
                bool noDoor = true;
                foreach (StageDoor needOpen in item.needOpenDoor) //�J���Ă��Ȃ���΂Ȃ�Ȃ��h�A�͊J���Ă��邩�`�F�b�N
                {
                    if (needOpen.ReturnIsOpen == false)
                    {
                        noDoor = false; break;
                    }
                }
                if (noDoor)
                {
                    foreach (StageDoor needClose in item.needCloseDoor)//���Ă��Ȃ���΂Ȃ�Ȃ��h�A�͕��Ă��邩�`�F�b�N
                    {
                        if (needClose.ReturnIsOpen == true)
                        {
                            noDoor = false;
                            break;
                        }
                    }
                }
                if (noDoor) {
                    enemyVisivilityGridPosition.Add(item);
                }
            }







            if (debugMode)
            {
                for (int e = 0; e < enemyVisivilityGridPosition.Count; e++)
                {
                    if (enemyVisivilityGridPosition[e].range < visivilityRange) Debug.DrawLine((ToVector3(enemyGridPositionX, enemyGridPositionY, enemyGridPositionZ) * gridRange) + centerPosition, (ToVector3(enemyVisivilityGridPosition[e].x, enemyVisivilityGridPosition[e].y, enemyVisivilityGridPosition[e].z) * gridRange) + centerPosition, Color.green, 1f);
                }
            }

            //�����͂��\���̂���}�X���擾
            byte rightGridPositionX, rightGridPositionY,rightGridPositionZ;

            rightGridPositionX = (byte)Mathf.FloorToInt((float)(playerPosition.x - centerPosition.x + 0.5 * gridRange) / gridRange);
            rightGridPositionY = (byte)Mathf.FloorToInt((float)(playerPosition.y - centerPosition.y + 0.5 * gridRange) / gridRange);
            rightGridPositionZ = (byte)Mathf.FloorToInt((float)(playerPosition.z - centerPosition.z + 0.5 * gridRange) / gridRange);
            Debug.Log(rightGridPositionY);
            List<TripleByteAndMonoFloat> rightingGridPosition = new List<TripleByteAndMonoFloat>();//�����Ă邱�Ƃ͂��̐�ɂ����ăv���C���[����h�A�̖��Ȃ�������}�X�����𒊏o���邱��
            foreach (TripleByteAndMonoFloat item in visivilityAreaGrid[rightGridPositionX][rightGridPositionY][rightGridPositionZ].canVisivleAreaPosition)
            {
                //�h�A�Ɋ֘A���Č���������ɂ��邩���ׂ�
                bool noDoor = true;
                foreach (StageDoor needOpen in item.needOpenDoor) //�J���Ă��Ȃ���΂Ȃ�Ȃ��h�A�͊J���Ă��邩�`�F�b�N
                {
                    if (needOpen.ReturnIsOpen == false)
                    {
                        noDoor = false; break;
                    }
                }
                if (noDoor)
                {
                    foreach (StageDoor needClose in item.needCloseDoor)//���Ă��Ȃ���΂Ȃ�Ȃ��h�A�͕��Ă��邩�`�F�b�N
                    {
                        if (needClose.ReturnIsOpen == true)
                        {
                            noDoor = false;
                            break;
                        }
                    }
                }
                if (noDoor)
                {
                    rightingGridPosition.Add(item);
                }
            }


            if (debugMode)//����`��
            {
                for (int r = 0; r < rightingGridPosition.Count; r++)
                {
                    if (rightingGridPosition[r].range < lightRange) { }
                    Debug.DrawLine((ToVector3(rightGridPositionX, rightGridPositionY, rightGridPositionZ) * gridRange) + centerPosition, (ToVector3(rightingGridPosition[r].x, rightingGridPosition[r].y, rightingGridPosition[r].z) * gridRange) + centerPosition, Color.yellow, 1f);
                }
            }


         

            //���邱�Ƃ̂ł���ł����邢�}�X������

            bool canLookLight = false;
            byte mostShiningGridPositionX = 0,mostShiningGridPositionY = 0, mostShiningGridPositionZ = 0;
            float shining = 0;
            for (int e = 0; e < enemyVisivilityGridPosition.Count; e++)
            {
                for (int r = 0; r < rightingGridPosition.Count; r++)
                {
                    if (enemyVisivilityGridPosition[e].x == rightingGridPosition[r].x && enemyVisivilityGridPosition[e].z == rightingGridPosition[r].z)
                    {//光が届く可能性があり見えているマスを取得
                        if (enemyVisivilityGridPosition[e].range < visivilityRange && rightingGridPosition[r].range < lightRange)

                        { //�������Ɍ����͂�
                            if (debugMode) { DrawCross((ToVector3(rightingGridPosition[r].x, rightingGridPosition[r].y, rightingGridPosition[r].z) * gridRange) + centerPosition, 2, Color.yellow, 1); }
                            if (shining < lightRange - rightingGridPosition[r].range)//�ł����邢�}�X�ł���

                            {
                                mostShiningGridPositionX = rightingGridPosition[r].x;
                                mostShiningGridPositionY = rightingGridPosition[r].y;
                                mostShiningGridPositionZ = rightingGridPosition[r].z;
                                shining = lightRange - rightingGridPosition[r].range;
                                canLookLight = true;
                            }
                        }
                    }
                }
            }

            //情報を返す
            if (canLookLight)
            {

                NextPosition = (ToVector3(mostShiningGridPositionX, mostShiningGridPositionY, mostShiningGridPositionZ) * gridRange) + centerPosition;
                if (debugMode) { DrawCross(NextPosition, 5, Color.yellow, 1); Debug.Log("�����������I"); Debug.DrawLine(NextPosition, NextPosition + ToVector3(0, 20, 0), Color.magenta, 3); }

                return true;
            }
            else
            {
                if (debugMode) { Debug.Log("光は見えなかった"); }
                return false;
            }
        }

        /// <summary>
        /// 全てのマス目の見た回数を規定回数変更する
        /// </summary>
        /// <param name="change">変化させる数</param>
        /// /// <param name="plus">足すならtrue、引くならfalse</param>
        public void ChangeEveryGridWatchNum(byte change, bool plus)
        {
            VisivilityArea newVisivilityArea;
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte y = 0; y < visivilityAreaGrid[0].Count();y++) {
                    for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)
                    {
                        if (plus)
                        {
                            if ((byte)(visivilityAreaGrid[x][y][z].watchNum) < byte.MaxValue - change)
                            {
                                newVisivilityArea = ToVisivilityArea((byte)(visivilityAreaGrid[x][y][z].watchNum + change), visivilityAreaGrid[x][y][z].canVisivleAreaPosition);
                                visivilityAreaGrid[x][y][z] = newVisivilityArea;
                            }
                            else
                            {
                                newVisivilityArea = ToVisivilityArea(byte.MaxValue, visivilityAreaGrid[x][y][z].canVisivleAreaPosition);
                                visivilityAreaGrid[x][y][z] = newVisivilityArea;
                            }
                        }
                        else
                        {
                            if ((byte)(visivilityAreaGrid[x][y][z].watchNum) < byte.MinValue + change)
                            {
                                newVisivilityArea = ToVisivilityArea((byte)(visivilityAreaGrid[x][y][z].watchNum - change), visivilityAreaGrid[x][y][z].canVisivleAreaPosition);
                                visivilityAreaGrid[x][y][z] = newVisivilityArea;
                            }
                            else
                            {
                                newVisivilityArea = ToVisivilityArea(byte.MinValue, visivilityAreaGrid[x][y][z].canVisivleAreaPosition);
                                visivilityAreaGrid[x][y][z] = newVisivilityArea;
                            }

                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// 全てのマス目の見た回数をセットする
        /// </summary>
        /// <param name="num"></param>
        public void SetEveryGridWatchNum(byte num)
        {
            VisivilityArea newVisivilityArea;
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte y = 0; y < visivilityAreaGrid[0].Count();y++) {
                    for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)
                    {
                        newVisivilityArea = ToVisivilityArea(num, visivilityAreaGrid[x][y][z].canVisivleAreaPosition);
                        visivilityAreaGrid[x][y][z] = newVisivilityArea;
                    }
                }
            }
        }

        /// <summary>
        /// 特定のグリッドの見た回数をセットする
        /// </summary>
        /// <param name="position">マスのある位置</param>
        /// <param name="num">セットする数</param>
        public void SetGridWatchNum(Vector3 position, byte num)
        {
            VisivilityArea newVisivilityArea;
            if (!(position.x < centerPosition.x + (visivilityAreaGrid.Count - 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < position.x))
            {
                Debug.LogError("Position.xが範囲外です");
            }
            if (!(position.y < centerPosition.y + (visivilityAreaGrid[0].Count - 0.5) * gridRange) && (centerPosition.y - 0.5 * gridRange < position.x)) {
                Debug.LogError("positionY���͈͊O�ł�");
            }
            if (!(position.z < centerPosition.z + (visivilityAreaGrid[0][0].Count - 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < position.z))
            {
                Debug.LogError("Position.zが範囲外です");
            }
            byte gridPositionX, gridPositionY,gridPositionZ;
            gridPositionX = (byte)Mathf.FloorToInt((float)(position.x - centerPosition.x + 0.5 * gridRange) / gridRange);
            gridPositionY = (byte)Mathf.FloorToInt((float)(position.y - centerPosition.y + 0.5 * gridRange) / gridRange);
            gridPositionZ = (byte)Mathf.FloorToInt((float)(position.z - centerPosition.z + 0.5 * gridRange) / gridRange);
            newVisivilityArea = ToVisivilityArea(num, visivilityAreaGrid[gridPositionX][gridPositionY][gridPositionZ].canVisivleAreaPosition);
            visivilityAreaGrid[gridPositionX][gridPositionY][gridPositionZ] = newVisivilityArea;
        }



        /// <summary>
        /// プレイヤーの周辺に最初近づかないようにするために使用
        /// </summary>
        public void DontApproachPlayer()
        {
            Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
            if (debugMode) Debug.Log("プレイヤーにスポーン直後接近しないように対処");
            VisivilityArea newVisivilityArea;
            if ((playerPosition.x < centerPosition.x + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < playerPosition.x))

            {//x���W���}�b�v�͈͓̔��ł��邩�ǂ���
                if ((playerPosition.y < centerPosition.y + (visivilityAreaGrid[0].Count + 0.5) * gridRange) && (centerPosition.y - 0.5 * gridRange < playerPosition.y))

                {
                    if ((playerPosition.z < centerPosition.z + (visivilityAreaGrid[0][0].Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < playerPosition.z)) //z���W���}�b�v�͈͓̔��ł��邩�ǂ���
                    {
                        for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                        {

                            for (byte y = 0; y < visivilityAreaGrid[0].Count(); y++)

                            {
                                for (byte z = 0; z < visivilityAreaGrid[0][0].Count(); z++)
                                {
                                    //�}�X���Ώ۔͈�(�n�[�h�R�[�h��50�ɂ��Ă���)�����ׂ�                          
                                    if (50 > Vector3.Magnitude(playerPosition - (centerPosition + ToVector3(x, y, z) * gridRange)))
                                    {
                                        //�Ώۓ��̏ꍇ�����񐔂�0�Ƃ���
                                        newVisivilityArea = ToVisivilityArea((byte)(visivilityAreaGrid[x][y][z].watchNum + 1), visivilityAreaGrid[x][y][z].canVisivleAreaPosition);
                                        visivilityAreaGrid[x][y][z] = newVisivilityArea;
                                        if (debugMode) { DrawCross((centerPosition + ToVector3(x, y, z) * gridRange), 5, Color.magenta, 2f); }
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("z���W���}�b�v����͂ݏo�Ă��܂�");
                    }
                }
                else
                {

                    Debug.LogError("y���W���}�b�v����͂ݏo�Ă��܂�");
                }
            }
            else {
                Debug.LogError("x���W���}�b�v����͂ݏo�Ă��܂�");
            }


        }



        //###################################
        //�֗��Ȋ֐�����
        //###################################

        Vector2 translation2 = Vector2.zero;
        Vector3 translation3 = Vector3.zero;
        VisivilityArea vA = new VisivilityArea((byte)0);
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
        private VisivilityArea ToVisivilityArea(byte setNum,List<TripleByteAndMonoFloat> setList)
        {
            vA.watchNum = setNum;
            vA.canVisivleAreaPosition = setList;
            return vA;
        }

        private void DrawCross(Vector3 position, float size, Color color, float time)
        {
            Debug.DrawLine(position + ToVector3(size / 2, 0, size / 2), position + ToVector3(-size, 0, -size), color, time);
            Debug.DrawLine(position + ToVector3(-size / 2, 0, size / 2), position + ToVector3(size / 2, 0, -size / 2), color, time);
        }


    }
}
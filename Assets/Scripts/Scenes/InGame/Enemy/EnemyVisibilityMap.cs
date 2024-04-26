using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms;

namespace Scenes.Ingame.Enemy
{
    public class EnemyVisibilityMap : MonoBehaviour
    {
        public List<List<VisivilityArea>> visivilityAreaGrid;//Unity�̍��W�n��D��A��ڂ�x����ڂ�y���̃C���[�W������[0][0]���オ[0][max]
        public float maxVisivilityRange;//���̋����𒴂��Ă���G���A�͌����邱�Ƃ͂Ȃ����̂Ƃ���
        public bool debugMode;
        public float gridRange;
        public Vector3 centerPosition;

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

        public struct VisivilityArea
        {//�e�G���A�̏��A���������邱�Ƃ̂ł��鑼�̈ʒu�Ƃ��̃G���A��������
            public byte watchNum;//���̃G���A��������
            public List<DoubleByteAndMonoFloat> canVisivleAreaPosition;
            public VisivilityArea(byte sWatchNum)
            {
                watchNum = sWatchNum;
                canVisivleAreaPosition = new List<DoubleByteAndMonoFloat>();
            }
        }

        private void Start()
        {
            /*
            GridMake(9,9,5.8f,new Vector3(-46.4f,1,-43.7f));
            MapScan();
            */
        }

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
                    item.Add(new VisivilityArea(0));

                    if (debugMode) Debug.DrawLine(setCenterPosition + new Vector3(i, 0, j) * range, setCenterPosition + new Vector3(i, 0, j) * range + new Vector3(0, 10, 0), Color.yellow, 10);//�O���b�h�̈ʒu��\��
                }
                visivilityAreaGrid.Add(item);
            }
            if (debugMode) Debug.Log("firstSize(x)" + visivilityAreaGrid.Count());
            if (debugMode) Debug.Log("SecondSize(z)" + visivilityAreaGrid[0].Count());
        }

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
                                    hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, -1, QueryTriggerInteraction.Collide);
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


        public EnemyVisibilityMap DeepCopy()
        {
            if (debugMode) Debug.Log("�f�B�[�v�R�s�[�J�n");
                EnemyVisibilityMap copy;
            copy = new EnemyVisibilityMap();
            copy.visivilityAreaGrid = new List<List<VisivilityArea>>();
            foreach (List<VisivilityArea> item in visivilityAreaGrid)//�񎟌����X�g���R�s�[
            {
                copy.visivilityAreaGrid.Add(new List<VisivilityArea>(item));
            }
            copy.gridRange = gridRange;
            copy.maxVisivilityRange = maxVisivilityRange;
            copy.debugMode = debugMode;
            copy.centerPosition = centerPosition;
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
    }
}
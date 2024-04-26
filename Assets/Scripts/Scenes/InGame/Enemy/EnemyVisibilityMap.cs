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
        public List<List<VisivilityArea>> visivilityAreaGrid;//Unityの座標系を優先、一個目がx軸二個目がy軸のイメージ左下が[0][0]左上が[0][max]
        public float maxVisivilityRange;//この距離を超えているエリアは見えることはないものとする
        public bool debugMode;
        public float gridRange;
        public Vector3 centerPosition;

        public struct DoubleByteAndMonoFloat
        {//位置と距離
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
        {//各エリアの情報、そこを見ることのできる他の位置とこのエリアを見た回数
            public byte watchNum;//このエリアを見た回数
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
        { //マップを作成。xとzはグリッドの配置数。rangeはグリッドの距離。centerPositionは左下の位置
            if (debugMode) Debug.Log("グリッド作成開始");
            visivilityAreaGrid = new List<List<VisivilityArea>>();
            gridRange = range;
            centerPosition = setCenterPosition;
            for (byte i = 0; i < x; i++)
            { //配列の要素を作成
                List<VisivilityArea> item = new List<VisivilityArea>();
                for (byte j = 0; j < z; j++)
                {
                    item.Add(new VisivilityArea(0));

                    if (debugMode) Debug.DrawLine(setCenterPosition + new Vector3(i, 0, j) * range, setCenterPosition + new Vector3(i, 0, j) * range + new Vector3(0, 10, 0), Color.yellow, 10);//グリッドの位置を表示
                }
                visivilityAreaGrid.Add(item);
            }
            if (debugMode) Debug.Log("firstSize(x)" + visivilityAreaGrid.Count());
            if (debugMode) Debug.Log("SecondSize(z)" + visivilityAreaGrid[0].Count());
        }

        public void MapScan()
        {//マップをスキャンして実際の視界がどのように通っているかを設定
            if (debugMode) Debug.Log("マップスキャン開始");
            //各マス目へとアクセス
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    //対象のマスから他のマス目が見えるかを確認
                    for (byte vX = 0; vX < visivilityAreaGrid.Count(); vX++)
                    {
                        for (byte vZ = 0; vZ < visivilityAreaGrid.Count(); vZ++)
                        {
                            if ((x != vX) || (z != vZ))
                            { //自分自身ではない場合                               
                                float range2 = Mathf.Pow((x - vX) * 5.8f, 2) + Mathf.Pow((z - vZ) * 5.8f, 2);
                                if (range2 <= Mathf.Pow(maxVisivilityRange, 2))
                                { //視界が通るとされる距離でない場合
                                    float range = Mathf.Sqrt(range2);//平方根を求めるのはすごくコストが重いらしいので確実に計算が必要になってからしてます
                                    //視界が通るか＝Rayが通るか
                                    bool hit;
                                    Ray ray = new Ray(centerPosition + new Vector3(x * gridRange, 1, z * gridRange), new Vector3(vX - x, 0, vZ - z));
                                    hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, -1, QueryTriggerInteraction.Collide);
                                    if (!hit)
                                    { //何にもあたっていなかった場合
                                        if (debugMode) Debug.DrawRay(ray.origin, ray.direction * range, Color.green, 10);
                                        visivilityAreaGrid[x][z].canVisivleAreaPosition.Add(new DoubleByteAndMonoFloat(vX, vZ, range));
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


        public EnemyVisibilityMap DeepCopy()
        {
            if (debugMode) Debug.Log("ディープコピー開始");
                EnemyVisibilityMap copy;
            copy = new EnemyVisibilityMap();
            copy.visivilityAreaGrid = new List<List<VisivilityArea>>();
            foreach (List<VisivilityArea> item in visivilityAreaGrid)//二次元リストをコピー
            {
                copy.visivilityAreaGrid.Add(new List<VisivilityArea>(item));
            }
            copy.gridRange = gridRange;
            copy.maxVisivilityRange = maxVisivilityRange;
            copy.debugMode = debugMode;
            copy.centerPosition = centerPosition;
            if (debugMode)
            { //マス目の情報が正常にコピーできているかを表示する
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
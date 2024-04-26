using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Scenes.Ingame.Enemy
{
    public class EnemyVisibilityMap : MonoBehaviour
    {
        public List<List<VisivilityArea>> visivilityAreaGrid;//Unityの座標系を優先、一個目がx軸二個目がy軸のイメージ左下が[0][0]左上が[0][max]
        public float maxVisivilityRange;//この距離を超えているエリアは見えることはないものとする
        public bool debugMode;
        public float gridRange;
        public Vector3 centerPosition;//いちばん左下のグリッドの中央

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
            public VisivilityArea(byte sWatchNum,List<DoubleByteAndMonoFloat> sDoubleByteAndFloat)
            {
                watchNum = sWatchNum;
                canVisivleAreaPosition = new List<DoubleByteAndMonoFloat>(sDoubleByteAndFloat);
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

        public void WatchNumKeepSmaller() {//グリッドを見た回数の大小関係はそのままに値を小さくする
            if (debugMode) Debug.Log("グリッドを見た回数を大小関係はそのままに小さくする");
            byte smallestWatchNum = byte.MaxValue;
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    if (smallestWatchNum > visivilityAreaGrid[x][z].watchNum) { smallestWatchNum = visivilityAreaGrid[x][z].watchNum; }
                }
            }
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    VisivilityArea newVisivilityArea = new VisivilityArea((byte)(visivilityAreaGrid[x][z].watchNum - smallestWatchNum), visivilityAreaGrid[x][z].canVisivleAreaPosition); ;
                    visivilityAreaGrid[x][z] = newVisivilityArea;
                }
            }
        }

        public Vector3 GetNextNearWatchPosition(Vector3 nowPosition) {//WatchNumKeepSmallerメソッドの機能も内蔵
            if (debugMode) Debug.Log("次の移動先を取得");
            List<byte> nextPositionX = new List<byte>();
            List<byte> nextPositionZ = new List<byte>();
            byte smallestWatchNum = byte.MaxValue;
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    if (smallestWatchNum > visivilityAreaGrid[x][z].watchNum) { smallestWatchNum = visivilityAreaGrid[x][z].watchNum; }
                }
            }
            for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
            {
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    if (visivilityAreaGrid[x][z].watchNum == smallestWatchNum) { //最も小さい場合
                        nextPositionX.Add(x);
                        nextPositionZ.Add(z);
                    }
                    VisivilityArea newVisivilityArea = new VisivilityArea((byte)(visivilityAreaGrid[x][z].watchNum - smallestWatchNum), visivilityAreaGrid[x][z].canVisivleAreaPosition); ;
                    visivilityAreaGrid[x][z] = newVisivilityArea;
                }
            }
            //最も近い要素を考える
            float nearDistance = float.MaxValue;
            byte nearPositionX = 0;byte nearPositionZ = 0;
            for (byte i = 0; i < nextPositionX.Count; i++) {
                if (nearDistance > Vector3.Magnitude(nowPosition - (centerPosition + new Vector3(nextPositionX[i], 0, nextPositionZ[i]) * gridRange))) {
                    nearDistance = Vector3.Magnitude(nowPosition - (centerPosition + new Vector3(nextPositionX[i], 0, nextPositionZ[i]) * gridRange));
                    nearPositionX = nextPositionX[i];
                    nearPositionZ = nextPositionZ[i];
                }
            }

            //実際に次ぎに行くべき座標を示す
            Vector3 nextPosition = (new Vector3(nearPositionX,0,nearPositionZ) * gridRange) + centerPosition;
            if (debugMode) {//次に行くべき位置を描画
                Debug.DrawLine(nextPosition,nextPosition+new Vector3(0,20,0),Color.magenta,20);
            }
            return nextPosition;
        }

        public void CheckVisivility(Vector3 nowPosition ,float visivilityRange) {
            if (debugMode) Debug.Log("視界の通りをチェック");
            VisivilityArea newVisivilityArea;
            if ((nowPosition.x < centerPosition.x + (visivilityAreaGrid.Count+0.5)*gridRange) && (centerPosition.x - 0.5*gridRange < nowPosition.x)) {//x座標がマップの範囲内であるかどうか
                if ((nowPosition.z < centerPosition.z + (visivilityAreaGrid[0].Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < nowPosition.z)) //z座標がマップの範囲内であるかどうか
                {
                    if (debugMode) Debug.Log("マップの範囲内です");
                    byte myPositionx, myPositionz;//自分がどこのグリッドにいるかを確認する
                    myPositionx = (byte)Mathf.FloorToInt((float)(nowPosition.x - centerPosition.x + 0.5 * gridRange) / gridRange);
                    myPositionz = (byte)Mathf.FloorToInt((float)(nowPosition.z - centerPosition.z + 0.5 * gridRange) / gridRange);
                    foreach (DoubleByteAndMonoFloat item in visivilityAreaGrid[myPositionx][myPositionz].canVisivleAreaPosition) {
                        if (item.range < visivilityRange)
                        { //見える距離
                            //見た回数を足す。ただし構造体をListのFor文の中でいじれないのでコピーしていじって書き換える。オーバーフローしない場合
                            if ((byte)(visivilityAreaGrid[item.x][item.z].watchNum) < byte.MaxValue){
                                newVisivilityArea = new VisivilityArea((byte)(visivilityAreaGrid[item.x][item.z].watchNum + 1), visivilityAreaGrid[item.x][item.z].canVisivleAreaPosition);
                                visivilityAreaGrid[item.x][item.z] = newVisivilityArea;
                            }
                            if (debugMode)
                            {//見たエリアを線で表示
                                Debug.DrawLine(centerPosition + new Vector3(myPositionx, 0, myPositionz) * gridRange, centerPosition + new Vector3(item.x, 0, item.z) * gridRange, Color.green, 1f);
                            }
                        }
                    }
                    //自分が今いる場所に見た回数を足す。ただし構造体をListのFor文の中でいじれないのでコピーしていじって書き換える
                    if ((byte)(visivilityAreaGrid[myPositionx][myPositionz].watchNum) < byte.MaxValue)
                    {
                        newVisivilityArea = new VisivilityArea((byte)(visivilityAreaGrid[myPositionx][myPositionz].watchNum + 1), visivilityAreaGrid[myPositionx][myPositionz].canVisivleAreaPosition);
                        visivilityAreaGrid[myPositionx][myPositionz] = newVisivilityArea;
                    }
                }
                else {
                    if (debugMode) Debug.Log("z座標がマップからはみ出ています");
                }
                if (debugMode) Debug.Log("x座標がマップからはみ出ています");
            }

            if (debugMode) { //各マス目がどれだけ見られているかを確認する
                for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                {
                    for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                    {
                        Color drawColor;
                        if (visivilityAreaGrid[x][z].watchNum < 25) { drawColor = new Color32((byte)(10 * visivilityAreaGrid[x][z].watchNum), 0, (byte)(byte.MaxValue - (10 * visivilityAreaGrid[x][z].watchNum)), byte.MaxValue); } 
                        else {
                            drawColor = Color.red;
                        }
                        
                        Debug.DrawLine(centerPosition + new Vector3(x, 0, z) * gridRange, centerPosition + new Vector3(x, 0, z) * gridRange + new Vector3(0, 10, 0), drawColor, 1f);
                    }
                }
            }
        }




    }
}
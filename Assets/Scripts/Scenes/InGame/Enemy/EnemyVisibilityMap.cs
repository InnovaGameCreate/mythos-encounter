using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Scenes.Ingame.Enemy
{
    /// <summary>
    /// マップの視線の通り方とマップのどのあたりをどのくらい確認したかを記録してゆくクラス。敵キャラがマップを認識するのに使用されるクラス。
    /// </summary>
    public class EnemyVisibilityMap : MonoBehaviour
    {
        public List<List<VisivilityArea>> visivilityAreaGrid;//Unityの座標系を優先、一個目がx軸二個目がy軸のイメージ左下が[0][0]左上が[0][max]
        public float maxVisivilityRange;//この距離を超えているエリアは見えることはないものとする
        public bool debugMode;
        public float gridRange;
        public Vector3 centerPosition;//いちばん左下のグリッドの中央

        /// <summary>マス目の位置を2つのbyteで表し疎のマス目までの距離をfoatであらわしている</summary>
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

        /// <summary>
        /// マス目が何度見られたかをbyteで記録し、このマス目から視線の通るマス目をListで記録している
        /// </summary>
        public struct VisivilityArea
        {
            public byte watchNum;//このエリアを見た回数
            public List<DoubleByteAndMonoFloat> canVisivleAreaPosition;
            public VisivilityArea(byte sWatchNum)
            {
                watchNum = sWatchNum;
                canVisivleAreaPosition = new List<DoubleByteAndMonoFloat>();
            }
            public VisivilityArea(byte sWatchNum, List<DoubleByteAndMonoFloat> sDoubleByteAndFloat)
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

        /// <summary>
        /// マス目の集合である二次元Listを作成する。
        /// </summary>
        /// <param name="x">x座標方向にマス目をいくつ並べるか</param>
        /// <param name="z">z座標方向にマス目をいくつ並べるか</param>
        /// <param name="range">この距離以上の視線は通らないものと考えてシミュレートされる距離</param>
        /// <param name="setCenterPosition">左下のマス目の中心位置</param>
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

        /// <summary>
        /// マップをスキャンしてマス目同士での視界の通っている情報を決定する
        /// </summary>
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

        /// <summary>
        /// 自身のディープコピーを作成して返す
        /// </summary>
        /// <returns>自身のディープコピー</returns>
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


        /// <summary>
        /// 次に確認すべき最も見ておらず最も近い位置を取得。
        /// </summary>
        /// <param name="nowPosition">現在のcharacterの座標</param>
        /// <returns>次に行くべき座標</returns>
        public Vector3 GetNextNearWatchPosition(Vector3 nowPosition)
        {
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
                    if (visivilityAreaGrid[x][z].watchNum == smallestWatchNum)
                    { //最も小さい場合
                        nextPositionX.Add(x);
                        nextPositionZ.Add(z);
                    }
                    VisivilityArea newVisivilityArea = new VisivilityArea((byte)(visivilityAreaGrid[x][z].watchNum - smallestWatchNum), visivilityAreaGrid[x][z].canVisivleAreaPosition); ;
                    visivilityAreaGrid[x][z] = newVisivilityArea;
                }
            }
            //最も近い要素を考える
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

            //実際に次ぎに行くべき座標を示す
            Vector3 nextPosition = (new Vector3(nearPositionX, 0, nearPositionZ) * gridRange) + centerPosition;
            if (debugMode)
            {//次に行くべき位置を描画
                Debug.DrawLine(nextPosition, nextPosition + new Vector3(0, 20, 0), Color.magenta, 3);
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
            {//x座標がマップの範囲内であるかどうか
                if ((nowPosition.z < centerPosition.z + (visivilityAreaGrid[0].Count - 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < nowPosition.z)) //z座標がマップの範囲内であるかどうか
                {
                    if (debugMode) Debug.Log("マップの範囲内です");
                    byte myPositionx, myPositionz;//自分がどこのグリッドにいるかを確認する
                    myPositionx = (byte)Mathf.FloorToInt((float)(nowPosition.x - centerPosition.x + 0.5 * gridRange) / gridRange);
                    myPositionz = (byte)Mathf.FloorToInt((float)(nowPosition.z - centerPosition.z + 0.5 * gridRange) / gridRange);
                    foreach (DoubleByteAndMonoFloat item in visivilityAreaGrid[myPositionx][myPositionz].canVisivleAreaPosition)
                    {
                        if (item.range < visivilityRange)
                        { //見える距離
                            //見た回数を足す。ただし構造体をListのFor文の中でいじれないのでコピーしていじって書き換える。オーバーフローしない場合
                            if ((byte)(visivilityAreaGrid[item.x][item.z].watchNum) < byte.MaxValue)
                            {
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
                else
                {
                    Debug.LogError("z座標がマップからはみ出ています");
                }

            }
            else
            {
                if (debugMode) Debug.Log("x座標がマップからはみ出ています");
            }

            if (debugMode)
            { //各マス目がどれだけ見られているかを確認する
                for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                {
                    for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                    {
                        Color drawColor;
                        if (visivilityAreaGrid[x][z].watchNum < 25) { drawColor = new Color32((byte)(10 * visivilityAreaGrid[x][z].watchNum), 0, (byte)(byte.MaxValue - (10 * visivilityAreaGrid[x][z].watchNum)), byte.MaxValue); }
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
            {//x座標がマップの範囲内であるかどうか
                if ((position.z < centerPosition.z + (visivilityAreaGrid[0].Count - 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < position.z)) //z座標がマップの範囲内であるかどうか
                {
                    for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                    {
                        for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                        {
                            //マスが対象範囲か調べる                          
                            if (resetRange > Vector3.Magnitude(position - (centerPosition + new Vector3(x, 0, z) * gridRange)))
                            {
                                //対象内の場合見た回数を0とする
                                newVisivilityArea = new VisivilityArea((byte)(0), visivilityAreaGrid[x][z].canVisivleAreaPosition);
                                visivilityAreaGrid[x][z] = newVisivilityArea;
                                if (debugMode) { DrawCross((centerPosition + new Vector3(x, 0, z) * gridRange), 5, Color.magenta, 2f); }

                            }
                            else
                            {
                                //対象でない場合見た回数を1追加する(何度も音を聞いた場合に最も新しい音を対象とするため)
                                if (periodic)
                                {//細かく走りまくることで音のしていないエリアが極端に捜索先にならないようにするグリッチの対策
                                    newVisivilityArea = new VisivilityArea((byte)(visivilityAreaGrid[x][z].watchNum + 1), visivilityAreaGrid[x][z].canVisivleAreaPosition);
                                    visivilityAreaGrid[x][z] = newVisivilityArea;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("z座標がマップからはみ出ています");
                }
                
            }
            else
            {
                Debug.LogError("x座標がマップからはみ出ています");
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
            if (!((enemyPosition.z < centerPosition.z + (visivilityAreaGrid.Count - 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < enemyPosition.z)))
            {
                Debug.LogError("EnemyPosition.zが範囲外です");
                return false;
            }
            if (!((playerPosition.x < centerPosition.x + (visivilityAreaGrid.Count - 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < playerPosition.x)))
            {
                Debug.LogError("PlayerPosition.xが範囲外です");
                return false;
            }
            if (!((playerPosition.z < centerPosition.z + (visivilityAreaGrid.Count - 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < playerPosition.z)))
            {
                Debug.LogError("EPlayerPosition.zが範囲外です");
                return false;
            }
            //Enemyから見れる可能性のあるマスを取得
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

            //光が届く可能性のあるマスを取得
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

            //見ることのできる最も明るいマスを決定
            bool canLookLight = false;
            byte mostShiningGridPositionX = 0, mostShiningGridPositionZ = 0;
            float shining = 0;
            for (byte e = 0; e < enemyVisivilityGridPosition.Count; e++)
            {
                for (byte r = 0; r < rightingGridPosition.Count; r++)
                {
                    if (enemyVisivilityGridPosition[e].x == rightingGridPosition[r].x && enemyVisivilityGridPosition[e].z == rightingGridPosition[r].z)
                    {//光が届く可能性があり見えているマスを取得
                        if (enemyVisivilityGridPosition[e].range < visivilityRange && rightingGridPosition[r].range < lightRange)
                        { //見える上に光も届く
                            if (debugMode) { DrawCross((new Vector3(rightingGridPosition[r].x, 0, rightingGridPosition[r].z) * gridRange) + centerPosition, 2, Color.yellow, 1); }
                            if (shining < lightRange - rightingGridPosition[r].range)//最も明るいマスである
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

            //情報を返す
            if (canLookLight)
            {
                NextPosition = (new Vector3(mostShiningGridPositionX, 0, mostShiningGridPositionZ) * gridRange) + centerPosition;
                if (debugMode) { DrawCross(NextPosition, 5, Color.yellow, 1); Debug.Log("光が見えた！"); Debug.DrawLine(NextPosition, NextPosition + new Vector3(0, 20, 0), Color.magenta, 3); }
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
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    if (plus)
                    {
                        if ((byte)(visivilityAreaGrid[x][z].watchNum) < byte.MaxValue - change)
                        {
                            newVisivilityArea = new VisivilityArea((byte)(visivilityAreaGrid[x][z].watchNum + change), visivilityAreaGrid[x][z].canVisivleAreaPosition);
                            visivilityAreaGrid[x][z] = newVisivilityArea;
                        }
                        else
                        {
                            newVisivilityArea = new VisivilityArea(byte.MaxValue, visivilityAreaGrid[x][z].canVisivleAreaPosition);
                            visivilityAreaGrid[x][z] = newVisivilityArea;
                        }
                    }
                    else
                    {
                        if ((byte)(visivilityAreaGrid[x][z].watchNum) < byte.MinValue + change)
                        {
                            newVisivilityArea = new VisivilityArea((byte)(visivilityAreaGrid[x][z].watchNum - change), visivilityAreaGrid[x][z].canVisivleAreaPosition);
                            visivilityAreaGrid[x][z] = newVisivilityArea;
                        }
                        else
                        {
                            newVisivilityArea = new VisivilityArea(byte.MinValue, visivilityAreaGrid[x][z].canVisivleAreaPosition);
                            visivilityAreaGrid[x][z] = newVisivilityArea;
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
                for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                {
                    newVisivilityArea = new VisivilityArea(num, visivilityAreaGrid[x][z].canVisivleAreaPosition);
                    visivilityAreaGrid[x][z] = newVisivilityArea;
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
            if (!(position.x < centerPosition.x + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.x - 0.5 * gridRange < position.x))
            {
                Debug.LogError("Position.xが範囲外です");
            }
            if (!(position.z < centerPosition.z + (visivilityAreaGrid.Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < position.z))
            {
                Debug.LogError("Position.zが範囲外です");
            }
            byte gridPositionX, gridPositionZ;
            gridPositionX = (byte)Mathf.FloorToInt((float)(position.x - centerPosition.x + 0.5 * gridRange) / gridRange);
            gridPositionZ = (byte)Mathf.FloorToInt((float)(position.z - centerPosition.z + 0.5 * gridRange) / gridRange);
            newVisivilityArea = new VisivilityArea(num, visivilityAreaGrid[gridPositionX][gridPositionZ].canVisivleAreaPosition);
            visivilityAreaGrid[gridPositionX][gridPositionZ] = newVisivilityArea;
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
            {//x座標がマップの範囲内であるかどうか
                if ((playerPosition.z < centerPosition.z + (visivilityAreaGrid[0].Count + 0.5) * gridRange) && (centerPosition.z - 0.5 * gridRange < playerPosition.z)) //z座標がマップの範囲内であるかどうか
                {
                    for (byte x = 0; x < visivilityAreaGrid.Count(); x++)
                    {
                        for (byte z = 0; z < visivilityAreaGrid[0].Count(); z++)
                        {
                            //マスが対象範囲(ハードコードで50にしてある)か調べる                          
                            if (50 > Vector3.Magnitude(playerPosition - (centerPosition + new Vector3(x, 0, z) * gridRange)))
                            {

                                //対象内の場合見た回数を0とする
                                newVisivilityArea = new VisivilityArea((byte)(visivilityAreaGrid[x][z].watchNum + 1), visivilityAreaGrid[x][z].canVisivleAreaPosition);
                                visivilityAreaGrid[x][z] = newVisivilityArea;
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
                    if (debugMode) Debug.Log("z座標がマップからはみ出ています");
                }
                if (debugMode) Debug.Log("x座標がマップからはみ出ています");


            }

        }

        private void DrawCross(Vector3 position, float size, Color color, float time)
        {
            Debug.DrawLine(position + new Vector3(size / 2, 0, size / 2), position + new Vector3(-size, 0, -size), color, time);
            Debug.DrawLine(position + new Vector3(-size / 2, 0, size / 2), position + new Vector3(size / 2, 0, -size / 2), color, time);
        }


    }
}
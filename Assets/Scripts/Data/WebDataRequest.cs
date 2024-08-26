using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public enum DataType
{
    ItemTable = 0,
    SpellTable = 1,
    CharacterTable = 2,
    EnemyTable = 3
}

public enum ItemFormat
{
    id = 0,
    name = 1,
    explaranation = 2,
    category = 3,
    price = 4
}

public enum SpellFormat
{
    id = 0,
    name = 1,
    explaranation = 2,
    unlockExplaranation = 3
}
public enum PlayerFormat
{
    id = 0,
    name = 1,
    careate_date = 2,
    end_date = 3,
    money = 4,
    item = 5,
    enemy = 6,
    epell = 7
}
public enum EnemyFormat
{
    id = 0,
    name = 1,
    hp = 2,
    stamina = 3,
    armor = 4,
    walkSpeed = 5,
    dashSpeed = 6,
    attack = 7,
    actionCooltime = 8,
    hearing = 9,
    visoin = 10,
    spell = 11,
    san = 12
}

public class WebDataRequest : MonoBehaviour
{
    // データベースの情報を取得するためのURL
    private string[] databaseUrl =
        { "https://igc.deca.jp/mythos-encounter/item-get.php",
          "https://igc.deca.jp/mythos-encounter/spell-get.php",
          "https://igc.deca.jp/mythos-encounter/player-get.php",
          "https://igc.deca.jp/mythos-encounter/enemy-get.php"};
    private static List<EnemyDataStruct> EnemyDataArrayList = new List<EnemyDataStruct>();
    private static List<ItemDataStruct> ItemDataArrayList = new List<ItemDataStruct>();
    private static List<SpellStruct> SpellDataArrayList = new List<SpellStruct>();
    private static List<PlayerDataStruct> PlayerDataArrayList = new List<PlayerDataStruct>();
    private CancellationTokenSource _timeOutToken;
    private CancellationTokenSource _loadSuccessToken;
    private const int TIMEOUTMILISECOND = 10000;//タイムアウトする10秒(ミリ単位)
    private List<string[]>[] DataArrayList;
    private bool debugMode = true;
    public static List<ItemDataStruct> GetItemDataArrayList { get => ItemDataArrayList; }
    public static List<SpellStruct> GetSpellDataArrayList { get => SpellDataArrayList; }
    public static List<PlayerDataStruct> GetPlayerDataArrayList { get => PlayerDataArrayList; }
    public static List<EnemyDataStruct> GetEnemyDataArrayList { get => EnemyDataArrayList; }
    public static bool OnCompleteLoadData = false;
    void Awake()
    {
        _timeOutToken = new CancellationTokenSource();
        _loadSuccessToken = new CancellationTokenSource();
        TimeOutTimer(_loadSuccessToken.Token).Forget();
        GetData(_timeOutToken.Token).Forget();
    }

    private async UniTaskVoid GetData(CancellationToken token)
    {
        Debug.Log($"Table count = {databaseUrl.Length}");

        DataArrayList = new List<string[]>[databaseUrl.Length];

        UnityWebRequest[] request = new UnityWebRequest[databaseUrl.Length];
        //WebRequestの作成
        for (int i = 0; i < databaseUrl.Length; i++)
        {
            request[i] = UnityWebRequest.Get(databaseUrl[i]);
        }
        //データ取得まで待機
        for (int i = 0; i < databaseUrl.Length; i++)
        {
            await request[i].SendWebRequest();
        }

        //error処理
        foreach (var requestResult in request)
        {
            if (requestResult.result == UnityWebRequest.Result.ConnectionError)
            {
                throw new ApplicationException("サーバーとの接続に失敗しました");
            }
            else if (requestResult.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new ApplicationException("Status 500 ,サーバーからのデータ取得に失敗しました");
            }
        }

        for (int i = 0; i < databaseUrl.Length; i++)
        {
            DataArrayList[i] = ConvertToArrayListFrom(System.Web.HttpUtility.HtmlDecode(request[i].downloadHandler.text));
        }
        ConvertStringToItemData(DataArrayList[(int)DataType.ItemTable]);
        ConvertStringToSpellData(DataArrayList[(int)DataType.SpellTable]);
        ConvertStringToPlayerData(DataArrayList[(int)DataType.CharacterTable]);
        ConvertStringToEnemyData(DataArrayList[(int)DataType.EnemyTable]);
        OnCompleteLoadData = true;
        _loadSuccessToken.Cancel();
    }
    /// <summary>
    /// 読み込みが終わらなかったらタイムアウトさせて処理を中断する
    /// </summary>
    private async UniTaskVoid TimeOutTimer(CancellationToken token)
    {
        await UniTask.Delay(TIMEOUTMILISECOND, cancellationToken: token);
        _timeOutToken.Cancel();
        throw new TimeoutException();
    }
    /// <summary>
    /// 読み込んだスプレットシートの各要素を配列にする
    /// </summary>
    static List<string[]> ConvertToArrayListFrom(string text)
    {
        List<string[]> cardDataStringsList = new List<string[]>();
        text = text.Replace("<br>", "\n");
        StringReader reader = new StringReader(text);
        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            if (line != null)
            {
                string[] elements = line.Split(':');
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i] == "\"\"")
                    {
                        continue;
                    }

                    elements[i] = elements[i].TrimStart('"').TrimEnd('"');
                }
                cardDataStringsList.Add(elements);
            }
        }

        return cardDataStringsList;
    }
    /// <summary>
    /// 配列のデータをEnemyDataStructの型に変更させる
    /// </summary>
    private void ConvertStringToEnemyData(List<string[]> _dataArray)
    {
        EnemyDataArrayList.Clear();
        EnemyDataStruct inputTempData = new EnemyDataStruct();
        foreach (var dataRecord in _dataArray)
        {
            string[] spell = dataRecord[(int)EnemyFormat.spell].Split(',');
            inputTempData.EnemyDataSet(
                int.Parse(dataRecord[(int)EnemyFormat.id]),//ID
                dataRecord[(int)EnemyFormat.name],//名前
                int.Parse(dataRecord[(int)EnemyFormat.hp]),//hp
                int.Parse(dataRecord[(int)EnemyFormat.stamina]),//stamina
                int.Parse(dataRecord[(int)EnemyFormat.armor]),//armor
                float.Parse(dataRecord[(int)EnemyFormat.walkSpeed]),//walkSpeed
                float.Parse(dataRecord[(int)EnemyFormat.dashSpeed]),//dashSpeed
                int.Parse(dataRecord[(int)EnemyFormat.attack]),//attack
                int.Parse(dataRecord[(int)EnemyFormat.hearing]),//hearing
                int.Parse(dataRecord[(int)EnemyFormat.visoin]),//vision
                int.Parse(dataRecord[(int)EnemyFormat.actionCooltime]),//actionCooltime
                spell,//spell
                float.Parse(dataRecord[(int)EnemyFormat.san])//san
                );
            EnemyDataArrayList.Add(inputTempData);
        }
        if (debugMode) Debug.Log($"EnemyDataLoadEnd : {EnemyDataArrayList.Count}");
    }
    /// <summary>
    /// 配列のデータをItemDataStructの型に変更させる
    /// </summary>
    private void ConvertStringToItemData(List<string[]> _dataArray)
    {
        ItemDataArrayList.Clear();
        ItemDataStruct inputTempData = new ItemDataStruct();
        ItemCategory _itemCategory = new ItemCategory();
        foreach (var dataRecord in _dataArray)
        {
            switch (dataRecord[(int)ItemFormat.category])
            {
                case "shop":
                    _itemCategory = ItemCategory.Shop;
                    break;
                case "stage":
                    _itemCategory = ItemCategory.Stage;
                    break;
                case "unique":
                    _itemCategory = ItemCategory.Unique;
                    break;
                default:
                    break;
            }
            inputTempData.ItemDataSet(
                int.Parse(dataRecord[(int)ItemFormat.id]),//ID
                dataRecord[(int)ItemFormat.name],//名前
                dataRecord[(int)ItemFormat.explaranation],//説明
                _itemCategory,//category
                int.Parse(dataRecord[(int)ItemFormat.price])//price
                );
            ItemDataArrayList.Add(inputTempData);
        }
        if (debugMode) Debug.Log($"ItemDataLoadEnd : {ItemDataArrayList.Count}");
    }
    private void ConvertStringToSpellData(List<string[]> _dataArray)
    {
        SpellDataArrayList.Clear();
        SpellStruct inputTempData = new SpellStruct();
        foreach (var dataRecord in _dataArray)
        {
            inputTempData.SpellDataSet(
                int.Parse(dataRecord[(int)SpellFormat.id]),//ID
                dataRecord[(int)SpellFormat.name],//名前
                dataRecord[(int)SpellFormat.explaranation],//説明
                dataRecord[(int)SpellFormat.unlockExplaranation]//説明
                );
            SpellDataArrayList.Add(inputTempData);
        }
        if (debugMode) Debug.Log($"SpellDataLoadEnd : {SpellDataArrayList.Count}"); 
    }
    /// <summary>
    /// 配列のデータをPlayerDataStructの型に変更させる
    /// </summary>
    private void ConvertStringToPlayerData(List<string[]> _dataArray)
    {
        PlayerDataArrayList.Clear();
        PlayerDataStruct inputTempData = new PlayerDataStruct();
        foreach (var dataRecord in _dataArray)
        {
            string[] items = dataRecord[(int)PlayerFormat.item].Split(',');
            string[] spell = dataRecord[(int)PlayerFormat.epell].Split(',');
            string[] enemy = dataRecord[(int)PlayerFormat.enemy].Split(',');
            inputTempData.PlayerDataSet(
                int.Parse(dataRecord[(int)PlayerFormat.id]),//ID
                dataRecord[(int)PlayerFormat.name],//名前
                DateTime.Parse(dataRecord[(int)PlayerFormat.careate_date]),//createdDate
                DateTime.Parse(dataRecord[(int)PlayerFormat.end_date]),//endDate
                int.Parse(dataRecord[(int)PlayerFormat.money]),//money
                items,//items
                enemy,//mythPoint
                spell,//spell
                enemy.Length//escape
                );
            PlayerDataArrayList.Add(inputTempData);
        }
        if (debugMode) Debug.Log($"PlayerDataLoadEnd : {PlayerDataArrayList.Count}");
    }
    private void OnDestroy()
    {
        _loadSuccessToken.Cancel();
        _loadSuccessToken.Dispose();
        _timeOutToken.Cancel();
        _timeOutToken.Dispose();
    }
}

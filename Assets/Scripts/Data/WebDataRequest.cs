using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public enum DataType
{
    CharacterTable = 0,
    ItemTable = 1,
    EnemyDataTable = 2,
}
public class WebDataRequest : MonoBehaviour
{
    private const string SheetID = "18s4lIZRvuOdiEjQVsyX7BKPhbGpw6SdjSY6NIHYWL1k";
    private string[] SheetName = { "CharacterTable", "ItemTable", "EnemyDataTable" };
    public List<EnemyDataStruct> EnemyDataArrayList = new List<EnemyDataStruct>();
    public List<ItemDataStruct> ItemDataArrayList = new List<ItemDataStruct>();
    public List<PlayerDataStruct> PlayerDataArrayList = new List<PlayerDataStruct>();
    private CancellationTokenSource _timeOutToken;
    private CancellationTokenSource _loadSuccessToken;
    private const int TIMEOUTMILISECOND = 10000;//10秒
    private List<string[]>[] DataArrayList;
    private bool debugMode = true;
    void Start()
    {

        _timeOutToken = new CancellationTokenSource();
        _loadSuccessToken = new CancellationTokenSource();
        TimeOutTimer(_loadSuccessToken.Token).Forget();
        GetData(_timeOutToken.Token).Forget();
    }

    private async UniTaskVoid GetData(CancellationToken token)
    {
        DataArrayList = new List<string[]>[SheetName.Length];
        UnityWebRequest[] request = new UnityWebRequest[SheetName.Length];
        for (int i = 0; i < SheetName.Length; i++)
        {
            request[i] = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/" + SheetID + "/gviz/tq?tqx=out:csv&sheet=" + SheetName[i]);
        }
        for (int i = 0; i < SheetName.Length; i++)
        {
            await request[i].SendWebRequest();
        }

        foreach (var requestResult in request)
        {
            if (requestResult.result == UnityWebRequest.Result.ConnectionError)
            {
                throw new ApplicationException("サーバーとの接続に失敗しました");
            }
            else if (requestResult.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new ApplicationException("サーバーからのデータ取得に失敗しました");
            }
        }

        if (request[SheetName.Length - 1].isHttpError || request[SheetName.Length - 1].isNetworkError)
        {
            Debug.Log(request[SheetName.Length].error);
        }
        else
        {
            for (int i = 0; i < SheetName.Length; i++)
            {
                DataArrayList[i] = ConvertToArrayListFrom(request[i].downloadHandler.text);
            }
            ConvertStringToPlayerData(DataArrayList[(int)DataType.CharacterTable]);
            ConvertStringToItemData(DataArrayList[(int)DataType.ItemTable]);
            ConvertStringToEnemyData(DataArrayList[(int)DataType.EnemyDataTable]);
        }
        _loadSuccessToken.Cancel();
    }
    private async UniTaskVoid TimeOutTimer(CancellationToken token)
    {
        await UniTask.Delay(TIMEOUTMILISECOND, cancellationToken: token);
        _timeOutToken.Cancel();
        throw new TimeoutException();
    }
    static List<string[]> ConvertToArrayListFrom(string text)
    {
        List<string[]> cardDataStringsList = new List<string[]>();
        StringReader reader = new StringReader(text);
        reader.ReadLine();
        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            if (line != null)
            {
                string[] elements = line.Split(',');
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
    private void ConvertStringToEnemyData(List<string[]> _dataArray)
    {
        EnemyDataArrayList.Clear();
        EnemyDataStruct inputTempData = new EnemyDataStruct();
        foreach (var dataRecord in _dataArray)
        {
            string[] armor = dataRecord[4].Split('/');
            string[] moveSpeed = dataRecord[5].Split('/');
            inputTempData.EnemyDataSet(
                int.Parse(dataRecord[0]),//ID
                dataRecord[1],//名前
                int.Parse(dataRecord[2]),//hp
                int.Parse(dataRecord[3]),//stamina
                int.Parse(armor[0]),//adArmor
                int.Parse(armor[1]),//mrArmor
                int.Parse(moveSpeed[0]),//walkSpeed
                int.Parse(moveSpeed[1]),//dashSpeed
                int.Parse(dataRecord[6]),//attackPower
                int.Parse(dataRecord[7]),//hearing
                int.Parse(dataRecord[8]),//vision
                int.Parse(dataRecord[9]),//actionCooltime
                dataRecord[10],//magic
                int.Parse(dataRecord[11]),//magicCount
                float.Parse(dataRecord[12])//san
                );
            EnemyDataArrayList.Add(inputTempData);
        }
        if (debugMode) Debug.Log("EnemyDataLoadEnd");
    }
    private void ConvertStringToItemData(List<string[]> _dataArray)
    {
        ItemDataArrayList.Clear();
        ItemDataStruct inputTempData = new ItemDataStruct();
        ItemCategory _itemCategory = new ItemCategory();
        foreach (var dataRecord in _dataArray)
        {
            switch (dataRecord[3])
            {
                case "Shop":
                    _itemCategory = ItemCategory.Shop;
                    break;
                case "Stage":
                    _itemCategory = ItemCategory.Stage;
                    break;
                case "Unique":
                    _itemCategory = ItemCategory.Unique;
                    break;
                default:
                    break;
            }
            inputTempData.ItemDataSet(
                int.Parse(dataRecord[0]),//ID
                dataRecord[1],//名前
                dataRecord[2],//説明
                _itemCategory,//category
                int.Parse(dataRecord[4])//price
                );
            ItemDataArrayList.Add(inputTempData);
        }
        if (debugMode) Debug.Log("ItemDataLoadEnd");
    }
    private void ConvertStringToPlayerData(List<string[]> _dataArray)
    {
        PlayerDataArrayList.Clear();
        PlayerDataStruct inputTempData = new PlayerDataStruct();
        foreach (var dataRecord in _dataArray)
        {
            string[] items = dataRecord[3].Split('/');
            string[] spell = dataRecord[5].Split('/');
            string[] mythCreature = dataRecord[6].Split('/');
            inputTempData.PlayerDataSet(
                int.Parse(dataRecord[0]),//ID
                dataRecord[1],//名前
                int.Parse(dataRecord[2]),//money
                items,//items
                int.Parse(dataRecord[4]),//mythPoint
                spell,//spell
                mythCreature,//mythCreature
                int.Parse(dataRecord[7]),//escape
                int.Parse(dataRecord[8])//dispersingEscape
                );
            PlayerDataArrayList.Add(inputTempData);
        }
        if (debugMode) Debug.Log("PlayerDataLoadEnd");
    }
    private void OnDestroy()
    {
        _loadSuccessToken.Cancel();
        _loadSuccessToken.Dispose();
        _timeOutToken.Cancel();
        _timeOutToken.Dispose();
    }
}

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
    public List<EnemyData> EnemyDataArrayList = new List<EnemyData>();
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
        EnemyData inputTempData = new EnemyData();
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
    private void OnDestroy()
    {
        _loadSuccessToken.Cancel();
        _loadSuccessToken.Dispose();
        _timeOutToken.Cancel();
        _timeOutToken.Dispose();
    }
}

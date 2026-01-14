using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class ResultManager : MonoBehaviour
{
    public static ResultManager instance;

    [SerializeField]
    private List<ResultEntryData> resultEntryDatas = new List<ResultEntryData>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// リザルトデータを生成する（ゲーム終了時に1回だけ呼ぶ）
    /// </summary>
    public void CreateResultEntryList(List<PlayerData> playerDataList)
    {
        resultEntryDatas.Clear();

        foreach(var playerData in playerDataList)
        {
            ResultEntryData entry = new ResultEntryData(
                playerData.playerName,
                playerData.money,
                playerData.targetMoney,
                0
                );
            resultEntryDatas.Add(entry);
        }
        SortAndSetRank();
    }

    private void SortAndSetRank()
    {
        resultEntryDatas = resultEntryDatas
            .OrderByDescending(e => e.money).ToList();
        for(int i = 0; i < resultEntryDatas.Count; i++)
        {
            resultEntryDatas[i].rank = i + 1;
        }
    }

    public List<ResultEntryData> GetResultEntryList()
    {
        return resultEntryDatas;
    }
}

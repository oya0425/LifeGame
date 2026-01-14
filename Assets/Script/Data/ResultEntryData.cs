using UnityEngine;
[System.Serializable]
public class ResultEntryData
{
    [Tooltip("–¼‘O")]
    public string playerName;
    [Tooltip("Š‹à")]
    public int money;
    [Tooltip("–Ú•W‹àŠz")]
    public int targetMoney;
    [Tooltip("‡ˆÊ")]
    public int rank;

    public ResultEntryData(string playerName, int money, int targetMoney, int rank)
    {
        this.playerName = playerName;
        this.money = money;
        this.targetMoney = targetMoney;
        this.rank = rank;
    }
}

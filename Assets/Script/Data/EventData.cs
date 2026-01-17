using UnityEngine;

[CreateAssetMenu(
    fileName = "EventData",
    menuName = "LifeGame/Event"
)]
public class EventData : ScriptableObject
{
    [Tooltip("イベントの名前")]
    public string eventName;

    [Tooltip("")]public string[] texts;          // 順番に表示するテキスト
    [Tooltip("選択肢上")]
    public string choiceAText;  
    [Tooltip("選択肢下")]
    public string choiceBText;
    [Tooltip("選択肢上の結果テキスト")]
    public string resultAText;
    [Tooltip("選択肢下の結果テキスト")]
    public string resultBText;

    public int choiceAMinMoney;
    public int choiceAMaxMoney;

    public int choiceBMinMoney;
    public int choiceBMaxMoney;

    public Sprite backGround;
    public Sprite mainImg;
    public Sprite resultAImage;
    public Sprite resultBImage;

    /// <summary>
    /// 選択肢Aを選んだときの金額（ランダム）
    /// </summary>
    public int GetChoiceAMoney()
    {
        return Random.Range(choiceAMinMoney, choiceAMaxMoney + 1);
    }

    /// <summary>
    /// 選択肢Bを選んだときの金額（ランダム）
    /// </summary>
    public int GetChoiceBMoney()
    {
        return Random.Range(choiceBMinMoney, choiceBMaxMoney + 1);
    }
}

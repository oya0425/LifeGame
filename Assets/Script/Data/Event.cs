using UnityEngine;

public class Event : MonoBehaviour
{
    [Tooltip("")]public string[] texts;          // 順番に表示するテキスト
    [Tooltip("選択肢上")]
    public string choiceAText;  
    [Tooltip("選択肢下")]
    public string choiceBText;
    [Tooltip("選択肢上の結果テキスト")]
    public string resultAText;
    [Tooltip("選択肢下の結果テキスト")]
    public string resultBText;
}

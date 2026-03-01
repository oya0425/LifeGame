using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnTextUI : MonoBehaviour
{
    [Tooltip("現在のターン数表示")]
    [SerializeField] private TextMeshProUGUI nowTurnText;

    public void UpdateTurnText(int currentTurn,int allTurn)
    {
        nowTurnText.gameObject.SetActive(true);
        if(currentTurn==allTurn)
        {
            nowTurnText.text = $"<color=red>ラストターン</color>：{currentTurn}/{allTurn}";
        }
        else
        {
            nowTurnText.text = $"ターン：{currentTurn}/{allTurn}";
        }
    }

}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnTextUI : MonoBehaviour
{
    [Tooltip("åªç›ÇÃÉ^Å[Éìêîï\é¶")]
    [SerializeField] private TextMeshProUGUI nowTurnText;

    public void UpdateTurnText(int currentTurn,int allTurn)
    {
        nowTurnText.gameObject.SetActive(true);
        nowTurnText.text = $"TURN:{currentTurn}/{allTurn}";
    }

}

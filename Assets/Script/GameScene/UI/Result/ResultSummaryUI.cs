using UnityEngine;
using TMPro;

public class ResultSummaryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI titleName;
    [SerializeField] TextMeshProUGUI targetMoneyText;
    [SerializeField] TextMeshProUGUI currentMoneyText;
    [SerializeField] TextMeshProUGUI diffMoneyText;

    public void Setup(PlayerData playerData)
    {
        nameText.text = playerData.playerName;
        titleName.text =
            $"「{playerData.targetGoalData.title}」";

        targetMoneyText.text =
            $"目標金額：\n{MyUtility.FormatMoneyManEn(playerData.targetMoney)}";

        currentMoneyText.text =
            $"所持金：\n{MyUtility.FormatMoneyManEn(playerData.money)}";

        diffMoneyText.text =
            $"差額：\n{MyUtility.FormatEventMoneyManEn(playerData.money - playerData.targetMoney)}";    }
}

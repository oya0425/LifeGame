using TMPro;
using UnityEngine;

public class RankingItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rankText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI moneyText;

    public void Setup(int rank, string name, int money,Color playerColor)
    {
        if (rankText != null)
        {
            rankText.text = rank.ToString();
        }
        if (nameText != null)
        {
            nameText.text = name.ToString();
            nameText.color = playerColor;
        }
        if(moneyText != null)
        {
            moneyText.text = money.ToString(); 
        }
    }

}

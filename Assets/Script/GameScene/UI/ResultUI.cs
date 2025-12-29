using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [SerializeField] GameObject root;

    [SerializeField] TextMeshProUGUI resultText;
    
    /// <summary>
    /// ƒŠƒUƒ‹ƒg•\Ž¦
    /// </summary>
    public void Show(List<PlayerData> players)
    {
        root.SetActive(true);
        resultText.text = "";

        int rank = 1;
        foreach (var player in players)
        {
            resultText.text +=
                $"{rank}ˆÊ  {player.name}  Money:{player.money}–œ‰~\n";
            rank++;
        }
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}

using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStatusView : MonoBehaviour
{
    [Tooltip("薄暗い背景")]
    [SerializeField] Image BackGround;

    [Tooltip("playerの色")]
    [SerializeField] Image PlayerColor;

    [Tooltip("お金数値")]
    [SerializeField] TextMeshProUGUI moneyValueText;

    [Tooltip("お金単位")]
    [SerializeField] TextMeshProUGUI moneyTani;

    [Tooltip("現在表示しているプレイヤーデータ")]
    PlayerData currentPlayerData;

    [Tooltip("表示内容をセットする")]
    public void SetPlayer(PlayerData playerData)
    {
        currentPlayerData = playerData;
        RefreshView();
    }

    [Tooltip("表示を更新する")]
    void RefreshView()
    {
        if (currentPlayerData == null) return;

        SetPlayerColor(currentPlayerData.playerColor);
        SetMoney(currentPlayerData.money);
    }

    [Tooltip("プレイヤーカラー反映")]
    void SetPlayerColor(Color color)
    {
        PlayerColor.color = color;
    }

    [Tooltip("所持金反映")]
    void SetMoney(int money)
    {   
        moneyValueText.text = money.ToString();
    }

    [Tooltip("UI表示ON")]
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    [Tooltip("UI非表示")]
    public void Hide()
    {
        this.gameObject?.SetActive(false);
    }

}

using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class PlayerStatusView : MonoBehaviour
{
    [Tooltip("薄暗い背景")]
    [SerializeField] Image BackGround;

    [Tooltip("playerの色")]
    [SerializeField] Image PlayerColor;

    [Tooltip("お金")]
    [SerializeField] TextMeshProUGUI moneyValueText;

    [Tooltip("目標金額")]
    [SerializeField] TextMeshProUGUI moneyTargetText;

    int displayedMoney;
    Coroutine moneyAnimCoroutine;

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
        displayedMoney = currentPlayerData.money;
        SetPlayerColor(currentPlayerData.playerColor);
        SetMoney(currentPlayerData.money);
        SetTargetMoney(currentPlayerData.targetMoney);
    }

    [Tooltip("プレイヤーカラー反映")]
    void SetPlayerColor(Color color)
    {
        PlayerColor.color = color;
    }

    [Tooltip("所持金反映")]
    void SetMoney(int money)
    {
        moneyValueText.text = MyUtility.FormatMoneyManEn(money);
    }
    public void ChangeSetMoney(int money)
    {
        // 非表示中は即時反映のみ（アニメしない）
        if (!gameObject.activeInHierarchy)
        {
            StopMoneyAnimationIfNeeded();

            displayedMoney = money;
            moneyValueText.text = MyUtility.FormatMoneyManEn(money);
            return;
        }

        // 既存アニメ停止
        StopMoneyAnimationIfNeeded();

        // 新しいアニメ開始
        moneyAnimCoroutine = StartCoroutine(
            MyUtility.AnimateMoney(
                displayedMoney,
                money,
                0.8f,
                value =>
                {
                    displayedMoney = value;
                    moneyValueText.text = MyUtility.FormatMoneyManEn(value);
                }
            )
        );
    }

    private void StopMoneyAnimationIfNeeded()
    {
        if (moneyAnimCoroutine != null)
        {
            StopCoroutine(moneyAnimCoroutine);
            moneyAnimCoroutine = null;
        }
    }

    [Tooltip("目標金額反映")]
    void SetTargetMoney(int money)
    {
        moneyTargetText.text = MyUtility.FormatMoneyManEn(money);
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

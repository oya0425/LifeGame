using UnityEngine;
using TMPro;
using System;

public class EventTextManager : TextBoxBase
{
    [SerializeField, Header("UI全体の親")] GameObject window;
    [SerializeField,Header("メッセージテキスト")] TextMeshProUGUI messageText;

    [Tooltip("クリック通知用")]
    public Action OnClicked;

    // --- 音 ---
    [SerializeField] private AudioManager audioManager;

    public void Show()
    {
        window.SetActive(true);
    }
    public void Hide()
    {
        window.SetActive(false);
        HideNextArrow();
    }

    public void SetMessageText(string text)
    {
        messageText.text = text;
        ShowNextArrow();
    }

    // UI の Button / EventTrigger から呼ぶ
    public void Click()
    {
        audioManager.PlaySE("MouseClickSE");
        OnClicked?.Invoke();
    }
}

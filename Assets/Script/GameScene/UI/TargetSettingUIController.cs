using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TargetSettingUIController : MonoBehaviour
{
    [SerializeField] GameObject window;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button[] choiceButtons;
    [SerializeField] TextMeshProUGUI[] choiceButtonTexts;

    [Tooltip("選択肢が押された通知")]
    public Action<int> OnChoiceSelected;

    // --- 追加 ---
    string defaultMessage;  
    string[] currentDescriptions;//説明文を上書きではなく、最初に登録したものを選択して表示する
    bool isHovering;

    private void Start()
    {
        Hide();

        // ボタン押下登録
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i;
            choiceButtons[i].onClick.AddListener(() =>
            {
                OnChoiceSelected?.Invoke(index);
            });
        }
    }

    /// <summary> UI表示 </summary>
    public void Show()
    {
        ResetButtonVisuals();
        window.SetActive(true);
    }

    /// <summary> UI非表示 </summary>
    public void Hide()
    {
        window.SetActive(false);
    }

    public void HideChoices()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 通常メッセージ設定（ホバーしてない時用）
    /// </summary>
    public void SetMessage(string text)
    {
        defaultMessage = text;

        if (!isHovering)
        {
            messageText.text = text;
        }
    }

    /// <summary>
    /// 選択肢タイトル＋説明文を同時に設定
    /// </summary>
    public void SetChoices(string[] titles, string[] descriptions)
    {
        
        currentDescriptions = descriptions;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < titles.Length)
            {
                choiceButtonTexts[i].text = titles[i];
                choiceButtons[i].gameObject.SetActive(true);
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 説明文表示
    /// </summary>
    public void ShowDescription(string text)
    {
        isHovering = true;
        messageText.text = text;
    }

    /// <summary>
    /// 説明文解除
    /// </summary>
    public void ClearDescription()
    {
        isHovering = false;
        messageText.text = defaultMessage;
    }

    /// <summary>
    /// ボタン hover 時
    /// </summary>
    public void OnHoverEnter(int index)
    {
        if (currentDescriptions == null) return;
        if (index < 0 || index >= currentDescriptions.Length) return;

        ShowDescription(currentDescriptions[index]);
    }

    /// <summary>
    /// ボタン hover 解除
    /// </summary>
    public void OnHoverExit()
    {
        ClearDescription();
    }


    /// <summary>
    /// ボタンの初期化
    /// </summary>
    void ResetButtonVisuals()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Button button = choiceButtons[i];

            button.interactable = true;

            //状態をNormalに戻す
            button.OnPointerExit(null);
            // Text の色を明示的に戻す（重要）
            if (choiceButtonTexts != null && i < choiceButtonTexts.Length)
            {
                choiceButtonTexts[i].color = Color.white;
            }
        }
    }

}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUIController : MonoBehaviour
{
    public static ItemUIController instance;
 
    [SerializeField] private GameObject window;

    [Tooltip("所持アイテム画像入れる場所")][SerializeField]
    private Image[] imgItem;

    [Tooltip("使用ボタン(ホールド時説明出す)")][SerializeField]
    Button[] itemButtons;

    [Tooltip("アイテムの説明文"), SerializeField]
    TextMeshProUGUI messageText;

    [Tooltip("選択肢が押された通知")]
    public Action<int> OnChoiceSelected;


    public string defaultMessage;
    string[] currentDescriptions;//説明文を上書きではなく、最初に登録したものを選択して表示する
    bool isHovering;


    private void Awake()
    {
        instance = this;
        window.SetActive(false);
    }

    private void Start()
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            int index = i;

            itemButtons[i].onClick.AddListener(() =>
                OnChoiceSelected?.Invoke(index)
            );
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
    public void SetChoices(PlayerData playerData, string[] descriptions)
    {

        currentDescriptions = descriptions;

        for (int i = 0; i < 3; i++)
        {
            if (i < playerData.itemList.Count)
            {
                imgItem[i].gameObject.SetActive(true);
                imgItem[i].sprite = playerData.itemList[i].itemImage;
            }
            else
            {
                imgItem[i].gameObject.SetActive(false);
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
        for (int i = 0; i < itemButtons.Length; i++)
        {
            Button button = itemButtons[i];

            button.interactable = true;
        }
    }

}

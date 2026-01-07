using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
public class EventUIController : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] GameObject eventWindow;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI eventText;

    [Header("Choices")]
    [SerializeField] Button choiceAButton;
    [SerializeField] Button choiceBButton;
    [SerializeField] TextMeshProUGUI choiceAText;
    [SerializeField] TextMeshProUGUI choiceBText;

    /// <summary>
    /// テキスト枠がクリックされた通知
    /// </summary>
    public Action OnTextClicked;

    /// <summary>
    /// 選択肢Aが押された通知
    /// </summary>
    public Action OnChoiceASelected;

    /// <summary>
    /// 選択肢Bが押された通知
    /// </summary>
    public Action OnChoiceBSelected;

    void Start()
    {
        // 初期状態はすべて非表示
        HideAll();

        // ボタン登録
        choiceAButton.onClick.AddListener(() =>
        {
            OnChoiceASelected?.Invoke();
        });

        choiceBButton.onClick.AddListener(() =>
        {
            OnChoiceBSelected?.Invoke();
        });
    }

    /// <summary>
    /// イベントUI全体を表示 </summary>
    public void ShowWindow()
    {
        eventWindow.SetActive(true);
    }

    /// <summary>
    /// イベントUI全体を非表示
    /// </summary>
    public void HideAll()
    {
        eventWindow.SetActive(false);
        choiceAButton.gameObject.SetActive(false);
        choiceBButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// イベントテキストを表示
    /// </summary>
    public void SetEventText(string text)
    {
        eventText.text = text;
    }

    /// <summary>
    /// 選択肢を表示
    /// </summary>
    public void ShowChoices(string aText, string bText)
    {
        choiceAText.text = aText;
        choiceBText.text = bText;

        choiceAButton.gameObject.SetActive(true);
        choiceBButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// 選択肢を非表示
    /// </summary>
    public void HideChoices()
    {
        choiceAButton.gameObject.SetActive(false);
        choiceBButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// テキスト枠クリック用（EventTextにEventTriggerやButtonで紐付け）
    /// </summary>
    public void OnTextAreaClicked()
    {
        Debug.Log("テキストクリック通った");
        OnTextClicked?.Invoke();
    }
}

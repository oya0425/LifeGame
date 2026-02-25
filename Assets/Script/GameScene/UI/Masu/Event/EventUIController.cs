using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
public class EventUIController : TextBoxBase
{
    [Header("Window")]
    [SerializeField] GameObject eventWindow;
    [SerializeField] CanvasGroup canvasGroup;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI eventText;
    [SerializeField] TextMeshProUGUI eventNameText;

    [Header("Choices")]
    [SerializeField] Button choiceAButton;
    [SerializeField] Button choiceBButton;
    [SerializeField] TextMeshProUGUI choiceAText;
    [SerializeField] TextMeshProUGUI choiceBText;

    [Header("Image")]
    [SerializeField] Image backGround;
    [SerializeField] Image mainImg;
    [SerializeField] Image resultAImage;
    [SerializeField] Image resultBImage;

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
        // CanvasGroupの自動取得
        if (canvasGroup == null) canvasGroup = eventWindow.GetComponent<CanvasGroup>();

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


    // --- フェードアウト機能 ---

    /// <summary>
    /// UI全体をゆっくり消す
    /// </summary>
    public void FadeOut(Action onComplete)
    {
        // 二重でコルーチンが走らないよう、念のため止めてから開始
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(onComplete));
    }

    private IEnumerator FadeOutRoutine(Action onComplete)
    {
        float duration = 0.5f; // 消える時間
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        HideAll(); // 完全に消えたら非アクティブ化
        onComplete?.Invoke();
    }

    /// <summary>
    /// イベントUI全体を表示 </summary>
    public void ShowWindow()
    {
        canvasGroup.alpha = 1f;
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
        //eventText.text = $"{text}\n" + "<align=right>クリックで次へ</align>";
        eventText.text = text;
        ShowNextArrow();
    }

    public void SetEventNameText(string text)
    {
        eventNameText.text = $"～{text}～";
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
        
        //選択肢が出たときにクリック状態を消す（逆△）
        HideNextArrow();
    }

    /// <summary>
    /// 選択肢を非表示
    /// </summary>
    public void HideChoices()
    {
        choiceAButton.gameObject.SetActive(false);
        choiceBButton.gameObject.SetActive(false);
        ShowNextArrow();
    }

    /// <summary>
    /// テキスト枠クリック用（EventTextにEventTriggerやButtonで紐付け）
    /// </summary>
    public void OnTextAreaClicked()
    {
        Debug.Log("テキストクリック通った");
        OnTextClicked?.Invoke();
    }

    public void SetBackGround(Sprite sprite)
    {
        if (backGround == null) return;

        backGround.sprite = sprite;
    }
    public void SetresultAImage(Sprite sprite)
    {
        if (resultAImage == null) return;

        resultAImage.sprite = sprite;
    }
    public void SetresultBImage(Sprite sprite)
    {
        if (resultBImage == null) return;

        resultBImage.sprite = sprite;
    }
    public void SetMainImage(Sprite sprite)
    {
        if (mainImg == null) return;

        mainImg.sprite = sprite;
    }


}

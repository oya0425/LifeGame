using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class LuckyUIController : TextBoxBase
{
    [SerializeField] GameObject window;

    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField, Tooltip("獲得したアイテムの画像")]
    Image imgItem;



    [SerializeField, Tooltip("アイテム説明テキスト")]
    TextMeshProUGUI itemDiscriptionText;
    [SerializeField, Tooltip("～をもらったテキスト")]
    TextMeshProUGUI resultText;

    // --- 音 ---
    [SerializeField] private AudioManager audioManager;



    /// <summary>
    /// テキスト枠がクリックされた通知
    /// </summary>
    public System.Action OnTextClicked;

    private void Start()
    {
        if (canvasGroup == null) canvasGroup = window.GetComponent<CanvasGroup>();

        itemDiscriptionText.text = "";
        resultText.text = "";
        imgItem.enabled = false;
        Hide();
    }

    /// <summary>
    /// 全体の表示 
    /// </summary>
    public void Show()
    {
        canvasGroup.alpha = 1f;
        window.SetActive(true);
    }
    /// <summary>
    /// 全体の非表示 
    /// </summary>
    public void Hide()
    {
        window.SetActive(false);
        HideNextArrow();

    }

    /// <summary>
    /// 全体をまとめてフェードアウトさせる
    /// </summary>
    public void FadeOut(System.Action onComplete)
    {
        StartCoroutine(FadeOutRoutine(onComplete));
    }

    // --- 画像全体を薄くして消す ---
    private IEnumerator FadeOutRoutine(System.Action onComplete)
    {
        float duration = 0.5f; // 消えるまでの秒数
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        Hide(); // 完全に消えたら非アクティブ化
        onComplete?.Invoke();
    }

    /// <summary>
    /// 説明文をセット 
    /// </summary>
    public void SetItemDiscriptionText(string discription)
    {
        itemDiscriptionText.text = $"説明\n{discription}";
    }
    /// <summary>
    /// 何のアイテムを獲得したかのset　
    /// </summary>
    /// <param name="itemName"></param>
    public void SetResultText(string itemName)
    {
        resultText.text = $"{itemName}をもらった。\n"
           /* + "<align=right>クリックで次へ</align>"*/;
        ShowNextArrow();

    }

    public void SetItemImage(Sprite itemImg)
    {
        imgItem.sprite = itemImg;
        imgItem.enabled = (itemImg != null);
    }

    /// <summary>
    /// テキスト枠クリック用(Buttonにつける) 
    /// </summary>
    public void OnTextAreaClicked()
    {
        audioManager.PlaySE("MouseClickSE");
        OnTextClicked?.Invoke();
    }



}

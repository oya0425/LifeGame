using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckyUIController : TextBoxBase
{
    [SerializeField] GameObject window;
    [SerializeField, Tooltip("獲得したアイテムの画像")]
    Image imgItem;

    [SerializeField, Tooltip("アイテム説明テキスト")]
    TextMeshProUGUI itemDiscriptionText;
    [SerializeField, Tooltip("～をもらったテキスト")]
    TextMeshProUGUI resultText;

    /// <summary>
    /// テキスト枠がクリックされた通知
    /// </summary>
    public System.Action OnTextClicked;

    private void Start()
    {
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
        Debug.Log("テキストクリック通った");
        OnTextClicked?.Invoke();
    }



}

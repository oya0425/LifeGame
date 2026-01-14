using UnityEngine;
using TMPro;

public class OrderSelectUI : MonoBehaviour
{
    [SerializeField, Header("Window")]
    GameObject window;

    [SerializeField, Header("テキストメッセージ")]
    TextMeshProUGUI messageText;

    /// <summary>
    ///  UI全体の表示
    /// </summary>
    public void Show()
    {
        window.SetActive(true);
    }
    /// <summary>
    ///  UI全体の非表示
    /// </summary>
    public void Hide()
    {
        window.SetActive(false);
    }

    public void SetMessage(string text)
    {
        if (messageText == null) return;
        messageText.text = text;
    }
}

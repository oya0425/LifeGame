using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverTextColor : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler,
    ISelectHandler,
    IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI targetText;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.black;

    private void ResetColor()
    {
        if (targetText != null)
            targetText.color = normalColor;
    }

    /// <summary>
    /// マウスが乗ったとき
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetText.color = hoverColor;
    }

    /// <summary>
    /// マウスが離れたとき
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        ResetColor();
    }

    /// <summary>
    /// クリックされたとき
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 一度選択された状態を即解除する
        EventSystem.current.SetSelectedGameObject(null);
        ResetColor();
    }

    /// <summary>
    /// キーボード / コントローラ操作などで選択されたとき
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        targetText.color = hoverColor;
    }

    /// <summary>
    /// 選択解除されたとき
    /// </summary>
    public void OnDeselect(BaseEventData eventData)
    {
        ResetColor();
    }

    /// <summary>
    /// 非表示・無効化時の保険
    /// </summary>
    private void OnDisable()
    {
        ResetColor();
    }
}

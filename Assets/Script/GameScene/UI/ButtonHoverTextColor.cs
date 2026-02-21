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
    [SerializeField] private RectTransform targetImageRect;

    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.black;

    [Header("Animation Settings")]
    [SerializeField] private float lerpSpeed = 0.2f; // C++版の 0.2f に合わせました
    [SerializeField] private float targetSize = 1.2f;
    private bool isHovered = false;
    private float currentScale = 1.0f; // 現在のスケール値
    private Vector3 initialScale;

    private void Awake()
    {
        if (targetImageRect != null)
            initialScale = targetImageRect.localScale;
    }

    private void Update()
    {
        if (targetImageRect == null) return;

        // C++側のロジックを再現
        float targetScaleValue = isHovered ? targetSize : 1.0f;

        // 目標サイズに向けて計算： current += (target - current) * speed
        // Unityでは Time.deltaTime を掛けることでフレームレートに依存しない動きになります
        currentScale += (targetScaleValue - currentScale) * (lerpSpeed * 60f * Time.deltaTime);

        // スケールを適用
        targetImageRect.localScale = initialScale * currentScale;
    }

    private void SetState(bool hovered)
    {
        isHovered = hovered;
        if (targetText != null)
            targetText.color = hovered ? hoverColor : normalColor;
    }

    // --- イベントハンドラ ---
    /// <summary>
    /// マウスが乗ったとき
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData) => SetState(true);

    /// <summary> マウスが離れたとき
    /// </summary>
    public void OnPointerExit(PointerEventData eventData) => SetState(false);

    /// <summary>
    /// キーボード / コントローラ操作などで選択されたとき
    /// </summary>
    public void OnSelect(BaseEventData eventData) => SetState(true);

    public void OnDeselect(BaseEventData eventData) => SetState(false);

    /// <summary>
    /// クリックされたとき
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // クリックしたら選択解除して、状態をリセット（縮小させる）
        EventSystem.current.SetSelectedGameObject(null);
        SetState(false);
    }

    /// <summary>
    /// 非表示・無効化時の保険
    /// </summary>
    private void OnDisable()
    {
        isHovered = false;
        currentScale = 1.0f;
        if (targetImageRect != null) targetImageRect.localScale = initialScale;
        if (targetText != null) targetText.color = normalColor;
    }
}


using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private Selectable selectable;// Buttonなどのコンポーネント保持用

    [SerializeField] private AudioManager audioManager;


    private void Awake()
    {
        if (targetImageRect != null)
            initialScale = targetImageRect.localScale;
        // 同じオブジェクトにあるButtonやToggleなどのコンポーネントを取得
        selectable = GetComponent<Selectable>();
    }
    // インタラクティブ（有効）かどうかを判定するプロパティ
    private bool IsInteractable => selectable == null || selectable.interactable;

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
        if (!IsInteractable)
        {
            isHovered = false;
            UpdateVisuals(false);
            return;
        }
        if (hovered && !isHovered)
        {
            if (audioManager != null)
            {
                audioManager.PlaySE("CursorSE");
            }
        }
        isHovered = hovered;
        if (targetText != null)
            targetText.color = hovered ? hoverColor : normalColor;

    }
    private void UpdateVisuals(bool hovered)
    {
        if (targetText == null) return;

        if (!IsInteractable)
            targetText.color = normalColor;
        else
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


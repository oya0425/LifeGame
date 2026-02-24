using UnityEngine;
using UnityEngine.EventSystems;

// --- ホバー時に拡大縮小 ---
public class RouletteHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private RectTransform targetImageRect;

    private Vector3 initialScale;
    [SerializeField] private float lerpSpeed = 0.2f; // C++版の 0.2f に合わせました
    [SerializeField] private float targetSize = 1.2f;
    private bool isHovered = false;
    private float currentScale = 1.0f; // 現在のスケール値

    void Start()
    {
        if (targetImageRect != null)
            initialScale = targetImageRect.localScale;
        if (initialScale == Vector3.zero) initialScale = Vector3.one;
    }
    private void SetState(bool hovered)
    {
        isHovered = hovered;
    }

    private void Update()
    {
        if (targetImageRect == null) return;
        // C++側のロジックを再現
        float targetScaleValue = isHovered ? targetSize : 1.0f;
        if (DiceSpinner.instance.GetIsSpinning())
        {
            // 回転中はホバーに関係なく強制的に「1.0」を目指す
            targetScaleValue = 1.0f;
        }
        else
        {
            // 回転していない時だけ、ホバー状態を見る
            targetScaleValue = isHovered ? targetSize : 1.0f;
        }

        // 目標サイズに向けて計算： current += (target - current) * speed
        // Unityでは Time.deltaTime を掛けることでフレームレートに依存しない動きになります
        currentScale += (targetScaleValue - currentScale) * (lerpSpeed * 60f * Time.deltaTime);

        // スケールを適用
        targetImageRect.localScale = initialScale * currentScale;
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
    /// クリックされたとき
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // クリックしたら選択解除して、状態をリセット（縮小させる）
        EventSystem.current.SetSelectedGameObject(null);
        SetState(false);
    }
    private void OnDisable()
    {
        isHovered = false;
        currentScale = 1.0f;
        if (targetImageRect != null) targetImageRect.localScale = initialScale;
    }


}
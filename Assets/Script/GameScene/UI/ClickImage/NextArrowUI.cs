using UnityEngine;

public class NextArrowUI : MonoBehaviour
{
    [SerializeField] float moveRange = 10f;   // è„â∫Ç…ìÆÇ≠ãóó£
    [SerializeField] float speed = 2f;         // ìÆÇ≠ë¨Ç≥

    RectTransform rect;
    Vector2 startPos;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }
    private void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, moveRange);
        rect.anchoredPosition = startPos + Vector2.up * offset;

    }
}

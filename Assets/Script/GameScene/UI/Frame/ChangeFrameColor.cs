using UnityEngine;
using UnityEngine.UI;

public class ChangeFrameColor : MonoBehaviour
{
    [SerializeField]
    private Image[] frameImages; // 枠用Image（2つ想定）

    /// <summary>
    /// 枠の色を変更する
    /// </summary>
    public void SetFrameColor(Color color)
    {
        if (frameImages == null) return;

        foreach (var image in frameImages)
        {
            image.gameObject.SetActive(true);
            if (image == null) continue;
            image.color = color;
        }
    }
}

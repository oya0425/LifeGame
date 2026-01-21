using UnityEngine;

public class FrameColorController : MonoBehaviour
{
    private ChangeFrameColor[] frameColorTargets;

    private void Awake()
    {
        // Ž©•ª‚ÌŽqŠK‘w‚É‚ ‚é ChangeFrameColor ‚ð‘S•”Žæ“¾
        frameColorTargets = GetComponentsInChildren<ChangeFrameColor>(true);
    }

    public void SetColor(Color color)
    {
        foreach (var target in frameColorTargets)
        {
            target.SetFrameColor(color);
        }
    }
}

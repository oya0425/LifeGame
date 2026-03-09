using UnityEngine;
using System.Collections;
public class ScreenManager : MonoBehaviour
{
    private int defaultWidth;
    private int defaultHeight;

    void Awake()
    {
        defaultWidth = Screen.currentResolution.width;
        defaultHeight = Screen.currentResolution.height;

    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            && Input.GetKeyDown(KeyCode.Return))
        {
            ToggleResolution();
        }
    }

    void ToggleResolution()
    {
        if (Screen.fullScreen)
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
        else
        {
            Screen.SetResolution(defaultWidth, defaultHeight, FullScreenMode.FullScreenWindow);
        }

    }

}
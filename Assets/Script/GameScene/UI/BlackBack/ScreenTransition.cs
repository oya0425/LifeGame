using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ScreenTransition : MonoBehaviour
{
    public static ScreenTransition instance;

    [SerializeField] private RectTransform blackImage;
    [SerializeField] float expandDuration = 0.5f;
    [SerializeField] float shrinkDuration = 1.0f;
    
    private void Awake()
    {
        instance = this;

    }

    /// <summary>
    /// èkè¨ÇæÇØ </summary>
    public IEnumerator PlayShrink()
    {
        blackImage.gameObject.SetActive(true);
        blackImage.localScale=Vector3.one;

        yield return new WaitForSeconds(0.5f);
        yield return Scale(Vector3.one, Vector3.zero, shrinkDuration);
        blackImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// ägëÂÇæÇØ </summary>
    public IEnumerator PlayerExpandShrink()
    { 
        blackImage.gameObject.SetActive(true);
        blackImage.localScale = Vector3.one;

        yield return Scale(Vector3.zero, Vector3.one, expandDuration);
    }


    //ã§í ÇÃägëÂèkè¨
    private IEnumerator Scale(Vector3 from, Vector3 to, float d)
    {
        float time = 0f;
        while (time < d)
        {
            time += Time.deltaTime;
            float t = time / d;
            t = 1f - Mathf.Pow(1f - t, 2f);
            blackImage.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        blackImage.localScale = Vector3.one * 0.001f;
    }
}

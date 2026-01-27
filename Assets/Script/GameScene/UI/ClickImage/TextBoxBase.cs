using UnityEngine;

public class TextBoxBase : MonoBehaviour 
{
    [SerializeField] GameObject nextArrow;

    public void ShowNextArrow()
    {
        nextArrow.SetActive(true);
    }

    public void HideNextArrow()
    {
        nextArrow.SetActive(false);
    }
}

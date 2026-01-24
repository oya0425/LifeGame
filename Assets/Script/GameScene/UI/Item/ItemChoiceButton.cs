using UnityEngine;
using UnityEngine.EventSystems;


public class ItemChoiceButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public ItemUIController itemUIcontroller;

    public void OnPointerEnter(PointerEventData eventData)
    {
        itemUIcontroller.OnHoverEnter(index);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        itemUIcontroller.OnHoverExit();

    }
}

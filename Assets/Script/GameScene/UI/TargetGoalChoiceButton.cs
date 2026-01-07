using UnityEngine;
using UnityEngine.EventSystems;

public class TargetGoalChoiceButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public TargetSettingUIController uiController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiController.OnHoverEnter(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiController.OnHoverExit();
    }
}

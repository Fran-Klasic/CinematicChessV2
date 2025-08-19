using UnityEngine;
using UnityEngine.EventSystems;

public class ConfirmationUI_Delete : MonoBehaviour
{
    public void OnClick()
    {
        if (MainController.Instance.isAnimating) return;

        MainController.Instance.hasDeleted = true;

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationSettingsUI_PlayAnimation : MonoBehaviour
{
    [SerializeField] Animator transition;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        AnimationSettingsUI_Core.SaveAllData();

        //Load AnimationScene

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }
}
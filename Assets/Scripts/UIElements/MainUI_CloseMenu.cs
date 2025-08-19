using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainUI_CloseMenu : MonoBehaviour
{
    [SerializeField] private Animator playMenuAnimator;
    [SerializeField] private GameObject playMenuGroup;

    public void OnClick()
    {
        if (playMenuAnimator == null) return;
        if (MainController.Instance.isAnimating) return;

        if (MainController.Instance.isPlayMenuActive)
        {
            StartCoroutine(EndPlayMenu());
        }

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator EndPlayMenu()
    {
        MainController.Instance.isAnimating = true;

        playMenuAnimator.SetTrigger("End");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        playMenuGroup.SetActive(false);

        MainController.Instance.isPlayMenuActive = false;
        MainController.Instance.isAnimating = false;
    }
}
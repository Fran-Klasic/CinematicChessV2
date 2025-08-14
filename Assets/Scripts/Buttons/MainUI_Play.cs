using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainUI_Play : MonoBehaviour
{
    [SerializeField] private Animator playMenuAnimator;
    [SerializeField] private GameObject playMenuGroup;

    public void OnClick()
    {
        if (playMenuAnimator == null) return;
        if (MainController.Instance.isAnimating) return;

        if (!MainController.Instance.isPlayMenuActive)
            StartCoroutine(StartPlayMenu());
        else
            //StartCoroutine(EndPlayMenu());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartPlayMenu()
    {
        MainController.Instance.isAnimating = true;

        playMenuGroup.SetActive(true);

        playMenuAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        MainController.Instance.isPlayMenuActive = true;
        MainController.Instance.isAnimating = false;
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
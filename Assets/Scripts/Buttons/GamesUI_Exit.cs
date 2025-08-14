using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GamesUI_Exit : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private GameObject games_Group;
    [SerializeField] private GameObject titleScreen_Group;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(StartTitleScreen());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartTitleScreen()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");
        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        titleScreen_Group.SetActive(true);

        MainController.Instance.isAnimating = false;
        games_Group.SetActive(false);

        transition.SetTrigger("In");
        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
    }

}
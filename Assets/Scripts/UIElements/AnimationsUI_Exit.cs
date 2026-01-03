using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class AnimationsUI_Exit : MonoBehaviour
{
    [SerializeField] private Animator transition;

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

        MainController.Instance.isAnimating = false;

        //Due to button animations and OnClick bugs the only way to load TitleScreen is to reload the entire scene ):
        SceneManager.Instance.LoadMainUI();
    }

}
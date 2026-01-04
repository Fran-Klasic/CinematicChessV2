using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainUI_PlayVSBot : MonoBehaviour
{
    [SerializeField] private Animator transition;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(PlayBotButton());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator PlayBotButton()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        MainController.Instance.isAnimating = false;
        SceneManager.Instance.LoadChess2dVSBot();
    }
}
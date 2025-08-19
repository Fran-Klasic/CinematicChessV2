using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainUI_PlayVSFriend : MonoBehaviour
{
    [SerializeField] private Animator transition;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(PlayFriendButton());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator PlayFriendButton()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        MainController.Instance.isAnimating = false;
        SceneManager.Instance.LoadChess2dVSFriend();
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainUI_Quit : MonoBehaviour
{
    [SerializeField] private Animator transition;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(QuitGame());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator QuitGame()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        MainController.Instance.isAnimating = false;
    }
}
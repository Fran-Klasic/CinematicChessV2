using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Chess2dUI_PlayAgain : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private Animator gameEndCanvasAnimator;
    [SerializeField] private GameObject gameEndCanvas;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(TransitionOut());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator TransitionOut()
    {
        transition.SetTrigger("Out");
        gameEndCanvasAnimator.SetTrigger("End");
        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
        gameEndCanvas.SetActive(false);

        //Get currnet scene and load it again
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        SceneManager.Scenes currentScene = SceneManager.Instance.GetScene(scene.name);
        SceneManager.Instance.LoadScene(currentScene);
    }
}
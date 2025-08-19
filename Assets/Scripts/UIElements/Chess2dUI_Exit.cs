using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Chess2dUI_Exit : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private Animator gameEndCanvasAnimator;
    [SerializeField] private GameObject gameEndCanvas;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(StartMainUI());
    }

    private IEnumerator StartMainUI()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");
        gameEndCanvasAnimator.SetTrigger("End");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
        gameEndCanvas.SetActive(false);

        MainController.Instance.isAnimating = false;

        SceneManager.Instance.LoadMainUI();
    }
}
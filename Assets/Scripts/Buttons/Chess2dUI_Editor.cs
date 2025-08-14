using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Chess2dUI_Editor : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private Animator gameEndCanvasAnimator;
    [SerializeField] private GameObject gameEndCanvas;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        FileManager.GameRecord record = GameStateManager.Instance.newestGame;

        StartCoroutine(StartEditor(record));

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartEditor(FileManager.GameRecord record)
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");
        gameEndCanvasAnimator.SetTrigger("End");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
        gameEndCanvas.SetActive(false);

        MainController.Instance.isAnimating = false;

        MainController.Instance.OpenEditor(record);
    }
}
using UnityEngine;
using System.Collections;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class Chess2dUI_Resign : MonoBehaviour
{
    [SerializeField] private Animator gameEndCanvasAnimator;
    [SerializeField] private GameObject gameEndCanvas;

    public void OnClick()
    {
        if (MainController.Instance.isAnimating) return;

        GameStateManager.Instance.didResign = true;

        gameEndCanvas.SetActive(true);
        StartCoroutine(StartGameEndCanvas());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartGameEndCanvas()
    {
        MainController.Instance.isAnimating = true;
        yield return new WaitForEndOfFrame();

        #region Result Parsing
        TextMeshProUGUI result = gameEndCanvas.transform.Find("GameEndScreen/Result").gameObject.GetComponent<TextMeshProUGUI>();
        string formatedResult;
        switch (GameStateManager.Instance.EndState)
        {
            case GameStateManager.End.BlackWon:
                formatedResult = "Black Won!!!";
                break;
            case GameStateManager.End.WhiteWon:
                formatedResult = "White Won!!!";
                break;
            case GameStateManager.End.Draw:
                formatedResult = "Draw!!!";
                break;
            default:
                formatedResult = "null";
                break;
        }
        result.text = formatedResult;
        #endregion

        #region Time Parsing
        TextMeshProUGUI totalTime = gameEndCanvas.transform.Find("GameEndScreen/VerticalLayout/TotalTime").gameObject.GetComponent<TextMeshProUGUI>();
        float seconds = StopwatchManager.Instance.GetTime();
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

        string formatedTime;

        if (timeSpan.Hours > 0)
            formatedTime = $"{timeSpan.Hours}h {timeSpan.Minutes}min {timeSpan.Seconds}sec";
        else if (timeSpan.Minutes > 0)
            formatedTime = $"{timeSpan.Minutes}min {timeSpan.Seconds}sec";
        else
            formatedTime = $"{timeSpan.Seconds}sec";
        totalTime.text = "Game Lasted: " + formatedTime;
        #endregion

        gameEndCanvasAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
        MainController.Instance.isAnimating = false;
    }
}
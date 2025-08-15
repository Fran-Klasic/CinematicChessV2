using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static ChessPieceBase;

public class GameStateManager : MonoBehaviour
{
    public enum State { Playing, Promoting, GameEnd, GameStart }
    public enum Promotion { White, Black }
    public enum End { WhiteWon, BlackWon, Draw, None }

    [HideInInspector] public PieceColor CurrentTurn = PieceColor.White;
    [HideInInspector] public State GameState;
    [HideInInspector] public Promotion PromotionState;
    [HideInInspector] public End EndState = End.None;
    [HideInInspector] public bool didResign = false;
    [HideInInspector] public FileManager.GameRecord newestGame;

    [SerializeField] private GameObject gameEndCanvas;
    [SerializeField] private Animator gameEndCanvasAnimator;

    public static GameStateManager Instance;

    private bool updateState = true;

    private void Update()
    {
        if (updateState)
        {
            var logicBoardState = Logic.GetLogicBoardState(BoardManager.Instance.BoardState);
            bool isWhiteInCheckmate = MoveValidator.Instance.IsCheckmate(logicBoardState, PieceColor.White);
            //bool isWhiteInCheck = MoveValidator.Instance.IsCheck(logicBoardState, PieceColor.White); //For audio
            bool isBlackInCheckmate = MoveValidator.Instance.IsCheckmate(logicBoardState, PieceColor.Black);
            //bool isBlackInCheck = MoveValidator.Instance.IsCheck(logicBoardState, PieceColor.Black); //For audio
            bool isWhiteInStalemate = MoveValidator.Instance.IsStalemate(logicBoardState, PieceColor.White);
            bool isBlackInStalemate = MoveValidator.Instance.IsStalemate(logicBoardState, PieceColor.Black);

            #region Game End Checks
            if (isWhiteInStalemate || isBlackInStalemate)
            {
                //Game ended in a draw
                EndState = End.Draw;
                //Update game state
                GameState = State.GameEnd;
                //No need to check anything again
                updateState = false;
            }

            if (isWhiteInCheckmate)
            {
                //Game ended, black won
                EndState = End.BlackWon;
                //Update game state
                GameState = State.GameEnd;
                //No need to check anything again
                updateState = false;
            }

            if (isBlackInCheckmate)
            {
                //Game ended, white won
                EndState = End.WhiteWon;
                //Update game state
                GameState = State.GameEnd;
                //No need to check anything again
                updateState = false;
            }

            if (didResign)
            {
                //Game ended, someone resigned
                if(CurrentTurn == PieceColor.White)
                {
                    EndState = End.BlackWon;
                }
                else
                {
                    EndState = End.WhiteWon;
                }

                //Update game state
                GameState = State.GameEnd;

                //No need to check anything again
                updateState = false;
            }

            #endregion

            if (GameState == State.GameEnd)
            {
                newestGame = FileManager.Instance.SaveHistoryToGames(EndState, StopwatchManager.Instance.GetTime());

                GameEndAnimation();

                Debug.Log("Game has been saved");
                Debug.Log(EndState);
                updateState = false;
            }
        }
    }

    private void GameEndAnimation()
    {
        gameEndCanvas.SetActive(true);
        StartCoroutine(StartGameEndCanvas());
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

    //GameStateManager.Instance.--- -> Singleton
    private void Awake()
    {
        //TEMP
        GameState = State.Playing;
        EndState = End.None;

        //History Clearing
        FileManager.Instance.History.Clear();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SwitchTurn()
    {
        CurrentTurn = (CurrentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
    }
}
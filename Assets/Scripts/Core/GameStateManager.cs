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

    public static GameStateManager Instance;

    private bool updateState = true;

    public float GetTime()
    {
        return Time.time;
    }

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

            if (GameState == State.GameEnd)
            {
                GameHistoryManager.Instance.SaveHistoryToGames(EndState);
                //Start end cutscene (fade out)
                //Change scene to editor
                Debug.Log("Game has been saved");
                updateState = false;
            }
        }
    }

    //GameStateManager.Instance.--- -> Singleton
    private void Awake()
    {
        //TEMP
        GameState = State.Playing;
        EndState = End.None;

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
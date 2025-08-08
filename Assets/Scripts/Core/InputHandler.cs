using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using static ChessPieceBase;
using static GameHistoryManager;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;

    public ChessUIManager UImanager;

    private ChessPieceBase selectedPiece = null;
    private List<Vector2Int> legalMoves = null;
    private List<Vector2Int> castleMoves = null;
    private List<Vector2Int> enPassantMove = null;
    private PieceColor EnPassantIsPossibleOnColor = PieceColor.None;
    private ChessPieceBase promotionData = null;
    private Vector2Int promotionOrigin;
    private bool promotionCaptured;

    private SceneManager.Scenes currentScene;

    private float targetCameraSpeed = 0;
    private float currentCameraSpeed = 0;
    private float velocity = 0f;
    private float smoothTime = 2f;

    private void ClearValues()
    {
        selectedPiece = null;
        legalMoves = null;
        castleMoves = null;
        enPassantMove = null;
        UImanager.ClearAll();
    }

    private void ClickedOnLegalMove(Vector2Int from, Vector2Int to)
    {
        ClearValues();



        //EN PASSANT
        if (EnPassantIsPossibleOnColor != GameStateManager.Instance.CurrentTurn)
        {
            EnPassantIsPossibleOnColor = PieceColor.None;
            BoardManager.Instance.EnPassantAttribute.Clear();
        }

        //Check if a pawn moved two forward
        var piece = BoardManager.Instance.BoardState[from.x, from.y];
        if (piece != null && piece.Type == PieceType.Pawn)
        {
            int direction = (piece.Color == PieceColor.White) ? 1 : -1;
            Vector2Int forwardTwo = from + new Vector2Int(0, 2 * direction);
            if (forwardTwo == to)
            {
                //Pawn has moved two forward
                var logicBoardState = Logic.GetLogicBoardState(BoardManager.Instance.BoardState);
                //Clear old value, set new value, start counter for turn
                EnPassantIsPossibleOnColor = MoveValidator.Instance.MovedTwoForward(logicBoardState, from);
            }
        }

        bool hasCaptured = BoardManager.Instance.BoardState[to.x, to.y] != null;
        promotionCaptured = hasCaptured;

        //Actual move
        BoardManager.Instance.MovePiece(from, to);


        //PROMOTION
        promotionData = null;

        //Before switching the turn check if piece is now valid for promotion
        var pieceAtPositionTo = BoardManager.Instance.BoardState[to.x, to.y];
        //If piece is valid for promotion update game state and display promotion UI
        if (PromotionManager.Instance.Promote(pieceAtPositionTo.BoardPosition, pieceAtPositionTo.Color))
        {
            //Save promotionData if promote is possible
            promotionData = pieceAtPositionTo;
            promotionOrigin = from;
            return;
        }

        GameStateManager.Instance.SwitchTurn();

        float time = GameStateManager.Instance.GetTime();

        //Logging move
        GameHistoryManager.Instance.LogMove(from, to, pieceAtPositionTo.Type, pieceAtPositionTo.Color, SpecialMove.None, PieceType.None, time, hasCaptured);
    }

    private void ClickedOnCastleMove(Vector2Int from, Vector2Int to)
    {
        ClearValues();

        float time = GameStateManager.Instance.GetTime();

        if (EnPassantIsPossibleOnColor != GameStateManager.Instance.CurrentTurn)
        {
            EnPassantIsPossibleOnColor = PieceColor.None;
            BoardManager.Instance.EnPassantAttribute.Clear();
        }

        BoardManager.Instance.MovePieceCastling(from, to);

        GameStateManager.Instance.SwitchTurn();

        //Logging move
        var pieceAtPositionTo = BoardManager.Instance.BoardState[to.x, to.y];
        Instance.LogMove(from, to, pieceAtPositionTo.Type, pieceAtPositionTo.Color, SpecialMove.Castle, PieceType.None, time, false);
    }

    private void ClickedOnEnPassantMove(Vector2Int from, Vector2Int to)
    {
        ClearValues();

        float time = GameStateManager.Instance.GetTime();

        if (EnPassantIsPossibleOnColor != GameStateManager.Instance.CurrentTurn)
        {
            EnPassantIsPossibleOnColor = PieceColor.None;
            BoardManager.Instance.EnPassantAttribute.Clear();
        }

        BoardManager.Instance.MovePieceEnPassant(from, to);

        GameStateManager.Instance.SwitchTurn();

        //Logging move
        var pieceAtPositionTo = BoardManager.Instance.BoardState[to.x, to.y];
        Instance.LogMove(from, to, pieceAtPositionTo.Type, pieceAtPositionTo.Color, SpecialMove.EnPassant, PieceType.None, time, true);
    }

    private void Start()
    {
        mainCamera = Camera.main;

        //Check for scene
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        currentScene = SceneManager.Instance.GetScene(scene.name);
    }

    //Update functions for "Chess2d"
    private void HandleClickChess2d(Collider2D hit)
    {
        if (GameStateManager.Instance.GameState == GameStateManager.State.Playing)
        {
            if (hit)
            {
                //Get hit GameObject
                ChessPieceBase piece = hit.gameObject.GetComponentInParent<ChessPieceBase>();
                Tile tile = hit.gameObject.GetComponentInParent<Tile>();

                //Clears highlights if clicked piece is not a legal move
                if (piece != null)
                {
                    Vector2Int boardPosition = piece.BoardPosition;

                    bool isLegalMove = legalMoves != null && legalMoves.Contains(boardPosition);

                    if (!isLegalMove)
                    {
                        ClearValues();
                    }
                    else
                    {
                        //Selected piece is a legal move
                        ClickedOnLegalMove(selectedPiece.BoardPosition, boardPosition);
                        return;
                    }
                }

                //Clicked a tile
                if (tile != null)
                {
                    Vector2Int boardPosition = tile.BoardPosition;

                    Debug.Log($"Clicked on a tile: {boardPosition}");

                    //Check if clicked tile is a legal move
                    bool isTileLegalMove = legalMoves != null ? legalMoves.Contains(boardPosition) : false;
                    bool isTileCastlingMove = castleMoves != null ? castleMoves.Contains(boardPosition) : false;
                    bool isTileEnPassantMove = enPassantMove != null ? enPassantMove.Contains(boardPosition) : false;

                    //First check for special moves: 

                    //Check for castling
                    if (isTileCastlingMove && selectedPiece != null && selectedPiece.Type == ChessPieceBase.PieceType.King)
                    {
                        //Clicked tile is a castling move
                        ClickedOnCastleMove(selectedPiece.BoardPosition, boardPosition);
                        return;
                    }

                    //Check for En Passant
                    if (isTileEnPassantMove && selectedPiece != null && selectedPiece.Type == ChessPieceBase.PieceType.Pawn)
                    {
                        ClickedOnEnPassantMove(selectedPiece.BoardPosition, boardPosition);
                        return;
                    }

                    //if it is not a legal move clear all highlights
                    if (!isTileLegalMove)
                    {
                        ClearValues();
                    }
                    else
                    {
                        //Clicked tile is a legal move
                        ClickedOnLegalMove(selectedPiece.BoardPosition, boardPosition);
                        return;
                    }
                }
                //Clicked a valid piece
                if (piece != null && GameStateManager.Instance.CurrentTurn == piece.Color)
                {
                    //Places Select Highlight and clears old Highlights
                    UImanager.ClearAll();
                    UImanager.PlaceSelect(piece.BoardPosition);

                    //Select logic
                    selectedPiece = piece;

                    //Get legal moves
                    legalMoves = piece.GetLegalMoves(BoardManager.Instance.BoardState);

                    var logicBoardState = Logic.GetLogicBoardState(BoardManager.Instance.BoardState);

                    //Filter legal moves
                    legalMoves = MoveValidator.Instance.RemoveUnsafeMoves(logicBoardState, legalMoves, piece.BoardPosition);

                    //Get castleMoves if king
                    if (piece.Type == PieceType.King)
                        castleMoves = MoveValidator.Instance.GetCastleMoves(logicBoardState, piece.Color);

                    //Get enPassantMove if pawn
                    if (piece.Type == PieceType.Pawn)
                        enPassantMove = MoveValidator.Instance.GetEnPassantMove(logicBoardState, piece.Color, piece.BoardPosition);

                    //Place legal moves highlight
                    UImanager.PlaceLegalMoves(legalMoves);
                    UImanager.PlaceSpecialMoves(castleMoves);
                    UImanager.PlaceSpecialMoves(enPassantMove);
                }
            }
            else
            {
                //Clicked outside the board

                ClearValues();
            }
        }
        if (GameStateManager.Instance.GameState == GameStateManager.State.Promoting)
        {
            if (hit)
            {
                var obj = hit.GameObject();


                PieceType chosenType = PieceType.None;

                switch (obj.name)
                {
                    case "QueenOption":
                        chosenType = PieceType.Queen;
                        break;
                    case "RookOption":
                        chosenType = PieceType.Rook;
                        break;
                    case "BishopOption":
                        chosenType = PieceType.Bishop;
                        break;
                    case "KnightOption":
                        chosenType = PieceType.Knight;
                        break;
                }

                Debug.Log($"Promoted to: {chosenType}");

                if (chosenType == PieceType.None)
                    return;

                int x = promotionData.BoardPosition.x;
                int y = promotionData.BoardPosition.y;

                PromotionManager.Instance.HasPromotedTo(x, y, chosenType, promotionData.Color);

                GameStateManager.Instance.GameState = GameStateManager.State.Playing;
                GameStateManager.Instance.SwitchTurn();

                //Logging move
                float time = GameStateManager.Instance.GetTime();
                Instance.LogMove(promotionOrigin, promotionData.BoardPosition, PieceType.Pawn, promotionData.Color, SpecialMove.Promotion, chosenType, time, promotionCaptured);
            }
        }
        if (GameStateManager.Instance.GameState == GameStateManager.State.GameEnd)
        {
            Debug.Log($"The game has ended and the result is: {GameStateManager.Instance.EndState}");
        }
    }

    private void HandleClickMainUI(Collider2D hit)
    {

    }

    private void HandleCameraSpeedMainUI()
    {
        if (currentCameraSpeed == targetCameraSpeed) return;

        currentCameraSpeed = Mathf.SmoothDamp(currentCameraSpeed, targetCameraSpeed, ref velocity, smoothTime);
        mainCamera.gameObject.transform.position += Vector3.right * currentCameraSpeed * Time.deltaTime;
    }

    private void Update()
    {
        if (currentScene == SceneManager.Scenes.MainUI)
            HandleCameraSpeedMainUI();


        //Needs Mouse
        if (Mouse.current == null) return;

        //Gets mouse position for parallax speed
        if (currentScene == SceneManager.Scenes.MainUI)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            bool isOnScreen = screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height;

            if (isOnScreen)
            {
                float screenLength = Screen.width;

                //0 - left / 1 - right
                float cursorPosition = screenPos.x / screenLength;
                float speed;

                if (cursorPosition > 0.5f)
                {
                    //Go forwards
                    speed = cursorPosition * 10;
                }
                else if (cursorPosition < 0.5f)
                {
                    float temp = 1f - cursorPosition;
                    //Go backwards
                    speed = -1 * temp * 10;
                }
                else
                {
                    speed = 0f;
                }

                targetCameraSpeed = speed;

            }
        }

        //Left Click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            switch (currentScene)
            {
                case SceneManager.Scenes.Chess2dVSFriend:
                    HandleClickChess2d(hit);
                    break;
                case SceneManager.Scenes.MainUI:
                    HandleClickMainUI(hit);
                    break;
            }
        }
    }
}

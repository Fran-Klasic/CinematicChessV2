using UnityEngine;
using System.Collections.Generic;
using static ChessPieceBase;

public class PromotionManager : MonoBehaviour
{
    [Header("Prefabs: ")]
    [SerializeField] private GameObject whitePromotionUIPrefab;
    [SerializeField] private GameObject blackPromotionUIPrefab;

    [Space(10)]
    [Header("Colliders: ")]
    [SerializeField] private GameObject queenOption;
    [SerializeField] private GameObject rookOption;
    [SerializeField] private GameObject bishopOption;
    [SerializeField] private GameObject knightOption;

    [Space(10)]
    [Header("Background: ")]
    [SerializeField] private GameObject background;

    [Header("White Pieces")]
    [SerializeField] private GameObject whiteRookPrefab;
    [SerializeField] private GameObject whiteKnightPrefab;
    [SerializeField] private GameObject whiteBishopPrefab;
    [SerializeField] private GameObject whiteQueenPrefab;

    [Space(10)]
    [Header("Black Pieces")]
    [SerializeField] private GameObject blackRookPrefab;
    [SerializeField] private GameObject blackKnightPrefab;
    [SerializeField] private GameObject blackBishopPrefab;
    [SerializeField] private GameObject blackQueenPrefab;


    private List<GameObject> allObjects = new List<GameObject>();

    private void GetAllObjects()
    {
        allObjects.Add(whitePromotionUIPrefab);
        allObjects.Add(blackPromotionUIPrefab);
        allObjects.Add(queenOption);
        allObjects.Add(rookOption);
        allObjects.Add(bishopOption);
        allObjects.Add(knightOption);
        allObjects.Add(background);
    }

    private void Deactivate()
    {
        foreach (var obj in allObjects)
        {
            obj.SetActive(false);
        }
    }

    private void Activate()
    {
        foreach (var obj in allObjects)
        {
            obj.SetActive(true);
        }
    }

    public void WhiteIsPromoting()
    {
        //Activate all objects and deactivate blackPromotionUIPrefab
        Activate();
        blackPromotionUIPrefab.SetActive(false);

        //Update game state
        GameStateManager.Instance.GameState = GameStateManager.State.Promoting;
        GameStateManager.Instance.PromotionState = GameStateManager.Promotion.White;
    }

    public void BlackIsPromoting()
    {
        //Activate all objects and deactivate whitePromotionUIPrefab
        Activate();
        whitePromotionUIPrefab.SetActive(false);

        //Update game state
        GameStateManager.Instance.GameState = GameStateManager.State.Promoting;
        GameStateManager.Instance.PromotionState = GameStateManager.Promotion.Black;
    }

    public void HasPromotedTo(int x, int y, PieceType type, PieceColor color)
    {
        GameObject prefab = null;

        if (color == PieceColor.White)
        {
            switch (type)
            {
                case PieceType.Queen:
                    prefab = whiteQueenPrefab;
                    break;
                case PieceType.Rook:
                    prefab = whiteRookPrefab;
                    break;
                case PieceType.Bishop:
                    prefab = whiteBishopPrefab;
                    break;
                case PieceType.Knight:
                    prefab = whiteKnightPrefab;
                    break;
            }
        }
        else if (color == PieceColor.Black)
        {
            switch (type)
            {
                case PieceType.Queen:
                    prefab = blackQueenPrefab;
                    break;
                case PieceType.Rook:
                    prefab = blackRookPrefab;
                    break;
                case PieceType.Bishop:
                    prefab = blackBishopPrefab;
                    break;
                case PieceType.Knight:
                    prefab = blackKnightPrefab;
                    break;
            }
        }

        if (prefab == null)
            return;

        //Remove old piece
        GameObject oldPiece = BoardManager.Instance.PieceGameObjects[new Vector2Int(x, y)];

        if (oldPiece == null)
            return;

        Destroy(oldPiece);

        //Replace old piece data and put new piece
        BoardManager.Instance.InitializePiece(x, y, prefab, type, color);

        //Update HasMoved to true
        BoardManager.Instance.BoardState[x, y].HasMoved = true;

        //Deactivate UI for promoting
        Deactivate();
    }

    public bool Promote(Vector2Int position, PieceColor color)
    {
        var piece = BoardManager.Instance.BoardState[position.x, position.y];
        if (piece.Type != PieceType.Pawn)
            return false;

        //If a white pawn is at the end of the board
        if (position.y == 7 && color == PieceColor.White)
        {
            WhiteIsPromoting();
            return true;
        }
        //If a black pawn is at the start of the board
        if (position.y == 0 && color == PieceColor.Black)
        {
            BlackIsPromoting();
            return true;
        }

        return false;
    }

    public static PromotionManager Instance;
    //PromotionManager.Instance.--- -> Singleton
    private void Awake()
    {
        GetAllObjects();
        Deactivate();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
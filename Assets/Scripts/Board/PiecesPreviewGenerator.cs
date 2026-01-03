using UnityEngine;
using System.Collections.Generic;
using static ChessPieceBase;
using Assets.Scripts.Pieces;

public class PiecesPreviewGenerator : MonoBehaviour
{
    public static PiecesPreviewGenerator Instance;
    //PiecesPreviewGenerator.Instance.--- -> Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [Header("White Pieces")]
    [SerializeField] private GameObject whitePawnPrefab;
    [SerializeField] private GameObject whiteRookPrefab;
    [SerializeField] private GameObject whiteKnightPrefab;
    [SerializeField] private GameObject whiteBishopPrefab;
    [SerializeField] private GameObject whiteQueenPrefab;
    [SerializeField] private GameObject whiteKingPrefab;

    [Space(10)]
    [Header("Black Pieces")]
    [SerializeField] private GameObject blackPawnPrefab;
    [SerializeField] private GameObject blackRookPrefab;
    [SerializeField] private GameObject blackKnightPrefab;
    [SerializeField] private GameObject blackBishopPrefab;
    [SerializeField] private GameObject blackQueenPrefab;
    [SerializeField] private GameObject blackKingPrefab;

    private float width;
    private float height;
    private float tileWidth;
    private float tileHeight;
    private float posX;
    private float posY;

    [HideInInspector] public Dictionary<Vector2Int, GameObject> pieceInfo = new Dictionary<Vector2Int, GameObject>();

    public void GeneratePieces()
    {
        pieceInfo.Clear();

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        tileWidth = width / 8f;
        tileHeight = height / 8f;

        int x, y;

        for (x = 0; x < 8; x++)
        {
            //White
            y = 1;
            InitializePiece(x, y, whitePawnPrefab, rectTransform);

            //Black
            y = 6;
            InitializePiece(x, y, blackPawnPrefab, rectTransform);
        }

        // Rooks
        InitializePiece(0, 0, whiteRookPrefab, rectTransform);
        InitializePiece(7, 0, whiteRookPrefab, rectTransform);
        InitializePiece(0, 7, blackRookPrefab, rectTransform);
        InitializePiece(7, 7, blackRookPrefab, rectTransform);

        // Knights
        InitializePiece(1, 0, whiteKnightPrefab, rectTransform);
        InitializePiece(6, 0, whiteKnightPrefab, rectTransform);
        InitializePiece(1, 7, blackKnightPrefab, rectTransform);
        InitializePiece(6, 7, blackKnightPrefab, rectTransform);

        // Bishops
        InitializePiece(2, 0, whiteBishopPrefab, rectTransform);
        InitializePiece(5, 0, whiteBishopPrefab, rectTransform);
        InitializePiece(2, 7, blackBishopPrefab, rectTransform);
        InitializePiece(5, 7, blackBishopPrefab, rectTransform);

        // Queens
        InitializePiece(3, 0, whiteQueenPrefab, rectTransform);
        InitializePiece(3, 7, blackQueenPrefab, rectTransform);

        // Kings
        InitializePiece(4, 0, whiteKingPrefab, rectTransform);
        InitializePiece(4, 7, blackKingPrefab, rectTransform);

    }

    //Initialize piece on board and in memory
    public void InitializePiece(int x, int y, GameObject piecePrefab, RectTransform rectTransform)
    {
        if (pieceInfo.ContainsKey(new Vector2Int(x, y)))
        {
            pieceInfo[new Vector2Int(x, y)].SetActive(false);
            Destroy(pieceInfo[new Vector2Int(x, y)]);
            pieceInfo.Remove(new Vector2Int(x, y));
        }
        GameObject piece = Instantiate(piecePrefab, rectTransform);

        RectTransform pieceRect = piece.GetComponent<RectTransform>();
        pieceRect.sizeDelta = new Vector2(tileWidth * 0.9f, tileHeight * 0.9f);//Made pieces a bit smaller

        posX = tileWidth * x + tileWidth / 2f - width / 2f;
        posY = tileHeight * y + tileHeight / 2f - height / 2f;
        piece.transform.localPosition = new Vector2(posX, posY);

        pieceInfo[new Vector2Int(x, y)] = piece;
    }

    public void InitializeRook(int x, int y, ChessPieceBase.PieceColor pieceColor)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        if (pieceColor == ChessPieceBase.PieceColor.White)
            InitializePiece(x, y, whiteRookPrefab, rectTransform);
        else
            InitializePiece(x, y, blackRookPrefab, rectTransform);
    }
    public void InitializeKnight(int x, int y, ChessPieceBase.PieceColor pieceColor)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        if (pieceColor == ChessPieceBase.PieceColor.White)
            InitializePiece(x, y, whiteKnightPrefab, rectTransform);
        else
            InitializePiece(x, y, blackKnightPrefab, rectTransform);
    }
    public void InitializeBishop(int x, int y, ChessPieceBase.PieceColor pieceColor)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        if (pieceColor == ChessPieceBase.PieceColor.White)
            InitializePiece(x, y, whiteBishopPrefab, rectTransform);
        else
            InitializePiece(x, y, blackBishopPrefab, rectTransform);
    }
    public void InitializeQueen(int x, int y, ChessPieceBase.PieceColor pieceColor)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        if (pieceColor == ChessPieceBase.PieceColor.White)
            InitializePiece(x, y, whiteQueenPrefab, rectTransform);
        else
            InitializePiece(x, y, blackQueenPrefab, rectTransform);
    }
}
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class ReplayManager : MonoBehaviour
{
    public static ReplayManager Instance;
    //ReplayManager.Instance.--- -> Singleton
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

    private bool isAnimating = false;
    private float duration = 0.5f;
    private List<GameObject> allPieces = new List<GameObject>();

    [HideInInspector] public bool isReplaying = false;
    [HideInInspector] public FileManager.GameRecord record;

    private void Start()
    {
        BoardPreviewGenerator.Instance.GenerateBoard();
        PiecesPreviewGenerator.Instance.GeneratePieces();
    }

    private IEnumerator MovePiece(Vector2Int from, Vector2Int to, GameObject piece)
    {
        if (isAnimating)
        {
            yield return null;
        }

        isAnimating = true;

        float elapsed = 0f;

        RectTransform pieceRect = piece.GetComponent<RectTransform>();

        Vector2 fromV2Float = BoardPreviewGenerator.Instance.tilePositions[from];
        Vector2 toV2Float = BoardPreviewGenerator.Instance.tilePositions[to];

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector2 current = Vector2.Lerp(fromV2Float, toV2Float, elapsed / duration);

            pieceRect.localPosition = current;

            yield return null;
        }

        pieceRect.localPosition = toV2Float;
        //Update Dictionary
        PiecesPreviewGenerator.Instance.pieceInfo.Remove(from);
        if (PiecesPreviewGenerator.Instance.pieceInfo.ContainsKey(to))
        {
            PiecesPreviewGenerator.Instance.pieceInfo[to].SetActive(false);
            PiecesPreviewGenerator.Instance.pieceInfo.Remove(to);
        }
        PiecesPreviewGenerator.Instance.pieceInfo.Add(to, piece);

        isAnimating = false;
    }

    private IEnumerator MovePieceEnPassant(Vector2Int from, Vector2Int to, ChessPieceBase.PieceColor pieceColor, GameObject piece)
    {
        if (isAnimating)
        {
            yield return null;
        }

        isAnimating = true;


        float elapsed = 0f;

        RectTransform pieceRect = piece.GetComponent<RectTransform>();

        Vector2 fromV2Float = BoardPreviewGenerator.Instance.tilePositions[from];
        Vector2 toV2Float = BoardPreviewGenerator.Instance.tilePositions[to];

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector2 current = Vector2.Lerp(fromV2Float, toV2Float, elapsed / duration);

            pieceRect.localPosition = current;

            yield return null;
        }

        pieceRect.localPosition = toV2Float;
        //Update Dictionary
        int direction = (pieceColor == ChessPieceBase.PieceColor.White) ? 1 : -1;
        Vector2Int enemyPosition = new Vector2Int(to.x, to.y - 1 * direction);
        PiecesPreviewGenerator.Instance.pieceInfo.Remove(from);
        PiecesPreviewGenerator.Instance.pieceInfo[enemyPosition].SetActive(false);
        PiecesPreviewGenerator.Instance.pieceInfo.Remove(enemyPosition);
        PiecesPreviewGenerator.Instance.pieceInfo.Add(to, piece);

        isAnimating = false;
    }

    private IEnumerator MovePieceCastling(Vector2Int from, Vector2Int to, ChessPieceBase.PieceColor pieceColor, GameObject piece)
    {
        if (isAnimating)
        {
            yield return null;
        }

        isAnimating = true;

        float elapsed = 0f;

        RectTransform pieceRect = piece.GetComponent<RectTransform>();

        Vector2 fromV2Float = BoardPreviewGenerator.Instance.tilePositions[from];
        Vector2 toV2Float = BoardPreviewGenerator.Instance.tilePositions[to];

        int colorY = (pieceColor == ChessPieceBase.PieceColor.White) ? 0 : 7;

        //piece is King, we need Rook as well
        //Check if left side castling or right side

        GameObject rook = null;
        Vector2Int rookFrom = new Vector2Int();
        Vector2Int rookTo = new Vector2Int();

        //Leftside
        if (to.x == 2)
        {
            rookFrom = new Vector2Int(0, colorY);
            rookTo = new Vector2Int(3, colorY);
            rook = PiecesPreviewGenerator.Instance.pieceInfo[rookFrom];
        }
        //Rightside
        if (to.x == 6)
        {
            rookFrom = new Vector2Int(7, colorY);
            rookTo = new Vector2Int(5, colorY);
            rook = PiecesPreviewGenerator.Instance.pieceInfo[rookFrom];
        }

        Vector2 rookFromV2Float = BoardPreviewGenerator.Instance.tilePositions[rookFrom];
        Vector2 rookToV2Float = BoardPreviewGenerator.Instance.tilePositions[rookTo];

        RectTransform rookRect = rook.GetComponent<RectTransform>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector2 current = Vector2.Lerp(fromV2Float, toV2Float, elapsed / duration);

            Vector2 currentRook = Vector2.Lerp(rookFromV2Float, rookToV2Float, elapsed / duration);

            pieceRect.localPosition = current;
            rookRect.localPosition = currentRook;

            yield return null;
        }

        pieceRect.localPosition = toV2Float;
        rookRect.localPosition = rookToV2Float;
        //Update Dictionary
        PiecesPreviewGenerator.Instance.pieceInfo.Remove(from);
        PiecesPreviewGenerator.Instance.pieceInfo.Remove(rookFrom);
        PiecesPreviewGenerator.Instance.pieceInfo.Add(to, piece);
        PiecesPreviewGenerator.Instance.pieceInfo.Add(rookTo, rook);

        isAnimating = false;
    }

    private IEnumerator MovePiecePromotion(Vector2Int from, Vector2Int to, ChessPieceBase.PieceColor pieceColor, ChessPieceBase.PieceType promotedInto, GameObject piece)
    {
        if (isAnimating)
        {
            yield return null;
        }

        isAnimating = true;

        float elapsed = 0f;

        RectTransform pieceRect = piece.GetComponent<RectTransform>();

        Vector2 fromV2Float = BoardPreviewGenerator.Instance.tilePositions[from];
        Vector2 toV2Float = BoardPreviewGenerator.Instance.tilePositions[to];

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Vector2 current = Vector2.Lerp(fromV2Float, toV2Float, elapsed / duration);

            pieceRect.localPosition = current;
            yield return null;
        }

        switch (promotedInto)
        {
            case ChessPieceBase.PieceType.Rook:
                PiecesPreviewGenerator.Instance.InitializeRook(to.x, to.y, pieceColor);
                break;
            case ChessPieceBase.PieceType.Knight:
                PiecesPreviewGenerator.Instance.InitializeKnight(to.x, to.y, pieceColor);
                break;
            case ChessPieceBase.PieceType.Bishop:
                PiecesPreviewGenerator.Instance.InitializeBishop(to.x, to.y, pieceColor);
                break;
            case ChessPieceBase.PieceType.Queen:
                PiecesPreviewGenerator.Instance.InitializeQueen(to.x, to.y, pieceColor);
                break;
        }


        pieceRect.localPosition = toV2Float;
        //Update Dictionary
        PiecesPreviewGenerator.Instance.pieceInfo.Remove(from);

        isAnimating = false;
    }

    private void ResetBoard()
    {
        //Adds all current GameObjects into a list
        foreach (var piece in PiecesPreviewGenerator.Instance.pieceInfo)
        {
            allPieces.Add(piece.Value);
        }
        //Destroys all GameObjects
        foreach (var piece in allPieces)
        {
            Destroy(piece);
        }
        allPieces.Clear();

        //Regenerates the board
        PiecesPreviewGenerator.Instance.GeneratePieces();
    }
    private void Update()
    {
        if (record != null && !isReplaying)
        {
            StartCoroutine(StartReplayCoroutine(record));
        }
    }

    public IEnumerator StartReplayCoroutine(FileManager.GameRecord record)
    {
        isReplaying = true;
        var history = record.History;

        for (int i = 0; i < history.Count; i++)
        {
            var from = history[i].from;
            Vector2Int fromV2Int = new Vector2Int(from.x, from.y);
            var to = history[i].to;
            Vector2Int toV2Int = new Vector2Int(to.x, to.y);
            var promotedInto = history[i].promotedInto;
            var pieceColor = history[i].pieceColor;
            var specialMove = history[i].specialMove;

            GameObject selectedPiece = PiecesPreviewGenerator.Instance.pieceInfo[fromV2Int];

            // Wait for the move animation to complete
            if (specialMove == FileManager.SpecialMove.None)
                yield return StartCoroutine(MovePiece(fromV2Int, toV2Int, selectedPiece));
            else if (specialMove == FileManager.SpecialMove.EnPassant)
                yield return StartCoroutine(MovePieceEnPassant(fromV2Int, toV2Int, pieceColor, selectedPiece));
            else if (specialMove == FileManager.SpecialMove.Castle)
                yield return StartCoroutine(MovePieceCastling(fromV2Int, toV2Int, pieceColor, selectedPiece));
            else if (specialMove == FileManager.SpecialMove.Promotion)
                yield return StartCoroutine(MovePiecePromotion(fromV2Int, toV2Int, pieceColor, promotedInto, selectedPiece));
        }

        yield return new WaitForSeconds(2f);

        ResetBoard();
        isReplaying = false;
    }
}
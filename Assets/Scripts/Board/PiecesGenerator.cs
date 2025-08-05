using UnityEngine;
using static ChessPieceBase;

public class PiecesGenerator : MonoBehaviour
{
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

    //Put the pieces on the board and in memory
    public void Generate()
    {
        // Pawns
        for (int x = 0; x < 8; x++)
        {
            BoardManager.Instance.InitializePiece(x, 1, whitePawnPrefab, PieceType.Pawn, PieceColor.White);
            BoardManager.Instance.InitializePiece(x, 6, blackPawnPrefab, PieceType.Pawn, PieceColor.Black);
        }

        // Rooks
        BoardManager.Instance.InitializePiece(0, 0, whiteRookPrefab, PieceType.Rook, PieceColor.White);
        BoardManager.Instance.InitializePiece(7, 0, whiteRookPrefab, PieceType.Rook, PieceColor.White);
        BoardManager.Instance.InitializePiece(0, 7, blackRookPrefab, PieceType.Rook, PieceColor.Black);
        BoardManager.Instance.InitializePiece(7, 7, blackRookPrefab, PieceType.Rook, PieceColor.Black);

        // Knights
        BoardManager.Instance.InitializePiece(1, 0, whiteKnightPrefab, PieceType.Knight, PieceColor.White);
        BoardManager.Instance.InitializePiece(6, 0, whiteKnightPrefab, PieceType.Knight, PieceColor.White);
        BoardManager.Instance.InitializePiece(1, 7, blackKnightPrefab, PieceType.Knight, PieceColor.Black);
        BoardManager.Instance.InitializePiece(6, 7, blackKnightPrefab, PieceType.Knight, PieceColor.Black);

        // Bishops
        BoardManager.Instance.InitializePiece(2, 0, whiteBishopPrefab, PieceType.Bishop, PieceColor.White);
        BoardManager.Instance.InitializePiece(5, 0, whiteBishopPrefab, PieceType.Bishop, PieceColor.White);
        BoardManager.Instance.InitializePiece(2, 7, blackBishopPrefab, PieceType.Bishop, PieceColor.Black);
        BoardManager.Instance.InitializePiece(5, 7, blackBishopPrefab, PieceType.Bishop, PieceColor.Black);

        // Queens
        BoardManager.Instance.InitializePiece(3, 0, whiteQueenPrefab, PieceType.Queen, PieceColor.White);
        BoardManager.Instance.InitializePiece(3, 7, blackQueenPrefab, PieceType.Queen, PieceColor.Black);

        // Kings
        BoardManager.Instance.InitializePiece(4, 0, whiteKingPrefab, PieceType.King, PieceColor.White);
        BoardManager.Instance.InitializePiece(4, 7, blackKingPrefab, PieceType.King, PieceColor.Black);

    }
}

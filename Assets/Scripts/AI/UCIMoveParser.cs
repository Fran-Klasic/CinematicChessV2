using UnityEngine;
using static ChessPieceBase;

public static class UCIMoveParser
{
    public static Vector2Int SquareToVector(string square)
    {
        int file = square[0] - 'a';
        int rank = square[1] - '1';
        return new Vector2Int(file, rank);
    }

    public static PieceType PromotionCharToPiece(char c)
    {
        return c switch
        {
            'q' => PieceType.Queen,
            'r' => PieceType.Rook,
            'b' => PieceType.Bishop,
            'n' => PieceType.Knight,
            _ => PieceType.None
        };
    }
}

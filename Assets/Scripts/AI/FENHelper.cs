using UnityEngine;
using static ChessPieceBase;

public static class FENHelper
{
    public static string GetCurrentFEN()
    {
        var board = BoardManager.Instance.BoardState;

        PieceColor sideToMove = GameStateManager.Instance.CurrentTurn;

        //Castling rights
        bool whiteKingSideCastle = CanCastle(PieceColor.White, true);
        bool whiteQueenSideCastle = CanCastle(PieceColor.White, false);
        bool blackKingSideCastle = CanCastle(PieceColor.Black, true);
        bool blackQueenSideCastle = CanCastle(PieceColor.Black, false);

        //En passant target
        Vector2Int? enPassantTarget = GetEnPassantSquare();

        //Halfmove clock (you don't track this yet → safe default)
        int halfmoveClock = 0;

        //Fullmove number (increment after Black moves)
        int fullmoveNumber = Mathf.Max(1, FileManager.Instance.History.Count / 2 + 1);

        return FENGenerator.GenerateFEN(
            board,
            sideToMove,
            whiteKingSideCastle,
            whiteQueenSideCastle,
            blackKingSideCastle,
            blackQueenSideCastle,
            enPassantTarget,
            halfmoveClock,
            fullmoveNumber
        );
    }

    private static bool CanCastle(PieceColor color, bool kingSide)
    {
        int y = (color == PieceColor.White) ? 0 : 7;

        var king = BoardManager.Instance.BoardState[4, y];
        if (king == null || king.HasMoved) return false;

        int rookX = kingSide ? 7 : 0;
        var rook = BoardManager.Instance.BoardState[rookX, y];
        if (rook == null || rook.HasMoved) return false;

        return true;
    }

    private static Vector2Int? GetEnPassantSquare()
    {
        foreach (var kvp in BoardManager.Instance.EnPassantAttribute)
        {
            if (kvp.Value)
                return kvp.Key;
        }
        return null;
    }
}

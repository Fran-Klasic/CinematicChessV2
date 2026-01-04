
using System.Text;
using UnityEngine;
using static ChessPieceBase;

public static class FENGenerator
{
    public static string GenerateFEN(
        ChessPieceBase[,] board,
        PieceColor sideToMove,
        bool whiteKingSideCastle,
        bool whiteQueenSideCastle,
        bool blackKingSideCastle,
        bool blackQueenSideCastle,
        Vector2Int? enPassantTarget,
        int halfmoveClock,
        int fullmoveNumber)
    {
        {
            StringBuilder fen = new StringBuilder();

            //1️ Board
            for (int rank = 7; rank >= 0; rank--)
            {
                int empty = 0;

                for (int file = 0; file < 8; file++)
                {
                    ChessPieceBase piece = board[file, rank];

                    if (piece == null)
                    {
                        empty++;
                    }
                    else
                    {
                        if (empty > 0)
                        {
                            fen.Append(empty);
                            empty = 0;
                        }

                        fen.Append(GetPieceChar(piece));
                    }
                }

                if (empty > 0)
                    fen.Append(empty);

                if (rank > 0)
                    fen.Append('/');
            }

            //2️ Side to move
            fen.Append(' ');
            fen.Append(sideToMove == PieceColor.White ? 'w' : 'b');

            //3️ Castling rights
            fen.Append(' ');
            string castling = "";

            if (whiteKingSideCastle) castling += "K";
            if (whiteQueenSideCastle) castling += "Q";
            if (blackKingSideCastle) castling += "k";
            if (blackQueenSideCastle) castling += "q";

            fen.Append(string.IsNullOrEmpty(castling) ? "-" : castling);

            //4️ En passant
            fen.Append(' ');
            if (enPassantTarget.HasValue)
            {
                fen.Append(ToAlgebraic(enPassantTarget.Value));
            }
            else
            {
                fen.Append("-");
            }

            //5️ Halfmove clock
            fen.Append(' ');
            fen.Append(halfmoveClock);

            //6️ Fullmove number
            fen.Append(' ');
            fen.Append(fullmoveNumber);

            return fen.ToString();
        }
    }
    private static char GetPieceChar(ChessPieceBase piece)
    {
        char c = piece.Type switch
        {
            PieceType.Pawn => 'p',
            PieceType.Knight => 'n',
            PieceType.Bishop => 'b',
            PieceType.Rook => 'r',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _ => '?'
        };

        return piece.Color == PieceColor.White ? char.ToUpper(c) : c;
    }

    private static string ToAlgebraic(Vector2Int pos)
    {
        char file = (char)('a' + pos.x);
        char rank = (char)('1' + pos.y);
        return $"{file}{rank}";
    }
}
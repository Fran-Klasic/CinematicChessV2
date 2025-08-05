using System.Collections.Generic;
using UnityEngine;

public class King : ChessPieceBase
{
    public override List<Vector2Int> GetLegalMoves(ChessPieceBase[,] boardState)
    {
        List<Vector2Int> legalMoves = new List<Vector2Int>();

        int direction = (Color == PieceColor.White) ? 1 : -1;

        Vector2Int[] possibleMoves = new Vector2Int[]
        {
                new Vector2Int(0,1) + BoardPosition,
                new Vector2Int(0,-1) + BoardPosition,
                new Vector2Int(1,0) + BoardPosition,
                new Vector2Int(-1,0) + BoardPosition,

                new Vector2Int(1,1) + BoardPosition,
                new Vector2Int(1,-1) + BoardPosition,
                new Vector2Int(-1,1) + BoardPosition,
                new Vector2Int(-1,-1) + BoardPosition,
        };

        int x;
        int y;

        foreach (var move in possibleMoves)
        {
            if (IsInsideBoard(move))
            {
                if (IsEmptyAt(move))
                {
                    legalMoves.Add(move);
                }
                if (IsEnemyAt(move, Color))
                {
                    legalMoves.Add(move);
                    x = move.x;
                    y = move.y;
                    boardState[x, y].IsCapturable = true;
                }
            }
        }

        return legalMoves;
    }
}

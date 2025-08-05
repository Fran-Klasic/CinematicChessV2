using UnityEngine;
using System.Collections.Generic;

public class Pawn : ChessPieceBase
{
    public override List<Vector2Int> GetLegalMoves(ChessPieceBase[,] boardState)
    {
        List<Vector2Int> legalMoves = new List<Vector2Int>();

        int direction = (Color == PieceColor.White) ? 1 : -1;


        Vector2Int forwardOne = BoardPosition + new Vector2Int(0, direction);
        Vector2Int forwardTwo = forwardOne + new Vector2Int(0, direction);
        Vector2Int sideTakeLeft = BoardPosition + new Vector2Int(-1, direction);
        Vector2Int sideTakeRight = BoardPosition + new Vector2Int(1, direction);

        int x;
        int y;

        if (IsEmptyAt(forwardOne))
        {
            legalMoves.Add(forwardOne);

            if (!HasMoved && IsEmptyAt(forwardTwo))
            {
                legalMoves.Add(forwardTwo);
            }
        }
        
        if (IsEnemyAt(sideTakeLeft, Color))
        {
            legalMoves.Add(sideTakeLeft);
            x = sideTakeLeft.x;
            y = sideTakeLeft.y;
            boardState[x, y].IsCapturable = true;
        }
        if (IsEnemyAt(sideTakeRight, Color))
        {
            legalMoves.Add(sideTakeRight);
            x = sideTakeRight.x;
            y = sideTakeRight.y;
            boardState[x, y].IsCapturable = true;
        }


        return legalMoves;
    }
}

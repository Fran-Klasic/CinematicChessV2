using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPieceBase
{
    public override List<Vector2Int> GetLegalMoves(ChessPieceBase[,] boardState)
    {
        List<Vector2Int> legalMoves = new List<Vector2Int>();

        int direction = (Color == PieceColor.White) ? 1 : -1;

        Vector2Int tempValue;
        tempValue = BoardPosition;

        int x;
        int y;

        //Left
        while (true)
        {
            tempValue.x--;
            if (IsInsideBoard(tempValue))
            {
                if (IsEmptyAt(tempValue))
                {
                    legalMoves.Add(tempValue);

                }
                if (IsEnemyAt(tempValue, Color))
                {
                    legalMoves.Add(tempValue);
                    x = tempValue.x;
                    y = tempValue.y;
                    boardState[x, y].IsCapturable = true;
                    break;
                }
                if (IsAllyAt(tempValue, Color))
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        tempValue = BoardPosition;

        //Right
        while (true)
        {
            tempValue.x++;
            if (IsInsideBoard(tempValue))
            {
                if (IsEmptyAt(tempValue))
                {
                    legalMoves.Add(tempValue);
                }
                if (IsEnemyAt(tempValue, Color))
                {
                    legalMoves.Add(tempValue);
                    x = tempValue.x;
                    y = tempValue.y;
                    boardState[x, y].IsCapturable = true;
                    break;
                }
                if (IsAllyAt(tempValue, Color))
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        tempValue = BoardPosition;

        //Top
        while (true)
        {
            tempValue.y++;
            if (IsInsideBoard(tempValue))
            {
                if (IsEmptyAt(tempValue))
                {
                    legalMoves.Add(tempValue);
                }
                if (IsEnemyAt(tempValue, Color))
                {
                    legalMoves.Add(tempValue);
                    x = tempValue.x;
                    y = tempValue.y;
                    boardState[x, y].IsCapturable = true;
                    break;
                }
                if (IsAllyAt(tempValue, Color))
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        tempValue = BoardPosition;

        //Bottom
        while (true)
        {
            tempValue.y--;
            if (IsInsideBoard(tempValue))
            {
                if (IsEmptyAt(tempValue))
                {
                    legalMoves.Add(tempValue);
                }
                if (IsEnemyAt(tempValue, Color))
                {
                    legalMoves.Add(tempValue);
                    x = tempValue.x;
                    y = tempValue.y;
                    boardState[x, y].IsCapturable = true;
                    break;
                }
                if (IsAllyAt(tempValue, Color))
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        tempValue = BoardPosition;

        return legalMoves;
    }
}

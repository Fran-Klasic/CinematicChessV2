using UnityEngine;
using System.Collections.Generic;

public abstract class ChessPieceBase : MonoBehaviour
{
    public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King, None }
    public enum PieceColor { White, Black, None }

    [HideInInspector] public PieceType Type;
    [HideInInspector] public PieceColor Color;
    [HideInInspector] public Vector2Int BoardPosition;
    [HideInInspector] public bool HasMoved;
    [HideInInspector] public bool IsCapturable;

    public abstract List<Vector2Int> GetLegalMoves(ChessPieceBase[,] boardState);

    //False if not inside board
    //True if is inside board
    public bool IsInsideBoard(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
    }

    //False if not inside board
    //False if is not empty
    //True if is empty
    public bool IsEmptyAt(Vector2Int position)
    {
        int x = position.x;
        int y = position.y;

        if (!IsInsideBoard(position))
            return false;

        return BoardManager.Instance.BoardState[x, y] == null;
    }

    //False if not inside board
    //False if is empty
    //True if is enemy
    public bool IsEnemyAt(Vector2Int position, PieceColor sourceColor)
    {
        int x = position.x;
        int y = position.y;

        if (!IsInsideBoard(position))
            return false;

        if (IsEmptyAt(position))
            return false;

        if (BoardManager.Instance.BoardState[x, y].Color == sourceColor)
            return false;

        return true;
    }

    //False if not inside board
    //False if is empty
    //True if is ally
    public bool IsAllyAt(Vector2Int position, PieceColor sourceColor)
    {
        int x = position.x;
        int y = position.y;

        if (!IsInsideBoard(position))
            return false;

        if (IsEmptyAt(position))
            return false;

        if (BoardManager.Instance.BoardState[x, y].Color == sourceColor)
            return true;

        return false;
    }
}

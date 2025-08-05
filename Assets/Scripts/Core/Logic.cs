using System.Collections.Generic;
using UnityEngine;
using static ChessPieceBase;

//Used for simulating moves while being detached from GameObjects
public static class Logic
{
    public class LogicPiece
    {
        public PieceType Type;
        public PieceColor Color;
        public Vector2Int BoardPosition;
        public bool HasMoved;

        public LogicPiece(ChessPieceBase piece)
        {
            Type = piece.Type;
            Color = piece.Color;
            BoardPosition = piece.BoardPosition;
            HasMoved = piece.HasMoved;
        }
        public LogicPiece(PieceType type, PieceColor color, Vector2Int boardPosition, bool hasMoved)
        {
            Type = type;
            Color = color;
            BoardPosition = boardPosition;
            HasMoved = hasMoved;
        }
    }

    //Clone boardState into a PieceClone type for logic
    public static LogicPiece[,] GetLogicBoardState(ChessPieceBase[,] boardState)
    {
        LogicPiece[,] logicBoardState = new LogicPiece[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = boardState[x, y];
                if (piece != null)
                {
                    logicBoardState[x, y] = new LogicPiece(piece);
                }
                else
                {
                    logicBoardState[x, y] = null;
                }
            }
        }

        return logicBoardState;
    }

    //Clone logicBoardState
    public static LogicPiece[,] CloneLogicBoardState(LogicPiece[,] logicBoardState)
    {
        LogicPiece[,] clone = new LogicPiece[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (logicBoardState[i, j] != null)
                {
                    clone[i, j] = new LogicPiece
                    (
                        logicBoardState[i, j].Type,
                        logicBoardState[i, j].Color,
                        logicBoardState[i, j].BoardPosition,
                        logicBoardState[i, j].HasMoved
                    );

                }
                else
                {
                    clone[i, j] = null;
                }
            }
        }

        return clone;
    }

    //False if not inside board
    //True if is inside board
    public static bool IsInsideBoard(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
    }

    //False if not inside board
    //False if is not empty
    //True if is empty
    public static bool IsEmptyAt(Vector2Int position, LogicPiece[,] logicBoardState)
    {
        int x = position.x;
        int y = position.y;

        if (!IsInsideBoard(position))
            return false;

        return logicBoardState[x, y] == null;
    }

    //False if not inside board
    //False if is empty
    //True if is enemy
    public static bool IsEnemyAt(Vector2Int position, PieceColor sourceColor, LogicPiece[,] logicBoardState)
    {
        int x = position.x;
        int y = position.y;

        if (!IsInsideBoard(position))
            return false;

        if (IsEmptyAt(position, logicBoardState))
            return false;

        if (logicBoardState[x, y].Color == sourceColor)
            return false;

        return true;
    }

    //False if not inside board
    //False if is empty
    //True if is ally
    public static bool IsAllyAt(Vector2Int position, PieceColor sourceColor, LogicPiece[,] logicBoardState)
    {
        int x = position.x;
        int y = position.y;

        if (!IsInsideBoard(position))
            return false;

        if (IsEmptyAt(position, logicBoardState))
            return false;

        if (logicBoardState[x, y].Color == sourceColor)
            return true;

        return false;
    }

    //Gets legal moves of current piece
    public static class LogicMoveGenerator
    {
        public static List<Vector2Int> GetLegalMoves(LogicPiece[,] logicBoardState, LogicPiece currentPiece)
        {
            return currentPiece.Type switch
            {
                PieceType.Pawn => LogicPawn.GetLegalMoves(logicBoardState, currentPiece),
                PieceType.Rook => LogicRook.GetLegalMoves(logicBoardState, currentPiece),
                PieceType.Knight => LogicKnight.GetLegalMoves(logicBoardState, currentPiece),
                PieceType.Bishop => LogicBishop.GetLegalMoves(logicBoardState, currentPiece),
                PieceType.Queen => LogicQueen.GetLegalMoves(logicBoardState, currentPiece),
                PieceType.King => LogicKing.GetLegalMoves(logicBoardState, currentPiece),
                _ => new List<Vector2Int>()
            };
        }
    }

    public static void SimulateMove(Vector2Int from, Vector2Int to, LogicPiece[,] logicBoardState)
    {
        //update boardState
        logicBoardState[to.x, to.y] = logicBoardState[from.x, from.y];
        logicBoardState[to.x, to.y].BoardPosition = new Vector2Int(to.x, to.y);
        logicBoardState[to.x, to.y].HasMoved = true;
        logicBoardState[from.x, from.y] = null;
    }

    //Legal moves:
    public static class LogicPawn
    {
        public static List<Vector2Int> GetLegalMoves(Logic.LogicPiece[,] logicBoardState, Logic.LogicPiece currentPiece)
        {
            List<Vector2Int> legalMoves = new List<Vector2Int>();

            int direction = (currentPiece.Color == PieceColor.White) ? 1 : -1;


            Vector2Int forwardOne = currentPiece.BoardPosition + new Vector2Int(0, direction);
            Vector2Int forwardTwo = forwardOne + new Vector2Int(0, direction);
            Vector2Int sideTakeLeft = currentPiece.BoardPosition + new Vector2Int(-1, direction);
            Vector2Int sideTakeRight = currentPiece.BoardPosition + new Vector2Int(1, direction);


            if (Logic.IsEmptyAt(forwardOne, logicBoardState))
            {
                legalMoves.Add(forwardOne);

                if (!currentPiece.HasMoved && Logic.IsEmptyAt(forwardTwo, logicBoardState))
                {
                    legalMoves.Add(forwardTwo);
                }
            }

            if (Logic.IsEnemyAt(sideTakeLeft, currentPiece.Color, logicBoardState))
            {
                legalMoves.Add(sideTakeLeft);
            }
            if (Logic.IsEnemyAt(sideTakeRight, currentPiece.Color, logicBoardState))
            {
                legalMoves.Add(sideTakeRight);
            }


            return legalMoves;
        }
    }

    public static class LogicRook
    {
        public static List<Vector2Int> GetLegalMoves(Logic.LogicPiece[,] logicBoardState, Logic.LogicPiece currentPiece)
        {
            List<Vector2Int> legalMoves = new List<Vector2Int>();

            int direction = (currentPiece.Color == PieceColor.White) ? 1 : -1;

            Vector2Int tempValue;
            tempValue = currentPiece.BoardPosition;

            //Left
            while (true)
            {
                tempValue.x--;
                if (Logic.IsInsideBoard(tempValue))
                {
                    if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    {
                        legalMoves.Add(tempValue);

                    }
                    if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                        break;
                    }
                    if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            //Right
            while (true)
            {
                tempValue.x++;
                if (Logic.IsInsideBoard(tempValue))
                {
                    if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                    }
                    if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                        break;
                    }
                    if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            //Top
            while (true)
            {
                tempValue.y++;
                if (Logic.IsInsideBoard(tempValue))
                {
                    if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                    }
                    if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                        break;
                    }
                    if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            //Bottom
            while (true)
            {
                tempValue.y--;
                if (Logic.IsInsideBoard(tempValue))
                {
                    if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                    }
                    if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                        break;
                    }
                    if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            return legalMoves;
        }
    }

    public static class LogicKnight
    {
        public static List<Vector2Int> GetLegalMoves(Logic.LogicPiece[,] logicBoardState, Logic.LogicPiece currentPiece)
        {
            List<Vector2Int> legalMoves = new List<Vector2Int>();
            Vector2Int pos = currentPiece.BoardPosition;

            Vector2Int[] possibleMoves = new Vector2Int[]
            {
            new Vector2Int(1, 2) + pos,
            new Vector2Int(-1, 2) + pos,
            new Vector2Int(1, -2) + pos,
            new Vector2Int(-1, -2) + pos,
            new Vector2Int(2, 1) + pos,
            new Vector2Int(2, -1) + pos,
            new Vector2Int(-2, 1) + pos,
            new Vector2Int(-2, -1) + pos,
            };

            foreach (var move in possibleMoves)
            {
                if (Logic.IsInsideBoard(move))
                {
                    if (Logic.IsEmptyAt(move, logicBoardState))
                    {
                        legalMoves.Add(move);
                    }
                    else if (Logic.IsEnemyAt(move, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(move);
                    }
                }
            }

            return legalMoves;
        }
    }

    public static class LogicBishop
    {
        public static List<Vector2Int> GetLegalMoves(Logic.LogicPiece[,] logicBoardState, Logic.LogicPiece currentPiece)
        {
            List<Vector2Int> legalMoves = new List<Vector2Int>();
            Vector2Int tempValue = currentPiece.BoardPosition;

            // Top-Right diagonal
            while (true)
            {
                tempValue.x++;
                tempValue.y++;
                if (Logic.IsInsideBoard(tempValue))
                {
                    if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                    }
                    else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                        break;
                    }
                    else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        break;
                    }
                }
                else break;
            }
            tempValue = currentPiece.BoardPosition;

            // Top-Left diagonal
            while (true)
            {
                tempValue.x--;
                tempValue.y++;
                if (Logic.IsInsideBoard(tempValue))
                {
                    if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                    }
                    else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                        break;
                    }
                    else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        break;
                    }
                }
                else break;
            }
            tempValue = currentPiece.BoardPosition;

            // Bottom-Right diagonal
            while (true)
            {
                tempValue.x++;
                tempValue.y--;
                if (Logic.IsInsideBoard(tempValue))
                {
                    if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                    }
                    else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                        break;
                    }
                    else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        break;
                    }
                }
                else break;
            }
            tempValue = currentPiece.BoardPosition;

            // Bottom-Left diagonal
            while (true)
            {
                tempValue.x--;
                tempValue.y--;
                if (Logic.IsInsideBoard(tempValue))
                {
                    if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                    }
                    else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(tempValue);
                        break;
                    }
                    else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                    {
                        break;
                    }
                }
                else break;
            }
            return legalMoves;
        }
    }

    public static class LogicQueen
    {
        public static List<Vector2Int> GetLegalMoves(Logic.LogicPiece[,] logicBoardState, Logic.LogicPiece currentPiece)
        {
            List<Vector2Int> legalMoves = new List<Vector2Int>();
            Vector2Int tempValue = currentPiece.BoardPosition;

            // Left
            while (true)
            {
                tempValue.x--;
                if (!Logic.IsInsideBoard(tempValue)) break;

                if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    legalMoves.Add(tempValue);
                else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    legalMoves.Add(tempValue);
                    break;
                }
                else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            // Right
            while (true)
            {
                tempValue.x++;
                if (!Logic.IsInsideBoard(tempValue)) break;

                if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    legalMoves.Add(tempValue);
                else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    legalMoves.Add(tempValue);
                    break;
                }
                else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            // Up
            while (true)
            {
                tempValue.y++;
                if (!Logic.IsInsideBoard(tempValue)) break;

                if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    legalMoves.Add(tempValue);
                else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    legalMoves.Add(tempValue);
                    break;
                }
                else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            // Down
            while (true)
            {
                tempValue.y--;
                if (!Logic.IsInsideBoard(tempValue)) break;

                if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    legalMoves.Add(tempValue);
                else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    legalMoves.Add(tempValue);
                    break;
                }
                else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            // Top-Right diagonal
            while (true)
            {
                tempValue.x++;
                tempValue.y++;
                if (!Logic.IsInsideBoard(tempValue)) break;

                if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    legalMoves.Add(tempValue);
                else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    legalMoves.Add(tempValue);
                    break;
                }
                else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            // Top-Left diagonal
            while (true)
            {
                tempValue.x--;
                tempValue.y++;
                if (!Logic.IsInsideBoard(tempValue)) break;

                if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    legalMoves.Add(tempValue);
                else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    legalMoves.Add(tempValue);
                    break;
                }
                else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            // Bottom-Right diagonal
            while (true)
            {
                tempValue.x++;
                tempValue.y--;
                if (!Logic.IsInsideBoard(tempValue)) break;

                if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    legalMoves.Add(tempValue);
                else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    legalMoves.Add(tempValue);
                    break;
                }
                else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    break;
                }
            }
            tempValue = currentPiece.BoardPosition;

            // Bottom-Left diagonal
            while (true)
            {
                tempValue.x--;
                tempValue.y--;
                if (!Logic.IsInsideBoard(tempValue)) break;

                if (Logic.IsEmptyAt(tempValue, logicBoardState))
                    legalMoves.Add(tempValue);
                else if (Logic.IsEnemyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    legalMoves.Add(tempValue);
                    break;
                }
                else if (Logic.IsAllyAt(tempValue, currentPiece.Color, logicBoardState))
                {
                    break;
                }
            }

            return legalMoves;
        }
    }

    public static class LogicKing
    {
        public static List<Vector2Int> GetLegalMoves(Logic.LogicPiece[,] logicBoardState, Logic.LogicPiece currentPiece)
        {
            List<Vector2Int> legalMoves = new List<Vector2Int>();
            Vector2Int pos = currentPiece.BoardPosition;

            Vector2Int[] possibleMoves = new Vector2Int[]
            {
            new Vector2Int(0, 1) + pos,
            new Vector2Int(0, -1) + pos,
            new Vector2Int(1, 0) + pos,
            new Vector2Int(-1, 0) + pos,
            new Vector2Int(1, 1) + pos,
            new Vector2Int(1, -1) + pos,
            new Vector2Int(-1, 1) + pos,
            new Vector2Int(-1, -1) + pos,
            };

            foreach (var move in possibleMoves)
            {
                if (Logic.IsInsideBoard(move))
                {
                    if (Logic.IsEmptyAt(move, logicBoardState))
                    {
                        legalMoves.Add(move);
                    }
                    else if (Logic.IsEnemyAt(move, currentPiece.Color, logicBoardState))
                    {
                        legalMoves.Add(move);
                    }
                }
            }

            return legalMoves;
        }
    }
}
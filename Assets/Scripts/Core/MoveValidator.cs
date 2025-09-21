using UnityEngine;
using static Logic;
using static ChessPieceBase;
using System.Collections.Generic;

public class MoveValidator : MonoBehaviour
{
    public bool IsCheck(LogicPiece[,] logicBoardState, PieceColor color)
    {
        LogicPiece king = null;
        List<Vector2Int> enemyMoves = new List<Vector2Int>();

        //Get king and get all enemy moves
        foreach (var piece in logicBoardState)
        {
            if (piece == null) continue;

            if (piece.Type == PieceType.King && piece.Color == color)
            {
                //Get current king
                king = piece;
            }
            if (piece != null && piece.Color != color)
            {
                //Get all enemy moves
                var moves = LogicMoveGenerator.GetLegalMoves(logicBoardState, piece);
                if (moves != null)
                    enemyMoves.AddRange(moves);
            }
        }

        if (king == null)
            throw new System.Exception("King not found on the board.");

        if (enemyMoves == null || enemyMoves.Count == 0)
            return false;

        //if the king is in danger return true, else return false
        return enemyMoves.Contains(king.BoardPosition);
    }

    public bool IsInsufficientMaterial(LogicPiece[,] logicBoardState)
    {
        List<LogicPiece> pieces = new List<LogicPiece>();

        foreach (var piece in logicBoardState)
        {
            if (piece == null) continue;

            pieces.Add(piece);
        }

        //If only the kings remain
        return pieces.Count == 2;
    }

    public bool IsCheckmate(LogicPiece[,] logicBoardState, PieceColor color)
    {
        //Check if it is currently in check
        if (IsCheck(logicBoardState, color))
        {
            //King is currently in check

            foreach (var piece in logicBoardState)
            {
                //Get all ally moves and simulate them and check if the king is in check
                if (piece != null && piece.Color == color)
                {
                    var moves = LogicMoveGenerator.GetLegalMoves(logicBoardState, piece);

                    if (moves == null || moves.Count == 0)
                        continue;

                    foreach (var move in moves)
                    {
                        //Have a fresh clone of boardState
                        var clone = Logic.CloneLogicBoardState(logicBoardState);

                        //Simulate move and check if the king is still in Check
                        Logic.SimulateMove(piece.BoardPosition, move, clone);
                        if (!IsCheck(clone, color))
                        {
                            //It is not check after simulating move, so it cant be checkmate
                            return false;
                        }
                    }
                }
            }
            //If after simulating all ally pieces it is always check then it is checkmate
            return true;
        }

        //It is not checkmate if it wasnt check in the first place
        return false;
    }

    public bool IsStalemate(LogicPiece[,] logicBoardState, PieceColor color)
    {
        //If it is check, then it is not stalemate
        if (IsCheck(logicBoardState, color))
            return false;

        foreach (var piece in logicBoardState)
        {
            //Get ally pieces
            if (piece != null && piece.Color == color)
            {

                //Simulate each ally pieces legal moves
                var moves = LogicMoveGenerator.GetLegalMoves(logicBoardState, piece);

                if (moves == null || moves.Count == 0)
                    continue;

                foreach (var move in moves)
                {
                    var simulatedBoard = Logic.CloneLogicBoardState(logicBoardState);
                    Logic.SimulateMove(piece.BoardPosition, move, simulatedBoard);
                    if (!IsCheck(simulatedBoard, color))
                    {
                        //If simulated piece doesnt result in check then it is not stalemate
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public List<Vector2Int> RemoveUnsafeMoves(LogicPiece[,] logicBoardState, List<Vector2Int> legalMoves, Vector2Int boardPosition)
    {
        int x = boardPosition.x;
        int y = boardPosition.y;

        var piece = logicBoardState[x, y];

        List<Vector2Int> safeLegalMoves = new List<Vector2Int>();

        if (legalMoves == null)
            return null;

        //Simulate each legal move to see if it is safe
        foreach (var move in legalMoves)
        {
            var clone = Logic.CloneLogicBoardState(logicBoardState);
            Logic.SimulateMove(boardPosition, move, clone);

            //If move is safe then add it to safeLegalMoves
            if (!IsCheck(clone, piece.Color))
            {
                safeLegalMoves.Add(move);
            }
        }

        return safeLegalMoves;
    }

    public bool IsTileInDanger(LogicPiece[,] logicBoardState, PieceColor color, Vector2Int tilePosition)
    {
        List<Vector2Int> allMoves = new List<Vector2Int>();

        foreach (var piece in logicBoardState)
        {
            //Get all legal moves for each enemy piece
            if (piece != null && piece.Color != color)
            {
                var moves = LogicMoveGenerator.GetLegalMoves(logicBoardState, piece);
                if (moves != null && moves.Count > 0)
                {
                    allMoves.AddRange(moves);
                }
            }
        }

        //Check if at least one enemy pieces move is at tilePosition
        return allMoves.Contains(tilePosition);
    }

    public bool CanQueenSideCastle(LogicPiece[,] logicBoardState, PieceColor color)
    {
        int y = color == PieceColor.White ? 0 : 7;

        //Get pieces required for castling
        LogicPiece king = logicBoardState[4, y];
        LogicPiece rookLeft = logicBoardState[0, y];

        if (king == null || king.Color != color || king.Type != PieceType.King || king.HasMoved)
            return false;

        bool canCastle = false;

        // Queenside (left rook)
        if (rookLeft != null && rookLeft.Color == color && rookLeft.Type == PieceType.Rook && !rookLeft.HasMoved)
        {
            if (logicBoardState[1, y] == null &&
                logicBoardState[2, y] == null &&
                logicBoardState[3, y] == null)
            {
                if (!IsTileInDanger(logicBoardState, color, new Vector2Int(4, y)) && // current
                    !IsTileInDanger(logicBoardState, color, new Vector2Int(3, y)) && // passing
                    !IsTileInDanger(logicBoardState, color, new Vector2Int(2, y)))   // destination
                {
                    canCastle = true;
                }
            }
        }

        return canCastle;
    }

    public bool CanKingSideCastle(LogicPiece[,] logicBoardState, PieceColor color)
    {

        int y = color == PieceColor.White ? 0 : 7;

        //Get pieces required for castling
        LogicPiece king = logicBoardState[4, y];
        LogicPiece rookRight = logicBoardState[7, y];

        if (king == null || king.Color != color || king.Type != PieceType.King || king.HasMoved)
            return false;

        bool canCastle = false;

        // Kingside (right rook)
        if (rookRight != null && rookRight.Color == color && rookRight.Type == PieceType.Rook && !rookRight.HasMoved)
        {
            if (logicBoardState[5, y] == null &&
                logicBoardState[6, y] == null)
            {
                if (!IsTileInDanger(logicBoardState, color, new Vector2Int(4, y)) && // current
                    !IsTileInDanger(logicBoardState, color, new Vector2Int(5, y)) && // passing
                    !IsTileInDanger(logicBoardState, color, new Vector2Int(6, y)))   // destination
                {
                    canCastle = true;
                }
            }
        }

        return canCastle;
    }

    public List<Vector2Int> GetCastleMoves(LogicPiece[,] logicBoardState, PieceColor color)
    {
        int y = color == PieceColor.White ? 0 : 7;

        List<Vector2Int> castleMoves = new List<Vector2Int>();

        if (CanQueenSideCastle(logicBoardState, color))
            castleMoves.Add(new Vector2Int(2, y));

        if (CanKingSideCastle(logicBoardState, color))
            castleMoves.Add(new Vector2Int(6, y));

        return castleMoves;
    }

    public PieceColor MovedTwoForward(LogicPiece[,] logicBoardState, Vector2Int oldPosition)
    {
        var piece = logicBoardState[oldPosition.x, oldPosition.y];
        BoardManager.Instance.EnPassantAttribute.Clear();

        int direction = (piece.Color == PieceColor.White) ? 1 : -1;
        Vector2Int forwardTwo = oldPosition + new Vector2Int(0, 2 * direction);

        //Set attribute at new pawn position
        BoardManager.Instance.EnPassantAttribute[forwardTwo] = true;

        //Returns the color of the piece that moved
        return piece.Color;
    }

    public bool CanLeftEnPassant(LogicPiece[,] logicBoardState, PieceColor color, Vector2Int position)
    {
        //Check if on the left tile is an enemy pawn, check the dictionary for the attribute, simulate and check if it is safe
        int x = position.x;
        int y = position.y;

        int direction = (color == PieceColor.White) ? 1 : -1;

        Vector2Int left = new Vector2Int(-1, 0) + position;
        Vector2Int targetTile = position + new Vector2Int(-1, direction);

        if (!IsInsideBoard(left) || !IsInsideBoard(targetTile))
            return false;

        var target = logicBoardState[targetTile.x, targetTile.y];
        var piece = logicBoardState[left.x, left.y];

        if (piece == null || piece.Type != PieceType.Pawn || piece.Color == color)
            return false;

        if (!BoardManager.Instance.EnPassantAttribute.TryGetValue(piece.BoardPosition, out bool attribute) || !attribute)
            return false;

        if (target != null)
            return false;

        var clone = Logic.CloneLogicBoardState(logicBoardState);

        //Move to target positon
        Logic.SimulateMove(position, targetTile, clone);

        //Remove the captured pawn manually
        clone[left.x, left.y] = null;

        if (IsCheck(clone, color))
            return false;

        return true;
    }

    public bool CanRightEnPassant(LogicPiece[,] logicBoardState, PieceColor color, Vector2Int position)
    {
        //Check if on the right tile is an enemy pawn, check the dictionary for the attribute, simulate and check if it is safe
        int x = position.x;
        int y = position.y;

        int direction = (color == PieceColor.White) ? 1 : -1;

        Vector2Int right = new Vector2Int(1, 0) + position;
        Vector2Int targetTile = position + new Vector2Int(1, direction);

        if (!IsInsideBoard(right) || !IsInsideBoard(targetTile))
            return false;

        var target = logicBoardState[targetTile.x, targetTile.y];
        var piece = logicBoardState[right.x, right.y];

        if (piece == null || piece.Type != PieceType.Pawn || piece.Color == color)
            return false;

        if (!BoardManager.Instance.EnPassantAttribute.TryGetValue(piece.BoardPosition, out bool attribute) || !attribute)
            return false;

        if (target != null)
            return false;

        var clone = Logic.CloneLogicBoardState(logicBoardState);

        //Move to target positon
        Logic.SimulateMove(position, targetTile, clone);

        //Remove the captured pawn manually
        clone[right.x, right.y] = null;

        if (IsCheck(clone, color))
            return false;

        return true;
    }

    public List<Vector2Int> GetEnPassantMove(LogicPiece[,] logicBoardState, PieceColor color, Vector2Int position)
    {
        //Function returns List<T> even though En Passant is one move because UImanagers functions only work with List<T>
        List<Vector2Int> moveList = new List<Vector2Int>();

        int direction = (color == PieceColor.White) ? 1 : -1;

        if (CanLeftEnPassant(logicBoardState, color, position))
        {
            Vector2Int targetTile = position + new Vector2Int(-1, direction);
            moveList.Add(targetTile);
            return moveList;
        }

        if (CanRightEnPassant(logicBoardState, color, position))
        {
            Vector2Int targetTile = position + new Vector2Int(1, direction);
            moveList.Add(targetTile);
            return moveList;
        }

        //Returns empty list if cannot En Passant
        return moveList;
    }

    public static MoveValidator Instance;
    //MoveValidator.Instance.--- -> Singleton
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
}
using System.Collections.Generic;
using UnityEngine;
using System;
using static ChessPieceBase;

public class BoardManager : MonoBehaviour
{

    //Links position with GameObject
    public Dictionary<Vector2Int, GameObject> PieceGameObjects = new();

    //Data for each piece and tile
    public ChessPieceBase[,] BoardState = new ChessPieceBase[8, 8];

    //Link pawn position and the ability to do En Passant
    public Dictionary<Vector2Int, bool> EnPassantAttribute = new();

    //Used for points
    [HideInInspector] public List<PieceType> WhiteTakenPieces = new List<PieceType>();
    [HideInInspector] public List<PieceType> BlackTakenPieces = new List<PieceType>();

    //Points
    [HideInInspector] public int WhitePoints;
    [HideInInspector] public int BlackPoints;


    public void InitializePiece(int x, int y, GameObject prefab, PieceType type, PieceColor color)
    {
        //Get values for world position
        float center = GameManager.Instance.center;
        float tileSize = GameManager.Instance.tileSize;

        //Calculate world position with tile position
        Vector3 position = new Vector3((x * tileSize) - center, (y * tileSize) - center, 1);
        //Generate the GameObject
        GameObject pieceObject = Instantiate(prefab, position, Quaternion.identity);
        //Get pieces script
        ChessPieceBase piece = pieceObject.GetComponent<ChessPieceBase>();
        //Add data to piece
        piece.Type = type;
        piece.Color = color;
        piece.BoardPosition = new Vector2Int(x, y);
        piece.HasMoved = false;
        piece.IsCapturable = false;
        //Link tile position with GameObjects
        PieceGameObjects[new Vector2Int(x, y)] = pieceObject;
        //Put piece in memory
        BoardState[x, y] = piece;
    }

    public void MovePiece(Vector2Int from, Vector2Int to)
    {
        //Get Main Piece GameObject
        if (PieceGameObjects.TryGetValue(from, out var pieceGO))
        {
            //Get Target Piece GameObject
            if (PieceGameObjects.TryGetValue(to, out var targetPieceGO))
            {
                //If piece is captured
                Destroy(targetPieceGO);

                if (BoardState[to.x, to.y].Color == PieceColor.White)
                {
                    WhiteTakenPieces.Add(BoardState[to.x, to.y].Type);
                }
                else
                {
                    BlackTakenPieces.Add(BoardState[to.x, to.y].Type);
                }

                PieceGameObjects.Remove(to);
                BoardState[to.x, to.y] = null;
            }

            float center = GameManager.Instance.center;
            float tileSize = GameManager.Instance.tileSize;

            //Move the piece object
            pieceGO.transform.position = new Vector3((to.x * tileSize) - center, (to.y * tileSize) - center, 1);

            //Update dictionary
            PieceGameObjects[to] = pieceGO;
            PieceGameObjects.Remove(from);

            //update BoardState
            BoardState[to.x, to.y] = BoardState[from.x, from.y];
            BoardState[to.x, to.y].BoardPosition = new Vector2Int(to.x, to.y);
            BoardState[to.x, to.y].HasMoved = true;
            BoardState[from.x, from.y] = null;
        }

        GetCurrentPoints();
    }

    public void MovePieceCastling(Vector2Int from, Vector2Int to)
    {
        //Check if it is QueenSide or KingSide
        var king = BoardState[from.x, from.y];

        if (to == new Vector2Int(2, from.y))
        {
            //QueenSide
            //Rook goes to 3,y from 0,y
            MovePiece(new Vector2Int(0, from.y), new Vector2Int(3, from.y));
            //King goes to 2,y
            MovePiece(from, to);
            return;
        }
        if (to == new Vector2Int(6, from.y))
        {
            //KingSide
            //Rook goes to 5,y from 7,y
            MovePiece(new Vector2Int(7, from.y), new Vector2Int(5, from.y));
            //King goes to 6,y
            MovePiece(from, to);
            return;
        }
    }

    public void MovePieceEnPassant(Vector2Int from, Vector2Int to)
    {
        var piece = BoardState[from.x, from.y];

        int direction = (piece.Color == PieceColor.White) ? 1 : -1;

        //1st remove pawn, then 2nd move to Vector2Int To
        Vector2Int capturedPawnPosition = new Vector2Int(to.x, to.y) - new Vector2Int(0, direction);

        //Remove captured pawn
        if (PieceGameObjects.TryGetValue(capturedPawnPosition, out var capturedPawnGO))
        {
            Destroy(capturedPawnGO);

            if (piece.Color == PieceColor.White)
            {
                BlackTakenPieces.Add(PieceType.Pawn);
            }
            else
            {
                WhiteTakenPieces.Add(PieceType.Pawn);
            }

            PieceGameObjects.Remove(capturedPawnPosition);
            BoardState[capturedPawnPosition.x, capturedPawnPosition.y] = null;
        }

        MovePiece(from, to);
    }

    public void GetCurrentPoints()
    {
        int totalBlackPoints = 0;
        int totalWhitePoints = 0;

        foreach (var piece in WhiteTakenPieces)
        {
            switch (piece)
            {
                case PieceType.Pawn:
                    totalBlackPoints += 1;
                    break;
                case PieceType.Rook:
                    totalBlackPoints += 5;
                    break;
                case PieceType.Knight:
                    totalBlackPoints += 3;
                    break;
                case PieceType.Bishop:
                    totalBlackPoints += 3;
                    break;
                case PieceType.Queen:
                    totalBlackPoints += 9;
                    break;
                default:
                    break;
            }
        }

        foreach (var piece in BlackTakenPieces)
        {
            switch (piece)
            {
                case PieceType.Pawn:
                    totalWhitePoints += 1;
                    break;
                case PieceType.Rook:
                    totalWhitePoints += 5;
                    break;
                case PieceType.Knight:
                    totalWhitePoints += 3;
                    break;
                case PieceType.Bishop:
                    totalWhitePoints += 3;
                    break;
                case PieceType.Queen:
                    totalWhitePoints += 9;
                    break;
                default:
                    break;
            }
        }

        if (totalWhitePoints - totalBlackPoints > 0)
        {
            //White is in the lead
            WhitePoints = totalWhitePoints - totalBlackPoints;//Is positive value
            BlackPoints = 0;
        }
        else if (totalWhitePoints - totalBlackPoints < 0)
        {
            //Black is in the lead
            BlackPoints = Math.Abs(totalWhitePoints - totalBlackPoints);//Is negative value
            WhitePoints = 0;
        }
        else
        {
            //Equal
            WhitePoints = 0;
            BlackPoints = 0;
        }
    }

    public static BoardManager Instance;
    //BoardManager.Instance.--- -> Singleton
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
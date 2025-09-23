using UnityEngine;

public class PieceData : MonoBehaviour
{
    [Header("Piece Data: ")]
    [SerializeField] public Vector2Int Position;
    [SerializeField] public ChessPieceBase.PieceType PieceType;
    [SerializeField] public ChessPieceBase.PieceColor PieceColor;

    public PieceData(Vector2Int position, ChessPieceBase.PieceType pieceType, ChessPieceBase.PieceColor pieceColor)
    {
        Position = position;
        PieceType = pieceType;
        PieceColor = pieceColor;
    }
    public void CopyFrom(PieceData piece)
    {
        Position = piece.Position;
        PieceType = piece.PieceType;
        PieceColor = piece.PieceColor;
    }
}

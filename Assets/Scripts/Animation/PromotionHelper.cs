using UnityEngine;

public class PromotionHelper : MonoBehaviour
{
    [Header("White Pieces For Promotion: ")]
    [SerializeField] GameObject whiteRook;
    [SerializeField] GameObject whiteKnight;
    [SerializeField] GameObject whiteBishop;
    [SerializeField] GameObject whiteQueen;

    [Space(10)]
    [Header("Black Pieces For Promotion: ")]
    [SerializeField] GameObject blackRook;
    [SerializeField] GameObject blackKnight;
    [SerializeField] GameObject blackBishop;
    [SerializeField] GameObject blackQueen;

    public void GeneratePieceAt(Vector3 position, PieceData piece)
    {
        GameObject pieceToGenerate = null;

        switch (piece.PieceType)
        {
            case ChessPieceBase.PieceType.Rook:
                pieceToGenerate = (piece.PieceColor == ChessPieceBase.PieceColor.White) ? whiteRook : blackRook;
                break;
            case ChessPieceBase.PieceType.Knight:
                pieceToGenerate = (piece.PieceColor == ChessPieceBase.PieceColor.White) ? whiteKnight : blackKnight;
                break;
            case ChessPieceBase.PieceType.Bishop:
                pieceToGenerate = (piece.PieceColor == ChessPieceBase.PieceColor.White) ? whiteBishop : blackBishop;
                break;
            case ChessPieceBase.PieceType.Queen:
                pieceToGenerate = (piece.PieceColor == ChessPieceBase.PieceColor.White) ? whiteQueen : blackQueen;
                break;
        }

        Transform parent = (piece.PieceColor == ChessPieceBase.PieceColor.White)
            ? PieceDataMemory.Instance.whitePiecesGroup.transform
            : PieceDataMemory.Instance.blackPiecesGroup.transform;


        GameObject generatedPiece = Instantiate(pieceToGenerate, position, Quaternion.identity, parent);
        PieceData newPiece = generatedPiece.AddComponent<PieceData>();
        newPiece.CopyFrom(piece);
    }
}
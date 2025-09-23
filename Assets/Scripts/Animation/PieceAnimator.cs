using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PieceAnimator : MonoBehaviour
{
    //Reads data from PersistantAnimationData.Instance.AnimationRecord
    private readonly List<FileManager.ChessMove> history = PersistantAnimationData.Instance.AnimationRecord.GameRecord.History;

    [SerializeField] private CameraAnimatorExit cameraAnimatorExit;
    [Header("Animation settings: ")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private int tileOffset = 1;
    [SerializeField] private int jumpHeight = 3;

    public void StartMovingPieces()
    {
        StartCoroutine(MovePieces());
    }

    private IEnumerator MovePieces()
    {
        Debug.Log("Animation has started!!!");

        if (PieceDataMemory.Instance == null) yield break;
        if (PersistantAnimationData.Instance == null) yield break;

        foreach (var move in history)
        {
            //Get logical board position
            Vector2Int startPosition = new Vector2Int(move.From.X, move.From.Y);
            Vector2Int targetPosition = new Vector2Int(move.To.X, move.To.Y);

            GameObject capturedPiece = null;


            //Get default captured piece
            if (move.HasCaptured && move.SpecialMove != FileManager.SpecialMove.EnPassant)
            {
                //Get captured piece
                foreach (var piece in PieceDataMemory.Instance.data)
                {
                    if (piece.Position == targetPosition)
                    {
                        capturedPiece = piece.gameObject;
                        break;
                    }
                }
            }

            //Get piece to move
            GameObject pieceToMove = null;
            foreach (var piece in PieceDataMemory.Instance.data)
            {
                if (piece.Position == startPosition)
                {
                    pieceToMove = piece.gameObject;
                    break;
                }
            }

            //Get special move support
            PieceData newPiece = null;
            GameObject rookGO = null;
            Vector2Int rookPosition = Vector2Int.zero;
            Vector2Int rookTargetPosition = Vector2Int.zero;
            Vector3 startPosition3FRook = Vector3.zero;
            Vector3 targetPosition3FRook = Vector3.zero;
            if (move.SpecialMove != FileManager.SpecialMove.None)
            {
                //En Passant
                if (move.SpecialMove == FileManager.SpecialMove.EnPassant)
                {
                    //Get captured piece
                    foreach (var piece in PieceDataMemory.Instance.data)
                    {
                        int direction = (pieceToMove.GetComponent<PieceData>().PieceColor == ChessPieceBase.PieceColor.White) ? -1 : 1;

                        if (piece.Position == targetPosition + new Vector2Int(0, direction))
                        {
                            capturedPiece = piece.gameObject;
                            break;
                        }
                    }
                }

                //Castling
                if (move.SpecialMove == FileManager.SpecialMove.Castle)
                {
                    capturedPiece = null;
                    int direction = (pieceToMove.GetComponent<PieceData>().PieceColor == ChessPieceBase.PieceColor.White) ? 0 : 7;

                    //Right castle
                    if (targetPosition.x == 6)
                    {
                        rookPosition = new Vector2Int(7, direction);
                    }

                    //Left castle
                    if (targetPosition.x == 2)
                    {
                        rookPosition = new Vector2Int(0, direction);
                    }

                    //Get rook GameObject
                    foreach (var piece in PieceDataMemory.Instance.data)
                    {
                        if (rookPosition == piece.Position)
                        {
                            rookGO = piece.gameObject;
                            break;
                        }
                    }

                    //Get rook world positions
                    int xOffset = 0;
                    startPosition3FRook = rookGO.transform.position;
                    if (rookPosition.x == 0) xOffset = 3;
                    if (rookPosition.x == 7) xOffset = -2;
                    targetPosition3FRook = rookGO.transform.position + new Vector3(xOffset, 0, 0);
                    rookTargetPosition = new Vector2Int(rookPosition.x + xOffset, rookPosition.y);
                }

                //Promotion
                if (move.SpecialMove == FileManager.SpecialMove.Promotion)
                {
                    //Remove current piece
                    capturedPiece = pieceToMove;

                    newPiece = new PieceData(targetPosition, move.PromotedInto, move.PieceColor);
                }
            }

            if (pieceToMove == null)
            {
                Debug.LogError($"Piece at position: {startPosition} is not found!!!");
                break;
            }


            float elapsed = 0f;
            //Get world positions
            Vector2Int offset = targetPosition - startPosition;
            Vector3 startPosition3F = pieceToMove.transform.position;
            Vector3 targetPosition3F = new Vector3
                (
                    x: startPosition3F.x + (offset.x * tileOffset),
                    y: startPosition3F.y,
                    z: startPosition3F.z + (offset.y * tileOffset)
                );



            //Animate the move
            if (move.SpecialMove != FileManager.SpecialMove.Castle)
            {
                if(pieceToMove.GetComponent<PieceData>().PieceType == ChessPieceBase.PieceType.Knight)
                {
                    float halfDuration = duration / 2;
                    Vector3 middlePiecePosition3F = (startPosition3F + targetPosition3F) / 2f+ Vector3.up * jumpHeight;

                    while (elapsed < halfDuration)
                    {
                        pieceToMove.transform.position = Vector3.Lerp(startPosition3F, middlePiecePosition3F, elapsed / halfDuration);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                    pieceToMove.transform.position = middlePiecePosition3F;

                    //Reset for second half
                    elapsed = 0;

                    while (elapsed < halfDuration)
                    {
                        pieceToMove.transform.position = Vector3.Lerp(middlePiecePosition3F, targetPosition3F, elapsed / halfDuration);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                    pieceToMove.transform.position = targetPosition3F;
                }
                else
                {
                    //Normal animation
                    while (elapsed < duration)
                    {
                        pieceToMove.transform.position = Vector3.Lerp(startPosition3F, targetPosition3F, elapsed / duration);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                    pieceToMove.transform.position = targetPosition3F;
                }
                    
            }
            else
            {
                Vector3 middleRookPosition3F = (startPosition3FRook + targetPosition3FRook) / 2f + Vector3.up * jumpHeight;
                Vector3 middlePiecePosition3F = (startPosition3F + targetPosition3F) / 2f;
                float halfDuration = duration / 2;

                while (elapsed < halfDuration)
                {
                    pieceToMove.transform.position = Vector3.Lerp(startPosition3F, middlePiecePosition3F, elapsed / halfDuration);
                    rookGO.transform.position = Vector3.Lerp(startPosition3FRook, middleRookPosition3F, elapsed / halfDuration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                pieceToMove.transform.position = middlePiecePosition3F;
                rookGO.transform.position = middleRookPosition3F;

                //Reset for second half
                elapsed = 0f;

                while (elapsed < halfDuration)
                {
                    pieceToMove.transform.position = Vector3.Lerp(middlePiecePosition3F, targetPosition3F, elapsed / halfDuration);
                    rookGO.transform.position = Vector3.Lerp(middleRookPosition3F, targetPosition3FRook, elapsed / halfDuration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                pieceToMove.transform.position = targetPosition3F;
                rookGO.transform.position = targetPosition3FRook;
            }

            //Promotion
            if (move.SpecialMove == FileManager.SpecialMove.Promotion)
            {
                var promotionHelper = PieceDataMemory.Instance.gameObject.GetComponent<PromotionHelper>();
                promotionHelper.GeneratePieceAt(targetPosition3F, newPiece);
            }

            //Remove captured piece if necessary
            if (capturedPiece != null)
            {
                Destroy(capturedPiece);
            }

            //Update memory
            PieceData pieceData = null;
            if (pieceToMove != null)
            {
                pieceData = pieceToMove.GetComponent<PieceData>();
            }
            if (pieceData != null) pieceData.Position = targetPosition;
            if (rookGO != null && rookGO.GetComponent<PieceData>() != null) rookGO.GetComponent<PieceData>().Position = rookTargetPosition;

            yield return null;
            PieceDataMemory.Instance.UpdatePieceDataMemory();
        }
    }
}
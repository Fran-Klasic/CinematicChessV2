using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class PieceAnimator : MonoBehaviour
{
    //Reads data from PersistantAnimationData.Instance.AnimationRecord
    private readonly List<FileManager.ChessMove> history = PersistantAnimationData.Instance.AnimationRecord.GameRecord.History;

    private bool started = false;

    [SerializeField] private CameraAnimatorExit cameraAnimatorExit;
    [Header("Animation settings: ")]
    [SerializeField] private float duration;
    [SerializeField] private int tileOffset = 1;

    private void Update()
    {
        //Checks if camera animator is playing intro
        if (cameraAnimatorExit.isEnabled || started) return;
        StartCoroutine(MovePieces());
    }

    private IEnumerator MovePieces()
    {
        started = true;

        foreach (var move in history)
        {
            //Get logical board position
            Vector2Int startPosition = new Vector2Int(move.From.X, move.From.Y);
            Vector2Int targetPosition = new Vector2Int(move.To.X, move.To.Y);

            GameObject capturedPiece = null;

            if (move.SpecialMove != FileManager.SpecialMove.None)
            {
                //TODO
                /*
                 * Add special move support
                */
                continue;
            }

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

            if (pieceToMove == null)
            {
                Debug.LogError($"Piece at position: {startPosition} is not found!!!");
                break;
            }


            float elapsed = 0f;
            //Get world positions
            Vector3 startPosition3F = pieceToMove.transform.position;
            Vector2Int offset = targetPosition - startPosition;
            Vector3 targetPosition3F = new Vector3
                (
                    x: startPosition3F.x + (offset.x * tileOffset),
                    y: startPosition3F.y,
                    z: startPosition3F.z + (offset.y * tileOffset)
                );

            //Animate the move
            while (elapsed < duration)
            {
                pieceToMove.transform.position = Vector3.Lerp(startPosition3F, targetPosition3F, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            pieceToMove.transform.position = targetPosition3F;

            //Remove captured piece if necessary
            if (capturedPiece != null)
            {
                Destroy(capturedPiece);
            }

            //Update memory
            PieceData pieceData = pieceToMove.GetComponent<PieceData>();
            if (pieceData != null) pieceData.Position = targetPosition;
            PieceDataMemory.Instance.UpdatePieceDataMemory();
        }
    }
}
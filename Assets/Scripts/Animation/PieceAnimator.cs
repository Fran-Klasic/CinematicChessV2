using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PieceAnimator : MonoBehaviour
{
    //Reads data from PersistantAnimationData.Instance.AnimationRecord
    private readonly List<FileManager.ChessMove> history = PersistantAnimationData.Instance.AnimationRecord.GameRecord.History;

    [SerializeField] private CameraAnimatorExit cameraAnimatorExit;
    [SerializeField] private CinemachineCamera vcamMain;
    [SerializeField] private CinemachineCamera vcamWhite;
    [SerializeField] private CinemachineCamera vcamBlack;
    [SerializeField] private Animator transition;

    [Space(10)]
    [Header("Audio: ")]
    [SerializeField] private AudioSource audioSource;

    [Space(10)]
    [Header("Animation settings: ")]
    [SerializeField] private int tileOffset = 1;
    [SerializeField] private float jumpHeight = 2.5f;


    [HideInInspector] public float pace;
    //Short 0.75f
    //Medium 1.5f
    //Long 2.5f
    private List<float> durations = new List<float>();
    private float tempDuration = 2f;
    private float durationOffset = 0f;

    public class FloatArrayWrapper
    {
        public float[] hits;
    }

    public void GetDurations(SongData songData)
    {
        var wrapper = JsonConvert.DeserializeObject<FloatArrayWrapper>(songData.timestampsJSON.text);
        float[] values = wrapper.hits;
        float oldTime = 0f;

        foreach (float value in values)
        {
            float localValue = value - oldTime;

            if (localValue >= pace + UnityEngine.Random.Range(0.3f, 2f) - durationOffset)
            {
                oldTime = value;
                durations.Add(localValue);
            }
        }

        if (history.Count > durations.Count)
        {
            if (durations[0] < 0.3f)
            {
                //Too short, return
                return;
            }
            durations.Clear();
            durationOffset += 0.1f;
            GetDurations(songData);
        }
    }

    public void StartMovingPieces()
    {
        switch (PersistantAnimationData.Instance.AnimationRecord.AnimationData.Pace)
        {
            case "Short":
                pace = 0.75f;
                break;
            case "Medium":
                pace = 1.5f;
                break;
            case "Long":
                pace = 2.5f;
                break;
        }

        GetDurations(AnimationController.Instance.songData);
        StartCoroutine(MovePieces());
    }

    private IEnumerator MovePieces()
    {
        Debug.Log("Animation has started!!!");

        if (PieceDataMemory.Instance == null) yield break;
        if (PersistantAnimationData.Instance == null) yield break;

        int counter = 0;
        float duration = tempDuration;

        foreach (var move in history)
        {
            if (durations.Count >= history.Count)
            {
                duration = durations[counter];
                counter++;
                Debug.Log($"{counter} : {duration} seconds");
            }

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
            ChessPieceBase.PieceColor pieceColor = ChessPieceBase.PieceColor.None;
            foreach (var piece in PieceDataMemory.Instance.data)
            {
                if (piece.Position == startPosition)
                {
                    pieceColor = piece.PieceColor;
                    pieceToMove = piece.gameObject;
                    break;
                }
            }

            //Set piece to move as tracking target
            if (pieceColor == ChessPieceBase.PieceColor.White)
            {
                vcamWhite.Target.TrackingTarget = pieceToMove.transform;
                vcamWhite.gameObject.SetActive(true);
            }
            else
            {
                vcamBlack.Target.TrackingTarget = pieceToMove.transform;
                vcamWhite.gameObject.SetActive(false);
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
                float halfDuration = duration / 2;
                Vector3 middlePiecePosition3F = (startPosition3F + targetPosition3F) / 2f + Vector3.up * jumpHeight;

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

            //Play SFX
            audioSource.Play();

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

        AnimationController.Instance.EndSong();
        yield return new WaitForSeconds(1f);

        vcamMain.gameObject.SetActive(true);
        vcamMain.gameObject.GetComponent<Animator>().SetTrigger("Out");
        yield return new WaitForSeconds(2f);

        AnimationController.Instance.PlayOutro();
        yield return new WaitForSeconds(3f);

        transition.SetTrigger("Out");
        yield return new WaitForSeconds(1f);

        SceneManager.Instance.LoadMainUI();
    }
}
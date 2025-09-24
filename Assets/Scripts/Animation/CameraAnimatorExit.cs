using UnityEngine;

public class CameraAnimatorExit : MonoBehaviour
{
    [SerializeField] private Animator MainCameraAnimator;

    public void OnCameraAnimFinished()
    {
        var pieceAnimatorGO = GameObject.Find("PieceAnimator");
        var pieceAnimator = pieceAnimatorGO.GetComponent<PieceAnimator>();
        pieceAnimator.StartMovingPieces();
        gameObject.SetActive(false);
    }

}
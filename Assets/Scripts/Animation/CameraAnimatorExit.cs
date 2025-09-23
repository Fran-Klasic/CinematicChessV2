using UnityEngine;

public class CameraAnimatorExit : MonoBehaviour
{
    [SerializeField] private Animator MainCameraAnimator;

    public void OnCameraAnimFinished()
    {
        MainCameraAnimator.enabled = false;
        var pieceAnimatorGO = GameObject.Find("PieceAnimator");
        var pieceAnimator = pieceAnimatorGO.GetComponent<PieceAnimator>();
        pieceAnimator.StartMovingPieces();
        gameObject.SetActive(false);
    }

}
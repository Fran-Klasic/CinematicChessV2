using UnityEngine;

public class CameraAnimatorExit : MonoBehaviour
{
    [SerializeField] private Animator MainCameraAnimator;

    public void OnCameraAnimFinished()
    {
        MainCameraAnimator.enabled = false;

    }

}
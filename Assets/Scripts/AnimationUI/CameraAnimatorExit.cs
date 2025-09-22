using UnityEngine;

public class CameraAnimatorExit : MonoBehaviour
{
    [SerializeField] private Animator MainCameraAnimator;

    [HideInInspector] public bool isEnabled = true;
    public void OnCameraAnimFinished()
    {
        MainCameraAnimator.enabled = false;
        isEnabled = false;
    }

}
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

public class CameraActivator : MonoBehaviour
{
    [Header("Cinemachine Camera: ")]
    [SerializeField] private CinemachineCamera CinemachineCamera;

    [Space(10)]
    [Header("GameObjects: ")]
    [SerializeField] private List<Transform> Pieces;

    public void DisableRotationControl()
    {
        CinemachineCamera.Target.TrackingTarget = null;
    }

    public void EnableRotationControl(Transform target)
    {
        CinemachineCamera.Target.TrackingTarget = target;
    }

    public static CameraActivator Instance;
    //CameraActivator.Instance.--- -> Singleton
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
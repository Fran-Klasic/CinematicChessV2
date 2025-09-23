using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [HideInInspector] public SongData songData;

    [SerializeField] AudioSource audioSource;

    public static AnimationController Instance;
    //AnimationController.Instance.--- -> Singleton
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
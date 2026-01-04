using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    [HideInInspector] public SongData songData;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public OutroLoader outroLoader;

    public void PlaySong()
    {
        audioSource.clip = songData.audioClipMP3;
        audioSource.Play();
    }

    public void EndSong()
    {
        StartCoroutine(FadeSong());
    }

    public void PlayOutro()
    {
        outroLoader.StartOutro();
    }

    private IEnumerator FadeSong()
    {
        float currentVolume = audioSource.volume;
        float elapsed = 0f;
        float duration = 6f;

        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(currentVolume, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }

    private void Update()
    {
        audioSource.volume = AudioManager.Volume;
    }

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
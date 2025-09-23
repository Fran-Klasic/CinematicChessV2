using System.Collections;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroLoader : MonoBehaviour
{
    [Header("GameObject: ")]
    [SerializeField] private GameObject GameIntoGroup;
    [SerializeField] private GameObject MusicDataGroup;
    [SerializeField] private GameObject Player1DataGroup;
    [SerializeField] private GameObject Player2DataGroup;

    [Space(10)]
    [Header("Animator: ")]
    [SerializeField] private Animator GameIntoGroupAnimator;
    [SerializeField] private Animator MusicDataGroupAnimator;
    [SerializeField] private Animator Player1DataGroupAnimator;
    [SerializeField] private Animator Player2DataGroupAnimator;

    [Space(10)]
    [Header("Text: ")]
    [SerializeField] private TextMeshProUGUI AnimationName;
    [SerializeField] private TextMeshProUGUI SongName;

    [Space(10)]
    [Header("Audio: ")]
    [SerializeField] private AudioSource audioSource;

    [Space(10)]
    [Header("Time: ")]
    [SerializeField] private float WaitTime = 1f;


    [HideInInspector] public SongData song;

    #region Scene loading and starting corutine
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        song = await GetSongDataAsync();
        if (song != null)
        {
            Debug.Log("Loaded song: " + song.songName);
            audioSource.clip = song.audioClipMP3;
        }

        StartCoroutine(PlayIntro());
    }

    #endregion

    private void GetAnimationName()
    {
        string gameName;
        if (PersistantAnimationData.Instance != null)
            gameName = PersistantAnimationData.Instance.AnimationRecord.GameRecord.Metadata.GameFileName;
        else
            gameName = "Temp Game Name";

        gameName = Path.GetFileNameWithoutExtension(gameName);
        AnimationName.text = gameName;
    }

    private async Task<SongData> GetSongDataAsync()
    {

        if (PersistantAnimationData.Instance != null)
        {
            string songAddress = PersistantAnimationData.Instance.AnimationRecord.AnimationData.Song;

            var handle = Addressables.LoadAssetAsync<SongData>(songAddress);
            SongData songData = await handle.Task; if (songData != null)
            {
                SongName.text = songData.songName;
                AnimationController.Instance.songData = songData;
                return songData;
            }
            else
            {
                SongName.text = "Failed to load song";
                return null;
            }
        }
        else
        {
            SongName.text = "Temp Song Name";
            return null;
        }
    }

    private IEnumerator PlayIntro()
    {
        audioSource.Play();

        yield return new WaitForSeconds(WaitTime / 2);//Wait for loader to fade

        GetAnimationName();
        if (PersistantAnimationData.Instance == null || PersistantAnimationData.Instance.AnimationRecord.AnimationData.EnableHud == "True")
        {
            GameIntoGroup.SetActive(true);
        }
        yield return new WaitForSeconds(WaitTime);//Play the game intro group if possible

        yield return new WaitForSeconds(WaitTime * 2);

        if (PersistantAnimationData.Instance == null || PersistantAnimationData.Instance.AnimationRecord.AnimationData.EnableHud == "True")
        {
            GameIntoGroupAnimator.SetTrigger("Out");
            MusicDataGroup.SetActive(true);
        }
        yield return new WaitForSeconds(WaitTime);
        if (PersistantAnimationData.Instance == null || PersistantAnimationData.Instance.AnimationRecord.AnimationData.EnableHud == "True")
        {
            GameIntoGroup.SetActive(false);
        }

        yield return new WaitForSeconds(WaitTime * 2);

        if (PersistantAnimationData.Instance == null || PersistantAnimationData.Instance.AnimationRecord.AnimationData.EnableHud == "True")
        {
            MusicDataGroupAnimator.SetTrigger("Out");
        }
        yield return new WaitForSeconds(WaitTime);
    }
}

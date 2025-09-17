using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    [HideInInspector] public static float universalTransitionTime = 1f;

    private SongData song;

    #region Scene loading and starting corutine
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(PlayIntro());
    }

    #endregion

    private IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(universalTransitionTime);

        //Play camera animation

        string gameName;
        if (PersistantAnimationData.Instance != null)
            gameName = PersistantAnimationData.Instance.AnimationRecord.GameRecord.Metadata.GameFileName;
        else
            gameName = "Temp Game Name";

        gameName = Path.GetFileNameWithoutExtension(gameName);
        AnimationName.text = gameName;

        GameIntoGroup.SetActive(true);
        yield return new WaitForSeconds(universalTransitionTime);

        yield return new WaitForSeconds(2f); //Wait 2 seconds

        if (PersistantAnimationData.Instance != null)
        {
            string songAddress = PersistantAnimationData.Instance.AnimationRecord.AnimationData.Song;

            Addressables.LoadAssetAsync<SongData>(songAddress).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                {
                    SongData songData = handle.Result;
                    SongName.text = songData.songName;

                    song = handle.Result; //For AudioManager
                }
            };
        }
        else
        {
            SongName.text = "Temp Song Name";
        }

        GameIntoGroupAnimator.SetTrigger("Out");
        MusicDataGroup.SetActive(true);
        yield return new WaitForSeconds(universalTransitionTime);

        GameIntoGroup.SetActive(false);
        yield return new WaitForSeconds(2f); //Wait 2 seconds

        MusicDataGroupAnimator.SetTrigger("Out");
        yield return new WaitForSeconds(universalTransitionTime);

    }
}

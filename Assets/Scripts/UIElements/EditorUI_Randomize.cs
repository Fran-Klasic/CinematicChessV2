using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class EditorUI_Randomize : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private ReplayManager replayManager;

    public async void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        var record = replayManager.record;
        if (record == null) return;
        SceneManager.Instance.SaveGameData(record);

        #region Random
        string randomTheme = AnimationSettingsUI_Core.validThemes.ElementAt(Random.Range(0, AnimationSettingsUI_Core.validThemes.Count));
        string randomPace = AnimationSettingsUI_Core.validPace.ElementAt(Random.Range(0, AnimationSettingsUI_Core.validPace.Count));
        string randomGenre = AnimationSettingsUI_Core.validGenre.ElementAt(Random.Range(0, AnimationSettingsUI_Core.validGenre.Count));
        string randomOrientation = AnimationSettingsUI_Core.validOrientation.ElementAt(Random.Range(0, AnimationSettingsUI_Core.validOrientation.Count));
        string randomEnableHud = AnimationSettingsUI_Core.validEnableHud.ElementAt(Random.Range(0, AnimationSettingsUI_Core.validEnableHud.Count));

        SongData song = await AudioManager.LoadRandomSongAsync(randomGenre);
        string randomSong = song.name;

        AnimationSettingsUI_Core.AnimationData animationData = new AnimationSettingsUI_Core.AnimationData(randomTheme, randomPace, randomGenre, randomOrientation, randomEnableHud, "player1", "player2", randomSong);
        AnimationSettingsUI_Core.SaveAllData(animationData);

        #endregion

        Debug.Log("Starting Animation...");
        //Load AnimationScene
        StartCoroutine(StartAnimation());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }
    private IEnumerator StartAnimation()
    {
        transition.SetTrigger("Out");
        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
        LoadAnimationScene();
    }
    private void LoadAnimationScene()
    {
        //Loads AnimationCore scene and deserializes the animation json,
        //then loads the theme scene (world)

        //Deserialize animation file
        FileManager.AnimationRecord animationRecord = FileManager.Instance.DeserializeAnimationFile();
        if (animationRecord == null) return;
        if (!FileManager.Instance.ValidateAnimationRecord(animationRecord))
        {
            //In case of invalid files reload the game
            SceneManager.Instance.LoadMainUI();
            return;
        }

        //Save data to PersistantAnimationData
        PersistantAnimationData.Instance.ResetData();
        PersistantAnimationData.Instance.AnimationRecord = animationRecord;

        string sceneName = $"Theme_{animationRecord.AnimationData.Theme}";
        var sceneToLoad = SceneManager.Instance.GetScene(sceneName);
        PersistantAnimationData.Instance.ActiveScene = sceneToLoad;

        SceneManager.Instance.LoadAnimationCore();
        SceneManager.Instance.LoadAdditiveScene(sceneToLoad);
    }

   
}
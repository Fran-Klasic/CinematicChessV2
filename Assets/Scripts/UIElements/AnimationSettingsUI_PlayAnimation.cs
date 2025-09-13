using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationSettingsUI_PlayAnimation : MonoBehaviour
{
    [SerializeField] Animator transition;

    public async void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        #region Random Song
        string genreLabel = AnimationSettingsUI_Core.musicGenre;

        SongData song = await AudioManager.LoadRandomSongAsync(genreLabel);
        string name = song.name;

        AnimationSettingsUI_Core.SaveSong(name);

        AnimationSettingsUI_Core.SaveAllData();
        #endregion

        Debug.Log("Starting Animation...");
        //Load AnimationScene

        StartCoroutine(StartAnimation());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
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

    private IEnumerator StartAnimation()
    {
        transition.SetTrigger("Out");
        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        LoadAnimationScene();
    }
}
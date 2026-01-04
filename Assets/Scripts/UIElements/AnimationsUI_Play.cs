using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationsUI_Play : MonoBehaviour
{
    [SerializeField] private Animator transition;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(PlayAnimation());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator PlayAnimation()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");
        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        var record = GetAnimationFile();
        PersistantAnimationData.Instance.AnimationRecord = record;

        string sceneName = $"Theme_{record.AnimationData.Theme}";


        var sceneToLoad = SceneManager.Instance.GetScene(sceneName);
        PersistantAnimationData.Instance.ActiveScene = sceneToLoad;

        SceneManager.Instance.LoadAnimationCore();
        SceneManager.Instance.LoadAdditiveScene(sceneToLoad);
    }

    private FileManager.AnimationRecord GetAnimationFile()
    {
        string fileName = string.Empty;
        var validatedFiles = MainController.Instance.ValidateAnimationsFiles();

        #region File Name Getting
        Transform current = gameObject.transform.parent;

        while (current != null)
        {
            if (current.name == "Background")
            {
                fileName = current.parent.gameObject.name; //Get file name
                break;
            }
            current = current.parent;
        }
        if (current == null) fileName = string.Empty;

        var allGames = validatedFiles.Item1;
        if (!allGames.Contains(fileName)) fileName = string.Empty;
        #endregion

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new FileNotFoundException("File not found! ", fileName);
        }

        #region File Record Getting
        //Get record from our file
        var data = validatedFiles.Item2;
        FileManager.AnimationRecord record = null;
        foreach (var file in data)
        {
            if (file.Item2 == fileName)
            {
                record = file.Item1;
                break;
            }
        }
        #endregion

        return record;
    }

}
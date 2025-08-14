using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.EventSystems;

public class GamesUI_Editor : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private GameObject games_Group;
    [SerializeField] private GameObject editor_Group;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        FileManager.GameRecord record = GetGameFile();

        StartCoroutine(StartEditor(record));

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartEditor(FileManager.GameRecord record)
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        games_Group.SetActive(false);
        editor_Group.SetActive(true);

        ReplayManager.Instance.record = record;

        transition.SetTrigger("In");
        MainController.Instance.isAnimating = false;

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
    }

    private FileManager.GameRecord GetGameFile()
    {
        string fileName = string.Empty;
        var validatedFiles = MainController.Instance.ValidateFiles();

        #region File Name Getting
        //gameObject is editor button
        //FileName->Background->GamePreview->ButtonGroup->Editor
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
        FileManager.GameRecord record = null;
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
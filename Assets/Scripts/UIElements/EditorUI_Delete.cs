using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.IO;
using Unity.VisualScripting;
using TMPro;

public class EditorUI_Delete : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private Animator deleteConfirmation_Animator;
    [SerializeField] private GameObject deleteConfirmation_Group;
    [SerializeField] private ReplayManager replayManager;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        //Ask for permission
        StartCoroutine(StartDelete());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartDelete()
    {
        MainController.Instance.isAnimating = true;

        MainController.Instance.hasCanceled = false;
        MainController.Instance.hasDeleted = false;

        deleteConfirmation_Group.SetActive(true);
        deleteConfirmation_Animator.SetTrigger("Start");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        MainController.Instance.isAnimating = false;

        //Wait for user input
        while (true)
        {
            if (MainController.Instance.hasCanceled)
            {
                //User has canceled deletion
                MainController.Instance.isAnimating = true;
                deleteConfirmation_Animator.SetTrigger("End");
                yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
                MainController.Instance.isAnimating = false;
                deleteConfirmation_Group.SetActive(false);
                yield break;
            }
            if (MainController.Instance.hasDeleted)
            {
                //User wants to delete file
                MainController.Instance.isAnimating = true;
                deleteConfirmation_Animator.SetTrigger("End");
                yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
                MainController.Instance.isAnimating = false;
                deleteConfirmation_Group.SetActive(false);
                break;
            }

            yield return null;
        }

        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        string fullFileName = replayManager.record.Metadata.GameFileName;

        fullFileName = Path.Combine(FileManager.Instance.pathToGamesDirectory, fullFileName);

        //Delete file
        File.Delete(fullFileName);

        MainController.Instance.isAnimating = false;

        SceneManager.Instance.LoadMainUI();
    }
}
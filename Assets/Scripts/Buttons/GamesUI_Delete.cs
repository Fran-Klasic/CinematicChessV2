using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.IO;
using Unity.VisualScripting;

public class GameUI_Delete : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private Animator deleteConfirmation_Animator;
    [SerializeField] private GameObject deleteConfirmation_Group;
    [SerializeField] private GameObject gamePrefab_None;

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

        GameObject gamePrefab = GetComponentInParent<GamePrefabComponent>().gameObject;
        string fullFileName = gamePrefab.name;

        var content = gamePrefab.transform.parent;
        bool willBeEmpty = content.childCount <= 1;

        //Check if there are other games 
        if (willBeEmpty)
        {
            GameObject newObj = Instantiate(gamePrefab_None, Vector3.zero, Quaternion.identity);
            newObj.transform.SetParent(content);
            newObj.transform.localScale = Vector3.one;
        }

        //Delete file
        File.Delete(fullFileName);
        //Destroy gamePrefab
        Destroy(gamePrefab);

        transition.SetTrigger("In");

        //Set isAnimating to false because next frame script will be destroyed
        MainController.Instance.isAnimating = false;
        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
    }
}
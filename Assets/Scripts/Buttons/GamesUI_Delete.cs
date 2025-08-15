using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.IO;

public class GameUI_Delete : MonoBehaviour
{
    [SerializeField] private Animator transition;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        //Ask for permission

        //Temp
        StartCoroutine(StartDelete());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartDelete()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        GameObject gamePrefab = GetComponentInParent<GamePrefabComponent>().gameObject;
        string fullFileName = gamePrefab.name;

        //Delete file
        File.Delete(fullFileName);
        //Destroy gamePrefab
        Destroy(gamePrefab);


        transition.SetTrigger("In");
        MainController.Instance.isAnimating = false;

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
    }
}
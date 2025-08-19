using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EditorUI_AnimationSettings : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private GameObject editorGroup;
    [SerializeField] private GameObject animationSettingsGroup;
    [SerializeField] private ReplayManager replayManager;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        var record = replayManager.record;
        if (record == null) return;

        SceneManager.Instance.SaveGameData(record);

        StartCoroutine(StartAnimationSettings());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartAnimationSettings()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        editorGroup.SetActive(false);
        animationSettingsGroup.SetActive(true);

        transition.SetTrigger("In");
        MainController.Instance.isAnimating = false;

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
    }

}
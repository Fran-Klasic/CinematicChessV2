using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainUI_Options : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private GameObject titleScreen_Group;
    [SerializeField] private GameObject options_Group;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(StartOptionsMenu());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator StartOptionsMenu()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        titleScreen_Group.SetActive(false);
        options_Group.SetActive(true);

        Slider slider = options_Group
        .transform.Find("AllOptionsGroup_Scroll/View/Content/Volume/Slider")
        .GetComponent<Slider>();

        slider.value = AudioManager.Volume;

        transition.SetTrigger("In");
        MainController.Instance.isAnimating = false;

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
    }
}
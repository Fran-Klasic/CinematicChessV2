using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour
{
    private string buttonName;
    private static bool isPlayMenuActive = false; //same value across all instances
    private static bool isAnimating = false; //value to check if an animation is playing before it plays a different animation
    private float universalTransitionTime = 1f; //animation length for every transition

    [Header("Play Button Only")]
    [SerializeField] private Animator playMenuAnimator;
    [SerializeField] private GameObject playMenuGroup;
    [Space(10)]

    [Header("General")]
    [SerializeField] private Animator transition;

    private void Awake()
    {
        buttonName = gameObject.name;
    }

    public void OnClick()
    {
        if (isAnimating) return; //Do not interrupt current animation

        Debug.Log("Clicked on " + buttonName);
        switch (buttonName)
        {
            case "Play_Button":
                HandlePlayButton();
                break;
            case "Games_Button":
                HandleGamesButton();
                break;
            case "Animations_Button":
                HandleAnimationsButton();
                break;
            case "Favorites_Button":
                HandleFavoritesButton();
                break;
            case "Options_Button":
                HandleOptionsButton();
                break;
            case "Credits_Button":
                HandleCreditsButton();
                break;
            case "Quit_Button":
                HandleQuitButton();
                break;
            case "CloseMenu":
                HandleCloseMenuButton();
                break;
            case "PlayFriendButton":
                HandlePlayFriendButton();
                break;
            case "PlayMultiplayerButton":
                HandlePlayMultiplayerButton();
                break;
            case "PlayBotButton":
                HandlePlayBotButton();
                break;
        }

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }
    private void transitionOut()
    {
        isAnimating = true;
        transition.SetTrigger("Out");
    }

    private void transitionIn()
    {
        isAnimating = true;
        transition.SetTrigger("In");
    }

    private IEnumerator QuitGame()
    {
        transitionOut();
        yield return new WaitForSeconds(universalTransitionTime);
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        isAnimating = false;
    }

    private IEnumerator PlayFriendButton()
    {
        transitionOut();
        yield return new WaitForSeconds(universalTransitionTime);
        isAnimating = false;

        SceneManager.Instance.LoadChess2dVSFriend();
    }

    private IEnumerator StartPlayMenu()
    {
        isAnimating = true;
        playMenuGroup.SetActive(true);

        playMenuAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(universalTransitionTime);

        isPlayMenuActive = true;
        isAnimating = false;
    }

    private IEnumerator EndPlayMenu()
    {
        isAnimating = true;

        playMenuAnimator.SetTrigger("End");

        yield return new WaitForSeconds(universalTransitionTime);

        playMenuGroup.SetActive(false);

        isPlayMenuActive = false;
        isAnimating = false;
    }

    private void HandlePlayButton()
    {
        if (playMenuAnimator == null) return;

        if (!isPlayMenuActive)
            StartCoroutine(StartPlayMenu());
        else
            StartCoroutine(EndPlayMenu());
    }

    private void HandleGamesButton()
    {
    }

    private void HandleAnimationsButton()
    {
    }

    private void HandleFavoritesButton()
    {
    }

    private void HandleOptionsButton()
    {
    }

    private void HandleCreditsButton()
    {
    }

    private void HandleQuitButton()
    {
        if (transition == null) return;
        if (isAnimating) return;

        StartCoroutine(QuitGame());
    }

    private void HandleCloseMenuButton()
    {
        if (playMenuAnimator == null) return;

        if (isPlayMenuActive)
        {
            StartCoroutine(EndPlayMenu());
        }
    }

    private void HandlePlayFriendButton()
    {
        if (transition == null) return;
        if (isAnimating) return;

        StartCoroutine(PlayFriendButton());
    }

    private void HandlePlayMultiplayerButton()
    {

    }

    private void HandlePlayBotButton()
    {

    }
}

using System.Collections;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    private string buttonName;


    [SerializeField] private Animator transition;

    private void Awake()
    {
        buttonName = gameObject.name;
    }
    public void OnClick()
    {
        Debug.Log("Cliked on "+buttonName);
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

        }
    }

    IEnumerator QuitGame()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }

    private void HandlePlayButton()
    {
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
        StartCoroutine(QuitGame());
    }
}

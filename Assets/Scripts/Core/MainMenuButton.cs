using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using TMPro;

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

    [Header("Games Button Only")]
    [SerializeField] private GameObject titleScreen_Group;
    [SerializeField] private GameObject games_Group;
    [SerializeField] private GameObject contentGameObject;
    [SerializeField] private GameObject gamePrefab;
    [SerializeField] private GameObject gamePrefab_None;

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
            case "Delete":
                HandleDeleteButton();
                break;
            case "Editor":
                HandleEditorButton();
                break;
        }

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }
    private void TransitionOut()
    {
        isAnimating = true;
        transition.SetTrigger("Out");
    }

    private void TransitionIn()
    {
        isAnimating = true;
        transition.SetTrigger("In");
    }

    private IEnumerator QuitGame()
    {
        TransitionOut();
        yield return new WaitForSeconds(universalTransitionTime);
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        isAnimating = false;
    }

    private IEnumerator PlayFriendButton()
    {
        TransitionOut();
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

    private IEnumerator StartGamesMenu()
    {
        TransitionOut();
        yield return new WaitForSeconds(universalTransitionTime);

        titleScreen_Group.SetActive(false);
        games_Group.SetActive(true);

        TransitionIn();
        yield return new WaitForSeconds(universalTransitionTime);

        isAnimating = false;
    }

    private void HandlePlayButton()
    {
        if (playMenuAnimator == null) return;
        if (isAnimating) return;

        if (!isPlayMenuActive)
            StartCoroutine(StartPlayMenu());
        else
            StartCoroutine(EndPlayMenu());
    }

    private void HandleGamesButton()
    {
        if (transition == null) return;
        if (isAnimating) return;

        StartCoroutine(StartGamesMenu());

        string folderPath = FileManager.Instance.pathToGamesDirectory;

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string[] files = Directory.GetFiles(folderPath);

        //Data
        string gamesDataFile; //Contains data on game layout
        List<string> allGames = new List<string>();
        allGames.AddRange(files);

        //Remove all files that dont end in .json
        allGames.RemoveAll(file => !file.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase));


        foreach (string file in allGames)
        {
            //Check each file to confirm it is valid
            //Remove gamesDataFile
        }


        if (allGames.Count == 0)
        {
            GameObject newObj = Instantiate(gamePrefab_None, Vector3.zero, Quaternion.identity);
            newObj.transform.SetParent(contentGameObject.transform);
            newObj.transform.localScale = Vector3.one;
        }
        else
        {
            //Add gamePrefabs based on file data
            foreach (string file in allGames)
            {
                GameObject game = Instantiate(gamePrefab, Vector3.zero, Quaternion.identity);
                game.transform.SetParent(contentGameObject.transform);
                game.transform.localScale = Vector3.one;
                //Open file and set child data
                string gameName = Path.GetFileNameWithoutExtension(file);

                TextMeshProUGUI gameNameData = game.transform.Find("Background/GameName").gameObject.GetComponent<TextMeshProUGUI>();
                if(gameName.Length > 50)
                    gameName = gameName.Substring(0, 50); //Limit the game name to 50 characters
                gameNameData.text = gameName;
            }
        }


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

    private void HandleDeleteButton()
    {

    }

    private void HandleEditorButton()
    {

    }

}

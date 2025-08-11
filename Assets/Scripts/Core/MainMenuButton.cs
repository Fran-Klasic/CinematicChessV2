using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using TMPro;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Linq;

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
    [SerializeField] private GameObject contentGameObject;
    [SerializeField] private GameObject gamePrefab;
    [SerializeField] private GameObject gamePrefab_None;
    [Space(10)]

    [Header("General")]
    [SerializeField] private Animator transition;
    [SerializeField] private GameObject titleScreen_Group;
    [SerializeField] private GameObject games_Group;
    [SerializeField] private GameObject animations_Group;
    [SerializeField] private GameObject favorites_Group;
    [SerializeField] private GameObject options_Group;
    [SerializeField] private GameObject credits_Group;

    private void Awake()
    {
        buttonName = gameObject.name;
    }

    private void Start()
    {
        //Makes Games button stutter less
        var scene = SceneManager.Instance.GetScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        if(buttonName == "LevelLoader" && scene == SceneManager.Scenes.MainUI)
        {
            ValidateFiles();
        }
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
            case "PlayBotButton":
                HandlePlayBotButton();
                break;
            case "Delete":
                HandleDeleteButton();
                break;
            case "Editor":
                HandleEditorButton();
                break;
            case "Exit":
                HandleExitButton();
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

    private IEnumerator StartTitleScreen()
    {
        TransitionOut();
        yield return new WaitForSeconds(universalTransitionTime);

        SceneManager.Instance.LoadMainUI();

        TransitionIn();
        isAnimating = false;
        yield return new WaitForSeconds(universalTransitionTime);
    }

    private IEnumerator PlayFriendButton()
    {
        TransitionOut();
        yield return new WaitForSeconds(universalTransitionTime);

        SceneManager.Instance.LoadChess2dVSFriend();
        isAnimating = false;
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
        isAnimating = false;
        yield return new WaitForSeconds(universalTransitionTime);
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

    private void ValidateFiles()
    {
        foreach(Transform child in contentGameObject.transform)
        {
            Destroy(child.gameObject);
        }

        string folderPath = FileManager.Instance.pathToGamesDirectory;

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string[] files = Directory.GetFiles(folderPath);
        Array.Sort(files);

        //Data
        List<string> allGames = new List<string>();
        allGames.AddRange(files);

        //Remove all files that dont end in .json
        allGames.RemoveAll(file => !file.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase));

        List<(FileManager.GameRecord, string)> data = new List<(FileManager.GameRecord, string)>();

        //Remove all invalid files
        foreach (string file in allGames.ToList())
        {
            if (!File.Exists(file))
                allGames.Remove(file);

            try
            {
                string jsonData = File.ReadAllText(file);

                var settings = new JsonSerializerSettings
                {
                    DateFormatString = "dd/MM/yyyy",
                    Converters = { new StringEnumConverter() }
                };

                FileManager.GameRecord record = JsonConvert.DeserializeObject<FileManager.GameRecord>(jsonData, settings);

                if (record.MetaData.totalTime <= 0)
                {
                    Debug.LogError(file + " Total time can not be 0 or below");
                    allGames.Remove(file);
                    continue;
                }
                if (record.MetaData.totalMoves <= 0)
                {
                    Debug.LogError(file + " Total moves can not be 0 or below");
                    allGames.Remove(file);
                    continue;
                }
                if (record.MetaData.EndState == GameStateManager.End.None)
                {
                    Debug.LogError(file + " EndState can not be null");
                    allGames.Remove(file);
                    continue;
                }
                if (record.MetaData.totalMoves != record.History.Count)
                {
                    Debug.LogError(file + " Total Moves do not match");
                    allGames.Remove(file);
                    continue;
                }

                //json is valid
                data.Add((record, file));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load game record: {ex.Message}");
                allGames.Remove(file);
                continue;
            }
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
            foreach (var record in data)
            {
                GameObject game = Instantiate(gamePrefab, Vector3.zero, Quaternion.identity);
                game.transform.SetParent(contentGameObject.transform);
                game.transform.localScale = Vector3.one;

                //Open file and set child data
                string gameName = Path.GetFileNameWithoutExtension(record.Item2);

                TextMeshProUGUI gameNameData = game.transform.Find("Background/GameName").gameObject.GetComponent<TextMeshProUGUI>();
                if (gameName.Length > 50)
                    gameName = gameName.Substring(0, 50); //Limit the game name to 50 characters
                gameNameData.text = gameName;

                game.name = gameName;

                TextMeshProUGUI resultData = game.transform.Find("Background/GameData/GameResult").gameObject.GetComponent<TextMeshProUGUI>();
                string formatedResult;
                switch (record.Item1.MetaData.EndState)
                {
                    case GameStateManager.End.BlackWon:
                        formatedResult = "Black Won!!!";
                        break;
                    case GameStateManager.End.WhiteWon:
                        formatedResult = "White Won!!!";
                        break;
                    case GameStateManager.End.Draw:
                        formatedResult = "Draw!!!";
                        break;
                    default:
                        formatedResult = "null";
                        break;
                }
                resultData.text = "Result: " + formatedResult;

                TextMeshProUGUI gameTimeData = game.transform.Find("Background/GameData/GameTime").gameObject.GetComponent<TextMeshProUGUI>();
                float seconds = record.Item1.MetaData.totalTime;
                TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

                string formatedTime;

                if (timeSpan.Hours > 0)
                    formatedTime = $"{timeSpan.Hours}h {timeSpan.Minutes}min {timeSpan.Seconds}sec";
                else if (timeSpan.Minutes > 0)
                    formatedTime = $"{timeSpan.Minutes}min {timeSpan.Seconds}sec";
                else
                    formatedTime = $"{timeSpan.Seconds}sec";

                gameTimeData.text = "Time: " + formatedTime;

                TextMeshProUGUI totalMovesData = game.transform.Find("Background/GameData/TotalMoves").gameObject.GetComponent<TextMeshProUGUI>();
                totalMovesData.text = "Total Moves: " + record.Item1.MetaData.totalMoves.ToString();

                TextMeshProUGUI dateData = game.transform.Find("Background/GameData/DateOfPlaying").gameObject.GetComponent<TextMeshProUGUI>();
                dateData.text = "Date: " + record.Item1.MetaData.date.ToString("dd/MM/yyyy");
            }
        }

    }

    private void HandleGamesButton()
    {
        if (transition == null) return;
        if (isAnimating) return;

        StartCoroutine(StartGamesMenu());

        ValidateFiles();
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

    private void HandlePlayBotButton()
    {

    }

    private void HandleDeleteButton()
    {
        //Get parent of button, parent of button is same as file name, delete parent + delete file
    }

    private void HandleEditorButton()
    {

    }

    private void HandleExitButton()
    {
        if (transition == null) return;
        if (isAnimating) return;

        StartCoroutine(StartTitleScreen());
    }


}

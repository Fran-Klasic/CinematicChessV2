using System.IO;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    //All scenes in a const string
    public enum Scenes { None, MainUI, Chess2dVSFriend, AnimationCore, Theme_Generic }

    public const string mainUI = "MainUI";
    public const string chess2dVSFriend = "Chess2dVSFriend";
    public const string animationCore = "AnimationCore";
    public const string theme_Generic = "Theme_Generic";

    public void LoadScene(Scenes scene)
    {
        string sceneToLoad = GetSceneString(scene);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    public void LoadAdditiveScene(Scenes scene)
    {
        string sceneToLoad = GetSceneString(scene);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
    }

    public void LoadMainUI()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainUI, LoadSceneMode.Single);
    }

    public void LoadChess2dVSFriend()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(chess2dVSFriend, LoadSceneMode.Single);
    }

    public void LoadAnimationCore()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(animationCore, LoadSceneMode.Single);
    }

    public void LoadMenu(string currentScene)
    {
        //Overlay Menu over current scene
    }

    public void OpenEditor(FileManager.GameRecord record)
    {
        StartCoroutine(LoadAndWait(record));
    }

    private IEnumerator LoadAndWait(FileManager.GameRecord record)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainUI", LoadSceneMode.Single);

        while (!asyncLoad.isDone)
            yield return null;

        var canvas = GameObject.Find("Canvas");

        Transform[] allChildren = canvas.transform.GetComponentsInChildren<Transform>(true);

        bool found1 = false;
        bool found2 = false;
        GameObject editor_Group = null;

        foreach (Transform child in allChildren)
        {
            if (found1 && found2) break;

            if (child.gameObject.name == "TitleScreen_Group")
            {
                child.gameObject.SetActive(false);
                found1 = true;
            }
            if (child.gameObject.name == "Editor_Group")
            {
                child.gameObject.SetActive(true);
                found2 = true;
                editor_Group = child.gameObject;
            }
        }

        ReplayManager.Instance.record = record;
        InitialzeEditor(record, editor_Group);

    }

    public void InitialzeEditor(FileManager.GameRecord record, GameObject editor_Group)
    {
        #region Name Parsing
        TextMeshProUGUI gameTitle = editor_Group.transform.Find("Background/GameTitle/Text Area/Placeholder").gameObject.GetComponent<TextMeshProUGUI>();
        gameTitle.text = Path.GetFileNameWithoutExtension(record.Metadata.GameFileName);
        #endregion

        #region Result Parsing
        TextMeshProUGUI result = editor_Group.transform.Find("Background/GameData/Result").gameObject.GetComponent<TextMeshProUGUI>();
        string formatedResult;
        switch (record.Metadata.EndState)
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
        result.text = "Result: " + formatedResult;
        #endregion

        #region Time Parsing
        TextMeshProUGUI gameTimeData = editor_Group.transform.Find("Background/GameData/TotalTime").gameObject.GetComponent<TextMeshProUGUI>();
        float seconds = record.Metadata.TotalTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

        string formatedTime;

        if (timeSpan.Hours > 0)
            formatedTime = $"{timeSpan.Hours}h {timeSpan.Minutes}min {timeSpan.Seconds}sec";
        else if (timeSpan.Minutes > 0)
            formatedTime = $"{timeSpan.Minutes}min {timeSpan.Seconds}sec";
        else
            formatedTime = $"{timeSpan.Seconds}sec";

        gameTimeData.text = "Time: " + formatedTime;
        #endregion

        #region Moves Parsing
        TextMeshProUGUI totalMovesData = editor_Group.transform.Find("Background/GameData/TotalMoves").gameObject.GetComponent<TextMeshProUGUI>();
        totalMovesData.text = "Total Moves: " + record.Metadata.TotalMoves.ToString();
        #endregion

        #region Date Parsing
        TextMeshProUGUI dateData = editor_Group.transform.Find("Background/GameData/Date").gameObject.GetComponent<TextMeshProUGUI>();
        dateData.text = "Date: " + record.Metadata.Date.ToString("dd/MM/yyyy");
        #endregion
    }

    private FileManager.GameRecord record = null;

    public void SaveGameData(FileManager.GameRecord record)
    {
        //Gets the record from a game and saves it
        Debug.Log("Saved Game Data!!!");
        this.record = record;
    }

    public void SaveAnimationData(AnimationSettingsUI_Core.AnimationData animationData)
    {
        //Gets the record from here and the animation data and puts it into a json in the Animations dir
        var animationRecord = new FileManager.AnimationRecord(record, animationData);
        FileManager.Instance.SaveAnimation(animationRecord);

        Debug.Log("Saved all animation data!!!");
    }

    public void DiscardAllData()
    {
        Debug.Log("Discarded Game Data!!!");
        record = null;
    }

    public Scenes GetScene(string sceneName)
    {
        switch (sceneName)
        {
            case chess2dVSFriend:
                return Scenes.Chess2dVSFriend;
            case mainUI:
                return Scenes.MainUI;
            case animationCore:
                return Scenes.AnimationCore;
            case theme_Generic:
                return Scenes.Theme_Generic;
        }

        return Scenes.None;
    }

    private string GetSceneString(Scenes scene)
    {
        switch (scene)
        {
            case Scenes.Chess2dVSFriend:
                return chess2dVSFriend;
            case Scenes.MainUI:
                return mainUI;
            case Scenes.AnimationCore:
                return animationCore;
            case Scenes.Theme_Generic:
                return theme_Generic;
        }

        return string.Empty;
    }

    public static SceneManager Instance;
    //SceneManager.Instance.--- -> Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Will persist always
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    [HideInInspector] public bool isPlayMenuActive = false;
    [HideInInspector] public bool isAnimating = false; //value to check if an animation is playing before it plays a different animation
    [HideInInspector] public float universalTransitionTime = 1f; //animation length for every transition

    #region Helper Functions
    public (List<string>, List<(FileManager.GameRecord, string)>) ValidateFiles()
    {
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

                #region Data Check
                if (record.MetaData.totalTime <= 0)
                {
                    Debug.LogError(file + " Total time can not be 0 or below");
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
                #endregion

                //The file was validated and saved to data list
                //Data list contains the json data and the path to file
                data.Add((record, file));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load game record: {ex.Message}");
                allGames.Remove(file);
                continue;
            }
        }

        return (allGames, data);
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
            }
        }

        ReplayManager.Instance.record = record;
    }

    #endregion

    //PUT IN ALL BUTTON SCRIPTS
    /*
    //Animator will try to change state into selected
    //Unselect button from the animator
    EventSystem.current.SetSelectedGameObject(null);
    */


    bool temp;
    private void Update()
    {
        if(isAnimating != temp)
        {
            temp = isAnimating;
            Debug.Log($"isAnimating value: {isAnimating}");
        }
    }


    private void Start()
    {
        temp = isAnimating;

        //Helps with stuttering on first time validating
        ValidateFiles();
    }


    public static MainController Instance;
    //MainController.Instance.--- -> Singleton
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

    //   TODO:
    //Button:
    //Animations
    //Favorites
    //Options
    //Credits
    //PlayVSBot
    //EditorExit
    //GamesDelete
    //EditorDelete
    //EditorRandomize
    //EditorMoveToAnimationSettings
    //Chess2dExit
}
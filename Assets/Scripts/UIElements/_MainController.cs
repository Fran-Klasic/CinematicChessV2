
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;

public class MainController : MonoBehaviour
{
    [HideInInspector] public bool isPlayMenuActive = false;
    [HideInInspector] public bool isAnimating = false; //value to check if an animation is playing before it plays a different animation
    [HideInInspector] public float universalTransitionTime = 1f; //animation length for every transition
    [HideInInspector] public bool hasCanceled = false;
    [HideInInspector] public bool hasDeleted = false;


    #region Helper Functions
    public (List<string>, List<(FileManager.GameRecord, string)>) ValidateGameFiles()
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
                if (record.Metadata.TotalTime <= 0)
                {
                    Debug.LogError(file + " Total time can not be 0 or below");
                    allGames.Remove(file);
                    continue;
                }
                if (record.Metadata.EndState == GameStateManager.End.None)
                {
                    Debug.LogError(file + " EndState can not be null");
                    allGames.Remove(file);
                    continue;
                }
                if (record.Metadata.TotalMoves != record.History.Count)
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

    public (List<string>, List<(FileManager.AnimationRecord, string)>) ValidateAnimationsFiles()
    {
        string folderPath = FileManager.Instance.pathToAnimationsDirectory;

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string[] files = Directory.GetFiles(folderPath);
        Array.Sort(files);

        //Data
        List<string> allAnimations = new List<string>();
        allAnimations.AddRange(files);

        //Remove all files that dont end in .json
        allAnimations.RemoveAll(file => !file.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase));

        List<(FileManager.AnimationRecord, string)> data = new List<(FileManager.AnimationRecord, string)>();

        //Remove all invalid files
        foreach (string file in allAnimations.ToList())
        {
            if (!File.Exists(file))
                allAnimations.Remove(file);

            try
            {
                string jsonData = File.ReadAllText(file);

                var settings = new JsonSerializerSettings
                {
                    DateFormatString = "dd/MM/yyyy",
                    Converters = { new StringEnumConverter() }
                };

                FileManager.AnimationRecord record = JsonConvert.DeserializeObject<FileManager.AnimationRecord>(jsonData, settings);

                #region Animation Data Check
                if (record.AnimationData == null)
                {
                    Debug.LogError(file + " AnimationData is null");
                    allAnimations.Remove(file);
                    continue;
                }

                if (string.IsNullOrEmpty(record.AnimationData.Theme))
                {
                    Debug.LogError(file + " Theme is missing");
                    allAnimations.Remove(file);
                    continue;
                }

                if (string.IsNullOrEmpty(record.AnimationData.Pace))
                {
                    Debug.LogError(file + " Pace is missing");
                    allAnimations.Remove(file);
                    continue;
                }

                if (string.IsNullOrEmpty(record.AnimationData.MusicGenre))
                {
                    Debug.LogError(file + " Music genre is missing");
                    allAnimations.Remove(file);
                    continue;
                }
                #endregion

                //The file was validated and saved to data list
                //Data list contains the json data and the path to file
                data.Add((record, file));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load animation record: {ex.Message}");
                allAnimations.Remove(file);
                continue;
            }
        }

        return (allAnimations, data);
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
        if (isAnimating != temp)
        {
            temp = isAnimating;
            Debug.Log($"isAnimating value: {isAnimating}");
        }
    }


    private void Start()
    {
        temp = isAnimating;

        EventSystem.current.SetSelectedGameObject(null);

        //Helps with stuttering on first time validating
        ValidateGameFiles();
    }


    public static MainController Instance;
    //MainController.Instance.--- -> Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class MainUI_Games : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private GameObject titleScreen_Group;
    [SerializeField] private GameObject games_Group;
    [SerializeField] private GameObject contentGameObject;
    [SerializeField] private GameObject gamePrefab_None;
    [SerializeField] private GameObject gamePrefab;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(StartGamesMenu());

        SetGamesData(MainController.Instance.ValidateFiles());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void SetGamesData((List<string>, List<(FileManager.GameRecord, string)>) fileData)
    {
        var allGames = fileData.Item1;
        var data = fileData.Item2;

        foreach (Transform child in contentGameObject.transform)
        {
            Destroy(child.gameObject);
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

                #region Name Parsing
                string gameName = Path.GetFileNameWithoutExtension(record.Item2);

                TextMeshProUGUI gameNameData = game.transform.Find("Background/GameName").gameObject.GetComponent<TextMeshProUGUI>();
                if (gameName.Length > 50)
                    gameName = gameName.Substring(0, 50); //Limit the game name to 50 characters
                gameNameData.text = gameName;

                game.name = record.Item2;
                #endregion

                #region Result Parsing
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
                #endregion

                #region Time Parsing
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
                #endregion

                #region Moves Parsing
                TextMeshProUGUI totalMovesData = game.transform.Find("Background/GameData/TotalMoves").gameObject.GetComponent<TextMeshProUGUI>();
                totalMovesData.text = "Total Moves: " + record.Item1.MetaData.totalMoves.ToString();
                #endregion

                #region Date Parsing
                TextMeshProUGUI dateData = game.transform.Find("Background/GameData/DateOfPlaying").gameObject.GetComponent<TextMeshProUGUI>();
                dateData.text = "Date: " + record.Item1.MetaData.date.ToString("dd/MM/yyyy");
                #endregion
            }
        }
    }


    private IEnumerator StartGamesMenu()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        titleScreen_Group.SetActive(false);
        games_Group.SetActive(true);

        transition.SetTrigger("In");
        MainController.Instance.isAnimating = false;

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
    }
}
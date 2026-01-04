using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class MainUI_Animations : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private GameObject titleScreen_Group;
    [SerializeField] private GameObject animations_Group;
    [SerializeField] private GameObject contentGameObject;
    [SerializeField] private GameObject animationPrefab_None;
    [SerializeField] private GameObject animationPrefab;

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        StartCoroutine(StartAnimationsMenu());

        SetAnimationsData(MainController.Instance.ValidateAnimationsFiles());

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }

  
    private void SetAnimationsData((List<string>, List<(FileManager.AnimationRecord, string)>) fileData)
    {
        var allGames = fileData.Item1;
        var data = fileData.Item2;

        foreach (Transform child in contentGameObject.transform)
        {
            Destroy(child.gameObject);
        }

        if (allGames.Count == 0)
        {
            GameObject newObj = Instantiate(animationPrefab_None, Vector3.zero, Quaternion.identity);
            newObj.transform.SetParent(contentGameObject.transform);
            newObj.transform.localScale = Vector3.one;
        }
        else
        {
            //Add gamePrefabs based on file data
            foreach (var record in data)
            {
                GameObject game = Instantiate(animationPrefab, Vector3.zero, Quaternion.identity);
                game.transform.SetParent(contentGameObject.transform);
                game.transform.localScale = Vector3.one;

                #region Name Parsing
                string gameName = Path.GetFileNameWithoutExtension(record.Item2);

                TextMeshProUGUI gameNameData = game.transform.Find("Background/AnimationName").gameObject.GetComponent<TextMeshProUGUI>();
                if (gameName.Length > 50)
                    gameName = gameName.Substring(0, 50);//Limit the game name to 50 characters
                gameNameData.text = gameName;

                game.name = record.Item2;
                #endregion

                #region Theme Parsing
                TextMeshProUGUI themeData = game.transform.Find("Background/AnimationData/AnimationTheme").gameObject.GetComponent<TextMeshProUGUI>();
                themeData.text += record.Item1.AnimationData.Theme;
                #endregion

                #region Pace Parsing
                TextMeshProUGUI paceData = game.transform.Find("Background/AnimationData/AnimationPace").gameObject.GetComponent<TextMeshProUGUI>();
                paceData.text += record.Item1.AnimationData.Pace;
                #endregion

                #region Genre Parsing
                TextMeshProUGUI genreData = game.transform.Find("Background/AnimationData/AnimationGenre").gameObject.GetComponent<TextMeshProUGUI>();
                genreData.text += record.Item1.AnimationData.MusicGenre;
                #endregion

                #region Orientation Parsing
                TextMeshProUGUI orientationData = game.transform.Find("Background/AnimationData/AnimationOrientation").gameObject.GetComponent<TextMeshProUGUI>();
                orientationData.text += record.Item1.AnimationData.Orientation;
                #endregion

                #region Hud Parsing
                TextMeshProUGUI hudData = game.transform.Find("Background/AnimationData/AnimationHud").gameObject.GetComponent<TextMeshProUGUI>();
                hudData.text += record.Item1.AnimationData.EnableHud;
                #endregion

                #region Player1 Parsing
                TextMeshProUGUI player1Data = game.transform.Find("Background/AnimationData/AnimationPlayer1").gameObject.GetComponent<TextMeshProUGUI>();
                player1Data.text += record.Item1.AnimationData.Player1;
                #endregion

                #region Player2 Parsing
                TextMeshProUGUI player2Data = game.transform.Find("Background/AnimationData/AnimationPlayer2").gameObject.GetComponent<TextMeshProUGUI>();
                player2Data.text += record.Item1.AnimationData.Player2;
                #endregion
            }
        }
    }


    private IEnumerator StartAnimationsMenu()
    {
        MainController.Instance.isAnimating = true;
        transition.SetTrigger("Out");

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);

        titleScreen_Group.SetActive(false);
        animations_Group.SetActive(true);

        transition.SetTrigger("In");
        MainController.Instance.isAnimating = false;

        yield return new WaitForSeconds(MainController.Instance.universalTransitionTime);
    }
}
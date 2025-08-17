using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public class EditorUI_InputField : MonoBehaviour
{
    public void OnValueChanged(string newValue)
    {
        //Make sure new file name is safe
        string sanitized = Regex.Replace(newValue, @"[^a-zA-Z0-9 _-]", "");
        newValue = sanitized.Trim();

        string oldFileName = ReplayManager.Instance.record.MetaData.gameFileName;
        string pathToOldFile = Path.Combine(FileManager.Instance.pathToGamesDirectory, oldFileName);

        string newFileName = newValue + ".json";
        string pathToNewFile = Path.Combine(FileManager.Instance.pathToGamesDirectory, newFileName);

        if (string.IsNullOrWhiteSpace(newValue))
        {
            Debug.Log("Invalid filename, no changes have been made!!! " + newValue);
            return;
        }

        try
        {
            int counter = 1;
            while (File.Exists(pathToNewFile))
            {
                newFileName = newValue + $"({counter})" + ".json";
                pathToNewFile = Path.Combine(FileManager.Instance.pathToGamesDirectory, newFileName);
                counter++;
            }
            if (File.Exists(pathToOldFile))
            {
                File.Move(pathToOldFile, pathToNewFile);
                Debug.Log($"Renamed {pathToOldFile} -> {pathToNewFile}");
            }
            else
            {
                Debug.LogWarning("Old file does not exist: " + pathToOldFile);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error with file renameing!!! " + ex);
        }

        //Replace MetaData file name with new file name
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        };

        ReplayManager.Instance.record.MetaData.gameFileName = newFileName;
        string gameData = JsonConvert.SerializeObject(ReplayManager.Instance.record, settings);

        try
        {
            File.WriteAllText(pathToNewFile, gameData);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error with overwriting file with new data!!! " + ex);
        }
    }
}
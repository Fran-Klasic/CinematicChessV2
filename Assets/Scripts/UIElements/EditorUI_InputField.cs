using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using TMPro;

public class EditorUI_InputField : MonoBehaviour
{
    public void OnValueChanged(string newValue)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        };

        //Make sure new file name is safe
        string sanitized = SanitizeFileName(newValue);

        if (string.IsNullOrWhiteSpace(sanitized))
        {
            Debug.Log("Invalid filename, reverting to original.");

            // Restore original (from placeholder or current Metadata)
            string placeholderName = transform.Find("Text Area/Placeholder").GetComponent<TextMeshProUGUI>().text + ".json";
            ReplayManager.Instance.record.Metadata.GameFileName = placeholderName;
            return;
        }

        string oldFileName = ReplayManager.Instance.record.Metadata.GameFileName;
        string pathToOldFile = Path.Combine(FileManager.Instance.pathToGamesDirectory, oldFileName);

        string newFileName = Path.ChangeExtension(sanitized, ".json");
        string pathToNewFile = GetUniqueFilePath(newFileName);

        try
        {
            // Rename file if old exists
            if (File.Exists(pathToOldFile))
            {
                File.Move(pathToOldFile, pathToNewFile);
                Debug.Log($"Renamed {oldFileName} -> {Path.GetFileName(pathToNewFile)}");
            }
            else
            {
                Debug.LogWarning("Old file does not exist: " + pathToOldFile);
            }

            // Update metadata and save JSON
            ReplayManager.Instance.record.Metadata.GameFileName = Path.GetFileName(pathToNewFile);
            string gameData = JsonConvert.SerializeObject(ReplayManager.Instance.record, settings);
            File.WriteAllText(pathToNewFile, gameData);
        }
        catch (Exception ex)
        {
            Debug.LogError("File rename/save error: " + ex);
        }
    }
    private string SanitizeFileName(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        string sanitized = Regex.Replace(input, @"[^a-zA-Z0-9 _-]", "");
        return sanitized.Trim();
    }

    private string GetUniqueFilePath(string desiredFileName)
    {
        string directory = FileManager.Instance.pathToGamesDirectory;
        string path = Path.Combine(directory, desiredFileName);

        string fileNameNoExt = Path.GetFileNameWithoutExtension(desiredFileName);
        string extension = Path.GetExtension(desiredFileName);

        int counter = 1;
        while (File.Exists(path))
        {
            string newFileName = $"{fileNameNoExt}({counter}){extension}";
            path = Path.Combine(directory, newFileName);
            counter++;
        }

        return path;
    }
}
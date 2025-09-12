using Newtonsoft.Json;
using UnityEngine;

public enum Genre
{
    Classic,
    Country,
    Electronic,
    Epic,
    Futuristic
}
public class TimestampData
{
    public float[] hits { get; set; }
}

[CreateAssetMenu(fileName = "SongData", menuName = "CinematicMusic/Song Data")]
//Used for storing song, the timespampts for the song and the genre
//Will also be addressable with genre as label
public class SongData : ScriptableObject
{
    [Header("Song Assets")]
    public AudioClip audioClipMP3;
    public TextAsset timestampsJSON;

    [Header("Metadata")]
    public Genre genre;

    public float[] GetTimestamps()
    {
        if (timestampsJSON == null || string.IsNullOrWhiteSpace(timestampsJSON.text))
        {
            Debug.LogWarning($"No JSON found for {name}");
            return new float[0];
        }

        try
        {
            TimestampData data = JsonConvert.DeserializeObject<TimestampData>(timestampsJSON.text);
            return data?.hits ?? new float[0];
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse JSON for {name}: {ex.Message}");
            return new float[0];
        }
    }
}

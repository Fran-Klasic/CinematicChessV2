using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AudioManager : MonoBehaviour
{


    public static async Task<SongData> LoadRandomSongAsync(string genreLabel)
    {
        // Load all SongData assets with the label genreLabel
        AsyncOperationHandle<IList<SongData>> handle = Addressables.LoadAssetsAsync<SongData>(genreLabel, null);

        await handle.Task; // Wait for async loading to finish

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            IList<SongData> songs = handle.Result;

            if (songs.Count == 0)
            {
                Debug.LogWarning($"No songs found with genre: {genreLabel}");
                return null;
            }

            // Pick a random song
            SongData chosen = songs[Random.Range(0, songs.Count)];
            Debug.Log($"Chosen random song: {chosen.name}");
            return chosen;
        }
        else
        {
            Debug.LogError($"Failed to load songs for genre: {genreLabel}");
            return null;
        }
    }
}

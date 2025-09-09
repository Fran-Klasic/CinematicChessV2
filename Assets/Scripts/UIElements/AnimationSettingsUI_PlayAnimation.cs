using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class AnimationSettingsUI_PlayAnimation : MonoBehaviour
{
    [SerializeField] Animator transition;

    //private async Task<List<TextAsset>> LoadHitsByGenre(string genreLabel)
    //{
    //    List<TextAsset> hits = new List<TextAsset>();

    //    await Addressables.LoadAssetsAsync<TextAsset>(
    //        new List<object> { "Song_Hits", genreLabel },
    //        file => hits.Add(file)
    //    ).Task;

    //    foreach (var hit in hits)
    //    {
    //        Debug.Log(hit.name);
    //    }

    //    return hits;
    //}

    public void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        //string genreLabel = AnimationSettingsUI_Core.musicGenre + "_Genre";

        //// Wait for hits JSON to load
        //List<TextAsset> hits = await LoadHitsByGenre(genreLabel);

        //if (hits.Count == 0)
        //{
        //    Debug.LogWarning("No hits JSON found for genre: " + AnimationSettingsUI_Core.musicGenre);
        //    return;
        //}
        //else
        //{
        //    Debug.Log($"Found {hits.Count} hits JSONs for genre: " + AnimationSettingsUI_Core.musicGenre);
        //}



        AnimationSettingsUI_Core.SaveAllData();

        //Load AnimationScene

        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }
}
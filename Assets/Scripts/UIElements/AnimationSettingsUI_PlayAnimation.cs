using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class AnimationSettingsUI_PlayAnimation : MonoBehaviour
{
    [SerializeField] Animator transition;

    public async void OnClick()
    {
        if (transition == null) return;
        if (MainController.Instance.isAnimating) return;

        string genreLabel = AnimationSettingsUI_Core.musicGenre;

        SongData song = await AudioManager.LoadRandomSongAsync(genreLabel);
        string name = song.name;

        AnimationSettingsUI_Core.SaveSong(name);

        AnimationSettingsUI_Core.SaveAllData();


        Debug.Log("Starting Animation...");
        //Load AnimationScene




        //Animator will try to change state into selected
        //Unselect button from the animator
        EventSystem.current.SetSelectedGameObject(null);
    }
}
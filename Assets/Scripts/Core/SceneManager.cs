using UnityEngine;

public class SceneManager : MonoBehaviour
{
    //All scenes in a const string
    public enum Scenes {None, MainUI ,Chess2d, AnimationEditor, Animation3d}

    public const string mainUI = "MainUI";
    public const string chess2d = "Chess2d";



    //Functions that handle scene switching
    public void LoadMainUI()
    {

    }

    public void LoadChess2d()
    {

    }

    public void LoadMenu(string currentScene)
    {
        //Overlay Menu over current scene
    }

    public void UnloadMenu()
    {

    }

    public Scenes GetScene(string sceneName)
    {
        switch (sceneName)
        {
            case chess2d:
                return Scenes.Chess2d;
            case mainUI:
                return Scenes.MainUI;
        }

        return Scenes.None;
    }


    public static SceneManager Instance;
    //SceneManager.Instance.--- -> Singleton
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
}

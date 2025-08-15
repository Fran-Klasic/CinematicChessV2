using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    //All scenes in a const string
    public enum Scenes { None, MainUI, Chess2dVSFriend, AnimationEditor, Animation3d }

    public const string mainUI = "MainUI";
    public const string chess2dVSFriend = "Chess2dVSFriend";

    //Functions that handle scene switching

    public void LoadScene(Scenes scene)
    {
        string sceneToLoad = GetSceneString(scene);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
    public void LoadMainUI()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainUI, LoadSceneMode.Single);
    }

    public void LoadChess2dVSFriend()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(chess2dVSFriend, LoadSceneMode.Single);
    }

    public void LoadMenu(string currentScene)
    {
        //Overlay Menu over current scene
    }

    public void ResetGame()
    {

    }

    public Scenes GetScene(string sceneName)
    {
        switch (sceneName)
        {
            case chess2dVSFriend:
                return Scenes.Chess2dVSFriend;
            case mainUI:
                return Scenes.MainUI;
        }

        return Scenes.None;
    }

    private string GetSceneString (Scenes scene)
    {
        switch (scene)
        {
            case Scenes.Chess2dVSFriend:
                return chess2dVSFriend;
            case Scenes.MainUI:
                return mainUI;
        }

        return string.Empty;
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

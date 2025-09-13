using UnityEngine;

public class PersistantAnimationData : MonoBehaviour
{
    //Holds all data for animation
    [HideInInspector] public FileManager.AnimationRecord AnimationRecord;
    [HideInInspector] public SceneManager.Scenes ActiveScene;

    public void ResetData()
    {
        AnimationRecord = null;
        ActiveScene = SceneManager.Scenes.None;
    }

    public static PersistantAnimationData Instance;
    //PersistantAnimationData.Instance.--- -> Singleton
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
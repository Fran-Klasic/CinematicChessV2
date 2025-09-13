using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PersistantAnimationData.Instance != null &&
            scene.name == PersistantAnimationData.Instance.ActiveScene.ToString())
        {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
            Debug.Log($"Scene {scene.name} is now active.");
        }
    }
}

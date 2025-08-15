#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RemoveMissingScripts
{
    [MenuItem("Tools/Remove All Missing Scripts In Scene")]
    static void RemoveMissingScriptsInScene()
    {
        int totalRemoved = 0;

        // Get all root objects in the active scene
        var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var root in roots)
        {
            totalRemoved += RemoveMissingScriptsRecursive(root);
        }

        Debug.Log($"Removed {totalRemoved} missing scripts from the scene.");
    }

    static int RemoveMissingScriptsRecursive(GameObject go)
    {
        // Remove missing scripts on this GameObject
        int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

        // Recurse through children
        foreach (Transform child in go.transform)
        {
            removed += RemoveMissingScriptsRecursive(child.gameObject);
        }

        return removed;
    }
}
#endif

using UnityEngine;
using System.Diagnostics;
using static GameStateManager;

public class StopwatchManager : MonoBehaviour
{
    //Tracks time
    private Stopwatch stopwatch;

    public void StartStopwatch()
    {
        stopwatch = Stopwatch.StartNew();
    }

    public float GetTime()
    {
        if (stopwatch != null && stopwatch.IsRunning)
        {
            return (float)stopwatch.Elapsed.TotalSeconds;
        }
        return 0f;
    }


    public static StopwatchManager Instance;
    //StopwatchManager.Instance.--- -> Singleton
    private void Awake()
    {
        Application.runInBackground = true;

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

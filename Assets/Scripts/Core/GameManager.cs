using UnityEngine;

public class GameManager : MonoBehaviour
{
    public readonly int rows = 8;
    public readonly int cols = 8;

    public readonly float tileSize = (200f / 128f) * 2f;
    [HideInInspector] public float boardSize;
    [HideInInspector] public float center;


    //Attach script in inspector
    public PiecesGenerator piecesGenerator;
    public BoardGenerator boardGenerator;


    public static GameManager Instance;
    //GameManager.Instance.--- -> Singleton
    private void Awake()
    {
        boardSize = rows * tileSize;
        center = boardSize / 2 - tileSize / 2;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        boardGenerator.Generate();
        piecesGenerator.Generate();
    }
}

using UnityEngine;
using System.Collections.Generic;

public class PieceDataMemory : MonoBehaviour
{
    [HideInInspector] public List<PieceData> data;

    [SerializeField] public GameObject whitePiecesGroup;
    [SerializeField] public GameObject blackPiecesGroup;

    public void UpdatePieceDataMemory()
    {
        data.Clear();

        foreach (Transform child in whitePiecesGroup.transform)
        {
            PieceData piece = child.gameObject.GetComponent<PieceData>();
            if (piece != null)
            {
                data.Add(piece);
            }
        }

        foreach (Transform child in blackPiecesGroup.transform)
        {
            PieceData piece = child.gameObject.GetComponent<PieceData>();
            if (piece != null)
            {
                data.Add(piece);
            }
        }
    }

    private void Start()
    {
        UpdatePieceDataMemory();
    }

    public static PieceDataMemory Instance;
    //PieceDataMemory.Instance.--- -> Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
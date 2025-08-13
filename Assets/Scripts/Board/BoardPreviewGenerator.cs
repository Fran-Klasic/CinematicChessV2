using UnityEngine;
using System.Collections.Generic;

public class BoardPreviewGenerator : MonoBehaviour
{
    public static BoardPreviewGenerator Instance;
    //BoardPreviewGenerator.Instance.--- -> Singleton
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
    [Header("Tile prefabs: ")]
    [SerializeField] private GameObject tileWhitePrefab;
    [SerializeField] private GameObject tileBlackPrefab;


    private float width;
    private float height;
    private float tileWidth;
    private float tileHeight;
    private float posX;
    private float posY;

    [HideInInspector] public Dictionary<Vector2Int, Vector2> tilePositions = new Dictionary<Vector2Int, Vector2>();

    public void GenerateBoard()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
        tileWidth = width / 8f;
        tileHeight = height / 8f;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject tileToSpawn = (x + y) % 2 == 0 ? tileBlackPrefab : tileWhitePrefab;
                GameObject tile = Instantiate(tileToSpawn, rectTransform);

                RectTransform tileRect = tile.GetComponent<RectTransform>();
                tileRect.sizeDelta = new Vector2(tileWidth, tileHeight);

                posX = tileWidth * x + tileWidth / 2f - width / 2f;
                posY = tileHeight * y + tileHeight / 2f - height / 2f;
                tile.transform.localPosition = new Vector2(posX, posY);

                tilePositions[new Vector2Int(x, y)] = tile.transform.localPosition;
            }
        }
    }
}
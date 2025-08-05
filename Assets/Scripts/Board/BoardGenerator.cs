using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tileWhitePrefab;
    [SerializeField] private GameObject tileBlackPrefab;

    public void Generate()
    {
        float center = GameManager.Instance.center;
        float tileSize = GameManager.Instance.tileSize;
        int rows = GameManager.Instance.rows;
        int cols = GameManager.Instance.cols;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Vector3 position = new Vector3((x * tileSize) - center, (y * tileSize) - center, 2);
                GameObject tileToSpawn = (x + y) % 2 == 0 ? tileBlackPrefab : tileWhitePrefab;
                GameObject tile = Instantiate(tileToSpawn, position, tileToSpawn.transform.rotation, transform);
                Tile tileComponent = tile.GetComponent<Tile>();
                tileComponent.BoardPosition = new Vector2Int(x, y);

                BoardManager.Instance.BoardState[x, y] = null;

                string color = (x + y) % 2 == 0 ? "Black" : "White";
                tile.name = $"{x},{y},{color}";
            }
        }
    }
}

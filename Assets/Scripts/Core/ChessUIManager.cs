using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChessUIManager : MonoBehaviour
{
    [SerializeField] private GameObject selectPrefab;
    [SerializeField] private GameObject legalMovePrefab;

    private GameObject currentSelectIndicator;
    private List<GameObject> currentLegalMoveIndicators = new List<GameObject>();

    private float center => GameManager.Instance.center;
    private float tileSize => GameManager.Instance.tileSize;

    public void PlaceSelect(Vector2Int position)
    {
        ClearSelection();

        Vector3 worldPosition = new Vector3((position.x * tileSize) - center, (position.y * tileSize) - center, 1);
        currentSelectIndicator = Instantiate(selectPrefab, worldPosition, Quaternion.identity);
    }

    public void PlaceLegalMoves(List<Vector2Int> positions)
    {
        ClearLegalMoves();

        foreach (var pos in positions)
        {
            Vector3 worldPosition = new Vector3((pos.x * tileSize) - center, (pos.y * tileSize) - center, 1);
            var indicator = Instantiate(legalMovePrefab, worldPosition, Quaternion.identity);
            currentLegalMoveIndicators.Add(indicator);
        }
    }

    public void PlaceSpecialMoves(List<Vector2Int> positions)
    {
        if (positions == null || positions.Count == 0)
        {
            return;
        }
        foreach (var pos in positions)
        {
            Vector3 worldPosition = new Vector3((pos.x * tileSize) - center, (pos.y * tileSize) - center, 1);
            var indicator = Instantiate(legalMovePrefab, worldPosition, Quaternion.identity);
            currentLegalMoveIndicators.Add(indicator);
        }
    }

    public void ClearSelection()
    {
        if (currentSelectIndicator != null)
        {
            Destroy(currentSelectIndicator);
            currentSelectIndicator = null;
        }
    }

    public void ClearLegalMoves()
    {
        foreach (var indicator in currentLegalMoveIndicators)
        {
            Destroy(indicator);
        }
        foreach (var piece in BoardManager.Instance.BoardState)
        {
            if (piece == null)
                continue;
            piece.IsCapturable = false;
        }
        currentLegalMoveIndicators.Clear();
    }

    public void ClearAll()
    {
        ClearSelection();
        ClearLegalMoves();
    }

    private void DisplayCurrentTurnPlayer()
    {
        TextMeshProUGUI textObject = gameObject.GetComponent<TextMeshProUGUI>();
        if (textObject != null)
        {
            var currentTurn = GameStateManager.Instance.CurrentTurn;
            string text = string.Empty;

            //currentTurn can be None
            if (currentTurn == ChessPieceBase.PieceColor.White)
                text = "White's Turn!";
            if (currentTurn == ChessPieceBase.PieceColor.Black)
                text = "Black's Turn!";
            textObject.text = text;
        }
    }

    private void DisplayCurrentPointValue()
    {
        TextMeshProUGUI textObject = gameObject.GetComponent<TextMeshProUGUI>();
        if (textObject.name.Contains("White"))
        {
            string text = $"White: +{BoardManager.Instance.WhitePoints}";
            textObject.text = text;
        }
        else if (textObject.name.Contains("Black"))
        {
            string text = $"Black: +{BoardManager.Instance.BlackPoints}";
            textObject.text = text;
        }
    }
    private void Update()
    {
        if (gameObject.name == "PlayerTurnData")
            DisplayCurrentTurnPlayer();
        if (gameObject.name.Contains("PointsText"))
            DisplayCurrentPointValue();
    }
}
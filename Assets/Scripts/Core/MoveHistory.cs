using UnityEngine;
using static ChessPieceBase;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


public class MoveHistory : MonoBehaviour
{
    public enum SpecialMove { None, EnPassant, Castle, Promotion }

    public struct ChessPosition
    {
        public int x { get; set; }
        public int y { get; set; }

        public ChessPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class ChessMove
    {
        public ChessPosition from { get; set; }
        public ChessPosition to { get; set; }
        public PieceType pieceType { get; set; }
        public PieceColor pieceColor { get; set; }
        public SpecialMove specialMove { get; set; }
        public PieceType promotedInto { get; set; }
        public float time { get; set; }
        public bool hasCaptured { get; set; }
    }

    [HideInInspector] public List<ChessMove> History = new List<ChessMove>();

    public void LogMove(Vector2Int from, Vector2Int to, PieceType type, PieceColor color, SpecialMove specialMove, PieceType promotedInto, float time, bool hasCaptured)
    {
        var move = new ChessMove
        {
            from = new ChessPosition(from.x, from.y),
            to = new ChessPosition(to.x, to.y),
            pieceType = type,
            pieceColor = color,
            specialMove = specialMove,
            promotedInto = promotedInto,
            time = time,
            hasCaptured = hasCaptured
        };

        History.Add(move);
    }



    public void HistoryToJSON()
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        };

        string gameHistoryJSON = JsonConvert.SerializeObject(History, settings);

        string path = Application.persistentDataPath;
        string pathToHistory = Path.Combine(path, "gameHistory.json");

        File.WriteAllText(pathToHistory, gameHistoryJSON);
    }

    public static MoveHistory Instance;
    //MoveHistory.Instance.--- -> Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

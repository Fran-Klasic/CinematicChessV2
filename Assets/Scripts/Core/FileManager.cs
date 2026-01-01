using UnityEngine;
using static ChessPieceBase;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;


public class FileManager : MonoBehaviour
{
    [HideInInspector] public string pathToTempDirectory;
    [HideInInspector] public string pathToGamesDirectory;
    [HideInInspector] public string pathToFavoriteGamesDirectory;
    [HideInInspector] public string pathToAnimationsDirectory;
    [HideInInspector] public string pathToPlayerDirectory;

    [HideInInspector] public static string AnimationJsonPathHolder;

    public enum SpecialMove { None, EnPassant, Castle, Promotion }

    public struct ChessPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public ChessPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class ChessMove
    {
        public ChessPosition From { get; set; }
        public ChessPosition To { get; set; }
        public PieceType PieceType { get; set; }
        public PieceColor PieceColor { get; set; }
        public SpecialMove SpecialMove { get; set; }
        public PieceType PromotedInto { get; set; }
        public float Time { get; set; }
        public bool HasCaptured { get; set; }
    }

    public class Metadata
    {
        public DateTime Date { get; set; }
        public float TotalTime { get; set; }
        public int TotalMoves { get; set; }
        public GameStateManager.End EndState { get; set; }

        public string GameFileName { get; set; }

        public Metadata(DateTime date, float totalTime, int totalMoves, GameStateManager.End endState, string gameFileName)
        {
            this.Date = date;
            this.TotalTime = totalTime;
            this.TotalMoves = totalMoves;
            EndState = endState;
            this.GameFileName = gameFileName;
        }
    }

    public class GameRecord
    {
        public List<ChessMove> History { get; set; }
        public Metadata Metadata { get; set; }

        public GameRecord(List<ChessMove> history, Metadata metadata)
        {
            History = history;
            Metadata = metadata;
        }
    }

    public class AnimationRecord
    {
        public GameRecord GameRecord { get; set; }
        public AnimationSettingsUI_Core.AnimationData AnimationData { get; set; }

        public AnimationRecord(GameRecord gameRecord, AnimationSettingsUI_Core.AnimationData animationData)
        {
            this.GameRecord = gameRecord;
            this.AnimationData = animationData;
        }
    }

    [HideInInspector] public List<ChessMove> History = new List<ChessMove>();

    public void LogMove(Vector2Int from, Vector2Int to, PieceType type, PieceColor color, SpecialMove specialMove, PieceType promotedInto, float time, bool hasCaptured)
    {
        var move = new ChessMove
        {
            From = new ChessPosition(from.x, from.y),
            To = new ChessPosition(to.x, to.y),
            PieceType = type,
            PieceColor = color,
            SpecialMove = specialMove,
            PromotedInto = promotedInto,
            Time = time,
            HasCaptured = hasCaptured
        };

        History.Add(move);

        UpdateTempGame();
    }

    public GameRecord SaveHistoryToGames(GameStateManager.End endState, float time)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        };

        int totalMoves = History.Count;

        GameRecord gameRecord = null;

        if (!string.IsNullOrEmpty(pathToGamesDirectory))
        {
            if (!Directory.Exists(pathToGamesDirectory))
            {
                Directory.CreateDirectory(pathToGamesDirectory);
            }

            int counter = 1;
            string path;
            string gameFileName = "Game.json";

            //Generate unique file
            while (true)
            {
                path = Path.Combine(pathToGamesDirectory, gameFileName);
                if (File.Exists(path))
                    gameFileName = $"Game({counter}).json";
                else
                    break;
                counter++;
            }

            Metadata metaData = new Metadata(DateTime.Now, time, totalMoves, endState, gameFileName);

            gameRecord = new GameRecord(History, metaData);
            string gameData = JsonConvert.SerializeObject(gameRecord, settings);

            try
            {
                File.WriteAllText(path, gameData);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to write game record JSON: " + ex.Message);
            }
        }

        return gameRecord;
    }

    public void SaveAnimation(AnimationRecord animationRecord)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        };

        if (!Directory.Exists(pathToAnimationsDirectory))
        {
            Directory.CreateDirectory(pathToAnimationsDirectory);
        }

        string baseFileName = animationRecord.GameRecord.Metadata.GameFileName;

        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
        string extension = Path.GetExtension(baseFileName);

        string path = Path.Combine(pathToAnimationsDirectory, baseFileName);

        //Gets unique file name if current one exists
        int counter = 1;
        string newFileName = baseFileName;
        while (File.Exists(path))
        {
            newFileName = $"{fileNameWithoutExt}({counter}){extension}";
            path = Path.Combine(pathToAnimationsDirectory, newFileName);
            counter++;
        }

        animationRecord.GameRecord.Metadata.GameFileName = newFileName;

        string animationData = JsonConvert.SerializeObject(animationRecord, settings);

        try
        {
            AnimationJsonPathHolder = path;
            File.WriteAllText(path, animationData);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to write animation record JSON: " + ex.Message);
        }
    }

    public AnimationRecord DeserializeAnimationFile()
    {
        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };

        if (string.IsNullOrWhiteSpace(AnimationJsonPathHolder))
        {
            Debug.LogError("AnimationJsonPathHolder is empty or null!");
            return null;
        }

        if (!File.Exists(AnimationJsonPathHolder))
        {
            Debug.LogError($"File not found at path: {AnimationJsonPathHolder}");
            return null;
        }

        try
        {
            string data = File.ReadAllText(AnimationJsonPathHolder);
            AnimationRecord animationRecord = JsonConvert.DeserializeObject<AnimationRecord>(data, settings);
            return animationRecord;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception found in DeserializeAnimationFile: {ex.Message}");
            return null;
        }
    }

    public bool ValidateAnimationRecord(AnimationRecord animationRecord)
    {
        if (animationRecord == null)
        {
            Debug.LogError("AnimationRecord is null!");
            return false;
        }

        var gameRecord = animationRecord.GameRecord;

        var history = gameRecord.History;
        var metadata = gameRecord.Metadata;
        var animationData = animationRecord.AnimationData;

        if (metadata.TotalTime < 0)
        {
            Debug.LogError($"Invalid TotalTime: {metadata.TotalTime}");
            return false;
        }

        if (metadata.TotalMoves < 0)
        {
            Debug.LogError($"Invalid TotalMoves: {metadata.TotalMoves}");
            return false;
        }

        if (!AnimationSettingsUI_Core.validThemes.Contains(animationData.Theme))
        {
            Debug.LogError($"Invalid Theme: {animationData.Theme}");
            return false;
        }


        if (!AnimationSettingsUI_Core.validPace.Contains(animationData.Pace))
        {
            Debug.LogError($"Invalid Pace: {animationData.Pace}");
            return false;
        }

        if (!AnimationSettingsUI_Core.validGenre.Contains(animationData.MusicGenre))
        {
            Debug.LogError($"Invalid MusicGenre: {animationData.MusicGenre}");
            return false;
        }

        if (!AnimationSettingsUI_Core.validOrientation.Contains(animationData.Orientation))
        {
            Debug.LogError($"Invalid Orientation: {animationData.Orientation}");
            return false;
        }

        if (!AnimationSettingsUI_Core.validEnableHud.Contains(animationData.EnableHud))
        {
            Debug.LogError($"Invalid EnableHud value: {animationData.EnableHud}");
            return false;
        }

        if (history == null || history.Count == 0)
        {
            Debug.LogError("GameRecord.History is null or empty");
            return false;
        }

        foreach (var move in history)
        {
            if (move.From.X < 0 || move.From.X > 7 || move.From.Y < 0 || move.From.Y > 7)
            {
                Debug.LogError($"Invalid move From coordinates: ({move.From.X},{move.From.Y})");
                return false;
            }

            if (move.To.X < 0 || move.To.X > 7 || move.To.Y < 0 || move.To.Y > 7)
            {
                Debug.LogError($"Invalid move To coordinates: ({move.To.X},{move.To.Y})");
                return false;
            }

            if (move.Time < 0)
            {
                Debug.LogError($"Move.Time is negative: {move.Time}");
                return false;
            }
        }

        //Is safe
        return true;
    }

    private void UpdateTempGame()
    {
        if (string.IsNullOrEmpty(pathToTempDirectory))
            return;

        if (!Directory.Exists(pathToTempDirectory))
            Directory.CreateDirectory(pathToTempDirectory);

        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        };

        string gameData = JsonConvert.SerializeObject(History, settings);

        string tempFileName = "Temp_Game_.json";

        string path = Path.Combine(pathToTempDirectory, tempFileName);

        try
        {
            File.WriteAllText(path, gameData);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to update temp game JSON: " + ex.Message);
        }
    }

    private void GeneratePersistentDataHierarchy()
    {
        string tempDirectory = "Temp";
        string gamesDirectory = "Games";
        string favoriteGamesDirectory = "Favorite Games";
        string animationsDirectory = "Animations";
        string playerDirectory = "Player";

        string path = Application.persistentDataPath;

        pathToTempDirectory = Path.Combine(path, tempDirectory);
        pathToGamesDirectory = Path.Combine(path, gamesDirectory);
        pathToFavoriteGamesDirectory = Path.Combine(path, favoriteGamesDirectory);
        pathToAnimationsDirectory = Path.Combine(path, animationsDirectory);
        pathToPlayerDirectory = Path.Combine(path, playerDirectory);

        if (!Directory.Exists(pathToTempDirectory))
            Directory.CreateDirectory(pathToTempDirectory);

        if (!Directory.Exists(pathToGamesDirectory))
            Directory.CreateDirectory(pathToGamesDirectory);

        if (!Directory.Exists(pathToFavoriteGamesDirectory))
            Directory.CreateDirectory(pathToFavoriteGamesDirectory);

        if (!Directory.Exists(pathToAnimationsDirectory))
            Directory.CreateDirectory(pathToAnimationsDirectory);

        if (!Directory.Exists(pathToPlayerDirectory))
            Directory.CreateDirectory(pathToPlayerDirectory);

    }

    public static FileManager Instance;
    //FileManager.Instance.--- -> Singleton
    private void Awake()
    {
        GeneratePersistentDataHierarchy();

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

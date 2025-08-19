using UnityEngine;
using static ChessPieceBase;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using static FileManager;


public class FileManager : MonoBehaviour
{
    [HideInInspector] public string pathToTempDirectory;
    [HideInInspector] public string pathToGamesDirectory;
    [HideInInspector] public string pathToFavoriteGamesDirectory;
    [HideInInspector] public string pathToAnimationsDirectory;
    [HideInInspector] public string pathToPlayerDirectory;

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

    public class MetaData
    {
        public DateTime date { get; set; }
        public float totalTime { get; set; }
        public int totalMoves { get; set; }
        public GameStateManager.End EndState { get; set; }

        public string gameFileName { get; set; }

        public MetaData(DateTime date, float totalTime, int totalMoves, GameStateManager.End endState, string gameFileName)
        {
            this.date = date;
            this.totalTime = totalTime;
            this.totalMoves = totalMoves;
            EndState = endState;
            this.gameFileName = gameFileName;
        }
    }

    public class GameRecord
    {
        public List<ChessMove> History { get; set; }
        public MetaData MetaData { get; set; }

        public GameRecord(List<ChessMove> history, MetaData metaData)
        {
            History = history;
            MetaData = metaData;
        }
    }

    public class AnimationRecord
    {
        public GameRecord gameRecord { get; set; }
        public AnimationSettingsUI_Core.AnimationData animationData { get; set; }

        public AnimationRecord(GameRecord gameRecord, AnimationSettingsUI_Core.AnimationData animationData)
        {
            this.gameRecord = gameRecord;
            this.animationData = animationData;
        }
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

            MetaData metaData = new MetaData(DateTime.Now, time, totalMoves, endState, gameFileName);

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

        string animationData = JsonConvert.SerializeObject(animationRecord, settings);

        string baseFileName = animationRecord.gameRecord.MetaData.gameFileName;

        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
        string extension = Path.GetExtension(baseFileName);

        string path = Path.Combine(pathToAnimationsDirectory, baseFileName);

        int counter = 1;
        while (File.Exists(path))
        {
            string newFileName = $"{fileNameWithoutExt}({counter}){extension}";
            path = Path.Combine(pathToAnimationsDirectory, newFileName);
            counter++;
        }

        try
        {
            File.WriteAllText(path, animationData);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to write animation record JSON: " + ex.Message);
        }
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
    //MoveHistory.Instance.--- -> Singleton
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
    /*
    TODO:


    Make PlayerData.JSON --> games played, time spent... // Update this, dont override
    Make GameData.JSON --> win, lose, draw, points, amount of special moves // For games
    GameHistory done // For games
    Make Directories:
    Player
    -> Games - holds all games --- Game-DateTime.JSON -> Game-17:59-8/7/2025.JSON
    -> Favourites - holds all favourite games --- Name.JSON -> default name is the same as original game
    -> Animations - holds the games and themes, song for a chosen game
    -> Data

    Make:
    Start screen UI:
        Start
        Previous Games -> Can add to favorites and open animation editor
        Favourites -> Can rename and open animation editor
        Animations -> Can rename and replay
        Options
        Credits
        Quit
    Options Menu UI:
        Audio settings
        Window size?
        On/Off for UI features (2D chess part)


    For 2D chess part
    Make a resign button
    Make a graveyard wih pixelart pieces -> Left
    Make a move history -> Right
    Make a visual UI feature for whoose turn it is
    Make a Timer in the top right
    Add sound effects for all pieces based on move importance - normal, check, checkmate, special
    Add sound effect for game end

    Make loading screens that wait for all prefabs to load, maybe await/async

    Make a game preview where you can choose to play a 3d animation or real life***
    Save Game To Favourites


    3D:
    you go into a editor:
    Options:
    Emotion: Happy, sad, angry, calm...
    Song type: pop, rock...
    World theme: Space, Desert, Forest, Castle
    Also take in account Time played game, win/lose/draw, amount of special moves, players total played games

    saves all that to a temp file, if player chooses to save the animation, move the temp file data into permenant data file

    2D: -> Real life robot board, probably wont happen
    Options:
    Connect to a server and wait for confrimation
    and press play


    for 3D:
    Make a algoritam for linking moves with beat up parts of the song -- Rythm
    Record real life chess pieces and turn them to 3d models with AliceVision
    Add a lot of randomisation and have the ability to save it to a favorite animation so you can replay it
    UI introduction at bottom left about Song name, Theme, song type, Emotion, Win/Lose/Draw, DateTime

    End screen UI:
    Options:
        Replay
        Exit
        Save Animation


    Optional:
        Real life robot board
        Multiplayer
        VS bot
        Add controller support
        Make Good Game Icon
        Make Game Trailer
        Release game on Steam for 4.99€ -> be sure the game is good
        Make game window be resizeable


     */
}

public static class AnimationSettingsUI_Core
{
    //Logs all settings and saves them to the scene manager to play the animation
    public static string theme;
    public static string pace;
    public static string musicGenre;
    public static string song;
    public static string orientation;
    public static string enableHud;
    public static string player1;
    public static string player2;

    public class AnimationData
    {
        public string Theme { get; set; }
        public string Pace { get; set; }
        public string MusicGenre { get; set; }
        public string Song { get; set; }
        public string Orientation { get; set; }
        public string EnableHud { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }

        public AnimationData(string theme, string pace, string musicGenre, string orientation, string enableHud, string player1, string player2, string song)
        {
            this.Theme = theme;
            this.Pace = pace;
            this.MusicGenre = musicGenre;
            this.Orientation = orientation;
            this.EnableHud = enableHud;
            this.Player1 = player1;
            this.Player2 = player2;
            this.Song = song;
        }
    }

    public static void SaveTheme(string value) => theme = value;
    public static void SavePace(string value) => pace = value;
    public static void SaveMusicGenre(string value) => musicGenre = value;
    public static void SaveSong(string value) => song = value;
    public static void SaveOrientation(string value) => orientation = value;
    public static void SaveEnableHud(string value) => enableHud = value;
    public static void SavePlayer1Name(string value) => player1 = value;
    public static void SavePlayer2Name(string value) => player2 = value;

    public static void SaveAllData()
    {
        AnimationData animationData = new AnimationData(theme, pace, musicGenre, orientation, enableHud, player1, player2, song);
        SceneManager.Instance.SaveAnimationData(animationData);
    }
}
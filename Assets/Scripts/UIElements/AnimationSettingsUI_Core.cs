public static class AnimationSettingsUI_Core
{
    //Logs all settings and saves them to the scene manager to play the animation
    private static string theme;
    private static string pace;
    private static string musicGenre;
    private static string orientation;
    private static string enableHud;
    private static string player1;
    private static string player2;

    public class AnimationData
    {
        public string theme { get; set; }
        public string pace { get; set; }
        public string musicGenre { get; set; }
        public string orientation { get; set; }
        public string enableHud { get; set; }
        public string player1 { get; set; }
        public string player2 { get; set; }

        public AnimationData(string theme, string pace, string musicGenre, string orientation, string enableHud, string player1, string player2)
        {
            this.theme = theme;
            this.pace = pace;
            this.musicGenre = musicGenre;
            this.orientation = orientation;
            this.enableHud = enableHud;
            this.player1 = player1;
            this.player2 = player2;
        }
    }

    public static void SaveTheme(string value) => theme = value;
    public static void SavePace(string value) => pace = value;
    public static void SaveMusicGenre(string value) => musicGenre = value;
    public static void SaveOrientation(string value) => orientation = value;
    public static void SaveEnableHud(string value) => enableHud = value;
    public static void SavePlayer1Name(string value) => player1 = value;
    public static void SavePlayer2Name(string value) => player2 = value;

    public static void SaveAllData()
    {
        AnimationData animationData = new AnimationData(theme, pace, musicGenre, orientation, enableHud, player1, player2);
        SceneManager.Instance.SaveAnimationData(animationData);
    }
}
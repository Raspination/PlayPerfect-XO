using UnityEngine;

namespace PlayPerfect.XO.Data
{
    public class Consts
    {
        private const string ATLAS = "XO_Atlas";

        public static readonly string X_SPRITE = $"{ATLAS}[XSprite]";
        public static readonly string O_SPRITE = $"{ATLAS}[OSprite]";
        public static readonly string BOARD_SPRITE = $"{ATLAS}[Board]";
        public static readonly string X_STRIKE = $"{ATLAS}[XSlash]";
        public static readonly string O_STRIKE = $"{ATLAS}[OSlash]";
        public static readonly string LEADERBOARD_BTN = $"{ATLAS}[LeaderboardBTN]";
        public static readonly string RESTART_BTN = $"{ATLAS}[RestartBtn]";
        public static readonly string PANEL_BG = $"{ATLAS}[LeaderboardPnl]";
        public static readonly string SAVE_DATA_PATH = Application.dataPath + "/SaveData/";
        
        
        public const int MIN_SCORE = 1;
        public const int MAX_WIN_SCORE = 100;
        public const int MIN_WIN_SCORE = 50;
        public const int MAX_TIE_SCORE = 49;
        public const int MIN_TIE_SCORE = 2;
        public const float FAST_TIME_THRESHOLD = 10f;
        public const float SLOW_TIME_THRESHOLD = 20f;
        public const string SCORE_DATA_KEY = "XO_ScoreData";
    }
}
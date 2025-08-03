using UnityEngine;

namespace PlayPerfect.XO
{
    [System.Serializable]
    public class ScoreData
    {
        public int totalScore;
        public int gamesPlayed;
        public int gamesWon;
        public int gamesLost;
        public int gamesTied;
        public float bestTime;
        public float averageTime;
        public int currentGameScore;

        public ScoreData()
        {
            Reset();
        }
        
        public void Reset()
        {
            totalScore = 0;
            gamesPlayed = 0;
            gamesWon = 0;
            gamesLost = 0;
            gamesTied = 0;
            bestTime = float.MaxValue;
            averageTime = 0f;
            currentGameScore = 0;
        }
        
        public void AddGameResult(int gameScore, float gameTime, Data.Enums.GameState result)
        {
            totalScore += gameScore;
            currentGameScore = gameScore;
            gamesPlayed++;
            
            switch (result)
            {
                case Data.Enums.GameState.PlayerWin:
                    gamesWon++;
                    break;
                case Data.Enums.GameState.ComputerWin:
                    gamesLost++;
                    break;
                case Data.Enums.GameState.Draw:
                    gamesTied++;
                    break;
            }
            
            if (gameTime < bestTime)
                bestTime = gameTime;
                
            averageTime = ((averageTime * (gamesPlayed - 1)) + gameTime) / gamesPlayed;
        }
    }
}

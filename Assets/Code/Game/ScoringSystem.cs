using System;
using UnityEngine;
using UnityEngine.Events;
using PlayPerfect.XO.Data;
using Zenject;

namespace PlayPerfect.XO
{
    public class ScoringSystem : IScoringSystem, IInitializable, IDisposable
    {
        private ScoreData scoreData;
        private float gameStartTime;
        private float totalPlayerTurnTime;
        
        public ScoreData ScoreData => scoreData;
        public UnityEvent OnCompleteGame { get; } = new UnityEvent();
        
        public void Initialize()
        {
            LoadScoreData();
        }
        
        public void Dispose()
        {
            SaveScoreData();
        }
        
        public void StartNewGame()
        {
            gameStartTime = Time.time;
            totalPlayerTurnTime = 0f;
        }
        
        public void AddPlayerTurnTime(float turnTime)
        {
            totalPlayerTurnTime += turnTime;
        }
        
        public int CalculateGameScore(Enums.GameState gameResult)
        {
            var averageTurnTime = totalPlayerTurnTime > 0 ? totalPlayerTurnTime / GetPlayerTurnCount() : Consts.SLOW_TIME_THRESHOLD;
            
            return gameResult switch
            {
                Enums.GameState.PlayerWin => CalculateWinScore(averageTurnTime),
                Enums.GameState.ComputerWin => Consts.MIN_SCORE,
                Enums.GameState.Draw => CalculateTieScore(averageTurnTime),
                _ => 0
            };
        }
        
        public void CompleteGame(Enums.GameState gameResult)
        {
            var gameTime = Time.time - gameStartTime;
            var gameScore = CalculateGameScore(gameResult);
            
            scoreData.AddGameResult(gameScore, gameTime, gameResult);
            SaveScoreData();
            
            OnCompleteGame?.Invoke();
        }
        
        private int CalculateWinScore(float averageTurnTime)
        {
            if (averageTurnTime <= Consts.FAST_TIME_THRESHOLD)
                return Consts.MAX_WIN_SCORE;
            
            if (averageTurnTime >= Consts.SLOW_TIME_THRESHOLD)
                return Consts.MIN_WIN_SCORE;
            
            var timeRatio = (Consts.SLOW_TIME_THRESHOLD - averageTurnTime) / (Consts.SLOW_TIME_THRESHOLD - Consts.FAST_TIME_THRESHOLD);
            return Mathf.RoundToInt(Mathf.Lerp(Consts.MIN_WIN_SCORE, Consts.MAX_WIN_SCORE, timeRatio));
        }
        
        private int CalculateTieScore(float averageTurnTime)
        {
            if (averageTurnTime <= Consts.FAST_TIME_THRESHOLD)
                return Consts.MAX_TIE_SCORE;
            
            if (averageTurnTime >= Consts.SLOW_TIME_THRESHOLD)
                return Consts.MIN_TIE_SCORE;
            
            var timeRatio = (Consts.SLOW_TIME_THRESHOLD - averageTurnTime) / (Consts.SLOW_TIME_THRESHOLD - Consts.FAST_TIME_THRESHOLD);
            return Mathf.RoundToInt(Mathf.Lerp(Consts.MIN_TIE_SCORE, Consts.MAX_TIE_SCORE, timeRatio));
        }
        
        private int GetPlayerTurnCount()
        {
            return Mathf.Max(1, Mathf.CeilToInt(totalPlayerTurnTime / 5f));
        }
        
        private void LoadScoreData()
        {
            if (PlayerPrefs.HasKey(Consts.SCORE_DATA_KEY))
            {
                var json = PlayerPrefs.GetString(Consts.SCORE_DATA_KEY);
                try
                {
                    scoreData = JsonUtility.FromJson<ScoreData>(json);
                }
                catch
                {
                    scoreData = new ScoreData();
                }
            }
            else
            {
                scoreData = new ScoreData();
            }
        }
        
        private void SaveScoreData()
        {
            var json = JsonUtility.ToJson(scoreData);
            PlayerPrefs.SetString(Consts.SCORE_DATA_KEY, json);
            PlayerPrefs.Save();
        }
        
        public void ResetAllScores()
        {
            scoreData.Reset();
            SaveScoreData();
        }
    }
}

using PlayPerfect.XO.Data;
using UnityEngine.Events;

namespace PlayPerfect.XO
{
    public interface IScoringSystem
    {
        UnityEvent OnCompleteGame { get; }
        ScoreData ScoreData { get; }
        void StartNewGame();
        void AddPlayerTurnTime(float turnTime);
        int CalculateGameScore(Enums.GameState gameResult);
        void CompleteGame(Enums.GameState gameResult);
        void ResetAllScores();
    }
}

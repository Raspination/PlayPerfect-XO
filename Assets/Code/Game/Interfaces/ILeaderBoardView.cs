using PlayPerfect.XO.Data;

namespace PlayPerfect.XO
{
    public interface ILeaderBoardView
    {
        void Initialize(IScoringSystem scoringSystem);
        void UpdateUI();
        void UpdateCurrentGameScore(int score);
    }
}

using UnityEngine;
using TMPro;
using Zenject;

namespace PlayPerfect.XO
{
    public class LeaderBoardView : MonoBehaviour, ILeaderBoardView
    {
        [SerializeField] private TextMeshProUGUI _totalScoreTxt;
        [SerializeField] private TextMeshProUGUI _latestScoreTxt;
        [SerializeField] private TextMeshProUGUI _gamesPlayed;
        [SerializeField] private TextMeshProUGUI _winrate;
        [SerializeField] private TextMeshProUGUI _bestTime;

        private IScoringSystem scoringSystem;
        private int lastDisplayedScore;

        [Inject]
        public void Construct(IScoringSystem scoringSystem)
        {
            this.scoringSystem = scoringSystem;
        }

        private void Start()
        {
            Initialize(scoringSystem);
        }

        public void Initialize(IScoringSystem scoringSystem)
        {
            this.scoringSystem = scoringSystem;
            lastDisplayedScore = scoringSystem.ScoreData.totalScore;
            scoringSystem.OnCompleteGame.AddListener(UpdateUI);
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (scoringSystem?.ScoreData == null) return;

            var scoreData = scoringSystem.ScoreData;

            if (_totalScoreTxt != null)
                _totalScoreTxt.text = $"Total Score: {scoreData.totalScore}";

            if (_gamesPlayed != null)
                _gamesPlayed.text = $"Games: {scoreData.gamesPlayed}";

            if (_winrate != null)
            {
                var winRate = scoreData.gamesPlayed > 0 ? (float)scoreData.gamesWon / scoreData.gamesPlayed * 100f : 0f;
                _winrate.text = $"Win Rate: {winRate:F1}%";
            }

            if (_bestTime != null)
            {
                var bestTime = scoreData.bestTime == float.MaxValue ? 0f : scoreData.bestTime;
                _bestTime.text = $"Best Time: {bestTime:F1}s";
            }
        }

        public void UpdateCurrentGameScore(int score)
        {
            if (_latestScoreTxt != null)
                _latestScoreTxt.text = $"Game Score: {score}";
        }

        public void OnDestroy()
        {
            scoringSystem.OnCompleteGame.RemoveListener(UpdateUI);
        }
    }
}
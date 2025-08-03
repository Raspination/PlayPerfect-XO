using System;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using PlayPerfect.XO.Data;

namespace PlayPerfect.XO
{
    public class GameManager : IGameManager, IInitializable, IDisposable
    {
        #region Events
        public event Action OnGameOver;
        public event Action<GameData> OnGameStateChanged;
        public event Action<int, int, Enums.CellState> OnCellChanged;
        public event Action<int, Enums.GameState> OnScoreCalculated;
        #endregion

        #region Private Properties
        private GameData _gameData;
        private AssetLoader _assetLoader;
        private ComputerPlayer _secondPlayer;
        private IScoringSystem _scoringSystem;
        private float _gameStartTime;
        private float _turnStartTime;
        private float _totalTurnTime;
        private bool _isPlayerTurn;
        private UniTaskCompletionSource playerTurnCompletionSource;
        #endregion

        #region Public Properties
        public GameData GameData => _gameData;
        public IScoringSystem ScoringSystem => _scoringSystem;
        public bool IsGameInProgress => _gameData != null && _gameData._gameState == Enums.GameState.Playing;
        #endregion
        
        
        [Inject]
        public void Construct(AssetLoader assetLoader, IScoringSystem scoringSystem)
        {
            _assetLoader = assetLoader;
            _secondPlayer = new ComputerPlayer();
            _scoringSystem = scoringSystem;
        }

        public void Initialize()
        {
            _gameData = new GameData();
        }

        public void Dispose()
        {
            OnGameOver = null;
            OnGameStateChanged = null;
            OnCellChanged = null;
        }

        public async UniTask LoadNewGameAsync(bool? isUserFirstTurn = null)
        {
            try
            {
                await UniTask.WaitUntil(() => _assetLoader.IsLoaded);
                
                _gameData ??= new GameData();
                
                var playerStartsFirst = isUserFirstTurn ?? UnityEngine.Random.Range(0, 2) == 0;
                
                _gameData.Reset(playerStartsFirst);
                _totalTurnTime = 0f;
                _gameStartTime = Time.time;
                _turnStartTime = Time.time;
                
                _scoringSystem.StartNewGame();
                
                OnGameStateChanged?.Invoke(_gameData);
                
                if (!_gameData.IsPlayerTurn)
                {
                    await HandleComputerTurn();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public async UniTask WaitForPlayerTurn()
        {
            if (!IsGameInProgress || !_gameData.IsPlayerTurn) return;
            
            _isPlayerTurn = true;
            playerTurnCompletionSource = new UniTaskCompletionSource();
            
            await playerTurnCompletionSource.Task;
        }

        public int GetFinalScore()
        {
            if (IsGameInProgress) return 0;
            
            var baseScore = _gameData._gameState switch
            {
                Enums.GameState.PlayerWin => 1000,
                Enums.GameState.ComputerWin => 0,
                Enums.GameState.Draw => 500,
                _ => 0
            };
            
            var timeBonus = Mathf.Max(0, 300f - _totalTurnTime) * 10f;
            return Mathf.RoundToInt(baseScore + timeBonus);
        }

        public void HandlePlayerMove(int row, int col)
        {
            if (!_gameData.IsPlayerTurn || !IsGameInProgress) return;

            if (!_gameData.IsCellValid(row, col)) return;
            
            var playerTurnTime = Time.time - _turnStartTime;
            _scoringSystem.AddPlayerTurnTime(playerTurnTime);
            
            _gameData.MakeMove(row, col);
                
            _totalTurnTime += playerTurnTime;
            _turnStartTime = Time.time;
                
            OnCellChanged?.Invoke(row, col, Enums.CellState.X);
            OnGameStateChanged?.Invoke(_gameData);
                
            if (!IsGameInProgress)
            {
                var gameScore = _scoringSystem.CalculateGameScore(_gameData._gameState);
                _scoringSystem.CompleteGame(_gameData._gameState);
                OnScoreCalculated?.Invoke(gameScore, _gameData._gameState);
                OnGameOver?.Invoke();
            }
            else if (!_gameData.IsPlayerTurn)
            {
                HandleComputerTurn().Forget();
            }

            if (!_isPlayerTurn) return;
            
            _isPlayerTurn = false;
            playerTurnCompletionSource?.TrySetResult();
        }

        private async UniTask HandleComputerTurn()
        {
            if (_gameData.IsPlayerTurn || !IsGameInProgress) return;
            
            var move = await _secondPlayer.GetMoveAsync(_gameData);
            
            if (move is { row: >= 0, col: >= 0 } && IsGameInProgress)
            {
                _gameData.MakeMove(move.row, move.col);
                
                OnCellChanged?.Invoke(move.row, move.col, Enums.CellState.O);
                OnGameStateChanged?.Invoke(_gameData);
                
                if (!IsGameInProgress)
                {
                    var gameScore = _scoringSystem.CalculateGameScore(_gameData._gameState);
                    _scoringSystem.CompleteGame(_gameData._gameState);
                    OnScoreCalculated?.Invoke(gameScore, _gameData._gameState);
                    OnGameOver?.Invoke();
                }
                else
                {
                    _turnStartTime = Time.time;
                }
            }
        }

        public void RestartGame()
        {
            LoadNewGameAsync().Forget();
        }
    }
}

using System;
using Cysharp.Threading.Tasks;
using PlayPerfect.XO.Data;

namespace PlayPerfect.XO
{
    public interface IGameManager
    {
        /// <summary>
        /// An event that triggers when the User finished or quit the game
        /// </summary>
        event Action OnGameOver;
        
        /// <summary>
        /// An event that triggers when the game state changes
        /// </summary>
        event Action<GameData> OnGameStateChanged;
        
        /// <summary>
        /// An event that triggers when a cell state changes
        /// </summary>
        event Action<int, int, Enums.CellState> OnCellChanged;
        
        /// <summary>
        /// An event that triggers when the game score is calculated
        /// </summary>
        event Action<int, Enums.GameState> OnScoreCalculated;

        /// <summary>
        /// Gets the current game data
        /// </summary>
        GameData GameData { get; }
        
        /// <summary>
        /// A property to check if the game is still in progress or has ended
        /// </summary>
        bool IsGameInProgress { get; }

        /// <summary>
        /// Loads the game assets and sets the state to the initial state of a game.
        /// </summary>
        /// <param name="isUserFirstTurn">Determines whose first turn is it, in case of null it will be randomised</param>
        UniTask LoadNewGameAsync(bool? isUserFirstTurn = null);
        
        /// <summary>
        /// Waiting for the end of the Computer Turn or the End of the Game
        /// </summary>
        UniTask WaitForPlayerTurn();
        
        /// <summary>
        /// Gets the score of the currently finished game
        /// Score is calculated as a function of Time spent on turns and victory state.
        /// </summary>
        /// <returns>The score</returns>
        int GetFinalScore();
        
        /// <summary>
        /// Handles a player move at the specified position
        /// </summary>
        /// <param name="row">The row position of the move</param>
        /// <param name="col">The column position of the move</param>
        void HandlePlayerMove(int row, int col);
        
        /// <summary>
        /// Restarts the game
        /// </summary>
        void RestartGame();
        
        /// <summary>
        /// Loads saved game
        /// </summary>
        UniTask LoadSavedGameAsync();
        
        /// <summary>
        /// deletes saved game
        /// </summary>
        void ClearSavedGame();
    }
}
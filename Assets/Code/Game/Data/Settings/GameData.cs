using System.Linq;
using PlayPerfect.XO.Data;

namespace PlayPerfect.XO
{
    [System.Serializable]
    public class GameData
    {
        #region Private Properties

        private (int row, int col)[] _finishedLine;

        #endregion
        
        #region Public Properties

        public Enums.CellState[,] _board = new Enums.CellState[3, 3];
        public Enums.CellState _currentPlayer = Enums.CellState.X;
        public Enums.GameState _gameState = Enums.GameState.Playing;
        public (int row, int col)[] FinishedLine => _finishedLine;
        public bool IsPlayerTurn = true;
        

        #endregion

        public void Reset(bool playerStartsFirst)
        {
            _finishedLine = null;
            _board = new Enums.CellState[3, 3];
            _currentPlayer = Enums.CellState.X;
            _gameState = Enums.GameState.Playing;
            IsPlayerTurn = playerStartsFirst;

            if (!playerStartsFirst)
            {
                _currentPlayer = Enums.CellState.O;
            }
        }

        public void MakeMove(int row, int col)
        {
            if (!IsCellValid(row, col)) return;

            _board[row, col] = _currentPlayer;
            CheckIfWon();

            if (_gameState != Enums.GameState.Playing) return;

            _currentPlayer = _currentPlayer == Enums.CellState.X ? Enums.CellState.O : Enums.CellState.X;
            IsPlayerTurn = !IsPlayerTurn;
        }

        #region Validations

        public bool IsCellValid(int row, int col)
        {
            return row is >= 0 and < 3 && col is >= 0 and < 3 && _board[row, col] == Enums.CellState.Empty &&
                   _gameState == Enums.GameState.Playing;
        }

        private void CheckIfWon()
        {
            var winner = ValidateLines();

            _gameState = winner switch
            {
                Enums.CellState.X => Enums.GameState.PlayerWin,
                Enums.CellState.O => Enums.GameState.ComputerWin,
                Enums.CellState.Empty when IsBoardFull() => Enums.GameState.Draw,
                _ => Enums.GameState.Playing
            };
        }

        private Enums.CellState ValidateLines()
        {
            (int row, int col)[][] lines =
            {
                new[] { (0, 0), (0, 1), (0, 2) },
                new[] { (1, 0), (1, 1), (1, 2) },
                new[] { (2, 0), (2, 1), (2, 2) },
                new[] { (0, 0), (1, 0), (2, 0) },
                new[] { (0, 1), (1, 1), (2, 1) },
                new[] { (0, 2), (1, 2), (2, 2) },
                new[] { (0, 0), (1, 1), (2, 2) },
                new[] { (0, 2), (1, 1), (2, 0) }
            };

            for (var i = 0; i < lines.Length; i++)
            {
                var a = lines[i][0];
                var b = lines[i][1];
                var c = lines[i][2];

                var first = _board[a.row, a.col];

                if (first == Enums.CellState.Empty)
                    continue;

                if (_board[b.row, b.col] == first && _board[c.row, c.col] == first)
                {
                    _finishedLine = lines[i];
                    return first;
                }
            }

            return Enums.CellState.Empty;
        }

        private bool IsBoardFull()
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    if (_board[i, j] == Enums.CellState.Empty)
                        return false;
                }
            }

            return true;
        }

        #endregion
    }
}
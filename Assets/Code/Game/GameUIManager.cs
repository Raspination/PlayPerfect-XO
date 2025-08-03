using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayPerfect.XO.Data;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace PlayPerfect.XO
{
    public class GameUIManager : MonoBehaviour
    {
        #region Private parameters
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Image _leaderboardLogo;
        [SerializeField] private Image _board;
        [SerializeField] private Image _statusPnl;
        [SerializeField] private Image _popupPnl;
        [SerializeField] private Transform _cellGrid;
        [SerializeField] private Vector2 cellSize = new Vector2(100, 100);
        [SerializeField] private Vector2 spacing = new Vector2(10, 10);
        [SerializeField] private float boardDrawDuration = 2f;
        [SerializeField] private Ease boardDrawEase = Ease.OutQuart;

        private GameManager _gameManager;
        private AssetLoader _assetLoader;
        private CellPool _cellPoolManager;
        private List<CellView> _activeCells = new List<CellView>();
        private GridLayoutGroup _cellsGridLayoutGroup;
        
        #endregion

        #region MonoBehaviour Callbacks
        
        [Inject]
        public void Construct(GameManager gameManager, AssetLoader assetLoader, CellPool cellPool)
        {
            _gameManager = gameManager;
            _assetLoader = assetLoader;
            _cellPoolManager = cellPool;
        }

        private async void Start()
        {
            try
            {
                await _gameManager.LoadSavedGameAsync();
                InitializeUI();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void OnDestroy()
        {
            _gameManager.OnGameStateChanged -=  UpdateStatusText;
            _gameManager.OnGameOver -= OnGameOver;
            _restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        #endregion
        
        #region Board Initialization
        private void InitializeUI()
        {
            _restartButton.image.sprite = _assetLoader.RestartButton;
            _leaderboardLogo.sprite = _assetLoader.LeaderboardButton;
            _statusPnl.sprite = _assetLoader.PanelBackground;
            _popupPnl.sprite = _assetLoader.PanelBackground;
            
            SetupBoard();
            SetupGrid();
            AnimateBoardDrawing().ContinueWith(() =>
            {
                SetupCells();
                _restartButton.onClick.AddListener(OnRestartClicked);
                _gameManager.OnGameStateChanged +=  UpdateStatusText;
                _gameManager.OnGameOver += OnGameOver;
                UpdateStatusText(_gameManager.GameData);
            }).Forget();
        }

        private void SetupBoard()
        {
            if (_board == null || _assetLoader.BoardSprite == null) return;

            _board.sprite = _assetLoader.BoardSprite;

            _board.transform.localScale = Vector3.zero;
            _board.color = new Color(1f, 1f, 1f, 0f);
        }

        private async UniTask AnimateBoardDrawing()
        {
            if (_board == null) return;

            var sequence = DOTween.Sequence();
            sequence.Append(_board.DOFade(1f, boardDrawDuration * 0.3f).SetEase(Ease.InQuart));
            sequence.Join(_board.transform.DOScale(Vector3.one, boardDrawDuration * 0.7f).SetEase(boardDrawEase));
            sequence.Join(_board.transform.DORotate(new Vector3(0, 0, 2f), boardDrawDuration * 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _board.transform.DORotate(Vector3.zero, boardDrawDuration * 0.2f).SetEase(Ease.InBack);
                }));

            await sequence.AsyncWaitForCompletion();
        }

        private void SetupGrid()
        {
            if (_cellsGridLayoutGroup == null)
            {
                _cellsGridLayoutGroup = _cellGrid.GetComponent<GridLayoutGroup>();
                if (_cellsGridLayoutGroup == null)
                {
                    _cellsGridLayoutGroup = _cellGrid.gameObject.AddComponent<GridLayoutGroup>();
                }
            }

            _cellsGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _cellsGridLayoutGroup.constraintCount = 3;
            _cellsGridLayoutGroup.cellSize = cellSize;
            _cellsGridLayoutGroup.spacing = spacing;
            _cellsGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
            _cellsGridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
            _cellsGridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        }

        private void SetupCells()
        {
            ClearCells();

            for (var i = 0; i < 9; i++)
            {
                var row = i / 3;
                var col = i % 3;

                var cell = _cellPoolManager.GetCell();

                cell.transform.SetParent(_cellGrid, false);

                var rectTransform = cell.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.zero;
                    rectTransform.sizeDelta = cellSize;
                }

                cell.Initialize(row, col, _gameManager.GameData._board[row,col]);
                _activeCells.Add(cell);
            }
        }

        private void ClearCells()
        {
            foreach (var cell in _activeCells)
            {
                _cellPoolManager.ReturnCell(cell);
            }

            _activeCells.Clear();
        }
        private void ResetCellVisuals()
        {
            foreach (var cell in _activeCells)
            {
                var symbolImage = cell.GetComponentInChildren<Image>();
                if (symbolImage != null)
                {
                    symbolImage.sprite = null;
                    symbolImage.color = Color.clear;
                }

                var button = cell.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = true;
                }
            }
        }
        
        #endregion

        #region Event Handlers
        private void OnGameOver()
        {
            // Show winning line if there's a winner (not a draw)
            if (_gameManager.GameData.FinishedLine != null)
            
            _popupPnl.gameObject.SetActive(true);
        }

        private void UpdateStatusText(GameData gameData)
        {
            switch (gameData._gameState)
            {
                case Enums.GameState.Playing:
                    _statusText.text = gameData.IsPlayerTurn ? "Your Turn (X)" : "Computer Turn (O)";
                    break;
                case Enums.GameState.PlayerWin:
                    _statusText.text = "You Win!";
                    break;
                case Enums.GameState.ComputerWin:
                    _statusText.text = "Computer Wins!";
                    break;
                case Enums.GameState.Draw:
                    _statusText.text = "It's a Draw!";
                    break;
            }
        }

        private void OnRestartClicked()
        {
            ResetCellVisuals();
            _popupPnl.gameObject.SetActive(false);
            _gameManager.RestartGame();
        }

        #endregion
    }
}
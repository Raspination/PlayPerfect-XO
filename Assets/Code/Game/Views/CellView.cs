using PlayPerfect.XO.Data;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PlayPerfect.XO
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private Button _cellButton;
        [SerializeField] private Image _image;

        private GameManager _gameManager;
        private AssetLoader _assetLoader;
        private int _row;
        private int _collumn;

        [Inject]
        public void Construct(GameManager gameManager, AssetLoader assetLoader)
        {
            _gameManager = gameManager;
            _assetLoader = assetLoader;
        }

        public void Initialize(int row, int col)
        {
            _row = row;
            _collumn = col;
        }
        
        private void Start()
        {
            _cellButton.onClick.AddListener(OnCellClicked);
            _gameManager.OnCellChanged += OnCellChanged;
            _gameManager.OnGameStateChanged += OnGameStateChanged;

            _image.sprite = null;
            _image.color = Color.clear;
        }

        private void OnCellClicked()
        {
            _gameManager.HandlePlayerMove(_row, _collumn);
        }

        private void OnCellChanged(int changedRow, int changedCol, Enums.CellState state)
        {
            if (changedRow == _row && changedCol == _collumn)
            {
                UpdateCellVisual(state);
            }
        }

        private void OnGameStateChanged(GameData gameData)
        {
            _cellButton.interactable = gameData._gameState == Enums.GameState.Playing &&
                                       gameData.IsPlayerTurn &&
                                       gameData._board[_row, _collumn] == Enums.CellState.Empty;
        }

        private void UpdateCellVisual(Enums.CellState state)
        {
            if (!_assetLoader.IsLoaded) return;

            switch (state)
            {
                case Enums.CellState.X:
                    _image.sprite = _assetLoader.XSprite;
                    _image.color = Color.white;
                    break;
                case Enums.CellState.O:
                    _image.sprite = _assetLoader.OSprite;
                    _image.color = Color.white;
                    break;
                default:
                    _image.sprite = null;
                    _image.color = Color.clear;
                    break;
            }
        }
        
        
        private void OnDestroy()
        {
            _gameManager.OnCellChanged -= OnCellChanged;
            _gameManager.OnGameStateChanged -= OnGameStateChanged;
            _cellButton.onClick.RemoveListener(OnCellClicked);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PlayPerfect.XO
{
    public class CellPool : MonoBehaviour
    {
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private Transform _cellsHolder;
        
        private int _poolSize = 9;
        private Queue<CellView> _availableCells = new Queue<CellView>();
        private List<CellView> _activeCells = new List<CellView>();
        private DiContainer _container;
        private bool _isInitialized = false;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
            if (container != null && _cellPrefab != null)
            {
                InitializePool();
            }
        }

        private void InitializePool()
        {
            if (_isInitialized || _container == null || _cellPrefab == null) return;
            
            for (var i = 0; i < _poolSize; i++)
            {
                var cell = _container.InstantiatePrefabForComponent<CellView>(_cellPrefab, _cellsHolder);
                cell.gameObject.SetActive(false);
                _availableCells.Enqueue(cell);
            }
            
            _isInitialized = true;
        }

        public CellView GetCell()
        {
            if (!_isInitialized && _container != null) 
            {
                InitializePool();
            }
            
            CellView cell;
            
            cell = _availableCells.Count > 0 ? _availableCells.Dequeue() : _container?.InstantiatePrefabForComponent<CellView>(_cellPrefab, _cellsHolder);
            cell?.gameObject.SetActive(true);
            
            _activeCells.Add(cell);
            return cell;
        }

        public void ReturnCell(CellView cell)
        {
            if (!_activeCells.Remove(cell)) return;
            cell.gameObject.SetActive(false);
            cell.transform.SetParent(_cellsHolder);
            _availableCells.Enqueue(cell);
        }
    }
}

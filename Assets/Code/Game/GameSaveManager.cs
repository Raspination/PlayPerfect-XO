using Code.Game.Interfaces;
using Zenject;
using UnityEngine;
using PlayPerfect.XO.Data;
using System;

namespace PlayPerfect.XO
{
    public class GameSaveManager
    {
        private readonly IGameSaveSystem _gsSystem;
        
        [Inject]
        public GameSaveManager(IGameSaveSystem gsSystem)
        {
            _gsSystem = gsSystem;
        }
        
        public void SaveGame<T>(string key, T state)
        {
            if (state is GameData gameData)
            {
                var simpleData = ConvertToSimpleData(gameData);
                _gsSystem.Save(key, simpleData);
            }
            else
            {
                _gsSystem.Save(key, state);
            }
        }

        public T LoadGame<T>(string key)
        {
            if (typeof(T) == typeof(GameData))
            {
                var simpleData = _gsSystem.Load<SimpleGameData>(key);
                var gameData = ConvertFromSimpleData(simpleData);
                return (T)(object)gameData;
            }
            else
            {
                return _gsSystem.Load<T>(key);
            }
        }

        private SimpleGameData ConvertToSimpleData(GameData gameData)
        {
            if (gameData == null) return null;

            var simple = new SimpleGameData
            {
                currentPlayer = (int)gameData._currentPlayer,
                gameState = (int)gameData._gameState,
                isPlayerTurn = gameData.IsPlayerTurn,
                board = new int[9]
            };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    simple.board[i * 3 + j] = (int)gameData._board[i, j];
                }
            }

            return simple;
        }

        private GameData ConvertFromSimpleData(SimpleGameData simpleData)
        {
            if (simpleData == null) return null;

            var gameData = new GameData();
            
            gameData._currentPlayer = (Enums.CellState)simpleData.currentPlayer;
            gameData._gameState = (Enums.GameState)simpleData.gameState;
            gameData.IsPlayerTurn = simpleData.isPlayerTurn;

            if (simpleData.board != null && simpleData.board.Length == 9)
            {
                gameData._board = new Enums.CellState[3, 3];
                
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        gameData._board[i, j] = (Enums.CellState)simpleData.board[i * 3 + j];
                    }
                }
            }

            return gameData;
        }

        [System.Serializable]
        private class SimpleGameData
        {
            public int currentPlayer;
            public int gameState;
            public bool isPlayerTurn;
            public int[] board;
        }
    }
}
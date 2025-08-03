using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PlayPerfect.XO.Data;

namespace PlayPerfect.XO
{
    public class ComputerPlayer
    {
        public async UniTask<(int row, int col)> GetMoveAsync(GameData gameData)
        {
            await UniTask.Delay(Random.Range(1000, 3000));
            
            var availableMoves = new List<(int row, int col)>();
            
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (gameData._board[row, col] == Enums.CellState.Empty)
                    {
                        availableMoves.Add((row, col));
                    }
                }
            }

            if (availableMoves.Count <= 0) return (-1, -1);
            var randIndex = Random.Range(0, availableMoves.Count);
            return availableMoves[randIndex];

        }
    }
}

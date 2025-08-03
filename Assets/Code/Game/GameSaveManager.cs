using Code.Game.Interfaces;
using Zenject;

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
            _gsSystem.Save(key, state);
        }

        public T LoadGame<T>(string key)
        {
            return _gsSystem.Load<T>(key);
        }
    }
}
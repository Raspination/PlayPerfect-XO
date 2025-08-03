namespace Code.Game.Interfaces
{
    public interface IGameSaveSystem
    {
        void Save<T>(string key, T data);
        T Load<T>(string key);
    }
}
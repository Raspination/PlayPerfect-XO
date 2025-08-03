using System.IO;
using Code.Game.Interfaces;
using PlayPerfect.XO.Data;
using UnityEngine;

namespace PlayPerfect.XO
{
    public class FSGameSaveSystem : IGameSaveSystem
    {
        public void Save<T>(string key, T data)
        {
            if (!Directory.Exists(Consts.SAVE_DATA_PATH))
                Directory.CreateDirectory(Consts.SAVE_DATA_PATH);

            var json = JsonUtility.ToJson(data);
            File.WriteAllText(Consts.SAVE_DATA_PATH + key + ".txt", json);
        }

        public T Load<T>(string key)
        {
            var path = Consts.SAVE_DATA_PATH + key + ".txt";
            if (!File.Exists(path))
                return default;

            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
    }
}
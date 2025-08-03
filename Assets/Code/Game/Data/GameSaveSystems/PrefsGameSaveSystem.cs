using Code.Game.Interfaces;
using UnityEngine;

namespace PlayPerfect.XO
{
    public class PrefsGameSaveSystem :IGameSaveSystem
    {
        public void Save<T>(string key, T data)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public T Load<T>(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                var json = PlayerPrefs.GetString(key);
                return JsonUtility.FromJson<T>(json);
            }

            Debug.LogWarning($"No data found");
            return default(T);
        }
    }
}
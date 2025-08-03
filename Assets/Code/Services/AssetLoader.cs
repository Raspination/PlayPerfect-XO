using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using PlayPerfect.XO.Data;
using Zenject;

namespace PlayPerfect.XO
{
    public class AssetLoader : IInitializable
    {
        public Sprite XSprite { get; private set; }
        public Sprite OSprite { get; private set; }
        public Sprite BoardSprite { get; private set; }
        public Sprite XStrike { get; private set; }
        public Sprite OStrike { get; private set; }
        public Sprite LeaderboardButton { get; private set; }
        public Sprite RestartButton { get; private set; }
        public Sprite PanelBackground { get; private set; }
        public bool IsLoaded { get; private set; }
        
        
        private bool _isInitializing = false;

        public void Initialize()
        {
            if (_isInitializing) return;
            _isInitializing = true;
            LoadAllAssets().Forget();
        }

        private async UniTaskVoid LoadAllAssets()
        {
            try
            {
                XSprite = await LoadSprite(Consts.X_SPRITE);
                OSprite = await LoadSprite(Consts.O_SPRITE);
                BoardSprite = await LoadSprite(Consts.BOARD_SPRITE);
                XStrike = await LoadSprite(Consts.X_STRIKE);
                OStrike = await LoadSprite(Consts.O_STRIKE);
                LeaderboardButton = await LoadSprite(Consts.LEADERBOARD_BTN);
                RestartButton = await LoadSprite(Consts.RESTART_BTN);
                PanelBackground = await LoadSprite(Consts.PANEL_BG);

                IsLoaded = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{e.Message}");
            }
        }

   

        private async UniTask<Sprite> LoadSprite(string key)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<Sprite>(key);
                var result = await handle.ToUniTask();
                return result;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{e.Message}");
                return null;
            }
        }
    }
}

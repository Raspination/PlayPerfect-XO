using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

namespace PlayPerfect.XO
{
    public class GameBootstrapper : MonoBehaviour
    {
        private AssetLoader _assetLoader;
        private GameManager _gameManager;

        [Inject]
        public void Construct(AssetLoader assetLoader, GameManager gameManager)
        {
            _assetLoader = assetLoader;
            _gameManager = gameManager;
        }

        private async void Start()
        {
            await WaitForInitialization();
        }

        private async UniTask WaitForInitialization()
        {
            await UniTask.WaitUntil(() => _assetLoader != null && _assetLoader.IsLoaded);
        }
    }
}

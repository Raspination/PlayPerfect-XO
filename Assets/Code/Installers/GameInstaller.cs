using Code.Game.Interfaces;
using Zenject;
using UnityEngine;

namespace PlayPerfect.XO
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private CellPool _cellPool;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AssetLoader>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScoringSystem>().AsSingle().NonLazy();
#if UNITY_EDITOR
            Container.Bind<IGameSaveSystem>().To<FSGameSaveSystem>().AsSingle().NonLazy();
#else
            Container.Bind<IGameSaveSystem>().To<PrefsGameSaveSystem>().AsSingle().NonLazy();
#endif
            Container.Bind<GameSaveManager>().AsSingle().NonLazy();

            if (_cellPool != null)
            {
                Container.Bind<CellPool>().FromInstance(_cellPool).AsSingle();
            }
        }
    }
}
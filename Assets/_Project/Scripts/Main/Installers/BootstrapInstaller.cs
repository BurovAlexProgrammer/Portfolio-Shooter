using System.IO;
using _Project.Scripts.Main.Localizations;
using _Project.Scripts.Main.Services;
using UnityEngine;
using Zenject;
using static _Project.Scripts.Main.Services.Services;

namespace _Project.Scripts.Main.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private SceneLoaderService _sceneLoaderServicePrefab;
        [SerializeField] private ScreenService _screenServicePrefab;
        [SerializeField] private SettingsService _settingsServicePrefab;
        [SerializeField] private GameManagerService _gameManagerServicePrefab;
        [SerializeField] private LocalizationService _localizationServicePrefab;
        [SerializeField] private ControlService _controlServicePrefab;
        [SerializeField] private DebugService _debugServicePrefab;

        public override void InstallBindings()
        {
            Application.logMessageReceived += LogToFile;
            InstallSceneLoaderService();
            InstallScreenService();
            InstallGameManagerService();
            InstallSettingService();
            InstallLocalizationService();
            InstallControlService();
            InstallDebugService();
        }

        private void InstallDebugService()
        {
            Container
                .Bind<DebugService>()
                .FromComponentInNewPrefab(_debugServicePrefab)
                .AsSingle()
                .OnInstantiated((ctx, instance) => SetService((DebugService)instance))
                .NonLazy();
        }

        private void LogToFile(string condition, string stacktrace, LogType type)
        {
            var path = Application.persistentDataPath + "/log.txt";
            using var streamWriter = File.AppendText(path);
            streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
            streamWriter.WriteLine($"{condition}");
            streamWriter.WriteLine("----");
            streamWriter.WriteLine($"{stacktrace}");
            streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        }

        private void InstallControlService()
        {
            Container
                .Bind<ControlService>()
                .FromComponentInNewPrefab(_controlServicePrefab)
                .AsSingle()
                .OnInstantiated((ctx, instance) =>
                {
                    var service = instance as ControlService;
                    service.Init();
                })
                .NonLazy();
        }

        private void InstallSettingService()
        {
            Container
                .Bind<SettingsService>()
                .FromComponentInNewPrefab(_settingsServicePrefab)
                .AsSingle()
                .OnInstantiated((ctx, instance) =>
                {
                    var service = (SettingsService)instance;
                    service.Init();
                    service.Load();
                });
        }

        private void InstallScreenService()
        {
            Container
                .Bind<ScreenService>()
                .FromComponentInNewPrefab(_screenServicePrefab)
                .AsSingle()
                .OnInstantiated((ctx, instance) => SetService((ScreenService)instance))
                .NonLazy();
        }

        private void InstallSceneLoaderService()
        {
           Container
                .Bind<SceneLoaderService>()
                .FromComponentInNewPrefab(_sceneLoaderServicePrefab)
                .AsSingle()
                .NonLazy();
        }

        private void InstallGameManagerService()
        {
            Container
                .Bind<GameManagerService>()
                .FromComponentInNewPrefab(_gameManagerServicePrefab)
                .AsSingle()
                .OnInstantiated((ctx, instance) =>
                {
                    var service = instance as GameManagerService;
                    SetService(service);
                    service.Init();
                })
                .NonLazy();
        }
        
        void InstallLocalizationService()
        {
            Container
                .Bind<LocalizationService>()
                .FromComponentInNewPrefab(_localizationServicePrefab)
                .AsSingle()
                .OnInstantiated((ctx, instance) => (instance as LocalizationService).Init())
                .NonLazy();
        }
    }
}

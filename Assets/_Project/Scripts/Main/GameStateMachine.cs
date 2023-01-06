using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using _Project.Scripts.Main.Services;
using static _Project.Scripts.Extension.Common;
using SceneName = _Project.Scripts.Main.Services.SceneLoaderService.Scenes;

namespace _Project.Scripts.Main
{
    public class GameStateMachine : MonoBehaviour
    {
        public Action StateChanged;
        
        private GameStates _activeState;
        
        [Inject] private SceneLoaderService _sceneLoader;
        [Inject] private ControlService _controlService;

        public GameStates ActiveState => _activeState;

        public void Init()
        {
            if (_sceneLoader.InitialSceneEquals(SceneName.Boot))
            {
                _ = EnterState(GameStates.Boot);
                return;
            }
            
            _ = EnterState(GameStates.CustomSceneBoot);
        }

        public async void SetState(GameStates newState)
        {
            await ExitState(_activeState);
            await EnterState(newState);
            StateChanged?.Invoke();
        }

        private async UniTask EnterState(GameStates newState)
        {
            Debug.Log("GameState Enter: " + newState, this);
            switch (newState)
            {
                case GameStates.CustomSceneBoot:
                    EnterStateCustomBoot();
                    break;
                case GameStates.Boot:
                    await EnterStateBoot();
                    break;
                case GameStates.MainMenu:
                    EnterStateMainMenu();
                    break;
                case GameStates.PlayGame:
                    EnterStatePlayGame();
                    break;
                case GameStates.GamePause:
                    break;
                case GameStates.GameQuit:
                    break;
                default:
                    throw new Exception("GameManager: unknown state.");
            }
        }
        
        private async UniTask ExitState(GameStates oldState)
        {
            Debug.Log("GameState ExitState: " + oldState, this);
            switch (oldState)
            {
                case GameStates.Boot:
                    await ExitStateBoot();
                    break;
                case GameStates.MainMenu:
                    break;
                case GameStates.PlayGame:
                    ExitStatePlayGame();
                    break;
                case GameStates.GamePause:
                    break;
                case GameStates.CustomSceneBoot:
                    break;
                default:
                    throw new Exception("GameManager: unknown state.");
            }
        }

        private async UniTask EnterStateBoot()
        {
            _sceneLoader.ShowScene();
            await Wait(1f);
            SetState(GameStates.MainMenu);
        }

        private void EnterStateCustomBoot()
        {
           _sceneLoader.ShowScene();
        }

        private void EnterStatePlayGame()
        {
            _controlService.LockCursor();
            _sceneLoader.LoadSceneAsync(SceneName.MiniGameLevel);
        }
        
        private void ExitStatePlayGame()
        {
            _controlService.UnlockCursor();
        }

        private async UniTask ExitStateBoot()
        {
            await UniTask.Delay(1);
        }

        private void EnterStateMainMenu()
        {
            _sceneLoader.LoadSceneAsync(SceneName.MainMenu);
        }
    }

    public enum GameStates
    {
        CustomSceneBoot, Boot, MainMenu, PlayGame, GamePause, GameQuit
    }
}
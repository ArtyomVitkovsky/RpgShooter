using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.Core.Services
{
    public interface IScenesService
    {
        List<string> LoadedScenes { get; }
        event Action<Scene> SceneLoaded;

        bool IsLoaded(string sceneName);

        UniTask LoadScene(
            string sceneName,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            Action<DiContainer> extraBindings = null
        );

        UniTask UnloadScene(string sceneName);
    }

    public class ScenesService : IScenesService
    {
        private readonly HashSet<string> _loadedScenes = new HashSet<string>();
    
        public List<string> LoadedScenes => _loadedScenes.ToList();

        public event Action<Scene> SceneLoaded = scene => { };

        private bool isLoading = false;

        private readonly DiContainer _parentResolver;

        public ScenesService(DiContainer parentResolver)
        {
            _parentResolver = parentResolver;
        }

        public bool IsLoaded(string sceneName) => _loadedScenes.Contains(sceneName);

        public async UniTask LoadScene(
            string sceneName,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            Action<DiContainer> extraBindings = null
        )
        {
            if (loadMode == LoadSceneMode.Single)
            {
                _loadedScenes.Clear();
            }

            if (_loadedScenes.Contains(sceneName))
            {
                throw new Exception($"Scene {sceneName} is already loaded");
            }

            if (isLoading)
            {
                await UniTask.WaitWhile(() => isLoading);
            }

            isLoading = true;

            try
            {
                var resolver = _parentResolver;

                extraBindings?.Invoke(resolver);

                var loadOperation = SceneManager.LoadSceneAsync(sceneName, loadMode);
                while (!loadOperation.isDone)
                {
                    await UniTask.Yield();
                }

                SceneLoaded(SceneManager.GetSceneByName(sceneName));

                _loadedScenes.Add(sceneName);
            }
            finally
            {
                isLoading = false;
            }
        }

        public async UniTask UnloadScene(string sceneName)
        {
            if (!_loadedScenes.Contains(sceneName))
            {
                return;
            }

            _loadedScenes.Remove(sceneName);

            var unloadOperation = SceneManager.UnloadSceneAsync(sceneName);
            if (unloadOperation != null)
            {
                await unloadOperation.ToUniTask();
            }
        }
    }
}
using System;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Core.Services
{
    public interface ISavingService
    {
        void Save<T>(string key, T data);

        T Load<T>(string key, T defaultValue = default);

        bool HasKey(string key);

        void DeleteKey(string key);

        void ClearAll();
    }

    public class SavingServiceInstaller : Installer<SavingServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerPrefsSavingService>().AsSingle().NonLazy();
        }
    }
    
    public class PlayerPrefsSavingService : ISavingService
    { 
        [Serializable]
        private class SaveData<T> { public T value; }

        public void Save<T>(string key, T data)
        {
            var wrapper = new SaveData<T> { value = data };
            var json = JsonUtility.ToJson(wrapper);
    
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public T Load<T>(string key, T defaultValue = default)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return defaultValue;
            }
            
            try
            {
                var json = PlayerPrefs.GetString(key);
                return JsonUtility.FromJson<SaveData<T>>(json).value;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load data for key {key}: {ex.Message}");
                return defaultValue;
            }
        }

        public bool HasKey(string key) => PlayerPrefs.HasKey(key);

        public void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);

        public void ClearAll() => PlayerPrefs.DeleteAll();
    }
}
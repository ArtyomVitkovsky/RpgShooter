using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameTemplate.UI
{
    public class UIStackNavigator : UINavigator
    {
        [Inject] private DiContainer diContainer;
        
        private Transform screensContainer;
        
        private List<UIStackScreen> registeredScreens = new List<UIStackScreen>();

        private readonly Stack<UIStackScreen> screenStack = new Stack<UIStackScreen>();

        private readonly Dictionary<System.Type, UIStackScreen> prefabByType = new Dictionary<System.Type, UIStackScreen>();

        public UIStackNavigator(List<UIStackScreen> screens, Transform container)
        {
            registeredScreens = screens;
            screensContainer = container;
        
            prefabByType.Clear();
            foreach (UIStackScreen screenPrefab in registeredScreens)
            {
                if (screenPrefab == null)
                {
                    continue;
                }

                System.Type type = screenPrefab.GetType();
                if (!prefabByType.ContainsKey(type))
                {
                    prefabByType.Add(type, screenPrefab);
                }
            }
        }

        public T Push<T>() where T : UIStackScreen
        {
            return Push<T>(null);
        }

        public T Push<T>(System.Action<DiContainer> initializer) where T : UIStackScreen
        {
            UIStackScreen prefab = GetPrefabForType<T>();
            if (prefab == null)
            {
                Debug.LogError($"[UIStackNavigator] No registered prefab found for stack screen type {typeof(T).Name}.");
                return null;
            }

            return ShowStackScreen(prefab, initializer) as T;
        }

        public void Pop()
        {
            if (screenStack.Count == 0)
            {
                return;
            }

            UIStackScreen top = screenStack.Pop();
            top.OnHide();
            top.OnCloseRequested -= HandleScreenCloseRequested;
            InstanceManager.DestroyInstance(top);

            if (screenStack.Count > 0)
            {
                UIStackScreen previous = screenStack.Peek();
                previous.OnShow();
            }
        }

        public T Replace<T>() where T : UIStackScreen
        {
            return Replace<T>(null);
        }

        public T Replace<T>(System.Action<DiContainer> initializer) where T : UIStackScreen
        {
            Pop();
            return Push<T>(initializer);
        }

        public void ClearStack()
        {
            while (screenStack.Count > 0)
            {
                UIStackScreen screen = screenStack.Pop();
                screen.OnHide();
                screen.OnCloseRequested -= HandleScreenCloseRequested;
                InstanceManager.DestroyInstance(screen);
            }
        }

        private UIStackScreen GetPrefabForType<T>() where T : UIStackScreen
        {
            System.Type type = typeof(T);
            if (prefabByType.TryGetValue(type, out UIStackScreen prefab))
            {
                return prefab;
            }

            foreach (UIStackScreen candidate in registeredScreens)
            {
                if (candidate != null && candidate is T)
                {
                    return candidate;
                }
            }

            return null;
        }

        private UIStackScreen ShowStackScreen(UIStackScreen prefab, System.Action<DiContainer> initializer = null)
        {
            if (prefab == null)
            {
                Debug.LogError("[UIStackNavigator] Stack screen prefab is null.");
                return null;
            }

            if (screenStack.Count > 0)
            {
                UIStackScreen top = screenStack.Peek();
                top.OnHide();
            }

            UIStackScreen instance = InstanceManager.CreateInstance(diContainer ,prefab, screensContainer, this, initializer);
            
            if (instance == null)
            {
                Debug.LogError("[UIStackNavigator] Failed to create stack screen instance.");
                return null;
            }

            instance.OnCloseRequested += HandleScreenCloseRequested;
            instance.OnShow();
            screenStack.Push(instance);

            return instance;
        }

        private void HandleScreenCloseRequested(UIScreenBase screen)
        {
            if (screen is UIStackScreen)
            {
                Pop();
            }
        }
    }
}



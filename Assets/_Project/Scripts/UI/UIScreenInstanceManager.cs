using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameTemplate.UI
{
    public class UIScreenInstanceManager
    {
        [Inject] private DiContainer diContainer;

        public TScreen CreateInstance<TScreen, TNavigator>
            (DiContainer navigatorContainer, TScreen prefab, Transform parent, TNavigator navigator, Action<DiContainer> initializer = null)
            where TScreen : UIScreenBase
            where TNavigator : UINavigator
        {
            if (prefab == null)
            {
                Debug.LogError("[UIScreenInstanceManager] Prefab is null. Cannot create screen instance.");
                return null;
            }

            if (parent == null)
            {
                Debug.LogError("[UIScreenInstanceManager] Parent is null. Cannot create screen instance.");
                return null;
            }
            
            TScreen instance = TryGetPooledInstance(prefab, parent);

            if (instance == null)
            {
                var subContainer = navigatorContainer.CreateSubContainer();

                initializer?.Invoke(subContainer);

                instance = subContainer.InstantiatePrefabForComponent<TScreen>(prefab, parent);

                instance.Initialize();
                instance.SetNavigator(navigator);
            }
            
            instance.OnHide();
            return instance;
        }

        public void DestroyInstance(UIScreenBase instance)
        {
            if (instance == null)
            {
                return;
            }

            if (instance.IsPoolable)
            {
                ReturnToPool(instance);
            }
            else
            {
                GameObject.Destroy(instance.gameObject);
            }
        }

        private readonly Dictionary<Type, List<UIScreenBase>> _pools = new Dictionary<Type, List<UIScreenBase>>();

        private TScreen TryGetPooledInstance<TScreen>(TScreen prefab, Transform parent) where TScreen : UIScreenBase
        {
            if (prefab == null || !prefab.IsPoolable)
            {
                return null;
            }

            Type key = prefab.GetType();
            if (_pools.TryGetValue(key, out List<UIScreenBase> pool))
            {
                for (int i = 0; i < pool.Count; i++)
                {
                    UIScreenBase candidate = pool[i];
                    if (candidate != null && !candidate.gameObject.activeSelf)
                    {
                        if (candidate.transform.parent != parent)
                        {
                            candidate.transform.SetParent(parent, false);
                        }

                        return candidate as TScreen;
                    }
                }
            }

            return null;
        }

        private void ReturnToPool(UIScreenBase instance)
        {
            if (instance == null)
            {
                return;
            }

            Type key = instance.GetType();
            if (!_pools.TryGetValue(key, out List<UIScreenBase> pool))
            {
                pool = new List<UIScreenBase>();
                _pools.Add(key, pool);
            }

            if (!pool.Contains(instance))
            {
                pool.Add(instance);
            }

            instance.OnHide();
        }
    }
}



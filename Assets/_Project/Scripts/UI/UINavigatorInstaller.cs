using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameTemplate.UI
{
    public abstract class UINavigatorInstaller<T> : MonoInstaller where T : UIScreenBase
    {
        [SerializeField] protected string navigatorId;
        [SerializeField] protected Transform contianer;
        [SerializeField] protected List<T> screens;

        public override void InstallBindings()
        {
            BindDependencies();
        }

        protected abstract void BindDependencies();
    }
}
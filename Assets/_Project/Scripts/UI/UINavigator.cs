using UnityEngine;
using Zenject;

namespace GameTemplate.UI
{
    public abstract class UINavigator
    {
        [Inject] protected UIScreenInstanceManager InstanceManager;
    }
}
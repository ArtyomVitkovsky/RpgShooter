using UnityEngine;
using Zenject;

namespace GameTemplate.UI
{
    public abstract class UIQueueScreen : UIScreenBase
    {
        public UIQueueNavigator Navigator { get; private set; } 
            
        public override void SetNavigator<TNavigator>(TNavigator navigator)
        {
            Navigator = navigator as UIQueueNavigator;
        }
    }
}



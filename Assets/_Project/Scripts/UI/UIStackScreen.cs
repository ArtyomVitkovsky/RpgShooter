using UnityEngine;
using Zenject;

namespace GameTemplate.UI
{
    public abstract class UIStackScreen : UIScreenBase
    {
        public UIStackNavigator Navigator { get; private set; }

        public override void SetNavigator<TNavigator>(TNavigator navigator)
        {
            Navigator = navigator as UIStackNavigator;
        }

    }
}



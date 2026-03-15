using System.Collections.Generic;
using UnityEngine;

namespace GameTemplate.UI
{
    public class UIStackNavigatorInstaller : UINavigatorInstaller<UIStackScreen>
    {
        protected override void BindDependencies()
        {
            Container.Bind<UIStackNavigator>()
                .WithId(navigatorId)
                .AsCached()
                .WithArguments(screens, contianer)
                .NonLazy();
        }
    }
}
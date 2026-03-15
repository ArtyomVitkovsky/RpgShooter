namespace GameTemplate.UI
{
    public class UIQueueNavigatorInstaller : UINavigatorInstaller<UIQueueScreen>
    {
        protected override void BindDependencies()
        {
            Container.Bind<UIQueueNavigator>()
                .WithId(navigatorId)
                .AsCached()
                .WithArguments(screens, contianer)
                .NonLazy();
        }
    }
}
using UI.Views;
using VContainer;
using VContainer.Unity;


public class DIInstaller : LifetimeScope
{
    public override void Configure(IContainerBuilder _builder)
    {
        _builder.RegisterComponentInHierarchy<ViewRepository>();
    }
}

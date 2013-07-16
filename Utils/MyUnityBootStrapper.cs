using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using Rhino.ServiceBus.Actions;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Unity;

namespace Utils
{
  public abstract class MyUnityBootStrapper : UnityBootStrapper
  {
    private IUnityContainer container;

    protected MyUnityBootStrapper() : base()
    {
    }

    protected MyUnityBootStrapper(IUnityContainer container) : base(container)
    {
      this.container = container;
    }

    public override void CreateContainer()
    {
      if (container == null)
        container = new UnityContainer();

      ConfigureContainer();
    }

    protected override void ConfigureContainer()
    {
      foreach (var assembly in Assemblies)
      {
        container.RegisterTypesFromAssembly<IDeploymentAction>(assembly);
        container.RegisterTypesFromAssembly<IEnvironmentValidationAction>(assembly);
        ConfigureConsumers(assembly);
      }
    }

    protected override void ConfigureConsumers(Assembly assembly)
    {
      var consumers = assembly.GetTypes().Where(type =>
                                                typeof(IMessageConsumer).IsAssignableFrom(type) &&
                                                !typeof(IOccasionalMessageConsumer).IsAssignableFrom(type) &&
                                                IsTypeAcceptableForThisBootStrapper(type)).ToList();
      consumers.ForEach(consumer => container.RegisterType(typeof(IMessageConsumer), consumer, consumer.FullName, new TransientLifetimeManager()));
    }
  }
}
using Autofac;
using Sample.UserManagement.Domain.Repositories;

namespace Sample.UserManagement
{
    public class UserServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>().AsImplementedInterfaces().SingleInstance();
        }
    }
}

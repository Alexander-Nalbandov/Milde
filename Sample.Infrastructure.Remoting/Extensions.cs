using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Sample.Infrastructure.Remoting
{
    public static class Extensions
    {
        public static ContainerBuilder WithRemoting(
            this ContainerBuilder builder, Action<bool> action)
        {
            return builder;
        }
    }
}

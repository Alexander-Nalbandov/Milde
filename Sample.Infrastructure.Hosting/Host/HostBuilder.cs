using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.Hosting.Service;
using Serilog;

namespace Sample.Infrastructure.Hosting.Host
{
    public class HostBuilder : IHostBuilder
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();
        private IConfiguration _configuration = new ConfigurationRoot(new List<IConfigurationProvider>());


        private HostBuilder()
        {
        }


        public static IHostBuilder Create()
        {
            return new HostBuilder();
        }


        public IHostBuilder WithConfiguration(Action<IConfigurationBuilder> configurator)
        {
            var builder = new ConfigurationBuilder();
            configurator(builder);

            this._configuration = builder.Build();

            this._builder.RegisterInstance(this._configuration).AsImplementedInterfaces().SingleInstance();

            return this;
        }

        public IHostBuilder WithLogger(Func<LoggerConfiguration, LoggerConfiguration> loggerConfigurator)
        {
            var config = new LoggerConfiguration();
            config = loggerConfigurator(config);

            var logger = config.CreateLogger();
            this._builder.RegisterInstance(logger).AsImplementedInterfaces().SingleInstance();

            return this;
        }

        public IHostBuilder With(Action<ContainerBuilder> serviceConfigurator)
        {
            serviceConfigurator(this._builder);
            return this;
        }

        public IHostBuilder With(Action<ContainerBuilder, IConfiguration> serviceConfigurator)
        {
            serviceConfigurator(this._builder, this._configuration);
            return this;
        }

        public IHostBuilder WithService<TService>() where TService : IService, new()
        {
            var service = new TService();
            service.RegisterDependencies(this._builder, this._configuration);

            return this;
        }

        public IHost Build()
        {
            var container = this._builder.Build();
            return new Host(container, this._configuration);
        }
    }
}

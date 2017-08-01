﻿// ———————————————————————————————
// <copyright file="Bootstrap.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides method(s) to bootstrap the dependency injection framework.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using Autofac;
    using Autofac.Extras.AttributeMetadata;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Provides method(s) to bootstrap the dependency injection framework.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Bootstrap
    {
        /// <summary>
        /// Builds a <see cref="IContainer"/> that has all the necessary types registered to run the application.
        /// </summary>
        /// <param name="configurationProvider">The provider used to access configuration information.</param>
        /// <param name="isDebugging">Flag that indicates if the application is in debugging modus.</param>
        /// <returns>A <see cref="IContainer"/>.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Bootstrapper for Autofac. So it is intented to hit all needed dependencies in one place.")]
        public static IContainer Build(IConfigurationProvider configurationProvider, bool isDebugging)
        {
            configurationProvider.ThrowIfNull(nameof(configurationProvider));

            var builder = new ContainerBuilder();

            builder
                .RegisterType<DialogInvoker>()
                .As<IDialogInvoker>();

            builder
                .RegisterInstance(configurationProvider);

            builder
                .RegisterModule<AttributedMetadataModule>();

            // Using a Telemetry Client per request, so user context, etc is unique per request.
            builder
                .RegisterType<TelemetryClient>()
                .SingleInstance();

            var microsoftAppCredentials = new MicrosoftAppCredentials(
                configurationProvider.GetValue(ConfigurationSettingName.MicrosoftApplicationId),
                configurationProvider.GetValue(ConfigurationSettingName.MicrosoftApplicationPassword));

            // When debugging with the bot emulator we need to use the listening url from the emulator.
            if (isDebugging && !string.IsNullOrEmpty(configurationProvider.GetValue(ConfigurationSettingName.EmulatorListeningUrl)))
            {
                builder.Register(c => new StateClient(
                    new Uri(configurationProvider.GetValue(ConfigurationSettingName.EmulatorListeningUrl)), microsoftAppCredentials));
            }
            else
            {
                builder.Register(c => new StateClient(microsoftAppCredentials));
            }

            builder
                .RegisterType<BotState>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<AuthenticationService>()
                .WithParameter("appSecret", configurationProvider.GetValue(ConfigurationSettingName.ApplicationSecret))
                .WithParameter("authorizeUrl", new Uri(configurationProvider.GetValue(ConfigurationSettingName.AuthorizeUrl)))
                .AsImplementedInterfaces();

            builder
                .RegisterType<BotService>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<VstsService>()
                .AsImplementedInterfaces();

            builder
                .RegisterControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterApiControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterAssemblyTypes(typeof(Bootstrap).Assembly)
                .Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IDialog<object>))))
                .Except<ConnectDialog>()
                .Except<RootDialog>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<ConnectDialog>()
                .WithParameter("appId", configurationProvider.GetValue(ConfigurationSettingName.ApplicationId))
                .WithParameter("appScope", configurationProvider.GetValue(ConfigurationSettingName.ApplicationScope))
                .WithParameter("authorizeUrl", new Uri(configurationProvider.GetValue(ConfigurationSettingName.AuthorizeUrl)))
                .AsImplementedInterfaces();

            builder
                .RegisterType<RootDialog>()
                .AsSelf();

            return builder.Build();
        }
    }
}
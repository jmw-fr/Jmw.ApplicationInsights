// <copyright file="DependencyInjectionExtensions.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Dawn;
    using Jmw.ApplicationInsights.Telemetry;
    using Jmw.ApplicationInsights.Telemetry.ActivityListeners;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Extensions to configure Telemery.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Jmw.ApplicationInsights section name in app settings.
        /// </summary>
        public const string JmwApplicationInsightsSectionName = "Jmw.ApplicationInsights";

        private static bool optionsRegistered = false;

        /// <summary>
        /// Register an activity listener to the DI service.
        /// </summary>
        /// <typeparam name="TService">Type of the listener.</typeparam>
        /// <param name="services">Instance of DI service.</param>
        /// <param name="configuration">Instance of configuration service.</param>
        /// <returns>Modified DI service.</returns>
        public static IServiceCollection AddActivityListener<TService>(this IServiceCollection services, IConfiguration configuration)
            where TService : class, IActivityListener
        {
            Guard.Argument(services, nameof(services)).NotNull();

            services.AddSingleton<IActivityListener, TService>();

            RegisterOptions(services, configuration);

            return services;
        }

        /// <summary>
        /// Register an activity listener to the DI service using a factory.
        /// </summary>
        /// <typeparam name="TService">Type of the listener.</typeparam>
        /// <param name="services">Instance of DI service.</param>
        /// <param name="implementationFactory">Service factory.</param>
        /// <param name="configuration">Instance of configuration service.</param>
        /// <returns>Modified DI service.</returns>
        public static IServiceCollection AddActivityListener<TService>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            IConfiguration configuration)
            where TService : class, IActivityListener
        {
            Guard.Argument(services, nameof(services)).NotNull();
            Guard.Argument(implementationFactory, nameof(implementationFactory)).NotNull();

            services.AddSingleton<IActivityListener, TService>(implementationFactory);

            RegisterOptions(services, configuration);

            return services;
        }

        private static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
        {
            if (!optionsRegistered)
            {
                services.Configure<ListenerOptions>(
                    configuration.GetSection(JmwApplicationInsightsSectionName));

                optionsRegistered = true;
            }
        }
    }
}

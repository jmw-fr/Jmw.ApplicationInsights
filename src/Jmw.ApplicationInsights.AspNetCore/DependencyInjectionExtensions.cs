// <copyright file="DependencyInjectionExtensions.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Dawn;
    using Jmw.ApplicationInsights.Telemetry;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    /// <summary>
    /// Extensions to configure Telemery.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Register an activity listener to the DI service.
        /// </summary>
        /// <typeparam name="T">Type of the listener.</typeparam>
        /// <param name="services">Instance of DI service.</param>
        /// <returns>Modified DI service.</returns>
        public static IServiceCollection AddActivityListener<T>(this IServiceCollection services)
            where T : class, IActivityListener
        {
            Guard.Argument(services, nameof(services)).NotNull();

            services.AddSingleton<IActivityListener, T>();

            return services;
        }
    }
}

// <copyright file="ConfigurationBuilderExtensions.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Microsoft.AspNetCore.Builder
{
    using System.Diagnostics;
    using Dawn;
    using Jmw.ApplicationInsights.Telemetry;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions for configuring the listers.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Configure Activity Listeners.
        /// </summary>
        /// <param name="builder">Configuration builder.</param>
        /// <returns>Modified configuration builder.</returns>
        public static IApplicationBuilder ConfigureActivityListeners(this IApplicationBuilder builder)
        {
            Guard.Argument(builder, nameof(builder)).NotNull();

            foreach (var listener in builder.ApplicationServices.GetServices<IActivityListener>())
            {
                ActivitySource.AddActivityListener(listener.ActivitySourceListener);
            }

            return builder;
        }
    }
}

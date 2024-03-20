// <copyright file="IPInitializer.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.Initializers
{
    using Dawn;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// Initializer adding the IP address to telemetry.
    /// </summary>
    public class IPInitializer : ITelemetryInitializer
    {
        private readonly string propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPInitializer"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the telemetry property.</param>
        public IPInitializer(string propertyName = "ip")
        {
            this.propertyName = Guard.Argument(propertyName).NotNull().NotEmpty();
        }

        /// <inheritdoc/>
        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is ISupportProperties supportPropertiesTelemetry
                && !supportPropertiesTelemetry.Properties.ContainsKey(this.propertyName))
            {
                string ip = Helpers.IPHelpers.GetDefaultIPv4Address()?.ToString();

                if (!string.IsNullOrWhiteSpace(ip))
                {
                    supportPropertiesTelemetry.Properties[this.propertyName] = ip;
                }
            }
        }
    }
}

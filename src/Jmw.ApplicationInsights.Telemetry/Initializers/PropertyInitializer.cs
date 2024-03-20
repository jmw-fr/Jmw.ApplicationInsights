// <copyright file="PropertyInitializer.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.Initializers
{
    using System.Collections.Generic;
    using System.Linq;
    using Dawn;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// Initializer in charge of adding properties to telemetry.
    /// </summary>
    public abstract class PropertyInitializer : ITelemetryInitializer
    {
        private readonly string[] keys;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInitializer"/> class.
        /// </summary>
        /// <param name="keys">Properties keys.</param>
        public PropertyInitializer(
            string[] keys)
        {
            this.keys = Guard.Argument(keys, nameof(keys)).NotNull().NotEmpty().Value;
        }

        /// <inheritdoc/>
        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is ISupportProperties supportPropertiesTelemetry)
            {
                var properties = this.keys
                    .Where(p => !supportPropertiesTelemetry.Properties.ContainsKey(p));

                if (properties.Any())
                {
                    var values = this.GetProperties(properties.AsEnumerable()) ?? Enumerable.Empty<KeyValuePair<string, string>>();

                    foreach (var value in values)
                    {
                        supportPropertiesTelemetry.Properties[value.Key] = value.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Abstract method to implement to retrieve properties values.
        /// </summary>
        /// <param name="properties">List of properties to retrieve.</param>
        /// <returns>Liste of properties retrieved and to be added to the telemetry.</returns>
        protected abstract IEnumerable<KeyValuePair<string, string>> GetProperties(IEnumerable<string> properties);
    }
}

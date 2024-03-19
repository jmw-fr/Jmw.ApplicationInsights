// <copyright file="PerformanceListener.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.ActivityListeners
{
    using System.Diagnostics;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// Activity listener used to capture performance telemetry.
    /// </summary>
    /// <remarks>
    /// The listener uses <see cref="RequestTelemetry"/> to meter performance and creates a request named "activitysource:activityname".
    /// </remarks>
    public class PerformanceListener : ActivityListener<RequestTelemetry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceListener"/> class.
        /// </summary>
        /// <param name="telemetryClient">Instance of the telemery client.</param>
        /// <param name="activitySourceName">Name of the ActivitySource.</param>
        public PerformanceListener(
            TelemetryClient telemetryClient,
            string activitySourceName)
            : base(telemetryClient, s => s.Name == activitySourceName)
        {
        }

        /// <inheritdoc/>
        protected override bool ProcessTelemetry(RequestTelemetry telemetry, Activity sourceActivity)
        {
            var result = base.ProcessTelemetry(telemetry, sourceActivity);

            // Overriding the Name and OperationName of the request telemetry.
            telemetry.Name = $"{sourceActivity.Source.Name}:{sourceActivity.OperationName}";
            telemetry.Context.Operation.Name = $"{sourceActivity.Source.Name}:{sourceActivity.OperationName}";
            return result;
        }
    }
}

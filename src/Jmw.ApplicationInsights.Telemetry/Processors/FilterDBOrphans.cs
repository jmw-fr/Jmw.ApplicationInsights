// <copyright file="FilterDBOrphans.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.Processors
{
    using Dawn;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// App Insight processor that filters SQL <see cref="DependencyTelemetry"/> with no parent.
    /// </summary>
    public class FilterDBOrphans : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor next;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterDBOrphans"/> class.
        /// </summary>
        /// <param name="next">Next processor.</param>
        public FilterDBOrphans(ITelemetryProcessor next)
        {
            this.next = Guard.Argument(next, nameof(next)).NotNull().Value;
        }

        /// <inheritdoc/>
        public void Process(ITelemetry item)
        {
            if (item is DependencyTelemetry dependencyTelemetry)
            {
                if (
                    dependencyTelemetry.Context.Operation.ParentId == null
                    && "SQL".Equals(dependencyTelemetry.Type, System.StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            this.next.Process(item);
        }
    }
}

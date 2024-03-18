// <copyright file="HttpDependencyFilter.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Dawn;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// Application Insight filtering Http <see cref="DependencyTelemetry" /> from the Url.
    /// </summary>
    public class HttpDependencyFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor nextProcessor;
        private readonly IEnumerable<Regex> excludePaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpDependencyFilter"/> class.
        /// </summary>
        /// <param name="next">Processeur suivant.</param>
        /// <param name="excludePaths">Regexps specifying the paths to exclude.</param>
        public HttpDependencyFilter(
            ITelemetryProcessor next,
            IEnumerable<string> excludePaths)
            : this(next, excludePaths?.Select(p => new Regex(p)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpDependencyFilter"/> class.
        /// </summary>
        /// <param name="next">Processeur suivant.</param>
        /// <param name="excludePaths">Regexps specifying the paths to exclude.</param>
        public HttpDependencyFilter(
            ITelemetryProcessor next,
            IEnumerable<Regex> excludePaths)
        {
            this.nextProcessor = Guard.Argument(next, nameof(next)).NotNull().Value;
            this.excludePaths = Guard.Argument(excludePaths, nameof(excludePaths)).NotNull().Value;
        }

        /// <inheritdoc/>
        public void Process(ITelemetry telemetry)
        {
            if (telemetry is DependencyTelemetry request
                && request.Type == "Http")
            {
                if (this.excludePaths
                    .Any(p =>
                        request.Data != null
                        && p.IsMatch(request.Data)))
                {
                    return;
                }
            }

            this.nextProcessor.Process(telemetry);
        }
    }
}

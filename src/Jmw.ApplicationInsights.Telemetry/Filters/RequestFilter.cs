// <copyright file="RequestFilter.cs" company="Jean-Marc Weeger">
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
    /// Application Insight filtering <see cref="RequestTelemetry" /> from the Url.
    /// </summary>
    public class RequestFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor nextProcessor;
        private readonly IEnumerable<Regex> excludePaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFilter"/> class.
        /// </summary>
        /// <param name="next">Processeur suivant.</param>
        /// <param name="excludePaths">Regexps specifying the paths to exclude.</param>
        public RequestFilter(
            ITelemetryProcessor next,
            IEnumerable<string> excludePaths)
            : this(next, excludePaths?.Select(p => new Regex(p)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestFilter"/> class.
        /// </summary>
        /// <param name="next">Processeur suivant.</param>
        /// <param name="excludePaths">Regexps specifying the paths to exclude.</param>
        public RequestFilter(
            ITelemetryProcessor next,
            IEnumerable<Regex> excludePaths)
        {
            this.nextProcessor = Guard.Argument(next, nameof(next)).NotNull().Value;
            this.excludePaths = Guard.Argument(excludePaths, nameof(excludePaths)).NotNull().Value;
        }

        /// <inheritdoc/>
        public void Process(ITelemetry telemetry)
        {
            if (telemetry is RequestTelemetry request)
            {
                if (this.excludePaths
                    .Any(p =>
                        request.Url != null
                        && request.Url.LocalPath != null
                        && p.IsMatch(request.Url.LocalPath)))
                {
                    return;
                }
            }

            this.nextProcessor.Process(telemetry);
        }
    }
}

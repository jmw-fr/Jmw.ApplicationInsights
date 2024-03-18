// <copyright file="IPFilter.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;
    using Dawn;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// Application Insight filtering any <see cref="ITelemetryProcessor" /> according to current computer v4 IP.
    /// </summary>
    public class IPFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor nextProcessor;
        private readonly IEnumerable<Regex> includeIPs;
        private readonly bool monitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPFilter"/> class.
        /// </summary>
        /// <param name="next">Processeur suivant.</param>
        /// <param name="includeIPs">Regexps specifying the IPs to include.</param>
        public IPFilter(
            ITelemetryProcessor next,
            IEnumerable<string> includeIPs)
            : this(next, includeIPs?.Select(p => new Regex(p)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPFilter"/> class.
        /// </summary>
        /// <param name="next">Processeur suivant.</param>
        /// <param name="includeIPs">Regexps specifying the IPs to include.</param>
        public IPFilter(
            ITelemetryProcessor next,
            IEnumerable<Regex> includeIPs)
        {
            this.nextProcessor = Guard.Argument(next, nameof(next)).NotNull().Value;
            this.includeIPs = Guard.Argument(includeIPs, nameof(includeIPs)).NotNull().Value;
            this.monitor = EvalShouldMonitor(this.includeIPs);
        }

        /// <inheritdoc/>
        public void Process(ITelemetry telemetry)
        {
            if (this.monitor)
            {
                this.nextProcessor.Process(telemetry);
            }
        }

        private static bool EvalShouldMonitor(IEnumerable<Regex> includeIPs)
        {
            var ips = Dns.GetHostEntry(Dns.GetHostName())
                                    .AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                                    .Select(ip => ip.ToString());

            return ips.Any(ip => includeIPs.Any(p => p.IsMatch(ip)));
        }
    }
}

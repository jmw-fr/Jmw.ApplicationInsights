﻿// <copyright file="NpgsqlListener.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.ActivityListeners
{
    using System.Diagnostics;
    using Jmw.ApplicationInsights.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// Listener for npgsql activity. See https://github.com/npgsql/npgsql/blob/main/src/Npgsql/NpgsqlActivitySource.cs.
    /// </summary>
    public class NpgsqlListener : ActivityListener<DependencyTelemetry>
    {
        /// <summary>
        /// Npgsql Activity Source Name.
        /// </summary>
        public const string ActivitySourceName = "Npgsql";

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlListener"/> class.
        /// </summary>
        /// <param name="telemetryClient">Instance of the telemery client.</param>
        public NpgsqlListener(
            TelemetryClient telemetryClient)
            : base(telemetryClient, s => s.Name == ActivitySourceName)
        {
        }

        /// <inheritdoc/>
        protected override bool ProcessTelemetry(DependencyTelemetry telemetry, Activity sourceActivity)
        {
            string dbName = string.Empty;
            string dbServer = "localhost";
            string dbServerPort = "5432";
            string command = sourceActivity.OperationName;

            foreach (var baggage in sourceActivity.Tags)
            {
                switch (baggage.Key)
                {
                    case "db.statement":
                        command = baggage.Value;
                        break;
                    case "db.name":
                        dbName = baggage.Value;
                        break;
                    case "net.peer.name":
                        dbServer = baggage.Value;
                        break;
                    case "net.peer.ip":
                        dbServer = baggage.Value;
                        break;
                    case "net.peer.port":
                        dbServerPort = baggage.Value;
                        break;
                }
            }

            telemetry.Type = "SQL";
            telemetry.Target = $"pgsql:{dbServer},{dbServerPort} | {dbName}";
            telemetry.Data = command;

            return base.ProcessTelemetry(telemetry, sourceActivity);
        }
    }
}

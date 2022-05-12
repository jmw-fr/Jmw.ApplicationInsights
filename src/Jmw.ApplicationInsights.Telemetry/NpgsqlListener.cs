// <copyright file="NpgsqlListener.cs" company="Jean-Marc Weeger">
// Copyright My Company under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// Listener for npgsql activity. See https://github.com/npgsql/npgsql/blob/main/src/Npgsql/NpgsqlActivitySource.cs.
    /// </summary>
    public class NpgsqlListener : ActivityListener<DependencyTelemetry>
    {
        /// <summary>
        /// Npgsql Actity Name.
        /// </summary>
        public const string ActivityName = "Npgsql";

        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlListener"/> class.
        /// </summary>
        /// <param name="telemetryClient">Instance of the telemery client.</param>
        public NpgsqlListener(TelemetryClient telemetryClient)
            : base(telemetryClient, s => s.Name == ActivityName)
        {
        }

        /// <inheritdoc/>
        protected override void EnrichTelemetry(DependencyTelemetry telemetry, Activity sourceActivity)
        {
            base.EnrichTelemetry(telemetry, sourceActivity);

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
        }
    }
}

// <copyright file="MySQLConnectorListener.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// MySQL Connector Activity listener. See https://mysqlconnector.net/tutorials/tracing/.
    /// </summary>
    public class MySQLConnectorListener : ActivityListener<DependencyTelemetry>
    {
        /// <summary>
        /// MySQLConnector Actity Name.
        /// </summary>
        public const string ActivityName = "MySqlConnector";

        private readonly ListenerOptions listenerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySQLConnectorListener"/> class.
        /// </summary>
        /// <param name="telemetryClient">Instance of the telemery client.</param>
        /// <param name="listenerOptions">Options for the listener.</param>
        public MySQLConnectorListener(
            TelemetryClient telemetryClient,
            ListenerOptions listenerOptions = null)
            : base(telemetryClient, s => s.Name == ActivityName)
        {
            this.listenerOptions = listenerOptions;
        }

        /// <inheritdoc/>
        protected override bool ProcessTelemetry(DependencyTelemetry telemetry, Activity sourceActivity)
        {
            string dbName = string.Empty;
            string dbServer = "localhost";
            string dbServerPort = "3306";
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
            telemetry.Target = $"mysql:{dbServer},{dbServerPort} | {dbName}";
            telemetry.Data = command;

            if (this.listenerOptions != null)
            {
                if (this.listenerOptions.MonitorDatabases != null
                    && this.listenerOptions.MonitorDatabases.Length > 0
                    && !this.listenerOptions.MonitorDatabases.Contains(dbName, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (this.listenerOptions.IgnoreDatabases != null
                    && this.listenerOptions.IgnoreDatabases.Contains(dbName, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return base.ProcessTelemetry(telemetry, sourceActivity);
        }
    }
}

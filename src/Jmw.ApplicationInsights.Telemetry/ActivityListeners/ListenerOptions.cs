// <copyright file="ListenerOptions.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.ActivityListeners
{
    /// <summary>
    /// Options for listeners.
    /// </summary>
    public class ListenerOptions
    {
        /// <summary>
        /// Gets or sets the list of databases to monitor with databases listeners.
        /// If <c>null</c> or empty, monitors all databases.
        /// </summary>
        public string[] MonitorDatabases { get; set; }

        /// <summary>
        /// Gets or sets the list of databases to ignore for databases listeners.
        /// </summary>
        public string[] IgnoreDatabases { get; set; }
    }
}

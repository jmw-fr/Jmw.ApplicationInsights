// <copyright file="IPHelpers.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Helper class for IP addresses.
    /// </summary>
    internal class IPHelpers
    {
        /// <summary>
        /// Gets the IPv4 addresses.
        /// </summary>
        /// <returns>List of IPv4 addresses.</returns>
        public static IEnumerable<IPAddress> GetIPv4Addresses()
        {
            string hostName = Dns.GetHostName();

            return Dns.GetHostAddresses(hostName).Where(
                address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Returns the default IPv4 address.
        /// </summary>
        /// <returns>Default address.</returns>
        public static IPAddress GetDefaultIPv4Address()
        {
            return GetIPv4Addresses().FirstOrDefault();
        }
    }
}

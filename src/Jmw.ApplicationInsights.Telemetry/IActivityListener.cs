// <copyright file="IActivityListener.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry
{
    using System.Diagnostics;

    /// <summary>
    /// Defines the activity listener required properties/methods.
    /// </summary>
    public interface IActivityListener
    {
        /// <summary>
        /// Gets the instance of the activity listener to be added with <see cref="ActivitySource.AddActivityListener(ActivityListener)"/>.
        /// </summary>
        ActivityListener ActivitySourceListener { get; }
    }
}

// <copyright file="ActivityHelpers.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry.Activities
{
    using System;
    using System.Diagnostics;
    using Dawn;

    /// <summary>
    /// Helper methods for Activity.
    /// </summary>
    public static class ActivityHelpers
    {
        /// <summary>
        /// Sets the status of the activity to failed with exception message as description.
        /// </summary>
        /// <param name="activity">CurrentActivity.</param>
        /// <param name="exception">Exception thrown.</param>
        public static void SetStatusOnException(this Activity activity, Exception exception)
        {
            Guard.Argument(activity).NotNull();
            Guard.Argument(exception).NotNull();

            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        }
    }
}

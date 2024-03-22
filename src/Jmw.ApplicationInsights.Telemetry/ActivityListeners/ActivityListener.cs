// <copyright file="ActivityListener.cs" company="Jean-Marc Weeger">
// Copyright Jean-Marc Weeger under MIT Licence. See https://opensource.org/licenses/mit-license.php.
// </copyright>

namespace Jmw.ApplicationInsights.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Dawn;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    /// <summary>
    /// .Net listener mapping an activity to an Application Insights Telemetry.
    /// </summary>
    /// <typeparam name="TTelemetry">Type of Application Insight Telemetry.</typeparam>
    public class ActivityListener<TTelemetry> : IActivityListener, IDisposable
        where TTelemetry : OperationTelemetry, new()
    {
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityListener{TTelemetry}"/> class.
        /// </summary>
        /// <param name="telemetryClient">Instance of the telemery client.</param>
        /// <param name="activityFilter">Callback filtering the activity to monitor.</param>
        public ActivityListener(
            TelemetryClient telemetryClient,
            Func<ActivitySource, bool> activityFilter)
        {
            this.telemetryClient = Guard.Argument(telemetryClient, nameof(telemetryClient)).NotNull();
            Guard.Argument(activityFilter, nameof(activityFilter)).NotNull();

            this.ActivitySourceListener = new ActivityListener()
            {
                ActivityStarted = this.ActivityStarted,
                ActivityStopped = this.ActivityStopped,
                ShouldListenTo = activityFilter,
                SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData,
                Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
            };
        }

        /// <summary>
        /// Gets the instance of the activity listener to be added with <see cref="ActivitySource.AddActivityListener(ActivityListener)"/>.
        /// </summary>
        public ActivityListener ActivitySourceListener { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.ActivitySourceListener.Dispose();
        }

        /// <summary>
        /// Function allowing to filter and enrich telemetry with custom data.
        /// </summary>
        /// <param name="telemetry">Telemetry to enrich.</param>
        /// <param name="sourceActivity">Source activity.</param>
        /// <returns><c>true</c> if the telemetry is to be processed or <c>false</c> if it should be filtered.</returns>
        protected virtual bool ProcessTelemetry(TTelemetry telemetry, Activity sourceActivity)
        {
            return true;
        }

        private static TTelemetry ActivityToTelemetry(Activity activity)
        {
            // From Application Insights code source
            var telemetry = new TTelemetry
            {
                Name = activity.OperationName,
                Timestamp = activity.StartTimeUtc,
                Duration = activity.Duration,
                Success = activity.Status != ActivityStatusCode.Error,
            };

            if (activity.Status == ActivityStatusCode.Error && !string.IsNullOrEmpty(activity.StatusDescription))
            {
                telemetry.Properties.Add(nameof(activity.StatusDescription), activity.StatusDescription);
            }

            if (telemetry is RequestTelemetry requestTelemetry)
            {
                requestTelemetry.ResponseCode = activity.Status != ActivityStatusCode.Error
                    ? "200" : string.IsNullOrEmpty(activity.StatusDescription) ? "500" : activity.StatusDescription;
            }

            OperationContext operation = telemetry.Context.Operation;
            operation.Name = activity.OperationName;
            if (activity.IdFormat == ActivityIdFormat.W3C)
            {
                operation.Id = activity.TraceId.ToHexString();
                telemetry.Id = activity.SpanId.ToHexString();
                if (string.IsNullOrEmpty(operation.ParentId) && activity.ParentSpanId != default)
                {
                    operation.ParentId = activity.ParentSpanId.ToHexString();
                }
            }
            else
            {
                operation.Id = activity.RootId;
                operation.ParentId = activity.ParentId;
                telemetry.Id = activity.Id;
            }

            foreach (KeyValuePair<string, string> item in activity.Baggage)
            {
                if (!telemetry.Properties.ContainsKey(item.Key))
                {
                    telemetry.Properties.Add(item);
                }
            }

            foreach (KeyValuePair<string, string> tag in activity.Tags)
            {
                if (!telemetry.Properties.ContainsKey(tag.Key))
                {
                    telemetry.Properties.Add(tag);
                }
            }

            return telemetry;
        }

        private void ActivityStarted(Activity activity)
        {
        }

        private void ActivityStopped(Activity activity)
        {
            if (this.telemetryClient.IsEnabled())
            {
                var telemetry = ActivityToTelemetry(activity);

                if (!this.ProcessTelemetry(telemetry, activity))
                {
                    return;
                }

                if (telemetry is DependencyTelemetry)
                {
                    this.telemetryClient.TrackDependency(telemetry as DependencyTelemetry);
                }
                else if (telemetry is RequestTelemetry)
                {
                    this.telemetryClient.TrackRequest(telemetry as RequestTelemetry);
                }
                else if (telemetry is MetricTelemetry)
                {
                    this.telemetryClient.TrackMetric(telemetry as MetricTelemetry);
                }
                else if (telemetry is EventTelemetry)
                {
                    this.telemetryClient.TrackEvent(telemetry as EventTelemetry);
                }
            }
        }
    }
}

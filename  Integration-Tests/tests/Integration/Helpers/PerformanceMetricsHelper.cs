using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.IntegrationTests.Helpers
{
    public static class PerformanceMetricsHelper
    {
        public static async Task<TimeSpan> MeasureExecutionTimeAsync(
            Func<Task> actionToMeasure,
            int warmUpRuns = 0,
            Func<Task>? warmUpAction = null)
        {
            var actionToWarmUp = warmUpAction ?? actionToMeasure;
            for (int i = 0; i < warmUpRuns; i++)
            {
                await actionToWarmUp();
            }

            var stopwatch = Stopwatch.StartNew();
            await actionToMeasure();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static TimeSpan MeasureExecutionTime(
            Action actionToMeasure,
            int warmUpRuns = 0,
            Action? warmUpAction = null)
        {
            var actionToWarmUp = warmUpAction ?? actionToMeasure;
            for (int i = 0; i < warmUpRuns; i++)
            {
                actionToWarmUp();
            }

            var stopwatch = Stopwatch.StartNew();
            actionToMeasure();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public static async Task<(TimeSpan AverageTime, List<TimeSpan> IndividualTimes)> MeasureAverageExecutionTimeAsync(
            Func<Task> actionToMeasure,
            int iterations,
            int warmUpRuns = 1,
            Func<Task>? warmUpAction = null)
        {
            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations), "Iterations must be greater than zero.");

            var actionToWarmUp = warmUpAction ?? actionToMeasure;
            for (int i = 0; i < warmUpRuns; i++)
            {
                await actionToWarmUp();
            }

            var individualTimes = new List<TimeSpan>(iterations);
            long totalTicks = 0;

            for (int i = 0; i < iterations; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                await actionToMeasure();
                stopwatch.Stop();
                individualTimes.Add(stopwatch.Elapsed);
                totalTicks += stopwatch.Elapsed.Ticks;
            }

            return (TimeSpan.FromTicks(totalTicks / iterations), individualTimes);
        }
    }
}
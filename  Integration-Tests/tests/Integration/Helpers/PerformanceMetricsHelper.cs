using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.IntegrationTests.Helpers
{
    public static class PerformanceMetricsHelper
    {
        /// <summary>
        /// Measures the execution time of an asynchronous action.
        /// </summary>
        /// <param name="actionToMeasure">The asynchronous action to measure.</param>
        /// <returns>The elapsed time for the action to complete.</returns>
        public static async Task<TimeSpan> MeasureExecutionTimeAsync(Func<Task> actionToMeasure)
        {
            var stopwatch = Stopwatch.StartNew();
            await actionToMeasure();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// Measures the execution time of a synchronous action.
        /// </summary>
        /// <param name="actionToMeasure">The synchronous action to measure.</param>
        /// <returns>The elapsed time for the action to complete.</returns>
        public static TimeSpan MeasureExecutionTime(Action actionToMeasure)
        {
            var stopwatch = Stopwatch.StartNew();
            actionToMeasure();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// Measures the execution time of an asynchronous function that returns a value.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="functionToMeasure">The asynchronous function to measure.</param>
        /// <returns>A tuple containing the result of the function and the elapsed time.</returns>
        public static async Task<(T Result, TimeSpan ElapsedTime)> MeasureExecutionTimeAsync<T>(Func<Task<T>> functionToMeasure)
        {
            var stopwatch = Stopwatch.StartNew();
            T result = await functionToMeasure();
            stopwatch.Stop();
            return (result, stopwatch.Elapsed);
        }

        /// <summary>
        /// Measures the execution time of a synchronous function that returns a value.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="functionToMeasure">The synchronous function to measure.</param>
        /// <returns>A tuple containing the result of the function and the elapsed time.</returns>
        public static (T Result, TimeSpan ElapsedTime) MeasureExecutionTime<T>(Func<T> functionToMeasure)
        {
            var stopwatch = Stopwatch.StartNew();
            T result = functionToMeasure();
            stopwatch.Stop();
            return (result, stopwatch.Elapsed);
        }
    }
}
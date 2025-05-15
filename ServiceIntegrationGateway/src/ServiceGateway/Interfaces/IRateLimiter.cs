using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Integration.Interfaces
{
    /// <summary>
    /// Interface for API rate limiting logic, enabling services to acquire permits
    /// before making external calls.
    /// </summary>
    public interface IRateLimiter
    {
        /// <summary>
        /// Acquires a permit for a specific resource, potentially blocking until a permit is available
        /// or the timeout/cancellation occurs.
        /// </summary>
        /// <param name="resourceKey">A key identifying the resource being accessed (e.g., service name, endpoint path).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A Task that completes when a permit is acquired.</returns>
        /// <exception cref="RateLimitExceededException">Thrown if the rate limit is exceeded and the queue limit is zero or configured to drop.</exception>
        Task AcquirePermitAsync(string resourceKey, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Custom exception for rate limit exceeded scenarios.
    /// </summary>
    [Serializable]
    public class RateLimitExceededException : Exception
    {
        public string ResourceKey { get; }

        public RateLimitExceededException(string resourceKey) : base($"Rate limit exceeded for resource: {resourceKey}")
        {
            ResourceKey = resourceKey;
        }
        public RateLimitExceededException(string resourceKey, string message) : base(message)
        {
            ResourceKey = resourceKey;
        }
        public RateLimitExceededException(string resourceKey, string message, Exception inner) : base(message, inner)
        {
            ResourceKey = resourceKey;
        }
        protected RateLimitExceededException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            ResourceKey = info.GetString(nameof(ResourceKey)) ?? string.Empty;
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ResourceKey), ResourceKey);
        }
    }
}
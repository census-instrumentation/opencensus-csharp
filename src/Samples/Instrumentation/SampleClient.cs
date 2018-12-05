using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenCensus.Trace;
using OpenCensus.Trace.Propagation;
using OpenCensus.Trace.Sampler;

namespace Samples.Instrumentation
{
    // Sample client implementation
    public class SampleClient
    {
        private readonly IAttributeValue<string> endpoint;
        private readonly ITracer tracer;
        private readonly ISampler sampler;
        private readonly ITextFormat propagatorFormat;

        private static readonly Action<Dictionary<string, string>, string, string> TraceContextSetter =
            (headers, key, value) => headers[key] = value;
            
        public SampleClient(string endpoint)
        {
            // Cache endpoint attribute.
            // Assuming client calls backend service, this is
            // service endpoint that includes user's account or tenant id
            // Storage request endpoint (URI) is a good  candidate. 
            this.endpoint = AttributeValue.StringAttributeValue(endpoint);

            this.tracer = Tracing.Tracer;
            this.sampler = Samplers.AlwaysSample; //TODO!
            this.propagatorFormat = Tracing.PropagationComponent.TextFormat;

            // initialization
        }

        /// <summary>
        /// Sample operation that gets some data from the remote endpoint.
        /// </summary>
        /// <param name="request">Request that contains path to resource to get.</param>
        /// <returns>Some response with data.</returns>
        public async Task<SampleResponse> GetAsync(SampleRequest request)
        {
            SampleResponse response = null;

            // First, start the scoped span
            // Span name follows 'component.operation name' pattern.
            using (this.tracer.SpanBuilder("Sample.Get")
                // Sampling could be configurable on per-library, per-span basis, so let's set sample 
                .SetSampler(this.sampler)
                .StartScopedSpan())
            {
                // Span is stored in AsyncLocal and we can always get current one:
                var currentSpan = this.tracer.CurrentSpan;

                // check if span is sampled, if not - this is Noop span
                bool isSampled = (currentSpan.Options & SpanOptions.RecordEvents) != 0;

                // Let's augment sampled spans only
                if (isSampled)
                {
                    currentSpan.Kind = SpanKind.Client;
                    currentSpan.PutAttribute("endpoint", endpoint);
                    currentSpan.PutAttribute("container", AttributeValue.StringAttributeValue(request.ContainerName));
                    this.propagatorFormat.Inject(currentSpan.Context, request.Headers, TraceContextSetter);
                }

                try
                {
                    // Get the data
                    response = await DoInternalWithRetriesAsync(request).ConfigureAwait(false);

                    if (isSampled)
                    {
                        // No exception happened. In some cases it means success
                        // but we may also get bad response
                        currentSpan.Status = ToStatus(response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    if (isSampled)
                    {
                        // failed, let's fill the status
                        currentSpan.Status = ToStatus(ex);
                    }

                    throw;
                }
            }

            return response;
        }
        
        // TODO: queues & requests
        // TODO: batching

        /// <summary>
        /// Gets data from the backend service, executes retries and possibly runs
        /// multiple calls to the backend.
        /// </summary>
        /// <param name="request">Sample request.</param>
        /// <returns>Sample response.</returns>
        public Task<SampleResponse> DoInternalWithRetriesAsync(SampleRequest request)
        {
            return Task.FromResult(new SampleResponse());
        }

        /// <summary>
        /// Converts library status code into OpenCensus response code.
        /// </summary>
        /// <param name="responseCode">Library's status code value.</param>
        /// <returns>OpenCensus status code.</returns>
        private Status ToStatus(int responseCode)
        {
            // HTTP status codes are used just an example
            if (responseCode >= 200 || responseCode < 400)
                return Status.Ok.WithDescription(responseCode.ToString());

            switch (responseCode)
            {
                case 400:
                    return Status.InvalidArgument;
                case 401:
                    return Status.Unauthenticated;
                case 403:
                    return Status.PermissionDenied;
                case 404:
                    return Status.NotFound;
                case 409:
                    return Status.AlreadyExists;
                case 429:
                    return Status.ResourceExhausted;
                case 499:
                    return Status.Cancelled;
                case 501:
                    return Status.Unimplemented;
                case 503:
                    return Status.Unavailable;
                case 504:
                    return Status.DeadlineExceeded;
                default:
                    return Status.Unknown.WithDescription(responseCode.ToString());
            }
        }

        /// <summary>
        /// Converts exception to OpenCensus status based on it's type.
        /// </summary>
        /// <param name="e">Exception instance.</param>
        /// <returns>OpenCensus status.</returns>
        private Status ToStatus(Exception e)
        {
            switch (e)
            {
                case TimeoutException toe:
                    return Status.DeadlineExceeded.WithDescription(toe.ToString());
                case TaskCanceledException tce:
                    return Status.Cancelled.WithDescription(tce.ToString());
                default:
                    return Status.Unknown.WithDescription(e.ToString());
            }
        }
    }

    /// <summary>
    /// Sample request.
    /// </summary>
    public class SampleRequest
    {
        /// <summary>
        /// Gets or sets unique data identifier.
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Request metadata.
        /// </summary>
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// Sample response.
    /// </summary>
    public class SampleResponse
    {
        public int StatusCode { get; set; }
    }
}

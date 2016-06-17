
namespace xray.Common
{
    using Microsoft.ServiceFabric.Services.Client;
    using System;
    using System.Fabric;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpServiceClient : HttpClientHandler
    {
        private const int MaxRetries = 10;
        private readonly HttpClientHandler handler;
        private readonly Random random = new Random();
        public HttpServiceClient()
            : this (new HttpClientHandler())
        {
        }

        protected HttpServiceClient(HttpClientHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// fabric:/app/service/#/partitionkey/any/api-path
        /// fabric:/app/service/#/partitionkey/primary/api-path
        /// fabric:/app/service/#/partitionkey/secondary/api-path
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ServicePartitionResolver resolver = ServicePartitionResolver.GetDefault();
            ResolvedServicePartition partition = null;

            Uri serviceName = new Uri(request.RequestUri.GetLeftPart(UriPartial.Path));
            string path = request.RequestUri.Fragment.Remove(0, 2);

            string[] segments = path.Split('/');
            long int64PartitionKey;

            ServicePartitionKey partitionKey = Int64.TryParse(segments[0], out int64PartitionKey)
                ? new ServicePartitionKey(int64PartitionKey)
                : new ServicePartitionKey(segments[0]);

            string selector = segments[1].ToUpperInvariant();
            string serviceUrlPath = String.Join("/", segments.Skip(2));

            int retries = MaxRetries;
            bool resolve = true;
            int retryDelay = 50; 

            while (retries --> 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (resolve)
                {
                    partition = partition != null
                        ? await resolver.ResolveAsync(partition, cancellationToken)
                        : await resolver.ResolveAsync(serviceName, partitionKey, cancellationToken);
                }

                string serviceUrl;

                switch (selector)
                {
                    case "PRIMARY":
                        serviceUrl = partition.GetEndpoint().Address;
                        break;
                    case "SECONDARY":
                        serviceUrl = partition.Endpoints.ElementAt(this.random.Next(1, partition.Endpoints.Count)).Address;
                        break;
                    case "ANY":
                    default:
                        serviceUrl = partition.Endpoints.ElementAt(this.random.Next(0, partition.Endpoints.Count)).Address;
                        break;
                }

                request.RequestUri = new Uri(new Uri(serviceUrl, UriKind.Absolute), new Uri(serviceUrlPath, UriKind.Relative));

                try
                {
                    HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    return response;
                }
                catch (TimeoutException)
                {
                    resolve = true;
                }
                catch (SocketException)
                {
                    resolve = true;
                }
                catch (Exception ex) when (ex is WebException || ex.InnerException is WebException)
                {
                    WebException we = ex as WebException;

                    if (we == null)
                    {
                        we = ex.InnerException as WebException;
                    }

                    if (we != null)
                    {
                        HttpWebResponse errorResponse = we.Response as HttpWebResponse;

                        // the following assumes port sharing
                        // where a port is shared by multiple replicas within a host process using a single web host (e.g., http.sys).
                        if (we.Status == WebExceptionStatus.ProtocolError)
                        {
                            if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                            {
                                // This could either mean we requested an endpoint that does not exist in the service API (a user error)
                                // or the address that was resolved by fabric client is stale (transient runtime error) in which we should re-resolve.
                                resolve = true;
                            }

                            if (errorResponse.StatusCode == HttpStatusCode.InternalServerError)
                            {
                                // The address is correct, but the server processing failed.
                                // This could be due to conflicts when writing the word to the dictionary.
                                // Retry the operation without re-resolving the address.
                                resolve = false;
                            }
                        }

                        if (we.Status == WebExceptionStatus.Timeout ||
                            we.Status == WebExceptionStatus.RequestCanceled ||
                            we.Status == WebExceptionStatus.ConnectionClosed ||
                            we.Status == WebExceptionStatus.ConnectFailure)
                        {
                            resolve = true;
                        }
                    }
                }

                retryDelay += retryDelay;
                await Task.Delay(retryDelay);
            }

            throw new Exception(); // throw something better
        }
    }
}

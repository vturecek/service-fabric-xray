
namespace xray.Common.Tests
{
    using System;
    using System.Fabric;
    using Xunit;

    public class HttpServiceUriTests
    {
        [Fact]
        public void ConstructorGetScheme()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder("http://fabric/App/Service/#/partitionkey/any/endpoint-name/api-path");
            Assert.Equal<string>("http", target.Scheme);
        }

        [Fact]
        public void ConstructorGetHost()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder("http://Fabric/App/Service/#/partitionkey/any/endpoint-name/api-path");
            Assert.Equal<string>("fabric", target.Host);
        }

        [Fact]
        public void ConstructorGetServiceName()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder("http://fabric/App/Service/Name/1/2/3/#/partitionkey/any/endpoint-name/api-path");
            Assert.Equal<Uri>(new Uri("fabric:/App/Service/Name/1/2/3"), target.ServiceName);
        }

        [Fact]
        public void ConstructorGetPartitionKey()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder("http://fabric/App/Service/Name/1/2/3/#/Partitionkey/any/endpoint-name/api-path");
            Assert.Equal<string>("Partitionkey", target.PartitionKey.Value as string);
        }

        [Theory]
        [InlineData(HttpServiceUriTarget.Primary, "http://fabric/App/Service/Name/1/2/3/#/Partitionkey/primary/endpoint-name/api-path")]
        [InlineData(HttpServiceUriTarget.Secondary, "http://fabric/App/Service/Name/1/2/3/#/Partitionkey/secondary/endpoint-name/api-path")]
        [InlineData(HttpServiceUriTarget.Any, "http://fabric/App/Service/Name/1/2/3/#/Partitionkey/any/endpoint-name/api-path")]
        public void ConstructorGetTarget(HttpServiceUriTarget expectedTarget, string url)
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder(url);
            Assert.Equal<HttpServiceUriTarget>(expectedTarget, target.Target);
        }


        [Fact]
        public void ConstructorGetEndpointName()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder("http://fabric/App/Service/Name/1/2/3/#/Partitionkey/any/endpoint-name/api-path");
            Assert.Equal<string>("endpoint-name", target.EndpointName);
        }

        [Fact]
        public void ConstructorGetServicePathAndQuery()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder("http://fabric/App/Service/Name/1/2/3/#/Partitionkey/any/endpoint-name/my/service/url?value=1");
            Assert.Equal<string>("my/service/url?value=1", target.ServicePathAndQuery);
        }

        [Fact]
        public void ConstructorSingletonPartition()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder("http://fabric/App/Service/Name/1/2/3/#//any/endpoint-name/my/service/url?value=1");
            Assert.Equal<string>("http", target.Scheme);
            Assert.Equal<string>("fabric", target.Host);
            Assert.Equal<Uri>(new Uri("fabric:/App/Service/Name/1/2/3"), target.ServiceName);
            Assert.Equal<ServicePartitionKind>(ServicePartitionKind.Singleton, target.PartitionKey.Kind);
            Assert.Null(target.PartitionKey.Value);
            Assert.Equal<HttpServiceUriTarget>(HttpServiceUriTarget.Any, target.Target);
            Assert.Equal<string>("endpoint-name", target.EndpointName);
            Assert.Equal<string>("my/service/url?value=1", target.ServicePathAndQuery);
        }

        [Fact]
        public void ConstructorSingletonPartitionDefaultEndpointName()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder("http://fabric/App/Service/Name/1/2/3/#//any//my/service/url?value=1");
            Assert.Equal<string>("http", target.Scheme);
            Assert.Equal<string>("fabric", target.Host);
            Assert.Equal<Uri>(new Uri("fabric:/App/Service/Name/1/2/3"), target.ServiceName);
            Assert.Equal<ServicePartitionKind>(ServicePartitionKind.Singleton, target.PartitionKey.Kind);
            Assert.Null(target.PartitionKey.Value);
            Assert.Equal<HttpServiceUriTarget>(HttpServiceUriTarget.Any, target.Target);
            Assert.Equal<string>("", target.EndpointName);
            Assert.Equal<string>("my/service/url?value=1", target.ServicePathAndQuery);
        }

        [Theory]
        [InlineData("https")]
        [InlineData("HTtps")]
        public void SetSchemeAndBuild(string scheme)
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetScheme("https")
                .SetServiceName("fabric:/my/service");

            Assert.Equal<string>("https", target.Scheme);
            Assert.Equal<string>(this.GetUrl("https", "fabric", "my/service"), target.Build().ToString());
        }

        [Fact]
        public void SetNullSchemeAndBuild()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetScheme(null)
                .SetServiceName("fabric:/my/service");

            Assert.Equal<string>(null, target.Scheme);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "my/service"), target.Build().ToString());
        }

        [Theory]
        [InlineData("fabric")]
        [InlineData("Fabric")]
        public void SetHostAndBuild(string host)
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetHost(host)
                .SetServiceName("fabric:/my/service");

            Assert.Equal<string>("fabric", target.Host);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "my/service"), target.Build().ToString());
        }

        [Fact]
        public void SetNullHostAndBuild()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetHost(null)
                .SetServiceName("fabric:/my/service");

            Assert.Equal<string>(null, target.Host);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "my/service"), target.Build().ToString());
        }

        [Fact]
        public void SetFQServiceAndBuild()
        {
            Uri expected = new Uri("fabric:/app/my/service");
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetServiceName(expected);

            Assert.Equal<Uri>(expected, target.ServiceName);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "app/my/service"), target.Build().ToString());
        }

        [Fact]
        public void SetNullUriServiceAndBuild()
        {
            Uri serviceName = null;

            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetServiceName("fabric:/my/app");

            target.SetServiceName(serviceName);

            Assert.Null(target.ServiceName);
            Assert.Throws<UriFormatException>(() => target.Build());
        }

        [Theory]
        [InlineData("/my/service")]
        [InlineData("/my/service")]
        public void SetInvalidUriService(string uri)
        {
            Uri serviceName = new Uri(uri, UriKind.RelativeOrAbsolute);
            HttpServiceUriBuilder target = new HttpServiceUriBuilder();

            Assert.Throws<UriFormatException>(() => target.SetServiceName(serviceName));            
        }

        [Fact]
        public void SetPartitionKeyString()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetServiceName("fabric:/test")
                .SetPartitionKey("pKey");

            Assert.Equal<string>("pKey", target.PartitionKey.Value as string);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "test", "pKey"), target.Build().ToString());
        }

        [Fact]
        public void SetPartitionKeyInt64()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetServiceName("fabric:/test")
                .SetPartitionKey(long.MaxValue);

            Assert.Equal<long>(long.MaxValue, (long)target.PartitionKey.Value);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "test", long.MaxValue.ToString()), target.Build().ToString());
        }

        [Theory]
        [InlineData(HttpServiceUriTarget.Primary)]
        [InlineData(HttpServiceUriTarget.Secondary)]
        [InlineData(HttpServiceUriTarget.Any)]
        public void SetTarget(HttpServiceUriTarget uriTarget)
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetServiceName("fabric:/test")
                .SetTarget(uriTarget);

            Assert.Equal<HttpServiceUriTarget>(uriTarget, target.Target);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "test", target: uriTarget.ToString()), target.Build().ToString());
        }

        [Fact]
        public void TargetDefault()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetServiceName("fabric:/test");

            Assert.Equal<HttpServiceUriTarget>(HttpServiceUriTarget.Default, target.Target);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "test"), target.Build().ToString());
        }

        [Fact]
        public void SetEndpointName()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetServiceName("fabric:/test")
                .SetEndpointName("myendpoint");

            Assert.Equal<string>("myendpoint", target.EndpointName);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "test", endpoint: "myendpoint"), target.Build().ToString());
        }

        [Theory]
        [InlineData("path/to/nowhere")]
        [InlineData("path/to/nowhere/")]
        [InlineData("/path/to/nowhere/")]
        public void SetPathAndQuery(string pathAndQuery)
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetServiceName("fabric:/test")
                .SetServicePathAndQuery(pathAndQuery);

            Assert.Equal<string>(pathAndQuery, target.ServicePathAndQuery);
            Assert.Equal<string>(this.GetUrl("http", "fabric", "test", pathAndQuery: pathAndQuery), target.Build().ToString());
        }
        public void SetAllTheThings()
        {
            HttpServiceUriBuilder target = new HttpServiceUriBuilder()
                .SetScheme("https")
                .SetHost("fabric")
                .SetServiceName("fabric:/app/service")
                .SetPartitionKey(34)
                .SetTarget(HttpServiceUriTarget.Secondary)
                .SetEndpointName("myendpoint")
                .SetServicePathAndQuery("path/to/nowhere?value=1");

            Assert.Equal<string>("https", target.Scheme);
            Assert.Equal<string>("fabric", target.Host);
            Assert.Equal<Uri>(new Uri("fabric:/app/service"), target.ServiceName);
            Assert.Equal<long>(34, (long)target.PartitionKey.Value);
            Assert.Equal<HttpServiceUriTarget>(HttpServiceUriTarget.Secondary, target.Target);
            Assert.Equal<string>("myendpoint", target.EndpointName);
            Assert.Equal<string>("path/to/nowhere?value=1", target.ServicePathAndQuery);
            Assert.Equal<string>("https:/fabric/app/service/#/34/secondary/myendpoint/path/to/nowhere?value=1", target.Build().ToString());
        }


        private string GetUrl(
            string scheme = "http",
            string host = "fabric",
            string service = "",
            string partitionKey = "",
            string target = "Default",
            string endpoint = "",
            string pathAndQuery = "")
        {
            return $"{scheme}://{host}/{service}/#/{partitionKey}/{target}/{endpoint}/{pathAndQuery}";
        }
    }
}
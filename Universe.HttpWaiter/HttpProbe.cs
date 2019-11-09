using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Universe.HttpWaiter
{
    public class HttpProbe
    {
        public static async Task<HttpProbeResult> Go(HttpConnectionString cs, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken == default(CancellationToken))
                cancellationToken = CancellationToken.None;

            // SslProtocols _SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> sslCallback =
                (message, certificate2, chain, sslErrors) =>
                {
                    return sslErrors == SslPolicyErrors.None || cs.AllowUntrusted;
                };
            
            HttpMessageHandler mHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = sslCallback
            };
            
            HttpClient c = new HttpClient(mHandler)
            {
                Timeout = TimeSpan.FromSeconds(cs.Timeout),
            };

            HttpRequestMessage req = new HttpRequestMessage(
                new HttpMethod(cs.Method.ToUpper()), 
                new Uri(cs.Uri)
                );

            if (cs.Payload != null)
                req.Content = new StringContent(cs.Payload, Encoding.UTF8);

            // if (cs.ConnectionString.IndexOf("Smart") >= 0 && Debugger.IsAttached) Debugger.Break();

            var copy = new List<HttpConnectionString.Header>(cs.Headers.ToList());
            var contentType = copy.FirstOrDefault(x => "Content-Type".Equals(x.Name, StringComparison.OrdinalIgnoreCase));
            if (contentType != null)
            {
                req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType.Values.First());
                copy.Remove(contentType);
            }

            foreach (var header in copy)
            {
                bool isOk = false;
                Exception exception = null;
                try
                {
                    req.Content.Headers.Add(header.Name, header.Values);
                    isOk = true;

                }
                catch (Exception ex)
                {
                    try
                    {
                        exception = ex;
                        req.Headers.Add(header.Name, header.Values);
                        isOk = true;
                    }
                    catch (Exception ex2)
                    {
                        exception = ex2;
                    }
                }

                if (!isOk)
                {
                    throw new InvalidOperationException(
                        $"Unable to add header '{header.Name}'", exception);
                }

            }

            // if (cs.ConnectionString.IndexOf("Smart") >= 0 && Debugger.IsAttached) Debugger.Break();

            HttpProbeResult ret = new HttpProbeResult();
            var response = await c.SendAsync(req, cancellationToken);
            var statusCode = response.StatusCode;
            string status = response.ReasonPhrase;
            int statusInt = (int) statusCode;
            ret.StatusCode = statusInt;
            ret.StatusPhrase = status;
            ret.Headers = new List<KeyValuePair<string, List<string>>>();
            foreach (KeyValuePair<string, IEnumerable<string>> hdr in response.Headers)
            {
                ret.Headers.Add(new KeyValuePair<string, List<string>>(hdr.Key, new List<string>(hdr.Value)));
            }

            var body = await response.Content.ReadAsByteArrayAsync();
            ret.Body = body;
            bool isValid = cs.ExpectedStatus.IsValid(statusInt);
            if (!isValid)
                throw new InvalidOperationException($"Returned status code {statusInt} does not conform expected '{cs.ExpectedStatus.OriginalString}'. Request: \"{cs.ConnectionString}\"");

            return ret;
        }

        public static async Task GoSimpler(HttpConnectionString cs, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken == default(CancellationToken))
                cancellationToken = CancellationToken.None;

            // SslProtocols _SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> sslCallback =
                (message, certificate2, chain, sslErrors) =>
                {
                    return sslErrors == SslPolicyErrors.None || cs.AllowUntrusted;
                };
            
            HttpMessageHandler mHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = sslCallback
            };
            
            HttpClient c = new HttpClient(mHandler)
            {
                Timeout = TimeSpan.FromSeconds(cs.Timeout),
            };

            HttpRequestMessage req = new HttpRequestMessage(
                new HttpMethod(cs.Method.ToUpper()),
                new Uri(cs.Uri)
                );

            if (cs.Payload != null)
                req.Content = new StringContent(cs.Payload, Encoding.UTF8);

            // if (cs.ConnectionString.IndexOf("Smart") >= 0 && Debugger.IsAttached) Debugger.Break();

            var copy = new List<HttpConnectionString.Header>(cs.Headers.ToList());
            var contentType = copy.FirstOrDefault(x => "Content-Type".Equals(x.Name, StringComparison.OrdinalIgnoreCase));
            if (contentType != null)
            {
                req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType.Values.First());
                copy.Remove(contentType);
            }

            foreach (var header in copy)
            {
                bool isOk = false;
                Exception exception = null;
                try
                {
                    req.Content.Headers.Add(header.Name, header.Values);
                    isOk = true;

                }
                catch (Exception ex)
                {
                    try
                    {
                        exception = ex;
                        req.Headers.Add(header.Name, header.Values);
                        isOk = true;
                    }
                    catch (Exception ex2)
                    {
                        exception = ex2;
                    }
                }

                if (!isOk)
                {
                    throw new InvalidOperationException(
                        $"Unable to add header '{header.Name}'", exception);
                }

            }

            // if (cs.ConnectionString.IndexOf("Smart") >= 0 && Debugger.IsAttached) Debugger.Break();

            var response = await c.SendAsync(req, cancellationToken);
            var statusCode = response.StatusCode;
            int statusInt = (int)statusCode;
            bool isValid = cs.ExpectedStatus.IsValid(statusInt);
            if (!isValid)
                throw new InvalidOperationException($"Returned status code {statusInt} does not conform expected '{cs.ExpectedStatus.OriginalString}'. Request: \"{cs.ConnectionString}\"");


        }




        private static readonly Dictionary<string, HttpMethod> MethodsByString = new Dictionary<string, HttpMethod>(StringComparer.OrdinalIgnoreCase)
        {
            {"Get", HttpMethod.Get},
            {"Put", HttpMethod.Put},
            {"Delete", HttpMethod.Delete},
            {"Head", HttpMethod.Head},
            {"Options", HttpMethod.Options},
            {"Post", HttpMethod.Post},
            {"Trace", HttpMethod.Trace},
        };

    }

    public class HttpProbeResult
    {
        public int StatusCode { get; internal set; }
        public string StatusPhrase { get; internal set; }
        public List<KeyValuePair<string, List<string>>> Headers { get; internal set; }
        public byte[] Body { get; internal set; }
    }
}

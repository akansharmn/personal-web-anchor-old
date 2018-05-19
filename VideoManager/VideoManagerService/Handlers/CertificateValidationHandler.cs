using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace VideoManagerService.Handlers
{
    public class CertificateValidationHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            HttpResponseMessage response = ValidateCertificate(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return await base.SendAsync(request, token);
            else
                return await Task<HttpResponseMessage>.Factory.StartNew(() => response);
        }

        private HttpResponseMessage ValidateCertificate(HttpRequestMessage request)
        {
         var cert =   request.GetClientCertificate();
            IEnumerable<string> thumbPrints;
            if (!request.Headers.TryGetValues("Thumbprint", out thumbPrints) || thumbPrints == null || (thumbPrints != null && thumbPrints.Count() == 0))
            {
                var response = request.CreateResponse(HttpStatusCode.NotAcceptable);
                response.Content = new StringContent("thumbprint request header is not available");
                return response;
            }

            try
            {
                List<StoreLocation> locations = new List<StoreLocation>
                {
                    StoreLocation.CurrentUser,
                    StoreLocation.LocalMachine
                };

                bool? verified = null;

                OpenFlags openFlags = OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly;

                List<string> thumbprintCollection = new List<string>();

                if (thumbPrints.FirstOrDefault().Contains(','))
                {
                    thumbprintCollection.AddRange(thumbPrints.FirstOrDefault().Split(','));
                }
                else
                {
                    thumbprintCollection.Add(thumbPrints.FirstOrDefault());
                }

                foreach (var thumbprint in thumbprintCollection)
                {
                    foreach (var location in locations)
                    {
                        X509Store store = new X509Store(StoreName.Root, location);
                        try
                        {
                            store.Open(openFlags);
                            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: true);
                            if (certificates != null && certificates.Count > 0)
                            {
                                foreach (var certificate in certificates)
                                {
                                    if (!string.IsNullOrWhiteSpace(certificate.Subject) && !string.IsNullOrWhiteSpace(request.RequestUri.Host) && certificate.Subject.ToLower().Contains(request.RequestUri.Host.ToLower()))
                                    {
                                        return request.CreateResponse(HttpStatusCode.OK);
                                    }
                                }
                            }
                            else
                            {
                                certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false);
                                if (certificates != null && certificates.Count > 0)
                                {
                                    verified = false;
                                }
                            }
                        }
                        finally
                        {
                            store.Close();
                        }
                    }
                }

                if (verified.HasValue && !verified.Value)
                {
                    return request.CreateResponse(HttpStatusCode.Unauthorized, "Certificate is available but not valid");
                }
                else
                {
                    return request.CreateResponse(HttpStatusCode.NotFound, "certificate is not available");
                }
            }
            catch (Exception ex)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, "error occured while processing the certificate" + ex.Message);
            }
        }

      }
}
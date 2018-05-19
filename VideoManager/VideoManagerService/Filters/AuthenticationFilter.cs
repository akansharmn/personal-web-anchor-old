using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace VideoManagerService.Filters
{
    public abstract class BasicAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }

        public bool AllowMultiple { get { return false; } }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authentication = request.Headers.Authorization;

            if (authentication == null || authentication.Scheme != "Basic")
                return;

            if (string.IsNullOrEmpty(authentication.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Credential", request);
            }

            Tuple<string, string> userNameAndPassword = ExtractUserNameAndPassword(authentication.Parameter);
            if (userNameAndPassword == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid credential", request);

            string username = userNameAndPassword.Item1;
            string password = userNameAndPassword.Item2;

            IPrincipal principal = await AuthenticateAsync(username, password, cancellationToken);
            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
            }
            else
                context.Principal = principal;
        }

        private Tuple<string, string> ExtractUserNameAndPassword(string parameter)
        {
            try
            {
                byte[] credentialBytes;

                credentialBytes = Convert.FromBase64String(parameter);
                Encoding encoding = Encoding.ASCII;
                encoding = (Encoding)encoding.Clone();
                encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
                string decodedCredentials = encoding.GetString(credentialBytes);
                int colonIndex = decodedCredentials.IndexOf(':');
                string username = decodedCredentials.Substring(0, colonIndex);
                string password = decodedCredentials.Substring(colonIndex + 1);
                return new Tuple<string, string>(username, password);
            }
            catch(Exception)
            {
                return null;
            }
        }

        protected abstract Task<IPrincipal> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
        

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Basic");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }
    }

    internal class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        private AuthenticationHeaderValue challenge;
        private IHttpActionResult result;

        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult result)
        {
            this.challenge = challenge;
            this.result = result;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken token)
        {
            HttpResponseMessage response = await result.ExecuteAsync(token);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (!response.Headers.WwwAuthenticate.Any(h => h.Scheme == challenge.Scheme))
                {
                    response.Headers.WwwAuthenticate.Add(challenge);
                }
            }
            return response;
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(string reason, HttpRequestMessage message)
        {
            this.ReasonPhrase = reason;
            this.Request = message;
        }

        public string ReasonPhrase { get; private set; }
        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken token)
        {
            return Task.FromResult(Execute());
        }

        public HttpResponseMessage Execute()
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            message.RequestMessage = Request;
            message.ReasonPhrase = ReasonPhrase;
            return message;
        }
    }
}
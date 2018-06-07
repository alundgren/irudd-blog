using System;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IruddBlog.Infra.Auth
{
    public class BearerTokenWithGoogleHandler : AuthenticationHandler<BearerTokenWithGoogleIdTokenOptions>
    {
        private IGoogleAccountService googleAccountService;
        public BearerTokenWithGoogleHandler(IOptionsMonitor<BearerTokenWithGoogleIdTokenOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IGoogleAccountService googleAccountService) : base(options, logger, encoder, clock)
        {
            this.googleAccountService = googleAccountService;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.NoResult();
           if(!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue headerValue))
                return AuthenticateResult.NoResult();
            if(!"Bearer".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.NoResult();
            var idToken = headerValue.Parameter;
            var principal = await this.googleAccountService.CreateUserFromIdToken(idToken, this.Scheme.Name);
            if(principal == null)
                return AuthenticateResult.Fail("Invalid google identity token");
            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
    }
}
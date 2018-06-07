using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Claims;
using IruddBlog.Infra.Settings;

namespace IruddBlog.Infra.Auth
{
    public class GoogleAccountService : IGoogleAccountService
    {
        private IBlogSettings blogSettings;
        public GoogleAccountService(IBlogSettings blogSettings) 
        {
            this.blogSettings = blogSettings;
        }

        private bool IsBlogOwnerToken(Google.Apis.Auth.GoogleJsonWebSignature.Payload payload)
        {
            return payload?.Subject == Settings?.BlogOwnerUserId;
        }

        public async Task<ClaimsPrincipal> CreateUserFromIdToken(string idToken, string authenticationSchemeName)
        {
            if(string.IsNullOrWhiteSpace(idToken)) 
                throw new ArgumentException("idToken null or empty", "idToken");
            try 
            {
                var t = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(idToken);

                var claims = new[] { new Claim(ClaimTypes.Name, t.Name), new Claim("iruddblog.isowner", IsBlogOwnerToken(t) ? "true" : "false")};
                var identity = new ClaimsIdentity(claims, authenticationSchemeName);
                return new ClaimsPrincipal(identity);
            } 
            catch(Google.Apis.Auth.InvalidJwtException)
            {
                return null;
            }
        }

        private GoogleSettings settings = null;
        public GoogleSettings Settings 
        {
            get 
            {
                if(settings == null) 
                {
                    settings = new GoogleSettings 
                    {
                        BlogOwnerUserId = this.blogSettings.GoogleLoginUserId,
                        ClientId = this.blogSettings.GoogleLoginClientId
                    };
                }
                return settings;
            }
        }


    }
}
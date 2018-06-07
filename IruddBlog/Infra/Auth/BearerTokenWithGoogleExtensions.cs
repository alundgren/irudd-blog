using System;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IruddBlog.Infra.Auth
{
    public static class BearerTokenWithGoogleExtensions
    {        
        public static AuthenticationBuilder AddBearerTokenWithGoogleId(this AuthenticationBuilder builder)
        {
            return AddBearerTokenWithGoogleId(builder, BearerTokenWithGoogleDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddBearerTokenWithGoogleId(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddBearerTokenWithGoogleId(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddBearerTokenWithGoogleId(this AuthenticationBuilder builder, Action<BearerTokenWithGoogleIdTokenOptions> configureOptions)
        {
            return AddBearerTokenWithGoogleId(builder, BearerTokenWithGoogleDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddBearerTokenWithGoogleId(this AuthenticationBuilder builder, string authenticationScheme, Action<BearerTokenWithGoogleIdTokenOptions> configureOptions)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<BearerTokenWithGoogleIdTokenOptions>, BearerTokenWithGoogleIdTokenPostConfigureOptions>();

            return builder.AddScheme<BearerTokenWithGoogleIdTokenOptions, BearerTokenWithGoogleHandler>(
                authenticationScheme, configureOptions);
        }
    }
}
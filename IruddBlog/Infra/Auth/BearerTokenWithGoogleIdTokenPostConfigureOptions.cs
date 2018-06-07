using Microsoft.Extensions.Options;

namespace IruddBlog.Infra.Auth
{
    public class BearerTokenWithGoogleIdTokenPostConfigureOptions : IPostConfigureOptions<BearerTokenWithGoogleIdTokenOptions>
    {
        public void PostConfigure(string name, BearerTokenWithGoogleIdTokenOptions options)
        {

        }
    }
}
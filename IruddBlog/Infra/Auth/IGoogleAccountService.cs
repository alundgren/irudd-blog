using System.Threading.Tasks;
using System.Security.Claims;

namespace IruddBlog.Infra.Auth
{
    public interface IGoogleAccountService 
    {
        GoogleSettings Settings {get;}
        Task<ClaimsPrincipal> CreateUserFromIdToken(string idToken, string authenticationSchemeName);
    }
}
using System;
using Xunit;
using IruddBlog.Commands;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IruddBlog.Infra.Auth;

namespace IruddBlog.Tests
{
    public class BearerTokenWithGoogleLoginTests 
    {
        [Fact]
        public async Task UnknownIdToken_ResultsInFailure()
        {
            var result = await RunTest("unknown token", "owner", "notowner");            
            Assert.Equal("Invalid google identity token", result.Failure.Message);
        }

        [Fact]
        public async Task NoToken_ResultsInNoAuthentication()
        {
            var result = await RunTest(null, "owner", "notowner");            
            Assert.True(result.None);
        }

        [Fact]
        public async Task OwnerIdToken_LogsIn_AsOwnerUser()
        {
            var result = await RunTest("owner", "owner", "notowner");            
            Assert.True(result.Principal.HasClaim(x => x.Type == "iruddblog.isowner" && x.Value == "true"));
        }

        [Fact]
        public async Task NonOwnerIdToken_LogsIn_AsNonOwnerUser()
        {
            var result = await RunTest("notowner", "owner", "notowner");            
            Assert.True(result.Principal.HasClaim(x => x.Type == "iruddblog.isowner" && x.Value == "false"));
        }        

        private async Task<AuthenticateResult> RunTest(string actualIdToken, string ownerIdToken, string nonOwnerIdToken)
        {
            var options = new Mock<IOptionsMonitor<BearerTokenWithGoogleIdTokenOptions>>();
            var loggerFactory = new Mock<ILoggerFactory>();            
            var logger = new Mock<ILogger>();
            loggerFactory
                .Setup(x => x.CreateLogger(Moq.It.IsAny<string>()))
                .Returns(logger.Object);
            var urlEncoder = System.Text.Encodings.Web.UrlEncoder.Default;
            var clock = new Mock<ISystemClock>();
            var googleAccountService = new Mock<IGoogleAccountService>();            
            googleAccountService
                .Setup(x => x.CreateUserFromIdToken(ownerIdToken, BearerTokenWithGoogleDefaults.AuthenticationScheme))
                .ReturnsAsync(CreateUser("owner", true));
            googleAccountService
                .Setup(x => x.CreateUserFromIdToken(nonOwnerIdToken, BearerTokenWithGoogleDefaults.AuthenticationScheme))
                .ReturnsAsync(CreateUser("not owner", false));                
            var context = new DefaultHttpContext();
            if(actualIdToken != null)
                context.Request.Headers["Authorization"] = $"Bearer {actualIdToken}";
            
            var h = new BearerTokenWithGoogleHandler(options.Object, loggerFactory.Object, urlEncoder, clock.Object, googleAccountService.Object);

            await h.InitializeAsync(new AuthenticationScheme(BearerTokenWithGoogleDefaults.AuthenticationScheme, "n", typeof(BearerTokenWithGoogleHandler)), context);
            
            return await h.AuthenticateAsync();
        }

        private ClaimsPrincipal CreateUser(string name, bool isBlogOwner) 
        {
                var claims = new[] { new Claim(ClaimTypes.Name, name), new Claim("iruddblog.isowner", isBlogOwner ? "true" : "false")};
                var identity = new ClaimsIdentity(claims, BearerTokenWithGoogleDefaults.AuthenticationScheme);
                return new ClaimsPrincipal(identity);            
        }
    }
}
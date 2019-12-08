using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace IruddBlog.Infra.Settings
{
    public class BlogSettings : IBlogSettings
    {
        private IConfiguration configuration;
        private IWebHostEnvironment environment;

        public BlogSettings(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }
        public string LocalPostsFolder => System.IO.Path.Combine(environment.WebRootPath, "posts");

        public string LocalImageTempFolder => System.IO.Path.Combine(environment.WebRootPath, "i");

        public string GoogleLoginUserId => Req("IRUDD_BLOG_GOOGLE_USERID");

        public string GoogleLoginClientId => Req("IRUDD_BLOG_GOOGLE_CLIENTID");

        public string GoogleDriveServiceAccountCredentialFile => Req("IRUDD_BLOG_GOOGLEDRIVE_CREDENTIALS_FILE");
        public string GoogleDriveTargetFolder => Req("GoogleDriveTargetFolder");
        public bool IsGoogleDriveSynchEnabled => Req("IsGoogleDriveSynchEnabled").ToLowerInvariant().Equals("true");
        private string Req(string settingName)
        {
            var v = configuration.GetValue<string>(settingName);
            if (string.IsNullOrWhiteSpace(v))
                throw new Exception($"Missing setting {settingName}");
            return v;
        }
    }
}
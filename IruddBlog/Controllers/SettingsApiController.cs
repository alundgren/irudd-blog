using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IruddBlog.Commands;
using IruddBlog.Infra.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IruddBlog.Controllers
{
    [Route("api")]
    public class SettingsApiController : Controller
    {
        private readonly IBlogSettings blogSettings;

        public SettingsApiController(IBlogSettings blogSettings)
        {
            this.blogSettings = blogSettings;
        }

        [HttpPost("settings")]
        public SettingsModel Settings()
        {
            return new SettingsModel 
            {
                GoogleSettings = new SettingsModel.GoogleSettingsModel 
                {
                    BlogOwnerUserId = this.blogSettings.GoogleLoginUserId,
                    ClientId = this.blogSettings.GoogleLoginClientId
                }
            };
        }

        public class SettingsModel 
        {
            public GoogleSettingsModel GoogleSettings { get; set; }

            public class GoogleSettingsModel 
            {
                public string BlogOwnerUserId { get; set; }
                public string ClientId { get; set; }
            }
        }
    }
}

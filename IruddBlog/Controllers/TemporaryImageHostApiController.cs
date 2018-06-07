using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IruddBlog.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IruddBlog.Controllers
{
    [Route("api/temporary-images")]
    [Authorize(Policy = "MustBeBlogOwner")]
    public class TemporaryImageHostApiController : Controller
    {
        private ITemporaryImageHostCommand temporaryImageHostCommand;
        public TemporaryImageHostApiController(ITemporaryImageHostCommand temporaryImageHostCommand)
        {
            this.temporaryImageHostCommand  = temporaryImageHostCommand;
        }

        [HttpPost("add-as-dataurl")]
        public AddImageResult AddImageAsDataUrl([FromBody]AddImageAsDataUrlRequest request)
        {
            var filename = temporaryImageHostCommand.AddImageAsDataUrl(request?.DataUrl);
            return new AddImageResult
            {
                RelativeUrl = $"/i/{filename}"
            };
        }
        
        public class AddImageAsDataUrlRequest 
        {
            public string DataUrl { get; set; }
        }

        public class AddImageResult
        {
            public string RelativeUrl { get; set; }
        }
    }
}

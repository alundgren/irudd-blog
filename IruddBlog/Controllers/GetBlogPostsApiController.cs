using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IruddBlog.Commands;
using IruddBlog.Infra.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace IruddBlog.Controllers
{
    [Route("api/posts")]
    public class GetBlogPostsApiController : Controller
    {
        private IGetBlogPostsCommand getBlogPostsCommand;

        public GetBlogPostsApiController(IGetBlogPostsCommand getBlogPostsCommand)
        {
            this.getBlogPostsCommand = getBlogPostsCommand;
        }

        [HttpPost("get-metadatas")]
        public IList<PostMetadata> GetPostsMetadata()
        {            
            return getBlogPostsCommand.GetAllPostsMetadata();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IruddBlog.Commands;
using IruddBlog.Infra.PostBackup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IruddBlog.Controllers
{
    [Route("api/posts")]
    [Authorize(Policy = "MustBeBlogOwner")]
    public class NewBlogPostApiController : Controller
    {
        private readonly INewBlogPostCommand newBlogPostCommand;
        private readonly IPostBackupServiceFactory postBackupServiceFactory;

        public NewBlogPostApiController(INewBlogPostCommand newBlogPostCommand, IPostBackupServiceFactory postBackupServiceFactory)
        {
            this.newBlogPostCommand = newBlogPostCommand;
            this.postBackupServiceFactory = postBackupServiceFactory;
        }

        [HttpPost("begin-create")]
        public BeginCreatePostResult BeginCreatePost([FromBody]BeginCreatePostRequest request)
        {
            var result = newBlogPostCommand.BeginCreatePost(request?.MarkdownContent, request?.Title);
            
            return new BeginCreatePostResult 
            {
                PostId = result?.PostId,
                LocalImageUrls = result?.LocalImageUrls
            };
        }

        [HttpPost("upload-image")]
        public ActionResult UploadImage([FromBody]UploadImageRequest request)
        {
            newBlogPostCommand.UploadImage(request?.PostId, request?.Base64ImageData, request?.Filename);            
            return this.Ok();
        }

        [HttpPost("commit-create")]
        public async Task<ActionResult> CommitCreatePost([FromBody]CommitCreatePostRequest request)
        {
            newBlogPostCommand.CommitCreateBlogPost(request?.PostId);
            var backupService = postBackupServiceFactory.CreateService();
            await backupService.UploadPostsToBackupIfLocalPostsExist();
            return this.Ok();
        }

        public class BeginCreatePostRequest
        {
            public string MarkdownContent {get; set;}
            public string Title {get; set;}
        }

        public class UploadImageRequest 
        {
            public string PostId {get; set;}
            public string Base64ImageData {get; set;}
            public string Filename {get; set;}
        }
        
        public class CommitCreatePostRequest 
        {
            public string PostId { get; set; }          
        }

        public class BeginCreatePostResult
        {
            public string PostId { get; set; }
            public List<string> LocalImageUrls { get; set; }
        }
    }
}

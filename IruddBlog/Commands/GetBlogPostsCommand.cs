using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using IruddBlog.Infra.Settings;

namespace IruddBlog.Commands
{
    public class GetBlogPostsCommand : IGetBlogPostsCommand
    {
        private readonly IBlogSettings blogSettings;
        private readonly IFileSystem fileSystem;

        public GetBlogPostsCommand(IBlogSettings blogSettings, IFileSystem fileSystem)
        {
            this.blogSettings = blogSettings;
            this.fileSystem = fileSystem;
        }

        private string GetPostDirectory(string postId) 
        {
            return Path.Combine(blogSettings.LocalPostsFolder, postId);
        }

        private IList<string> GetPostDirectories() 
        {
            return fileSystem
                .GetDirectories(blogSettings.LocalPostsFolder)
                .ToList();
        }

        //TODO: There is a hidden assumption in here that the post directory has to be the same as the postid or the post wont be served correctly. Make that be enforced
        public IList<PostMetadata> GetAllPostsMetadata() 
        {
            return GetPostDirectories()
                .Select(x => JsonConvert.DeserializeObject<PostMetadata>(fileSystem.ReadAllText(Path.Combine(x, "metadata.json"))))
                .ToList();
        }
    }
}
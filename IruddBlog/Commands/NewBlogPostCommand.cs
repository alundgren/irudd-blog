using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using IruddBlog.Infra.Settings;
using IruddBlog.Infra;

namespace IruddBlog.Commands
{

    public class NewBlogPostCommand : INewBlogPostCommand
    {
        private readonly IFileSystem fileSystem;
        private readonly IBlogSettings blogSettings;
        private readonly IRandomIdGenerator randomIdGenerator;
        private readonly IClock clock;


        public NewBlogPostCommand(IBlogSettings blogSettings, IRandomIdGenerator randomIdGenerator, IFileSystem fileSystem, IClock clock)
        {
            this.fileSystem = fileSystem;
            this.clock = clock;
            this.randomIdGenerator = randomIdGenerator;
            this.blogSettings = blogSettings;
        }

        /*
        The blog data 
        A post should be a folder like this which is then copied to wwwroot on commit:
        posts/
           <postid>/
             index.html
             content.md
             metadata.json ({ title, postid })
             img/<tempFilename1>, <tempfilename2>, ...
        index.html is the blogpost itself as html markdown contains the markdown that generate the html and the img folder has all the images
        */

        private string GetPostDirectory(string postId) 
        {
            return Path.Combine(this.blogSettings.LocalPostsFolder, postId);
        }

        private (List<string>, string) ExtractAndReplaceClientCachedImages(string postId, string markdownContent)
        {
            var r = new System.Text.RegularExpressions.Regex(@"(?:\/t\/)(e-[^\.]+.png)");
            var localImageUrls = new List<string>();
            return (localImageUrls, r.Replace(markdownContent, x => {
                localImageUrls.Add(x.Value);
                return $"/posts/{postId}/img/" + x.Groups[1].Value;
            }));
        }

        private (List<string>, string) ExtractAndReplaceServerCachedImages(string postId, string markdownContent)
        {
            var r = new System.Text.RegularExpressions.Regex(@"(?:\/i\/)(e-[^\.]+.png)");
            var imageFilenames = new List<string>();
            return (imageFilenames, r.Replace(markdownContent, x => {
                imageFilenames.Add(x.Groups[1].Value);
                return $"/posts/{postId}/img/" + x.Groups[1].Value;
            }));
        }

        public BeginCreatePostResult BeginCreatePost(string markdownContent, string title)
        {
            var postId = this.randomIdGenerator.GenerateId(8);

            var tempPath = GetPostDirectory(postId);
            fileSystem.CreateDirectory(tempPath);

            List<string> localImageUrls = null;
            (localImageUrls, markdownContent) = ExtractAndReplaceClientCachedImages(postId, markdownContent);

            List<string> serverCachedImagesNames = null;
            (serverCachedImagesNames, markdownContent) = ExtractAndReplaceServerCachedImages(postId, markdownContent);
            
            var imageDirectory = Path.Combine(Path.Combine(tempPath, "img"));
            if(serverCachedImagesNames.Count > 0)
                fileSystem.CreateDirectory(imageDirectory);

            foreach(var serverImageName in serverCachedImagesNames) 
            {
                var fromFilename = Path.Combine(this.blogSettings.LocalImageTempFolder, serverImageName);
                var toFileName = Path.Combine(Path.Combine(tempPath, "img"), serverImageName);
                fileSystem.MoveFile(fromFilename, toFileName);
            }

            fileSystem.WriteAllText(Path.Combine(tempPath, "content.md"), markdownContent);
            //TODO: PublicationDate should be deferred until commit so we should move metadata to commit in the future.
            var metadata = new PostMetadata 
            {
                Title = title,
                PostId = postId,
                PublicationDate = this.clock.Now
            };
            fileSystem.WriteAllText(Path.Combine(tempPath, "metadata.json"), JsonConvert.SerializeObject(metadata));

            return new BeginCreatePostResult 
            {
                PostId = postId,
                LocalImageUrls = localImageUrls
            };
        }

        public void UploadImage(string postId, string base64ImageData, string tempFilename)
        {
            var tempPath = Path.Combine(GetPostDirectory(postId), "img");
            fileSystem.CreateDirectory(tempPath);
            fileSystem.WriteAllBytes(Path.Combine(tempPath, tempFilename), Convert.FromBase64String(base64ImageData));
        }

        public void CommitCreateBlogPost(string postId) 
        {
            //Save the post to the database/backup/wherever that is not wwwroot
            //Add the id to the post registry
            fileSystem.WipeDirectory(this.blogSettings.LocalImageTempFolder);
        }
    }
    public class BeginCreatePostResult 
    {
        public string PostId { get; set; }
        public List<string> LocalImageUrls { get; set; }            
    }
}
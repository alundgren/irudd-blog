using System;
using Xunit;
using IruddBlog.Commands;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Moq;
using IruddBlog.Infra.Settings;

namespace IruddBlog.Tests
{
    public class GetBlogPostsCommandTests
    {
        [Fact]
        public void GetAllPostsMetadata_ReadsMetadata()
        {

            var expectedMetadata = JsonConvert.SerializeObject(new { PostId = "post1", Title = ExampleTitle, PublicationDate  = "2018-05-23T06:09:09.3706648+02:00" });

            fs.KnownDirectories[ExampleRoot] = new List<string> 
                {
                    System.IO.Path.Combine(ExampleRoot, "post1"),
                };
            fs.KnownTextFiles[System.IO.Path.Combine(System.IO.Path.Combine(ExampleRoot, "post1"), "metadata.json")] = expectedMetadata;
  
            var posts = cmd.GetAllPostsMetadata();

            Assert.Equal("post1", posts.Single().PostId);
            Assert.Equal(ExampleNow, posts.Single().PublicationDate);
            Assert.Equal(ExampleTitle, posts.Single().Title);            
        }

        #region "Setup"

        private MockFileSystem fs;
        private GetBlogPostsCommand cmd;
                
        public GetBlogPostsCommandTests()
        {
            fs = new MockFileSystem();
            var settings = new Mock<IBlogSettings>();
            settings.Setup(x => x.LocalPostsFolder).Returns(ExampleRoot);

            cmd = new GetBlogPostsCommand(settings.Object, fs);
        }
        private static DateTimeOffset ExampleNow = DateTimeOffset.ParseExact("2018-05-23T06:09:09.3706648+02:00", "o", System.Globalization.CultureInfo.InvariantCulture);

        private const string ExampleTitle = "A blogpost with spaces";

        private const string ExampleRoot = @"c:\temp\wwwroot\posts";        
        #endregion
    }
}

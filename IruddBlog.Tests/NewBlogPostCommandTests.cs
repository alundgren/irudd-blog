using System;
using Xunit;
using IruddBlog.Commands;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Moq;
using IruddBlog.Infra.Settings;

namespace IruddBlog.Tests
{    
    public class NewBlogPostCommandTests
    {
        [Fact]
        public void BeginCreatePost_Returns_PostId()
        {
            var postId = cmd.BeginCreatePost(ExampleMarkDown, ExampleTitle).PostId;
            cmd.CommitCreateBlogPost(postId);

            Assert.Equal("42", postId);
        }

        [Fact]
        public void CommitCreatePost_Wipes_ImageCache()
        {
            var postId = cmd.BeginCreatePost(ExampleMarkDown, ExampleTitle).PostId;

            Assert.False(fs.WasWipeDirectoryCalledWith(TempImageRoot), fs.GetOperationLogMessage());
            cmd.CommitCreateBlogPost(postId);

            Assert.True(fs.WasWipeDirectoryCalledWith(TempImageRoot), fs.GetOperationLogMessage());
        }        

        [Fact]
        public void BeginCreatePost_Returns_LocalImageUrls()
        {
            var localImageUrls = cmd.BeginCreatePost(ExampleMarkDown, ExampleTitle).LocalImageUrls;

            Assert.Equal("/t/e-cb49587a.png", localImageUrls.Single());
        }

        [Fact]
        public void BeginCreatePost_Writes_CorrectFilenamesAndContent()
        {
            var postId = cmd.BeginCreatePost(ExampleMarkDown, ExampleTitle).PostId;
            cmd.CommitCreateBlogPost(postId);

            //TODO: The actual code should work fine on linux or windows but the tests will likely fail on linux since Path.Combine will use forward slash ... can we make the tests independent too while still being readable?
            Assert.True(fs.WasWriteAllTextCalledWithFilename(@"c:\temp\wwwroot\posts\42\metadata.json"), fs.GetOperationLogMessage());
            Assert.True(fs.WasWriteAllTextCalledWithFilenameAndContent(@"c:\temp\wwwroot\posts\42\content.md", ExampleMarkDownTransformed), fs.GetOperationLogMessage());
        }

        [Fact]
        public void BeginCreatePost_Write_TitleAndPostedDateToMetadata()
        {
            var postId = cmd.BeginCreatePost(ExampleMarkDown, ExampleTitle).PostId;
            cmd.CommitCreateBlogPost(postId);

            var expectedMetadata = JsonConvert.SerializeObject(new { PostId = postId, Title = ExampleTitle, PublicationDate  = "2018-05-23T06:09:09.3706648+02:00" });

            Assert.True(fs.WasWriteAllTextCalledWithFilenameAndContent(@"c:\temp\wwwroot\posts\42\metadata.json", expectedMetadata), fs.GetOperationLogMessage());            
        }

        [Fact]
        public void NonLocalImage_Is_MovedToTarget()
        {
            var md = ExampleMarkDown.Replace("/t/e-cb49587a.png", "/i/e-cb49587a.png");
            fs.KnownBinaryFiles.Add(Path.Combine(TempImageRoot, "e-cb49587a.png"), new byte[] { 1 });

            var postId = cmd.BeginCreatePost(md, ExampleTitle).PostId;
            cmd.CommitCreateBlogPost(postId);

            var fromfn = Path.Combine(TempImageRoot, "e-cb49587a.png");
            var tofn = Path.Combine(PostsRoot, $"{postId}\\img\\e-cb49587a.png");

            Assert.True(fs.WasMoveFileCalledWith(fromfn, tofn), fs.GetOperationLogMessage());
        }

        #region "Setup"
      
        private MockFileSystem fs;
        private NewBlogPostCommand cmd;

        public NewBlogPostCommandTests()
        {
            var s = new Mock<IBlogSettings>();
            s.Setup(x => x.LocalPostsFolder).Returns(PostsRoot);
            s.Setup(x => x.LocalImageTempFolder).Returns(TempImageRoot);

            fs = new MockFileSystem();
            
            var random = new Mock<IRandomIdGenerator>();
            random.Setup(x => x.GenerateId(It.IsAny<int>())).Returns("42");
            var clock = new Mock<IruddBlog.Infra.IClock>();
            clock.Setup(x => x.Now).Returns(ExampleNow);

            cmd = new NewBlogPostCommand(s.Object, random.Object, fs, clock.Object);
        }
        private const string ExampleMarkDown = @"# The post
**test**
* 1
* 2
* 3
![](/t/e-cb49587a.png)";
        private const string ExampleMarkDownTransformed = @"# The post
**test**
* 1
* 2
* 3
![](/posts/42/img/e-cb49587a.png)";
        private static DateTimeOffset ExampleNow = DateTimeOffset.ParseExact("2018-05-23T06:09:09.3706648+02:00", "o", System.Globalization.CultureInfo.InvariantCulture);
        private const string ExampleTitle = "A blogpost with spaces";

        private const string PostsRoot = @"c:\temp\wwwroot\posts";
        private const string TempImageRoot = @"c:\temp\wwwroot\i";        
        #endregion
    }
}

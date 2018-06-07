using System;
using Xunit;
using IruddBlog.Commands;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Moq;
using IruddBlog.Infra.Settings;

namespace IruddBlog.Tests
{
    public class TemporaryImageHostCommandTests 
    {
        [Fact]
        public void RootPathShouldBeCreated() 
        {
            const string RootPath = @"c:\temp\wwwroot\i";
            var expectedImageData = new byte[] { 1, 2, 3 };
            var imageUrl = $"data:image/png;base64,{Convert.ToBase64String(expectedImageData)}";

            cmd.AddImageAsDataUrl(imageUrl);

            Assert.True(fs.WasCreateDirectoryCalledWith(RootPath), fs.GetOperationLogMessage());            
        }

        [Fact]
        public void FilesAreWrittenWithCorrectNameAndContent() 
        {
            var expectedImageData = new byte[] { 1, 2, 3 };

            Action<string> testWithImageType = imageType =>
            {
                var imageUrl = $"data:image/{imageType};base64,{Convert.ToBase64String(expectedImageData)}";
                var expectedFileName = @"c:\temp\wwwroot\i\e-yfesgj0s." + imageType;

                cmd.AddImageAsDataUrl(imageUrl);

                Assert.True(fs.WasWriteAllBytesCalledWithFilenameAndContent(expectedFileName, expectedImageData), $"Expected filename: {expectedFileName}" + Environment.NewLine + fs.GetOperationLogMessage()); 
            };
            testWithImageType("png");
            testWithImageType("gif");
        }

        [Fact]
        public void AddReturnsFilename() 
        {
            var expectedImageData = new byte[] { 1, 2, 3 };
            var imageUrl = $"data:image/png;base64,{Convert.ToBase64String(expectedImageData)}";

            var actualFilename = cmd.AddImageAsDataUrl(imageUrl);

            Assert.Equal("e-yfesgj0s.png", actualFilename);
        }        

        [Fact]
        public void DeleteFileWorks() 
        {
            var existingMatchingFilename = @"c:\temp\wwwroot\i\e-abc123.png";

            var notExistingMatchingFilename = @"c:\temp\wwwroot\i\e-123abc.png";
            fs.KnownBinaryFiles.Add(existingMatchingFilename, new byte[] { 1 });

            var existingNonMatchingFilename =  @"c:\temp\wwwroot\i\howto.txt";
            fs.KnownBinaryFiles.Add(existingNonMatchingFilename, new byte[] { 1 });

            Assert.True(cmd.TryRemoveImage(existingMatchingFilename), $"File: {existingMatchingFilename}" + Environment.NewLine + fs.GetOperationLogMessage());
            Assert.False(cmd.TryRemoveImage(notExistingMatchingFilename), $"File: {notExistingMatchingFilename}" + Environment.NewLine + fs.GetOperationLogMessage());
            Assert.ThrowsAny<Exception>(() => cmd.TryRemoveImage(existingNonMatchingFilename));            
        }

        #region "Setup"

        const string RootPath = @"c:\temp\wwwroot\i";
        private MockFileSystem fs;
        private TemporaryImageHostCommand cmd;
        
        public TemporaryImageHostCommandTests() 
        {
            fs = new MockFileSystem();    
            var random = new Mock<IRandomIdGenerator>();
            random.Setup(x => x.GenerateId(It.IsAny<int>())).Returns("yfesgj0s");            
            var s = new Mock<IBlogSettings>();            
            s.Setup(x => x.LocalImageTempFolder).Returns(RootPath);
            cmd = new TemporaryImageHostCommand(s.Object, fs, random.Object);
        }        
        #endregion
    }
}
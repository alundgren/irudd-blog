
using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using IruddBlog.Commands;
using System.Text.RegularExpressions;
using IruddBlog.Infra.Settings;

namespace IruddBlog.Commands
{
    public class TemporaryImageHostCommand : ITemporaryImageHostCommand
    {
        private readonly IBlogSettings blogSettings;
        private readonly IFileSystem fileSystem;
        private readonly IRandomIdGenerator randomIdGenerator;

        public TemporaryImageHostCommand(IBlogSettings blogSettings, IFileSystem fileSystem, IRandomIdGenerator randomIdGenerator) 
        {
            this.blogSettings = blogSettings;
            this.fileSystem = fileSystem;
            this.randomIdGenerator = randomIdGenerator;
        }

        //Returns something like e-fhwefh9hewf.png
        public string AddImageAsDataUrl(string dataUrl) 
        {
            var m = Regex.Match(dataUrl, @"data:image/(?<imageType>[^\;]+?);base64,(?<imageData>.+)");
            if(!m.Success)
                throw new Exception("Invalid image url");
            var id = randomIdGenerator.GenerateId(8);
            var imageType = m.Groups["imageType"].Value;
            var imageData = Convert.FromBase64String(m.Groups["imageData"].Value);
            var filenameOnly = $"e-{id}.{imageType}";
            var filename = Path.Combine(blogSettings.LocalImageTempFolder, filenameOnly);

            fileSystem.CreateDirectory(blogSettings.LocalImageTempFolder);
            fileSystem.WriteAllBytes(filename, imageData);

            return filenameOnly;
        }

        //Input is the same as returned by AddImageAsDataUrl ie something like /i/e-fhwefh9hewf.png or just e-fhwefh9hewf.png
        public bool TryRemoveImage(string filenameOrUrl)
        {
            var m = Regex.Match(filenameOrUrl, @"(?:\/i\/)?(e-[a-zA-Z0-9]+\.[a-zA-Z]+)");
            if(!m.Success)
                throw new Exception("Invalid url. It should have the format /i/e-<image id>.<image extension> or just e-<image id>.<image extension>");
            var filename = Path.Combine(blogSettings.LocalImageTempFolder, m.Groups[1].Value);
            return fileSystem.TryDeleteFile(filename);
        }
    }
}
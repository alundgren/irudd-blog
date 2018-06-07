using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using IruddBlog.Infra.Settings;

namespace IruddBlog.Infra.PostBackup 
{
    public class GoogleDriveBlogBackupService : IPostBackupService
    {
        private IBlogSettings blogSettings;
        public GoogleDriveBlogBackupService(IBlogSettings blogSettings)
        {
            this.blogSettings = blogSettings;
        }

        //TODO: Have some form of rotation so we can recove if this screws up
        //TODO: Make it more granular os only a small subset is uploaded each time

        private GoogleDriveClient CreateDriveClient() 
        {            
            ServiceAccountCredential credential;
            using (var stream = System.IO.File.OpenRead(this.blogSettings.GoogleDriveServiceAccountCredentialFile))
            {
                credential = GoogleCredential.FromStream(stream)
                                     .CreateScoped(new [] { DriveService.Scope.Drive })
                                     .UnderlyingCredential as ServiceAccountCredential;
            }
            
            
            var driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Iruddblog"                
            });

            return new GoogleDriveClient(() => driveService);
        }

        private const string PostsBackupFilename = "posts.zip";
        private async Task<string> GetSharedDriveFolderId(GoogleDriveClient client) 
        {
            var folderId = await client.FindIdOfFolderSharedWithMe(this.blogSettings.GoogleDriveTargetFolder);
            if(folderId == null)
                throw new Exception($"Missing shared folder {this.blogSettings.GoogleDriveTargetFolder}. Did you forget to share it with the service account?");
            return folderId;
        }

        public async Task<string> UploadPosts(SpecialCaseAction actionIfFolderEmpty)
        {
            if(IsLocalPostsFolderEmpty()) 
            {
                switch(actionIfFolderEmpty)
                {
                    case SpecialCaseAction.DoNothing: return null;
                    case SpecialCaseAction.Throw: throw new Exception($"The target directory '{this.blogSettings.LocalPostsFolder}' is empty.");
                    default: throw new NotImplementedException();
                }
            }            
            var tempFilename = System.IO.Path.Combine(System.IO.Path.Combine(System.IO.Path.GetTempPath()), $"{Guid.NewGuid().ToString()}.zip");
            try 
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(tempFilename));
                System.IO.Compression.ZipFile.CreateFromDirectory(this.blogSettings.LocalPostsFolder, tempFilename);

                var client = CreateDriveClient();
                var folderId = await GetSharedDriveFolderId(client);
                string fileId;
                using(var f = System.IO.File.OpenRead(tempFilename))
                {
                    fileId = await client.UploadFileToFolder(folderId, PostsBackupFilename, "application/zip",  "application/zip", f);
                }
                return fileId;
            }
            finally 
            {
                if(System.IO.File.Exists(tempFilename))
                    System.IO.File.Delete(tempFilename);
            }
        }

        public enum SpecialCaseAction
        {
            Throw,
            DoNothing
        }

        private bool IsLocalPostsFolderEmpty()
        {
            return System.IO.Directory.GetDirectories(this.blogSettings.LocalPostsFolder).Length == 0 &&  System.IO.Directory.GetFiles(this.blogSettings.LocalPostsFolder).Length == 0;
        }

        public async Task<Boolean> DownloadPosts(SpecialCaseAction actionIfFolderNotEmpty) 
        {
            System.IO.Directory.CreateDirectory(this.blogSettings.LocalPostsFolder);
            if(!IsLocalPostsFolderEmpty()) 
            {
                switch(actionIfFolderNotEmpty)
                {
                    case SpecialCaseAction.DoNothing: return false;
                    case SpecialCaseAction.Throw: throw new Exception($"The target directory '{this.blogSettings.LocalPostsFolder}' is not empty.");
                    default: throw new NotImplementedException();
                }
            }
            var client = CreateDriveClient();
            var sharedFolderId = await GetSharedDriveFolderId(client);
            var fileIds = await client.FindIdsOfFile(PostsBackupFilename, sharedFolderId);
            if(fileIds.Count > 1)
                throw new Exception($"There are multiple files named '{PostsBackupFilename}' on google drive. Manually weed out all but one and try again");
            else if(fileIds.Count == 0)
                return false;
            else
            {
                var tempfile = System.IO.Path.GetTempFileName();
                try 
                {
                    using(var fs = System.IO.File.OpenWrite(tempfile))
                    {
                        var status = await client.DownloadFile(fileIds[0], fs);
                        if(status != DownloadStatus.Completed)
                            throw new Exception("Download failed: " + status);
                    }
                    System.IO.Compression.ZipFile.ExtractToDirectory(tempfile, this.blogSettings.LocalPostsFolder);
                } 
                finally 
                {
                    System.IO.File.Delete(tempfile);
                }
                return true;
            }
        }

        public async Task<bool> DownloadPostsFromBackupIfNoLocalPostsExist()
        {
            return await DownloadPosts(GoogleDriveBlogBackupService.SpecialCaseAction.DoNothing);
        }

        public async Task<bool> UploadPostsToBackupIfLocalPostsExist()
        {
            var result = await UploadPosts(GoogleDriveBlogBackupService.SpecialCaseAction.DoNothing);
            return result != null;
        }
    }
}
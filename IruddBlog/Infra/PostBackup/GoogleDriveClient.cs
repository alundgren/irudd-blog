using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace IruddBlog.Infra.PostBackup 
{
    public class GoogleDriveClient 
    {
        private DriveService driveService;

        public GoogleDriveClient(Func<DriveService> createDriveService)
        {
            this.driveService = createDriveService();
        }

        public async Task DeleteFile(string fileId) 
        {
            var r = driveService.Files.Delete(fileId);
            await r.ExecuteAsync();
        }

        public async Task<string> FindIdOfFolderSharedWithMe(string folderName) 
        {
            var request = driveService.Files.List();
            request.Spaces = "drive";
            request.Fields = "files(id, sharedWithMeTime)";
            request.Q = $"name = '{folderName}' and mimeType = 'application/vnd.google-apps.folder'";
            request.PageSize = 2;
            
            var result = await request.ExecuteAsync();
            if(result.Files.Count == 0)
                return null;
            else if(result.Files.Count > 1)
            {
                var ids = string.Join(", ", result.Files.Select(x => x.Id));
                throw new Exception($"Got at least two hits for {folderName}. Ids: {ids}");
            }
            
            var file = result.Files[0];

            if(!file.SharedWithMeTime.HasValue)
                throw new Exception($"{folderName} is not a shared folder");

            return file.Id;            
        }

        public async Task<List<string>> FindIdsOfFile(string filename, string parentId) 
        {
            var request = driveService.Files.List();
            request.Spaces = "drive";
            request.Fields = "files(id)";
            request.Q = $"name = '{filename}' and '{parentId}' in parents";
            request.PageSize = 1000;
            
            var result = await request.ExecuteAsync();
            return result.Files.Select(x => x.Id).ToList();
        }

        public async Task<string> UploadFileToFolder(string folderId, string filename, string fileMimeType, string fileUploadContentType, System.IO.Stream fileContent) 
        {
            var existingFilesIds = await this.FindIdsOfFile(filename, folderId);

            if(existingFilesIds.Count > 1) 
                throw new Exception($"There are {existingFilesIds.Count} files named {filename} in the folder with id {folderId}. Get it down to one (will be replaced) or zero manually and then try again.");

            if(existingFilesIds.Count == 0)
            {
                var request = driveService.Files.Create(new File()
                    {
                        Name = filename,
                        Parents = new List<string>()
                        {
                            folderId
                        }                
                    }, fileContent, fileUploadContentType);
                request.Fields = "id";
                var result = await request.UploadAsync();
                if(result.Exception != null)
                    throw result.Exception;                
                return request.ResponseBody.Id;
            }
            else 
            {
                var fileId = existingFilesIds.Single();
                var request = driveService.Files.Update(new File(), existingFilesIds.Single(), fileContent, fileUploadContentType);
                var result = await request.UploadAsync();
                if(result.Exception != null)
                    throw result.Exception;
                return fileId;
            }
        }

        public async Task<DownloadStatus> DownloadFile(string fileId, System.IO.Stream target)
        {
            var e = new TaskCompletionSource<DownloadStatus>();
            var request = driveService.Files.Get(fileId);
            Action<IDownloadProgress> onProgress = (Google.Apis.Download.IDownloadProgress progress) =>
               {
                   switch (progress.Status)
                   {
                       case Google.Apis.Download.DownloadStatus.Downloading:
                           {
                               Console.WriteLine(progress.BytesDownloaded);
                               break;
                           }
                       case Google.Apis.Download.DownloadStatus.Completed:
                           {                               
                               e.TrySetResult(progress.Status);
                               break;
                           }
                       case Google.Apis.Download.DownloadStatus.Failed:
                           {
                               e.TrySetResult(progress.Status);
                               break;
                           }
                   }
               };
            request.MediaDownloader.ProgressChanged += onProgress;
            await request.DownloadAsync(target);

            return await e.Task;
        }        
    }
}
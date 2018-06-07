using System;
using System.Threading.Tasks;
using Google.Apis.Download;

namespace IruddBlog.Infra.PostBackup
{
    public class GoogleDrivePostBackupService : IPostBackupService
    {
        private GoogleDriveBlogBackupService googleDriveBlogBackupService;
        public GoogleDrivePostBackupService(GoogleDriveBlogBackupService googleDriveBlogBackupService)
        {
            this.googleDriveBlogBackupService = googleDriveBlogBackupService;
        }

        public async Task<bool> DownloadPostsFromBackupIfNoLocalPostsExist() 
        {
            return await this.googleDriveBlogBackupService.DownloadPosts(GoogleDriveBlogBackupService.SpecialCaseAction.DoNothing);
        }

        public async Task<bool> UploadPostsToBackupIfLocalPostsExist()
        {
            var result = await this.googleDriveBlogBackupService.UploadPosts(GoogleDriveBlogBackupService.SpecialCaseAction.DoNothing);
            return result != null;
        }
    }

    public class DoNothingPostBackupService : IPostBackupService
    {
        public Task<bool> DownloadPostsFromBackupIfNoLocalPostsExist()
        {
            return Task.FromResult(false);
        }

        public Task<bool> UploadPostsToBackupIfLocalPostsExist()
        {
            return Task.FromResult(false);
        }
    }
}
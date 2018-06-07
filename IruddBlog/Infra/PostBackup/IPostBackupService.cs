using System.Threading.Tasks;

namespace IruddBlog.Infra.PostBackup
{
    public interface IPostBackupService
    {
        Task<bool> DownloadPostsFromBackupIfNoLocalPostsExist();
        Task<bool> UploadPostsToBackupIfLocalPostsExist();
    }
}
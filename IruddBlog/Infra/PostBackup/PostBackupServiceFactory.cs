using IruddBlog.Infra.PostBackup;
using IruddBlog.Infra.Settings;

namespace IruddBlog.Infra.PostBackup
{
    public class PostBackupServiceFactory : IPostBackupServiceFactory
    {
        private IBlogSettings blogSettings;

        public PostBackupServiceFactory(IBlogSettings blogSettings) 
        {
            this.blogSettings = blogSettings;
        }

        public IPostBackupService CreateService()
        {
            if(this.blogSettings.IsGoogleDriveSynchEnabled)
            {
                return new GoogleDriveBlogBackupService(blogSettings);
            }
            else
            {
                return new DoNothingPostBackupService();
            }
        }
    }
}
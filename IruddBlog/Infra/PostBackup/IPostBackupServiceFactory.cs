namespace IruddBlog.Infra.PostBackup
{
    public interface IPostBackupServiceFactory
    {
        IPostBackupService CreateService();
    }
}
namespace IruddBlog.Infra.Settings
{
    public interface IBlogSettings 
    {
        //TODO: Design issue. This is sort of pseudo setting since the ui code implicitly depends on this being accessible as /posts. Encapsulate this concept more.
        string LocalPostsFolder { get; }

        //TODO: Design issue. This is sort of pseudo setting since the ui code implicitly depends on this being accessible as /i. Encapsulate this concept more.
        string LocalImageTempFolder { get; }

        string GoogleLoginUserId { get; }
        string GoogleLoginClientId { get; }

        string GoogleDriveServiceAccountCredentialFile { get; }
        string GoogleDriveTargetFolder {get;}

        bool IsGoogleDriveSynchEnabled { get; }
    }
}
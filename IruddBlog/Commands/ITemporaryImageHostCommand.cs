namespace IruddBlog.Commands
{
    public interface ITemporaryImageHostCommand
    {
        string AddImageAsDataUrl(string dataUrl);
        bool TryRemoveImage(string filenameOrUrl);
    }
}
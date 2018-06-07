namespace IruddBlog.Commands
{
    public interface INewBlogPostCommand 
    {
        BeginCreatePostResult BeginCreatePost(string markdownContent, string title);
        void UploadImage(string postId, string base64ImageData, string tempFilename);
        void CommitCreateBlogPost(string postId);
    }
}
using System.Collections.Generic;

namespace IruddBlog.Commands
{
    public interface IGetBlogPostsCommand
    {
        IList<PostMetadata> GetAllPostsMetadata();
    }
}
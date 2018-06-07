using System;

namespace IruddBlog.Commands
{
    public class PostMetadata 
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PublicationDate { get; set; }
    }
}
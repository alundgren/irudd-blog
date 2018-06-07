using System;

namespace IruddBlog.Infra
{
    public interface IClock
    {
        DateTimeOffset Now {get;}
    }
}
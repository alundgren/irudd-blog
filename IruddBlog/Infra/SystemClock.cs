using System;

namespace IruddBlog.Infra
{
    public class SystemClock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
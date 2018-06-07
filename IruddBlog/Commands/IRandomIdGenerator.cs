namespace IruddBlog.Commands
{
    public interface IRandomIdGenerator
    {
        string GenerateId(int length);
    }
}
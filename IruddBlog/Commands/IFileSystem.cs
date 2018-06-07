using System.Collections.Generic;

namespace IruddBlog.Commands
{
    public interface IFileSystem 
    {
        void CreateDirectory(string directory);
        void WriteAllText(string filename, string content);
        void WriteAllBytes(string filename, byte[] content);

        string ReadAllText(string filename);
        IList<string> GetDirectories(string directory);
        bool TryDeleteFile(string filename);
        void MoveFile(string fromFilename, string toFilename);
        void WipeDirectory(string directory);
    }
}
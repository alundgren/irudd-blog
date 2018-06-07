using System.Collections.Generic;
using System.IO;

namespace IruddBlog.Commands
{
    public class FileSystem : IFileSystem
    {
        public void CreateDirectory(string directory)
        {
           Directory.CreateDirectory(directory);
        }

        public void WriteAllBytes(string filename, byte[] content)
        {
            File.WriteAllBytes(filename, content);
        }

        public void WriteAllText(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }

        public IList<string> GetDirectories(string directory)
        {
            if(!Directory.Exists(directory))
                return new List<string>();
            return Directory.GetDirectories(directory);
        }

        public string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }

        public bool TryDeleteFile(string filename) 
        {
            if(!File.Exists(filename))
                return false;
            File.Delete(filename);
            return true;
        }

        public void MoveFile(string fromFilename, string toFilename)
        {
            File.Move(fromFilename, toFilename);
        }

        public void WipeDirectory(string directory)
        {
            foreach(var f in Directory.GetFiles(directory)) 
            {
                File.Delete(f);
            }
        }
    }
}
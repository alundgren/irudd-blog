using System;
using IruddBlog.Commands;
using System.Collections.Generic;
using System.Linq;

namespace IruddBlog.Tests
{
    public class MockFileSystem : IFileSystem
    {
        private List<(string, string, string)> operations = new List<(string, string, string)>();
        
        public Dictionary<string, List<string>> KnownDirectories = new Dictionary<string, List<string>>();
        public Dictionary<string, string> KnownTextFiles = new Dictionary<string, string>();
        public Dictionary<string, byte[]> KnownBinaryFiles = new Dictionary<string, byte[]>();

        public void CreateDirectory(string directory)
        {
            operations.Add(("CreateDirectory", directory, null));
        }

        public void WriteAllBytes(string filename, byte[] content)
        {
            operations.Add(("WriteAllBytes", filename, Convert.ToBase64String(content)));
        }

        public void WriteAllText(string filename, string content)
        {
            operations.Add(("WriteAllText", filename, content));
        }        

        public IList<string> GetDirectories(string directory)
        {
            operations.Add(("GetDirectories", directory, null));
            if(KnownDirectories.ContainsKey(directory))
                return KnownDirectories[directory];
            else
                return new List<string>();
        }

        public string ReadAllText(string filename)
        {
            operations.Add(("ReadAllText", filename, null));
            if(KnownTextFiles.ContainsKey(filename))
                return KnownTextFiles[filename];
            else
                throw new System.IO.FileNotFoundException($"Unable to find the specified file: {filename}" );
        }

        public bool TryDeleteFile(string filename)
        {
            operations.Add(("TryDeleteFile", filename, null));
            if(KnownBinaryFiles.ContainsKey(filename))
                return true;
            else
                return false;            
        }
        
        public void MoveFile(string fromFilename, string toFilename)
        {
            if(KnownBinaryFiles.ContainsKey(fromFilename) || KnownTextFiles.ContainsKey(fromFilename))
            {
                operations.Add(("MoveFile", $"{fromFilename}#{toFilename}", null));
            }
            else
                throw new System.IO.IOException();
        }

        public void WipeDirectory(string directory)
        {
            operations.Add(("WipeDirectory", directory, null));
        }

        public bool WasWriteAllTextCalledWithFilename(string filename) 
        {
            return operations.Any(x => x.Item1 == "WriteAllText" && x.Item2 == filename);
        }

        public bool WasWriteAllTextCalledWithFilenameAndContent(string filename, string content) 
        {
            return operations.Any(x => x.Item1 == "WriteAllText" && x.Item2 == filename && x.Item3 == content);
        }

        public bool WasWriteAllBytesCalledWithFilenameAndContent(string filename, byte[] content) 
        {
            return operations.Any(x => x.Item1 == "WriteAllBytes" && x.Item2 == filename && x.Item3 == Convert.ToBase64String(content));
        }        

        public bool WasCreateDirectoryCalledWith(string path) 
        {
            return operations.Any(x => x.Item1 == "CreateDirectory" && x.Item2 == path);
        }

        public bool WasMoveFileCalledWith(string fromFilename, string toFilename)
        {
            return operations.Any(x => x.Item1 == "MoveFile" && x.Item2 == $"{fromFilename}#{toFilename}");
        }

        public bool WasWipeDirectoryCalledWith(string directory) 
        {
            return operations.Any(x => x.Item1 == "WipeDirectory" && x.Item2 == directory);
        }

        public string GetOperationLogMessage() 
        {
            return string.Join(Environment.NewLine, operations.Select(x => $"{x.Item1}: {x.Item2} {Environment.NewLine}{x.Item3}{Environment.NewLine}{Environment.NewLine}"));
        }
    }
}

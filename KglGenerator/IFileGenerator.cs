using System.Collections.Generic;
using System.IO;
using Model;

namespace FileGenerators
{
    public interface IFileGenerator
    {
        MemoryStream GenerateKglFile(IEnumerable<Song> songs);

        MemoryStream GenerateTxtFile(IEnumerable<Song> songs);

        MemoryStream GenerateDiffFile(List<string> songs);
    }
}

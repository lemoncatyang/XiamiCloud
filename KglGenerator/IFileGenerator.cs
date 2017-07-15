using System.IO;
using Model;

namespace FileGenerators
{
    public interface IFileGenerator
    {
        MemoryStream GenerateKglFile(TransformResult result);
    }
}

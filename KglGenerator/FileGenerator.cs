using System.IO;
using System.Text;
using Model;

namespace FileGenerators
{
    public class FileGenerator : IFileGenerator
    {
        public MemoryStream GenerateKglFile(TransformResult results)
        {
            var recognizedSongsSb = new StringBuilder();
            var unrecognizedSongsSb = new StringBuilder();

            recognizedSongsSb.Append($"<?xml version=\"1.0\" ?>");
            recognizedSongsSb.Append($"<List ListName=\"虾米红心\">");
            results.RecognizedSongs.ForEach(s =>
            {
                recognizedSongsSb.Append($"<File><FileName>{s.Artist} - {s.SongName}.mp3</FileName></File>");
            });

            recognizedSongsSb.Append($"</List>");

            unrecognizedSongsSb.Append("没有正确识别的歌曲名单如下：");
            results.UnrecognizedSongs.ForEach(u =>
            {
                unrecognizedSongsSb.Append($"{u.Artist} - {u.SongName}");
                unrecognizedSongsSb.Append("\n");
            });

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(recognizedSongsSb.ToString());
            writer.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}

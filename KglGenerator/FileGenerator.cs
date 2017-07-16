using System;
using System.IO;
using System.Text;
using Model;
using System.Collections.Generic;
using System.Linq;
namespace FileGenerators
{
    public class FileGenerator : IFileGenerator
    {
        public MemoryStream GenerateKglFile(IEnumerable<Song> songs)
        {
            var recognizedSongsSb = new StringBuilder();

            recognizedSongsSb.Append($"<?xml version=\"1.0\" ?>");
            recognizedSongsSb.Append($"<List ListName=\"虾米红心\">");
            songs.ToList().ForEach(s =>
            {
                recognizedSongsSb.Append($"<File><FileName>{s.Artist} - {s.SongName}.mp3</FileName></File>");
            });

            recognizedSongsSb.Append($"</List>");

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(recognizedSongsSb.ToString());
            writer.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        public MemoryStream GenerateTxtFile(IEnumerable<Song> songs)
        {
            var unrecognizedSongsSb = new StringBuilder();
            songs.ToList().ForEach(s =>
            {
                unrecognizedSongsSb.Append($"歌曲：{s.Artist} - 艺术家：{s.SongName}");
                unrecognizedSongsSb.Append(Environment.NewLine);
            });
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(unrecognizedSongsSb.ToString());
            writer.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        public MemoryStream GenerateDiffFile(List<string> songs)
        {
            var unrecognizedSongsSb = new StringBuilder();
            songs.ToList().ForEach(s =>
            {
                unrecognizedSongsSb.Append($"歌曲：{s}");
                unrecognizedSongsSb.Append(Environment.NewLine);
            });
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(unrecognizedSongsSb.ToString());
            writer.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}

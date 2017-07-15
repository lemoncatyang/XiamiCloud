using System.Collections.Generic;

namespace Model
{
    public class TransformResult
    {
        public List<Song> RecognizedSongs { get; set; } = new List<Song>();
        public List<Song> UnrecognizedSongs { get; set; } = new List<Song>();
    }
}

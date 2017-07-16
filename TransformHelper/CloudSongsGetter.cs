using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Model;

namespace TransformHelper
{
    public class CloudSongsGetter : ISongsGetter
    {
        public HttpClient HttpClient { get; set; } = new HttpClient();

        public HtmlWeb WebClient { get; set; } = new HtmlWeb();

        public TransformResult TransformResult { get; set; } = new TransformResult();

        public async Task<TransformResult> GetSongsBasedOnUrl(string url)
        {
            var htmlDoc = WebClient.Load(url);
            var songNameNodes = htmlDoc.DocumentNode.SelectNodes("//ul[@class='f-hide']//li/a[@href!='/song?id=${song.id}'][1]");
            for (var i = 0; i != songNameNodes.Count; i++)
            {
                var song = new Song
                {
                    SongName = songNameNodes[i].InnerHtml
                };
                TransformResult.RecognizedSongs.Add(song);
            }
            return TransformResult;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Model;
using HtmlAgilityPack;

namespace TransformHelper
{
    public class XiamiSongsGetter : ISongsGetter
    {
        public HttpClient HttpClient { get; set; } = new HttpClient();

        public TransformResult TransformResult { get; set; } = new TransformResult();

        public async Task<TransformResult> GetSongsBasedOnUrl(string url)
        {
            var questionMarkPos = url.IndexOf("?", StringComparison.Ordinal);
            if (questionMarkPos <= 0 || url.Contains("/page"))
            {
                return null;
            }

            url = url.Remove(questionMarkPos) + "/page/1";

            var songsCount = await GetSongsCount(url);
            var totalPages = (int)Math.Ceiling(songsCount / 25.0);

            // 根据页面数字，构造不同的url
            var urls = new List<string>();
            for (var i = 1; i <= totalPages; i++)
            {
                urls.Add(url.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1) + i);
            }

            urls.ForEach(async u =>
            {
                await GetSongsInfo(u);
            });

            return TransformResult;
        }

        private async Task GetSongsInfo(string url)
        {
            var content = await GetHtmlContent(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);
            var songNameNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='song_name']/a[1]");
            var artistNameNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='song_name']/a[@class='artist_name'][1]");

            if (songNameNodes.Count != artistNameNodes.Count)
            {
                return;
            }
            for (var i = 0; i != songNameNodes.Count; i++)
            {
                var song = new Song
                {
                    SongName = songNameNodes[i].InnerHtml,
                    Artist = artistNameNodes[i].InnerHtml
                };

                // 只处理songName全部是汉字或者数字或者英文字母的情况
                var regexItem = new Regex("[`@#$^&*=|{};<>！@#￥……&*（）——|{}【】‘；：”“。，、]");
                
                if (song.SongName.Any(c => regexItem.IsMatch(c.ToString())) || song.Artist.Any(c => regexItem.IsMatch(c.ToString())))
                {
                    TransformResult.UnrecognizedSongs.Add(song);
                }
                else
                {
                    TransformResult.RecognizedSongs.Add(song);
                }
            }
        }

        private async Task<int> GetSongsCount(string url)
        {
            var content = await GetHtmlContent(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);
            var songCountNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='counts']");
            var songCount = songCountNode.InnerText.Remove(songCountNode.InnerText.Length - 1);
            return Convert.ToInt32(songCount);
        }

        private async Task<string> GetHtmlContent(string url)
        {
            var response = HttpClient.GetAsync(url).Result;
            var result = await response.Content.ReadAsStringAsync();
            return WebUtility.HtmlDecode(result);
        }
    }
}

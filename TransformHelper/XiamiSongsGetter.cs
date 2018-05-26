using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Model;
using HtmlAgilityPack;

namespace TransformHelper
{
    public class XiamiSongsGetter : ISongsGetter
    {
        public HttpClient HttpClient { get; set; } = new HttpClient();

        public HtmlWeb WebClient { get; set; } = new HtmlWeb();

        public TransformResult TransformResult { get; set; } = new TransformResult();

        public async Task<TransformResult> GetSongsBasedOnUrl(string url)
        {
            HttpClient.DefaultRequestHeaders.Add("Cookie", "gid=15076376171495; _unsign_token=9286b5aad9a9e92e264f6415c3c2ab5c; bdshare_firstime=1507637618238; UM_distinctid=15f0634664aa25-0f89036b57550e-c303767-1fa400-15f0634664b498; cna=obRXEpyFhUMCAXLarFWZL7OU; join_from=gTDMS49M6TxmgKc; _xiamitoken=66c7f9e512bceca8df5f063c7365feab; _m_h5_tk=9d5f9fab5ea4af2e3ae10bd026c23ec2_1513258783210; _m_h5_tk_enc=db1409de2b0489944091feaa44093c95; CNZZDATA2629111=cnzz_eid%3D1091665217-1507636031-http%253A%252F%252Fmail.126.com%252F%26ntime%3D1513256645; login_method=tblogin; member_auth=0jrIHYYYvWkyivfDTNhkcScctLDTEjKOxNxQ2bIrtVQgcIhYZdetkKuVQA5J3CWXrmFWRePLhm4VSoEBdJ%2BZyA; user=42514546%22yangkeke_lov%22images%2Fdefault%2Favatar_g.jpg%220%2210540%22%3Ca+href%3D%27http%3A%2F%2Fwww.xiami.com%2Fwebsitehelp%23help9_3%27+%3ELv7%3C%2Fa%3E%220%220%2227493%22ca4d51ee20%221513256843; t_sign_auth=1; CNZZDATA921634=cnzz_eid%3D547028261-1507635160-http%253A%252F%252Fmail.126.com%252F%26ntime%3D1513258319; isg=Ahoasd2RFCc_oJtl7XBkx86Ka8D8458jjb9vXCSTxq14l7rRDNvuNeDlE1Xw");
            var cookieResponse = await HttpClient.GetAsync("https://www.xiami.com/space/lib-song/u/42514546?spm=a1z1s.6928797.1561534497.4.4omzvX");

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

            urls.ForEach(u =>
            {
                GetSongsInfo(u).GetAwaiter().GetResult();
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
            //HttpClient.DefaultRequestHeaders.Add("Host", "www.xiami.com");
            HttpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");
            var response = await HttpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return WebUtility.HtmlDecode(result);
        }
    }
}

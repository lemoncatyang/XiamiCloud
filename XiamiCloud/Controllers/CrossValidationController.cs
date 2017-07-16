using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TransformHelper;
using XiamiCloud.ViewModels;
using System.Linq;
using FileGenerators;

namespace XiamiCloud.Controllers
{
    public class CrossValidationController : Controller
    {
        private readonly CloudSongsGetter _cloudGetter;

        private readonly XiamiSongsGetter _xiamiGetter;

        private readonly IFileGenerator _fileGenerator;

        public CrossValidationController(CloudSongsGetter cloudGetter, XiamiSongsGetter xiamiGetter, IFileGenerator fileGenerator)
        {
            _cloudGetter = cloudGetter;
            _xiamiGetter = xiamiGetter;
            _fileGenerator = fileGenerator;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrossValidation(UrlInfos urlInfos)
        {
            await _cloudGetter.GetSongsBasedOnUrl(urlInfos.CloudUrlInfo.UrlPrefix + urlInfos.CloudUrlInfo.UrlStr);
            await _xiamiGetter.GetSongsBasedOnUrl(urlInfos.XiamiUrlInfo.UrlPrefix + urlInfos.XiamiUrlInfo.UrlStr);

            var cloudSongs = new List<string>();
            var xiamiSongs = new List<string>();
            _cloudGetter.TransformResult.RecognizedSongs.ForEach(r =>
            {
                cloudSongs.Add(r.SongName);
            });

            _xiamiGetter.TransformResult.RecognizedSongs.ForEach(r =>
            {
                xiamiSongs.Add(r.SongName);
            });

            var diffList = new List<string>();

            xiamiSongs.ForEach(s =>
            {
                if (!cloudSongs.Contains(s))
                {
                    diffList.Add(s);
                }
            });

            var fileStream = _fileGenerator.GenerateDiffFile(diffList);
            return File(fileStream, "application/x-msdownload", "在虾米歌单中却不在网易云歌单中的歌曲名.txt");
        }
    }
}
using System.Threading.Tasks;
using FileGenerators;
using Microsoft.AspNetCore.Mvc;
using XiamiCloud.Infrastructure;
using Model;
using TransformHelper;
using XiamiCloud.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace XiamiCloud.Controllers
{
    public class XiamiToCloudController : Controller
    {
        private readonly ISongsGetter _songsGetter;

        private readonly IFileGenerator _fileGenerator;

        public XiamiToCloudController(IFileGenerator fileGenerator, ISongsGetter songsGetter)
        {
            _fileGenerator = fileGenerator;
            _songsGetter = songsGetter;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Result(UrlInfo url)
        {
            await _songsGetter.GetSongsBasedOnUrl(url.UrlPrefix + url.UrlStr);
            var result = _songsGetter.TransformResult;
            HttpContext.Session.SetJson("result", result);
            return View(result);
        }

        [HttpPost]
        public IActionResult DownloadRecognizedSongsKgl()
        {
            var result = HttpContext.Session.GetJson<TransformResult>("result");
            var fileStream = _fileGenerator.GenerateKglFile(result.RecognizedSongs);
            return File(fileStream, "application/x-msdownload", "我的虾米歌单.kgl");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DownloadUnRecognizedSongsTxt()
        {
            var result = HttpContext.Session.GetJson<TransformResult>("result");
            var fileStream = _fileGenerator.GenerateTxtFile(result.UnrecognizedSongs);
            return File(fileStream, "application/x-msdownload", "识别失败歌曲列表.txt");
        }
    }
}

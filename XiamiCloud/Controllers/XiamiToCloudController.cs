using System.Threading.Tasks;
using FileGenerators;
using Microsoft.AspNetCore.Mvc;
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

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransformFromXiamiToCloud(UrlInfo url)
        {
            await _songsGetter.GetSongsBasedOnUrl(url.UrlPrefix + url.UrlStr);
            var result = _songsGetter.TransformResult;
            var fileStream = _fileGenerator.GenerateKglFile(result);
            return File(fileStream, "application/x-msdownload", "我的虾米歌单.kgl");
        }
    }
}

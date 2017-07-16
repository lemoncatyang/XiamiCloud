using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TransformHelper;
using XiamiCloud.ViewModels;

namespace XiamiCloud.Controllers
{
    public class CloudToXiamiController : Controller
    {
        private readonly CloudSongsGetter _songsGetter;

        public CloudToXiamiController(CloudSongsGetter songsGetter)
        {
            _songsGetter = songsGetter;
        }


        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Result(UrlInfo info)
        {
            await _songsGetter.GetSongsBasedOnUrl(info.UrlPrefix + info.UrlStr);
            return null;
        }
    }
}
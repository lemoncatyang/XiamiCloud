using Microsoft.AspNetCore.Mvc;
using XiamiCloud.ViewModels;

namespace XiamiCloud.Controllers
{
    public class CloudToXiamiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Result(UrlInfo info)
        {

        }
    }
}
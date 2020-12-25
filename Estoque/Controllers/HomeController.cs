
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        //[AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult Sobre()
        {

            return View();
        }
    }
}
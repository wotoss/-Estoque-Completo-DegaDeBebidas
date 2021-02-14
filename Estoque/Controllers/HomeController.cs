
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class HomeController : BaseController
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

using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class RelatorioController : Controller
    {
       
        [Authorize]
        public ActionResult Ressuprimento()
        {
            return View();
        }
    }
}
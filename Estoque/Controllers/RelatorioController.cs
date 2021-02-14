
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class RelatorioController : BaseController
    {
       
        [Authorize]
        public ActionResult Ressuprimento()
        {
            return View();
        }
    }
}
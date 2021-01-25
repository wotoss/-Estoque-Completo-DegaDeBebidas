
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class OperacaoController : Controller
    {
       
        
        [Authorize]
        public ActionResult LancPerdaProdutos()
        {
            return View();
        }
       
    }
}
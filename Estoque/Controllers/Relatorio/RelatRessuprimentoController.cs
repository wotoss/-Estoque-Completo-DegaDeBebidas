
using Estoque.Models;
using Rotativa;
using System.Web.Mvc;

namespace Estoque.Controllers.Relatorio
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class RelatRessuprimentoController : BaseController
    {
        //este é o filtro vai exibir, apenas a visão, tela a página
        [HttpGet]
        public ActionResult Filtro()
        {
            return View("~/Views/Relatorio/FiltroRelatRessuprimentoView.cshtml");
        }



        //Este ira exibir o relátorio
        [HttpPost]
        public ActionResult ValidarFiltro(int? minimo)
        {
            var ok = true;
            var mensagem = "";
            if ((minimo ?? 0) <= 0)
            {
                ok = false;
                mensagem = "Informe a quantidade mínima de cada produto.";
            }

            return Json(new { ok, mensagem });
        }

        [HttpGet]
        public ActionResult Index(int minimo)
        {
            if (minimo == 0)
            {
                return RedirectToAction("Filtro");
            }

            var estoque = ProdutoModel.RecuperarRelatRessuprimento(minimo);

            return new ViewAsPdf("~/Views/Relatorio/RelatRessuprimentoView.cshtml", estoque);
        }
    }
}
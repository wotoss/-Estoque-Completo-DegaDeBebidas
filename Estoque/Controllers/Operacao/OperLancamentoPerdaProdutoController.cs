using Estoque.Models;
using Estoque.Models.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Estoque.Controllers.Operacao
{
    [Authorize(Roles = "Gerente,Administrativo")]
    public class OperLancamentoPerdaProdutoController : BaseController
    {
        public ActionResult Index()
        {
            //Aqui estou preenchendo a minha lista de inventarios e (passando lá para o meu HTML atraves da ViewBag.Inventarios) no meu Select 
            ViewBag.Inventarios = ProdutoModel.RecuperarListaInventarioComDiferenca();

            return View();
        }

        [HttpGet]
        public JsonResult RecuperarListaProdutoComDiferencaEmInventario(string inventario)
        {
            var ret = ProdutoModel.RecuperarListaProdutoComDiferencaEmInventario(inventario);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Salvar(List<LancamentoPerdaViewModel> dados)
        {
            var ret = ProdutoModel.SalvarLancamentoPerda(dados);
            return Json(ret);
        }
    }
}
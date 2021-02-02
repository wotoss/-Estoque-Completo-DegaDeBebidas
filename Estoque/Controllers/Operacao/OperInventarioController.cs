using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Estoque.Controllers.Operacao
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class OperInventarioController : Controller //BaseController
    {
        public ActionResult Index()
        {
            var model = ProdutoModel.RecuperarListaParaInventario();
            return View(model);
        }

        [HttpPost]
        public JsonResult Salvar(List<ItemInventarioViewModel> dados)
        {
            var ok = ProdutoModel.SalvarInventario(dados);
            return Json(new { OK = ok });
        }
    }
}
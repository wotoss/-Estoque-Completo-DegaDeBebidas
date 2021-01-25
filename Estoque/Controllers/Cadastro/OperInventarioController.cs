
using Estoque.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class OperInventarioController : Controller
    {
        public ActionResult Index()
        {
            //Existem três maneiras de passar e informação entre as Views 
            //1- ViewBag, ViewData, e assim return View(model)
            //LEMBRANDO QUE LÁ NO MEU HTML PRECISO PASSAR => @model.ProdutoInventarioViewModel
            var model = ProdutoModel.RecuperarListaParaInventario();
            return View(model);
        }

        //vamos criar o método para Salvar o Inventário
        //[HttpPost]
        //public JsonResult Salvar(List<ItemInventarioViewModel> dados)
        //{
        //    var ok = ProdutoModel.SalvarInventario(dados);
        //    return Json(new { Ok = ok });
        //}
    }
}
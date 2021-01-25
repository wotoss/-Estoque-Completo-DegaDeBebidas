using Estoque.Models;
using Estoque.Models.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Estoque.Controllers.Operacao
{
    //esta classe passa a ser abstrata => não posso instância-la diretamente
    public abstract class OperEntradaSaidaProdutoController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Produtos = ProdutoModel.RecuperarLista(somenteAtivos: true);

            return View();
        }

        protected abstract string SalvarPedido(EntradaSaidaProdutoViewModel dados);


        public JsonResult Salvar([ModelBinder(typeof(EntradaSaidaProdutoViewModelModelBinder))] EntradaSaidaProdutoViewModel dados)
        {
            var numPedido = SalvarPedido(dados);
            var ok = (numPedido != "");

            //Veja o meu retorno se olhar no JavaScript .pos eu tenho um Ok e tambem no meu resonse Numero
            return Json(new { Ok = ok, Numero = numPedido });
        }
    }
}

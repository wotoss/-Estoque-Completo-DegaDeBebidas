
using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class CadCidadeController : BaseController
    {

        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            //var lista = Mapper.Map<List<ProdutoViewModel>>(ProdutoModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina));

            //var lista = CidadeModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            ViewBag.Paises = Mapper.Map<List<PaisViewModel>>(PaisModel.RecuperarLista());
            var lista = Mapper.Map<List<CidadeViewModel>>(CidadeModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina));
            var quant = CidadeModel.RecuperarQuantidade();
            ViewBag.QuantidadeRegistros = quant; //Colocar isto em todos
            ViewBag.QuantPaginas = QuantidadePaginas (quant);
            //var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            //ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;
            
           
            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CidadePagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = CidadeModel.RecuperarLista(pagina, tamPag, filtro, ordem);
            var quantRegistro = CidadeModel.RecuperarQuantidade();
            var quantidade = QuantidadePaginas(quantRegistro);
            return Json(new { Lista = lista, Quantidade = quantidade });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarCidade(int id)
        {
            var vm = CidadeModel.RecuperarPeloId(id);

            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarCidadesDoEstado(int idEstado)
        {
            var lista = CidadeModel.RecuperarLista(idEstado: idEstado);
            //desta forma eu estou incluindo um item na posição [0] zero do meu select
            lista.Insert(0, new CidadeViewModel { Id = -1, Nome = "-- Não Selecionado --" });
            return Json(lista);
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Administrativo")]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirCidade(int id)
        {
            var ok = CidadeModel.ExcluirPeloId(id);
            var quant = CidadeModel.RecuperarQuantidade();
            return Json(new { Ok = ok, Quantidade = quant });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarCidade(CidadeViewModel model)
        {
            var resultado = "OK";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;
            var quant = 0; //definição da quantidade em todos

            if (!ModelState.IsValid)
            {
                resultado = "AVISO";
                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                try
                {
                    var vm = Mapper.Map<CidadeModel>(model);
                    var id = vm.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                        quant = CidadeModel.RecuperarQuantidade(); //mas um para fazer em todos
                    }
                    else
                    {
                        resultado = "ERRO";
                    }
                }
                catch (Exception ex)
                {
                    resultado = "ERRO";
                }
            }

            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo, Quantidade = quant });
        }
    }
}



//using AutoMapper;
//using Estoque.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace Estoque.Controllers.Cadastro
//{

//    [Authorize(Roles = "Gerente,Administrativo,Operador")]
//    public class CadCidadeController : Controller /*BaseController*/
//    {
//        private const int _quantMaxLinhasPorPagina = 5;

//        public ActionResult Index()
//        {
//            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
//            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
//            ViewBag.PaginaAtual = 1;

//            var lista = CidadeModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
//            //como retornar um (int) não precisa retornar...
//            var quant = CidadeModel.RecuperarQuantidade();

//            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
//            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;
//            ViewBag.Paises = Mapper.Map<List<PaisViewModel>>(PaisModel.RecuperarLista());

//            return View(lista);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult CidadePagina(int pagina, int tamPag, string ordem)
//        {
//            var lista = CidadeModel.RecuperarLista(pagina, tamPag, ordem);

//            return Json(lista);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult RecuperarCidade(int id)
//        {
//            var vm = CidadeModel.RecuperarPeloId(id);
//            return Json(vm);          
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult RecuperarCidadesDoEstado(int idEstado)
//        {
//            var lista = CidadeModel.RecuperarLista(idEstado: idEstado);

//            return Json(lista);
//        }

//        [HttpPost]
//        [Authorize(Roles = "Gerente,Administrativo")]
//        [ValidateAntiForgeryToken]
//        public JsonResult ExcluirCidade(int id)
//        {
//            return Json(CidadeModel.ExcluirPeloId(id));
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult SalvarCidade(CidadeViewModel model)
//        {
//            var resultado = "OK";
//            var mensagens = new List<string>();
//            var idSalvo = string.Empty;

//            if (!ModelState.IsValid)
//            {
//                resultado = "AVISO";
//                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
//            }
//            else
//            {
//                try
//                {
//                    var vm = Mapper.Map<CidadeModel>(model);
//                    var id = vm.Salvar();
//                    if (id > 0)
//                    {
//                        idSalvo = id.ToString();
//                    }
//                    else
//                    {
//                        resultado = "ERRO";
//                    }
//                }
//                catch (Exception ex)
//                {
//                    resultado = "ERRO";
//                }
//            }

//            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
//        }
//    }
//}

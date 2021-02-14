
using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers.Cadastro
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class CadMarcaProdutoController : BaseController
    {
       
        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = Mapper.Map<List<MarcaProdutoViewModel>>(MarcaProdutoModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina));
            var quant = MarcaProdutoModel.RecuperarQuantidade();
            ViewBag.QuantidadeRegistros = quant;//COLQUEI AGORA
            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MarcaProdutoPagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = Mapper.Map<List<MarcaProdutoViewModel>>(MarcaProdutoModel.RecuperarLista(pagina, tamPag, filtro, ordem));
            var quantRegistro = MarcaProdutoModel.RecuperarQuantidade();
            var quantidade = QuantidadePaginas(quantRegistro);
            return Json(new { Lista = lista, Quantidade = quantidade });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarMarcaProduto(int id)
        {
            var vm = Mapper.Map<MarcaProdutoViewModel>(MarcaProdutoModel.RecuperarPeloId(id));

            return Json(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Administrativo")]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirMarcaProduto(int id)
        {
            var ok = MarcaProdutoModel.ExcluirPeloId(id);
            var quant = MarcaProdutoModel.RecuperarQuantidade();
            return Json(new { Ok = ok, Quantidade = quant });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarMarcaProduto(MarcaProdutoViewModel model)
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
                    var vm = Mapper.Map<MarcaProdutoModel>(model);
                    var id = vm.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                        quant = MarcaProdutoModel.RecuperarQuantidade(); //mas um para fazer em todos
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



//using Estoque.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace Estoque.Controllers.Cadastro
//{
//    [Authorize(Roles = "Gerente,Administrativo,Operador")]
//    public class CadMarcaProdutoController : Controller
//    {
//            private const int _quantMaxLinhasPorPagina = 5;

//            public ActionResult Index()
//            {
//                ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
//                ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
//                ViewBag.PaginaAtual = 1;

//                var lista = MarcaProdutoModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
//                var quant = MarcaProdutoModel.RecuperarQuantidade();

//                var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
//                ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

//                return View(lista);
//            }

//            [HttpPost]
//            [ValidateAntiForgeryToken]
//            public JsonResult MarcaProdutoPagina(int pagina, int tamPag/*, string filtro, string ordem*/)
//            {
//                var lista = MarcaProdutoModel.RecuperarLista(pagina, tamPag /*filtro, ordem*/);

//                return Json(lista);
//            }

//            [HttpPost]
//            [ValidateAntiForgeryToken]
//            public JsonResult RecuperarMarcaProduto(int id)
//            {
//                var vm = MarcaProdutoModel.RecuperarPeloId(id);

//                return Json(vm);
//            }

//            [HttpPost]
//            [Authorize(Roles = "Gerente,Administrativo")]
//            [ValidateAntiForgeryToken]
//            public JsonResult ExcluirMarcaProduto(int id)
//            {
//                return Json(MarcaProdutoModel.ExcluirPeloId(id));
//            }

//            [HttpPost]
//            [ValidateAntiForgeryToken]
//            public JsonResult SalvarMarcaProduto(MarcaProdutoModel model)
//            {
//                var resultado = "OK";
//                var mensagens = new List<string>();
//                var idSalvo = string.Empty;

//                if (!ModelState.IsValid)
//                {
//                    resultado = "AVISO";
//                    mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
//                }
//            else
//            {
//                try
//                {

//                    var id = model.Salvar();
//                    //SE O ID FOR MAIOR DO QUE ZERO ... ELE RETORNA O  ID
//                    if (id > 0)
//                    {
//                        idSalvo = id.ToString();
//                    }
//                    //CASO ACONTEÇA ALGUM ERRO
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
//            //vou criar um objeto anonimo para fazer o meu retorno
//            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
//        }
//    }
//}

using Estoque.Controllers;
using Estoque.Models;
using Estoque.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers.Cadastro
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class CadUnidadeMedidaController : BaseController
    {
      
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = Mapper.Map<List<UnidadeMedidaViewModel>>(UnidadeMedidaModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina));
            var quant = UnidadeMedidaModel.RecuperarQuantidade();

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

            return View(lista);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult UnidadeMedidaPagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = Mapper.Map<List<UnidadeMedidaViewModel>>(UnidadeMedidaModel.RecuperarLista(pagina, tamPag, filtro, ordem));

            return Json(lista);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarUnidadeMedida(int id)
        {
            var vm = Mapper.Map<UnidadeMedidaViewModel>(UnidadeMedidaModel.RecuperarPeloId(id));

            return Json(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirUnidadeMedida(int id)
        {
            return Json(UnidadeMedidaModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarUnidadeMedida(UnidadeMedidaViewModel model)
        {
            var resultado = "OK";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;

            if (!ModelState.IsValid)
            {
                resultado = "AVISO";
                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                try
                {
                    var vm = Mapper.Map<UnidadeMedidaModel>(model);
                    var id = vm.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
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

            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }
    }
}



//using Estoque.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Estoque.Controllers.Cadastro
//{
//    public class CadUnidadeMedidaController : Controller
//    {
//        private const int _quanMaxLinhasPorPagina = 5;

//        //Ele já esta trazendo do banco de dados através do método RecuperarLista();
//        [Authorize]
//        public ActionResult Index()

//        {
//            /*
//             * TODO
//             * Quantidade Maxima de paginas
//             * Quantidade de Pagina
//             * Pagina Atual
//             * 
//             */
//            ViewBag.ListaTamPag = new SelectList(new int[] { _quanMaxLinhasPorPagina, 10, 15, 20 }, _quanMaxLinhasPorPagina);
//            ViewBag.QuantMaxLinhasPorPagina = _quanMaxLinhasPorPagina;
//            ViewBag.PaginaAtual = 1;


//            //uma das forma de passar para a visão a quantidade de paginas ViewBag
//            var lista = UnidadeMedidaModel.RecuperarLista(ViewBag.PaginaAtual, _quanMaxLinhasPorPagina);
//            var quant = UnidadeMedidaModel.RecuperarQuantidade();

//            //
//            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
//            //Esta ViewBag.QuantPaginas vai retornar a parte inteira e vai somar com Zero ou Um de acordo com o resto =>(difQuantPaginas)
//            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

//            return View(lista);
//        }

//        [HttpPost]
//        [Authorize]
//        [ValidateAntiForgeryToken]
//        public JsonResult UnidadeMedidaPagina(int pagina, int tamPag)
//        {
//            var lista = UnidadeMedidaModel.RecuperarLista(pagina, tamPag);

//            return Json(lista);
//        }


//        //Neste estou recuperando o produto pelo Id
//        [HttpPost]
//        [Authorize]
//        [ValidateAntiForgeryToken]
//        public JsonResult RecuperarUnidadeMedida(int id)
//        {
//            return Json(UnidadeMedidaModel.RecuperarPeloId(id));
//        }

//        //O meu método ExcluirPeloId() faz todo o processo de verificação eu apenas passo na controller.
//        [HttpPost]
//        [Authorize]
//        [ValidateAntiForgeryToken]
//        public JsonResult ExcluirUnidadeMedida(int id)
//        {

//            return Json(UnidadeMedidaModel.ExcluirPeloId(id));
//        }

//        //Salvar  e Editar no mesmo método
//        [HttpPost]
//        [Authorize]
//        [ValidateAntiForgeryToken]
//        public JsonResult SalvarUnidadeMedida(UnidadeMedidaModel model)
//        {
//            var resultado = "Ok";
//            var mensagens = new List<string>();
//            var idSalvo = string.Empty;

//            if (!ModelState.IsValid)
//            {
//                resultado = "AVISO";
//                //vou na ModelState e pego os valore, dou um SelectMany caso tenha mais de um para selecionar trago o objeto.
//                //depois Select no ErrorMessage que ai sim trás a minha (string ou mensagem para o usuario do ModelSatate)
//                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
//            }
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
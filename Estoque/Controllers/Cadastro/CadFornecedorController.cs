
using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class CadFornecedorController : BaseController
    {
     
        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = Mapper.Map<List<FornecedorViewModel>>(FornecedorModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina));
            var quant = FornecedorModel.RecuperarQuantidade();

            ViewBag.QuantidadeRegistros = quant; //Colocar isto em todos

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;
            var paises = Mapper.Map<List<PaisViewModel>>(PaisModel.RecuperarLista());
            //desta forma eu estou incluindo um item na posição [0] zero do meu select
            paises.Insert(0, new PaisViewModel { Id = -1, Nome = "-- Não Selecionado --" });
            ViewBag.Paises = paises;

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult FornecedorPagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = Mapper.Map<List<FornecedorViewModel>>(FornecedorModel.RecuperarLista(pagina, tamPag, filtro, ordem));
            var quantRegistro = FornecedorModel.RecuperarQuantidade();
            var quantidade = QuantidadePaginas(quantRegistro);
            return Json(new { Lista = lista, Quantidade = quantidade });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarFornecedor(int id)
        {
            var vm = Mapper.Map<FornecedorViewModel>(FornecedorModel.RecuperarPeloId(id));
            return Json(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Administrativo")]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirFornecedor(int id)
        {
            var ok = FornecedorModel.ExcluirPeloId(id);
            var quant = FornecedorModel.RecuperarQuantidade();
            return Json(new { Ok = ok, Quantidade = quant });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarFornecedor(FornecedorViewModel model)
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
                    var vm = Mapper.Map<FornecedorModel>(model);
                    var id = vm.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                        quant = FornecedorModel.RecuperarQuantidade(); //mas um para fazer em todos
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
//using System.Web;
//using System.Web.Mvc;

//namespace Estoque.Controllers.Cadastro
//{
//    [Authorize(Roles = "Gerente, Adminstrativo, Operador")]       
//    public class CadFornecedorController : Controller
//    {

//        private const int _quanMaxLinhasPorPagina = 5;

//        //Ele já esta trazendo do banco de dados através do método RecuperarLista();
//        public ActionResult Index()

//        {

//            ViewBag.ListaTamPag = new SelectList(new int[] { _quanMaxLinhasPorPagina, 10, 15, 20 }, _quanMaxLinhasPorPagina);
//            ViewBag.QuantMaxLinhasPorPagina = _quanMaxLinhasPorPagina;
//            ViewBag.PaginaAtual = 1;


//            //uma das forma de passar para a visão a quantidade de paginas ViewBag
//            var lista = FornecedorModel.RecuperarLista(ViewBag.PaginaAtual, _quanMaxLinhasPorPagina);
//            var quant = FornecedorModel.RecuperarQuantidade();          
//            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
//            //Esta ViewBag.QuantPaginas vai retornar a parte inteira e vai somar com Zero ou Um de acordo com o resto =>(difQuantPaginas)
//            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;
//            ViewBag.Paises = PaisModel.RecuperarLista();

//            return View(lista);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult FornecedorPagina(int pagina, int tamPag)
//        {
//            var lista = FornecedorModel.RecuperarLista(pagina, tamPag);

//            return Json(lista);
//        }

//        //Neste estou recuperando o produto pelo Id
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult RecuperarFornecedor(int id)
//        {
//            return Json(FornecedorModel.RecuperarPeloId(id));
//        }

//        //O meu método ExcluirPeloId() faz todo o processo de verificação eu apenas passo na controller.
//        [HttpPost]
//        [Authorize(Roles = "Gerente, Administrativo")]
//        [ValidateAntiForgeryToken]
//        public JsonResult ExcluirFornecedor(int id)
//        {

//            return Json(FornecedorModel.ExcluirPeloId(id));
//        }

//        //Salvar  e Editar no mesmo método
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult SalvarFornecedor(FornecedorModel model)
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
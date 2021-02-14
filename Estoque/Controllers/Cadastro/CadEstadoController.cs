
using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class CadEstadoController : BaseController
    {
        
        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = Mapper.Map<List<EstadoViewModel>>(EstadoModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina));
            var quant = EstadoModel.RecuperarQuantidade();
            ViewBag.QuantidadeRegistros = quant;

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;
            ViewBag.Paises = Mapper.Map<List<PaisViewModel>>(PaisModel.RecuperarLista());

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EstadoPagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = Mapper.Map<List<EstadoViewModel>>(EstadoModel.RecuperarLista(pagina, tamPag, filtro, ordem));
            var quantRegistro = EstadoModel.RecuperarQuantidade();
            var quantidade = QuantidadePaginas(quantRegistro);
            return Json(new { Lista = lista, Quantidade = quantidade });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarEstadosDoPais(int idPais)
        {
            var lista = Mapper.Map<List<EstadoViewModel>>(EstadoModel.RecuperarLista(idPais: idPais));
            //desta forma eu estou incluindo um item na posição [0] zero do meu select
            lista.Insert(0, new EstadoViewModel { Id = -1, Nome = "-- Não Selecionado --" });
            return Json(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarEstado(int id)
        {
            var vm = Mapper.Map<EstadoViewModel>(EstadoModel.RecuperarPeloId(id));

            return Json(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Administrativo")]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirEstado(int id)
        {
            var ok = EstadoModel.ExcluirPeloId(id);
            var quant = EstadoModel.RecuperarQuantidade();
            return Json(new { Ok = ok, Quantidade = quant });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarEstado(EstadoViewModel model)
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
                    var vm = Mapper.Map<EstadoModel>(model);
                    var id = vm.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                        quant = EstadoModel.RecuperarQuantidade(); //mas um para fazer em todos
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
//    public class CadEstadoController : Controller
//    {
//        private const int _quanMaxLinhasPorPagina = 5;

//        //Ele já esta trazendo do banco de dados através do método RecuperarLista();
//        public ActionResult Index()

//        {

//            ViewBag.ListaTamPag = new SelectList(new int[] { _quanMaxLinhasPorPagina, 10, 15, 20 }, _quanMaxLinhasPorPagina);
//            ViewBag.QuantMaxLinhasPorPagina = _quanMaxLinhasPorPagina;
//            ViewBag.PaginaAtual = 1;


//            //uma das forma de passar para a visão a quantidade de paginas ViewBag
//            var lista = EstadoModel.RecuperarLista(ViewBag.PaginaAtual, _quanMaxLinhasPorPagina);
//            var quant = EstadoModel.RecuperarQuantidade();

//            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
//            //Esta ViewBag.QuantPaginas vai retornar a parte inteira e vai somar com Zero ou Um de acordo com o resto =>(difQuantPaginas)
//            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

//            //1- Passo para mostrar um DromDow ou Lista de opções Select
//            //2- Faz a recuperação da base de dados
//            //3- Lá na View uso o DropDownList Lá dou um SelectList e passo esta ViewBag.Paises
//            ViewBag.Paises = PaisModel.RecuperarLista(); //este viewbag esta listando todos os paises. Por que aqui vamos selecionar select pais

//            return View(lista);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult EstadoPagina(int pagina, int tamPag, string filtro)
//        {
//            var lista = EstadoModel.RecuperarLista(pagina, tamPag, filtro);

//            return Json(lista);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult RecuperarEstadosDoPais(int idPais)
//        {
//            var lista = EstadoModel.RecuperarLista(idPais: idPais);
//            return Json(lista);
//        }

//        //Neste estou recuperando o produto pelo Id
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult RecuperarEstado(int id)
//        {
//            return Json(EstadoModel.RecuperarPeloId(id));
//        }

//        //O meu método ExcluirPeloId() faz todo o processo de verificação eu apenas passo na controller.
//        [HttpPost]
//        [Authorize(Roles = "Gerente, Administrativo")]
//        [ValidateAntiForgeryToken]
//        public JsonResult ExcluirEstado(int id)
//        {

//            return Json(EstadoModel.ExcluirPeloId(id));
//        }

//        //Salvar  e Editar no mesmo método
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult SalvarEstado(EstadoModel model)
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
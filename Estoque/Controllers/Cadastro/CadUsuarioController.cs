﻿
using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    [Authorize(Roles = "Gerente")]
    public class CadUsuarioController : BaseController
    {
       
        private const string _senhaPadrao = "{$127;$188}";

        public ActionResult Index()
        {
            ViewBag.SenhaPadrao = _senhaPadrao;
            ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = Mapper.Map<List<UsuarioViewModel>>(UsuarioModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina));
            var quant = UsuarioModel.RecuperarQuantidade();

            ViewBag.QuantidadeRegistros = quant; //Colocar isto em todos

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UsuarioPagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = Mapper.Map<List<UsuarioViewModel>>(UsuarioModel.RecuperarLista(pagina, tamPag, filtro, ordem));
            var quantRegistro = UsuarioModel.RecuperarQuantidade();
            var quantidade = QuantidadePaginas(quantRegistro);
            return Json(new { Lista = lista, Quantidade = quantidade });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarUsuario(int id)
        {
            var vm = Mapper.Map<UsuarioViewModel>(UsuarioModel.RecuperarPeloId(id));
            vm.Senha = _senhaPadrao;

            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirUsuario(int id)
        {
            var ok = UsuarioModel.ExcluirPeloId(id);
            var quant = UsuarioModel.RecuperarQuantidade();
            return Json(new { Ok = ok, Quantidade = quant });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalvarUsuario(UsuarioViewModel model)
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
                    if (model.Senha == _senhaPadrao)
                    {
                        model.Senha = "";
                    }

                    var vm = Mapper.Map<UsuarioModel>(model);
                    var id = vm.Salvar();
                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                        quant = UsuarioModel.RecuperarQuantidade(); //mas um para fazer em todos
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












//using Estoque;
//using Estoque.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;

//namespace Estoque.Controllers
//{
//    [Authorize(Roles = "Gerente")]
//    public class CadUsuarioController : Controller
//    {

//        private const int _quanMaxLinhasPorPagina = 5;
//        //Vou fazer uma constante
//        private const string _senhaPadrao = "{$127;$188}";

//        //Ele já esta trazendo do banco de dados através do método RecuperarLista();

//        public ActionResult Index()
//        {

//            //vou passar a viewBag para a view
//            ViewBag.SenhaPadrao = _senhaPadrao;
//            ViewBag.ListaTamPag = new SelectList(new int[] { _quanMaxLinhasPorPagina, 10, 15, 20 }, _quanMaxLinhasPorPagina);
//            ViewBag.QuantMaxLinhasPorPagina = _quanMaxLinhasPorPagina;
//            ViewBag.PaginaAtual = 1;


//            //uma das forma de passar para a visão a quantidade de paginas ViewBag
//            var lista = UsuarioModel.RecuperarLista(ViewBag.PaginaAtual, _quanMaxLinhasPorPagina);
//            var quant = GrupoProdutoModel.RecuperarQuantidade();

//            //
//            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
//            //Esta ViewBag.QuantPaginas vai retornar a parte inteira e vai somar com Zero ou Um de acordo com o resto =>(difQuantPaginas)
//            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

//            return View(lista);
//        }


//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public JsonResult UsuarioPagina(int pagina, int tamPag)
//        {
//            var lista = UsuarioModel.RecuperarLista(pagina, tamPag);

//            return Json(lista);
//        }



//        //Neste estou recuperando o produto pelo Id
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult RecuperarUsuario(int id)
//        {
//            return Json(UsuarioModel.RecuperarPeloId(id));
//        }

//        //O meu método ExcluirPeloId() faz todo o processo de verificação eu apenas passo na controller.
//        [HttpPost]
//        [Authorize]
//        [ValidateAntiForgeryToken]
//        public ActionResult ExcluirUsuario(int id)
//        {

//            return Json(UsuarioModel.ExcluirPeloId(id));
//        }

//        //Salvar  e Editar no mesmo método
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult SalvarUsuario(UsuarioModel model)
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
//                    //senha padrão esta passando pelo js, e pelo UsuarioModel
//                    if (model.Senha == _senhaPadrao)
//                    {
//                        model.Senha = "";
//                    }

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
using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    [Authorize(Roles = "Gerente, Administrador, Operador")]
    public class CadGrupoProdutoController : Controller
    {
        private const int _quanMaxLinhasPorPagina = 5;

        //Ele já esta trazendo do banco de dados através do método RecuperarLista();
        public ActionResult Index()

        {
            /*
             * TODO
             * Quantidade Maxima de paginas
             * Quantidade de Pagina
             * Pagina Atual
             * 
             */
            ViewBag.ListaTamPag = new SelectList(new int[] { _quanMaxLinhasPorPagina, 10, 15, 20 }, _quanMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quanMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;


            //uma das forma de passar para a visão a quantidade de paginas ViewBag
            var lista = GrupoProdutoModel.RecuperarLista(ViewBag.PaginaAtual, _quanMaxLinhasPorPagina);
            var quant = GrupoProdutoModel.RecuperarQuantidade();

            //
            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            //Esta ViewBag.QuantPaginas vai retornar a parte inteira e vai somar com Zero ou Um de acordo com o resto =>(difQuantPaginas)
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GrupoProdutoPagina(int pagina, int tamPag, string filtro, string ordem)
        {
            var lista = GrupoProdutoModel.RecuperarLista(pagina, tamPag, filtro, ordem);

            return Json(lista);
        }


        //Neste estou recuperando o produto pelo Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarGrupoProduto(int id)
        {
            return Json(GrupoProdutoModel.RecuperarPeloId(id));
        }

        //O meu método ExcluirPeloId() faz todo o processo de verificação eu apenas passo na controller.
        [HttpPost]
        [Authorize(Roles = "Gerente, Administrativo")]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirGrupoProduto(int id)
        {
      
            return Json(GrupoProdutoModel.ExcluirPeloId(id));
        }

        //Salvar  e Editar no mesmo método
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarGrupoProduto(GrupoProdutoModel model)
        {
            var resultado = "Ok";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;

            if (!ModelState.IsValid)
            {
                resultado = "AVISO";
                //vou na ModelState e pego os valore, dou um SelectMany caso tenha mais de um para selecionar trago o objeto.
                //depois Select no ErrorMessage que ai sim trás a minha (string ou mensagem para o usuario do ModelSatate)
                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                try
                {
                   var id = model.Salvar();
                   //SE O ID FOR MAIOR DO QUE ZERO ... ELE RETORNA O  ID
                   if (id > 0)
                    {
                        idSalvo = id.ToString();
                    }
                   //CASO ACONTEÇA ALGUM ERRO
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
            //vou criar um objeto anonimo para fazer o meu retorno
            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }
       
    }
}
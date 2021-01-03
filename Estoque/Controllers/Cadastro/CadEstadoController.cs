using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Estoque.Controllers.Cadastro
{
    public class CadEstadoController : Controller
    {
        private const int _quanMaxLinhasPorPagina = 5;

        //Ele já esta trazendo do banco de dados através do método RecuperarLista();
        public ActionResult Index()

        {

            ViewBag.ListaTamPag = new SelectList(new int[] { _quanMaxLinhasPorPagina, 10, 15, 20 }, _quanMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quanMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;


            //uma das forma de passar para a visão a quantidade de paginas ViewBag
            var lista = EstadoModel.RecuperarLista(ViewBag.PaginaAtual, _quanMaxLinhasPorPagina);
            var quant = EstadoModel.RecuperarQuantidade();

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            //Esta ViewBag.QuantPaginas vai retornar a parte inteira e vai somar com Zero ou Um de acordo com o resto =>(difQuantPaginas)
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

            //1- Passo para mostrar um DromDow ou Lista de opções Select
            //2- Faz a recuperação da base de dados
            //3- Lá na View uso o DropDownList Lá dou um SelectList e passo esta ViewBag.Paises
            ViewBag.Paises = PaisModel.RecuperarLista(); //este viewbag esta listando todos os paises. Por que aqui vamos selecionar select pais

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EstadoPagina(int pagina, int tamPag, string filtro)
        {
            var lista = EstadoModel.RecuperarLista(pagina, tamPag, filtro);

            return Json(lista);
        }

        //Neste estou recuperando o produto pelo Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarEstado(int id)
        {
            return Json(EstadoModel.RecuperarPeloId(id));
        }

        //O meu método ExcluirPeloId() faz todo o processo de verificação eu apenas passo na controller.
        [HttpPost]
        [Authorize(Roles = "Gerente, Administrativo")]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirEstado(int id)
        {

            return Json(EstadoModel.ExcluirPeloId(id));
        }

        //Salvar  e Editar no mesmo método
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarEstado(EstadoModel model)
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
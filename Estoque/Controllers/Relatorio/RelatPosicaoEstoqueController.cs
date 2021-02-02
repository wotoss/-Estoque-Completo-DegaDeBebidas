
using Estoque.Models;
using Rotativa;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class RelatPosicaoEstoqueController : Controller
    {
        // GET: RelatPosicaoEstoque
        public ActionResult Index()
        {
            //com a variavel estoque nos recuperamos a informação para exibir
            var estoque = ProdutoModel.RecuperarRelatPosicaoEstoque();
            //passamos ela como parametro para ser exibida na tela RelatPosicaoEstoqueView.cshtml !! Muito bom
            return new ViewAsPdf("~/Views/Relatorio/RelatPosicaoEstoqueView.cshtml", estoque);
        }
    }
}
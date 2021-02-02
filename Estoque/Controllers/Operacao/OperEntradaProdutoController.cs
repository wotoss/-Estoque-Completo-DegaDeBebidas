
using Estoque.Controllers.Operacao;
using Estoque.Models;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class OperEntradaProdutoController : OperEntradaSaidaProdutoController
    {
        protected override string SalvarPedido(EntradaSaidaProdutoViewModel dados)
        {
            return ProdutoModel.SalvarPedidoEntrada(dados.Data, dados.Produtos);
        }
    }
}

using Estoque.Controllers.Operacao;
using Estoque.Models;
using Estoque.Models.Binders;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class OperEntradaProdutoController : OperEntradaSaidaProdutoController
    {
        protected override string SalvarPedido(EntradaSaidaProdutoViewModel dados)
        {
            return ProdutoModel.SalvarPedidoEntrada(dados.Data, dados.Produtos);
        }
    }
}
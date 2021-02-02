using Estoque.Models;
using System.Web.Mvc;

namespace Estoque.Controllers.Operacao
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class OperSaidaProdutoController : OperEntradaSaidaProdutoController
    {

        protected override string SalvarPedido(EntradaSaidaProdutoViewModel dados)
        {
            return ProdutoModel.SalvarPedidoSaida(dados.Data, dados.Produtos);
            
        }
    }
}
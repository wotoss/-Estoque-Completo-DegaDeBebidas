using Estoque.Models;
using Estoque.Models.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Estoque.Controllers.Operacao
{
    public class OperSaidaProdutoController : OperEntradaSaidaProdutoController
    {

        protected override string SalvarPedido(EntradaSaidaProdutoViewModel dados)
        {
            return ProdutoModel.SalvarPedidoSaida(dados.Data, dados.Produtos);
            
        }
    }
}
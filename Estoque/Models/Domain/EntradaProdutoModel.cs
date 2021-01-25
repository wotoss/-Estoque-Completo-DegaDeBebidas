

using System;

namespace Estoque.Models.Domain
{
    public class EntradaProdutoModel
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        public DateTime Data { get; set; }

        public int Quantidade { get; set; }

        public int IdProduto { get; set; }

        //esta associação de produto com entradaProdutos
        public virtual ProdutoModel Produto { get; set; }
    }
}
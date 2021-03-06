﻿
using System;

namespace Estoque.Models
{
    public class InventarioEstoqueModel
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Motivo { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int QuantidadeInventario { get; set; }
        public int IdProduto { get; set; }
        public virtual ProdutoModel Produto { get; set; }
    }
}
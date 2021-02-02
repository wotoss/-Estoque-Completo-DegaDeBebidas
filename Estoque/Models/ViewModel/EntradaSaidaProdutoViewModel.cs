

using System;
using System.Collections.Generic;

namespace Estoque.Models
{
    public class EntradaSaidaProdutoViewModel
    {
        public DateTime Data { get; set; }

        //Diconario int, int => significa que termos o Id do produto e depois a quantidade
        public Dictionary<int, int> Produtos { get; set; }
    }
}
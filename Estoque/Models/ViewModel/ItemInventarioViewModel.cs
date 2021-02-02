

namespace Estoque.Models
{
    //CONSTRUINDO ESTÁ VIEW MODEL PARA TRAFEGAR OS DADOS
    public class ItemInventarioViewModel
    {
        public int IdProduto { get; set; }

        public int QuantidadeEstoque { get; set; }

        public int QuantidadeInventario { get; set; }

        public string Motivo { get; set; }
    }
}
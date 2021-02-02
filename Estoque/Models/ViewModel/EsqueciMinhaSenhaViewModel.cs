
using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class EsqueciMinhaSenhaViewModel
    {
        [Required(ErrorMessage = "Digite o login")]
        public string Login { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;


namespace Estoque.Models
{
    public class NovaSenhaViewModel
    {
        [Required(ErrorMessage = "Digite a senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [Compare("Senha", ErrorMessage = "Digite a confirmação de sua senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string ConfirmacaoSenha { get; set; }

        //conseguiremos enviar o id do usuario veja lá na controller {Usuario = id} no método RedefinirSenha
        public int Usuario { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Estoque.Models
{
    public class AlteracaoSenhaUsuarioViewModel
    {
        [Required(ErrorMessage = "Digite a senha atual.")]
        [Display(Name = "Senha Atual")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Digite a nova senha.")]
        [MinLength(3, ErrorMessage = "A nova senha deve ter no mínimo 3 caracteres")]
        [Display(Name = "Nova Senha")]
        public string NovaSenha { get; set; }

        //Esta ação Compare é para fazer a comparação das senhas
        [Required(ErrorMessage = "Digite a confirmação da nova senha.")]
        [Compare("NovaSenha", ErrorMessage = "A senha e a confirmação devem ser iguais.")]
        [Display(Name = "Confirmação da Nova Senha")]
        public string ConfirmacaoNovaSenha { get; set; }
    }
  }

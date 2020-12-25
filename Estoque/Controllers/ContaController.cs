

using Estoque.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace Estoque.Controllers
{
    public class ContaController : Controller
    {
        //[HttpGet]
        [AllowAnonymous] //pode ser acessado por todos os usuários
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            //1º Coisa que faço é olhar se os dados que o usuario digitou estão corretos. Pelo (!ModelState.IsValid) na LoginViewModel 
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            //caso não esteja correto ele retorna a pagina de login

            var achou = UsuarioModel.ValidarUsuario(login.Usuario , login.Senha);
            if (achou)
            {
                FormsAuthentication.SetAuthCookie(login.Usuario, login.LembrarMe);
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                  return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Login inválido");
            }

            return View(login);
        }

        //método de logout
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}

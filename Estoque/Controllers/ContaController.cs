

using Estoque.Models;
using System;
using System.Web;
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

            var usuario = UsuarioModel.ValidarUsuario(login.Usuario , login.Senha);
            if (usuario != null)
            {   
                //VAMOS CRIAR O COOKIE EM TRÊS PASSOS => 1º CRIA O TICKETS, 2º COOKIE  3º E DEPOIS COLOCANDO NO RESPONSE PARA ENVIAR AO NAVEGADOR  
                //Neste momento eu criei o meu (tickts de autenticação) que contem o nomeDoUsuario, DatadeInicio, QuantidadeDe expiração, a persistencia, Quem é o usuario
               var ticket =  FormsAuthentication.Encrypt(new FormsAuthenticationTicket(
                    1, usuario.Nome, DateTime.Now, DateTime.Now.AddHours(12), login.LembrarMe, usuario.RecuperarStringNomePerfis()));//neste momento eu estou pegando o meu perfil dinâmicamente
                //Agora que tenho o meu ticket, vou criar  o meu cookie baseado no ticktes
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticket);
                //Agora vou responder ao meu browser ou navegador e adicionar Add no meu cookie
                Response.Cookies.Add(cookie);

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

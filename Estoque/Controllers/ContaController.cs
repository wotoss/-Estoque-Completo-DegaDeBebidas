

using Estoque.Models;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
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

            var usuario = UsuarioModel.ValidarUsuario(login.Usuario, login.Senha);
            if (usuario != null)
            {
                //VAMOS CRIAR O COOKIE EM TRÊS PASSOS => 1º CRIA O TICKETS, 2º COOKIE  3º E DEPOIS COLOCANDO NO RESPONSE PARA ENVIAR AO NAVEGADOR  
                //Neste momento eu criei o meu (tickts de autenticação) que contem o nomeDoUsuario, DatadeInicio, QuantidadeDe expiração, a persistencia, Quem é o usuario
                var ticket = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(
                     1, usuario.Nome, DateTime.Now, DateTime.Now.AddHours(12), login.LembrarMe, usuario.Id + "|" + usuario.RecuperarStringNomePerfis()));//neste momento eu estou pegando o meu perfil dinâmicamente
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



        [AllowAnonymous]
        public ActionResult AlterarSenhaUsuario(AlteracaoSenhaUsuarioViewModel model)
        {
            ViewBag.Mensagem = null;

            if (HttpContext.Request.HttpMethod.ToUpper() == "POST")
            {
                var usuarioLogado = (HttpContext.User as AplicacaoPrincipal);
                var alterou = false;
                if (usuarioLogado != null)
                {
                    if (!usuarioLogado.Dados.ValidarSenhaAtual(model.SenhaAtual))
                    {
                        ModelState.AddModelError("SenhaAtual", "A senha atual não confere.");
                    }
                    else
                    {
                        alterou = usuarioLogado.Dados.AlterarSenha(model.NovaSenha);

                        if (alterou)
                        {
                            ViewBag.Mensagem = new string[] { "ok", "Senha alterada com sucesso." };
                        }
                        else
                        {
                            ViewBag.Mensagem = new string[] { "erro", "Não foi possível alterar a senha." };
                        }
                    }
                }
                return View();
            }
            else
            {
                ModelState.Clear();
                return View();
            }
        }

        //Este método vai servi para GET quanto para POST
        //Tanto serve para GET como Para POST
        [AllowAnonymous]
        public ActionResult EsqueciMinhaSenha(EsqueciMinhaSenhaViewModel model)
        {
            ViewBag.EmailEnviado = true;
            if(HttpContext.Request.HttpMethod.ToUpper() == "GET")
            {
                ViewBag.EmailEnviado = false;
                ModelState.Clear();
            }
            //Quando for POST
            else
            {
                var usuario = UsuarioModel.RecuperarPeloLogin(model.Login);
                if(usuario != null)
                {
                    EnviarEmailRedefinicaoSenha(usuario);
                }
            }
            return View(model);
        }

        //Criar a redefinição de senha em GET
        [AllowAnonymous]
        public ActionResult RedefinirSenha(int id)
        {
            var usuario = UsuarioModel.RecuperarPeloId(id);
            if (usuario == null)
            {
                id = -1;
            }
            var model = new NovaSenhaViewModel() { Usuario = id };

            ViewBag.Mensagem = null;

            return View(model);
        }

        //Criar a redefinição com o método POST
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RedefinirSenha(NovaSenhaViewModel model)
        {
            ViewBag.Mensagem = null;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = UsuarioModel.RecuperarPeloId(model.Usuario);
            if (usuario != null)
            {
                var ok = usuario.AlterarSenha(model.Senha);
                ViewBag.Mensagem = ok ? "Senha alterada com sucesso!" : "Não foi possivel alterar a senha!";
            }
            return View();
        }




        //        private void EnviarEmailRedefinicaoSenha(UsuarioModel usuario)
        //        {

        //            //1º ação (RedefinirSenha, vai ser criado no model) 2º (Controller Conta) 3º (new Inclusão do id recebendo = usuario.Id ), Pro fim passamos o Protocolo (Request.Url.Scheme)
        //            var callbackUrl = Url.Action("RedefinirSenha", "Conta", new { id = usuario.Id }, protocol: Request.Url.Scheme);
        //            var client = new SmtpClient()
        //            {
        //                Host = ConfigurationManager.AppSettings["EmailServidor"],
        //                Port = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPorta"]),
        //                EnableSsl = (ConfigurationManager.AppSettings["EmailSsl"] == "S"),
        //                UseDefaultCredentials = false,
        //                Credentials = new NetworkCredential(
        //                    ConfigurationManager.AppSettings["EmailUsuario"],
        //                    ConfigurationManager.AppSettings["EmailSenha"])
        //            };

        //            //Agora ´para envia a mensagem vamos utilizar o MailMessage
        //            var mensagem = new MailMessage();
        //            mensagem.From = new MailAddress(ConfigurationManager.AppSettings["EmailOrigem"], "controle-de-estoque@comoprogramarmelhor.com.br");
        //            //para quem irá este email. para o email cadastrado
        //            mensagem.To.Add(usuario.Email);
        //            mensagem.Subject = "Redefinição de senha !";
        //            //Quando o usuario receber este texto simples e clicar em (aqui) o link(callbackUrl) irá direciona-lo redefinicao de senha. conforme montamos acima no inicio do método(EnviarEmailRedefinicaoSenha_
        //            mensagem.Body = string.Format("Redefina a sua senha <a href='{0}'>aqui</a>", callbackUrl);
        //            //mensagem.Body = string.Format("Redefina a sua senha <a href='{0}'>aqui</a>", callbackUrl);
        //            mensagem.IsBodyHtml = true;
        //            client.Send(mensagem);
        //        }
        //    }
        //}

        private void EnviarEmailRedefinicaoSenha(UsuarioModel usuario)
        {
            var callbackUrl = Url.Action("RedefinirSenha", "Conta", new { id = usuario.Id }, protocol: Request.Url.Scheme);
            var client = new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["EmailServidor"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPorta"]),
                EnableSsl = (ConfigurationManager.AppSettings["EmailSsl"] == "S"),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["EmailUsuario"],
                    ConfigurationManager.AppSettings["EmailSenha"])
            };

            var mensagem = new MailMessage();
            mensagem.From = new MailAddress(ConfigurationManager.AppSettings["EmailOrigem"], "Controle de Estoque - Como Programar Melhor");
            mensagem.To.Add(usuario.Email);
            mensagem.Subject = "Redefinição de senha";
            mensagem.Body = string.Format("Redefina a sua senha <a href='{0}'>aqui</a>", callbackUrl);
            mensagem.IsBodyHtml = true;

            client.Send(mensagem);
        }
    }
}


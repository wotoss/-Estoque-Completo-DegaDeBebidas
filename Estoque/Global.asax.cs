using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Estoque
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            if (ex is HttpRequestValidationException)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.ContentType = "application/json";
                Response.Write("{\"Resultado\":\"AVISO\",\"Mensagens\":[\"Somente texto sem caracteres especiais pode ser enviado.\"],\"IdSalvo\":\"\"}");
                Response.End();
            }
            //neste else não vamos retornar nada
            //Fizemos de proposito para o hacker que tiver fazendo o ataque. A pagina não vai mostrar nada.
            else if (ex is HttpAntiForgeryException)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.End();
            //Inclusi pode futuramente gravar um log => para sabermos que tivemos alguma solictação errada.
            //Para fazer uma vistoria depois.
            }
        }

        //AQUI EU VOU OBTER O COOKIE QUE EU CRIEI  NA CONTROLLER
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            //para eu obter eu dou um context
            var cookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

            //Aqui eu  verifico se o meu cookie está vindo....
            if (cookie != null && cookie.Value != string.Empty)
            {
                FormsAuthenticationTicket ticket;
                try
                {
                    //O cookie vindo preenhido da controller,  ele vem Criptografado. Neste momento eu vou Decriptografar...
                    ticket = FormsAuthentication.Decrypt(cookie.Value);
                }
                catch
                {
                    return;
                }
                //VEJA A LOGICA TODA DESTA TELA => NÓS ESTAMOS COOKIE DA CONTROLLER E TRAZENDO PARA PREENCHER O GENERICpRINCIPAL

                //neste momento eu estou trazendo o meu usuario (Gerente) atráves do (UserData) lá do meu controller
                // O ponto e virgula (';') => signnifica que tem mais de uma roles
                var perfis = ticket.UserData.Split(';');

                if (Context.User != null)
                {
                    Context.User = new GenericPrincipal(Context.User.Identity, perfis);
                }
            }
        }
    }
}

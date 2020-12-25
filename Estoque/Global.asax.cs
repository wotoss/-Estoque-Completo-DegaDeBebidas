using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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
            //neste else n�o vamos retornar nada
            //Fizemos de proposito para o hacker que tiver fazendo o ataque. A pagina n�o vai mostrar nada.
            else if (ex is HttpAntiForgeryException)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.End();
            //Inclusi pode futuramente gravar um log => para sabermos que tivemos alguma solicta��o errada.
            //Para fazer uma vistoria depois.
            }
        }
    }
}

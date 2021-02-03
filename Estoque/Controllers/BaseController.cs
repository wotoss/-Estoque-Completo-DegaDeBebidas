using AutoMapper;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class BaseController : Controller
    {
        protected const int _quantMaxLinhasPorPagina = 5;

        protected int QuantidadePaginas(int quantRegistro)
        {
            var difQuantPaginas = (quantRegistro % _quantMaxLinhasPorPagina) > 0 ? 1 : 0;

            return (quantRegistro / _quantMaxLinhasPorPagina) + difQuantPaginas;
        }

        public IMapper Mapper
        {
            get
            {
                var ret = (HttpContext.Items["Mapper"] as IMapper);
                return ret;
            }
        }
    }
}
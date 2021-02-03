
using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class GraficoController : BaseController
    {
        [Authorize]
        public ActionResult PerdaMes()
        {
            var mes = DateTime.Today.Month;//estou obtendo o mês e colocando dentro da variavel "para uso"
            var ano = DateTime.Today.Year;//estou obtendo o ano e colocando dentro da variavel "para uso"
            var quantidadeDias = DateTime.DaysInMonth(ano, mes);//aqui estou obtendo o ultimo dia dentro daquela quantidade (ano, dia)

            //vou adicionar cada um dos dias em uma lista
            var dias = new List<int>();
            var perdas = new List<int>();

            for (var dia = 1; dia <= quantidadeDias; dia++)
            {
                dias.Add(dia);
                perdas.Add(0);
            }

            foreach (var perdaBd in ProdutoModel.PerdasNoMes(mes, ano))
            {
                perdas[perdaBd.Dia - 1] = perdaBd.Quant;
            }

            ViewBag.Dias = dias;
            ViewBag.Mes = mes;
            ViewBag.Ano = ano;
            ViewBag.Perdas = perdas;

            return View();
        }

        [Authorize]
        public ActionResult EntradaSaidaMesa()
        {
            return View();
        }
    }
}
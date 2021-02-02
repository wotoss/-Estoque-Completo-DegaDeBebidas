﻿using AutoMapper;
using Estoque.Models;
using Estoque.Models.Binders;
using Estoque.Models.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Estoque.Controllers.Operacao
{
    public abstract class OperEntradaSaidaProdutoController : Controller//BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Produtos = Mapper.Map<List<ProdutoViewModel>>(ProdutoModel.RecuperarLista(somenteAtivos: true));

            return View();
        }

        protected abstract string SalvarPedido(EntradaSaidaProdutoViewModel dados);

        public JsonResult Salvar([ModelBinder(typeof(EntradaSaidaProdutoViewModelModelBinder))] EntradaSaidaProdutoViewModel dados)
        {
            var numPedido = SalvarPedido(dados);
            var ok = (numPedido != "");

            return Json(new { OK = ok, Numero = numPedido });
        }
    }
}
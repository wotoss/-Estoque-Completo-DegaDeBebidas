﻿using Estoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Estoque.Controllers.Cadastro
{
    [Authorize(Roles = "Gerente")]
    public class CadPerfilController : Controller
    {
            private const int _quanMaxLinhasPorPagina = 5;

            //Ele já esta trazendo do banco de dados através do método RecuperarLista();
            public ActionResult Index()

            {
            /*
             * TODO
             * Quantidade Maxima de paginas
             * Quantidade de Pagina
             * Pagina Atual
             * 
             */
                ViewBag.ListaUsuario = UsuarioModel.RecuperarLista();//para preencher a lista
                ViewBag.ListaTamPag = new SelectList(new int[] { _quanMaxLinhasPorPagina, 10, 15, 20 }, _quanMaxLinhasPorPagina);
                ViewBag.QuantMaxLinhasPorPagina = _quanMaxLinhasPorPagina;
                ViewBag.PaginaAtual = 1;


                //uma das forma de passar para a visão a quantidade de paginas ViewBag
                var lista =PerfilModel.RecuperarLista(ViewBag.PaginaAtual, _quanMaxLinhasPorPagina);
                var quant =PerfilModel.RecuperarQuantidade();

                //
                var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
                //Esta ViewBag.QuantPaginas vai retornar a parte inteira e vai somar com Zero ou Um de acordo com o resto =>(difQuantPaginas)
                ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

                return View(lista);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public JsonResult PerfilPagina(int pagina, int tamPag)
            {
                var lista =PerfilModel.RecuperarLista(pagina, tamPag);

                return Json(lista);
            }


            //Neste estou recuperando o produto pelo Id
            [HttpPost]
            [ValidateAntiForgeryToken]
            public JsonResult RecuperarPerfil(int id)
            {
            var ret = PerfilModel.RecuperarPeloId(id);
            ret.CarregarUsuarios();
                return Json(ret);
            }

            //O meu método ExcluirPeloId() faz todo o processo de verificação eu apenas passo na controller.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public JsonResult ExcluirPerfil(int id)
            {

                return Json(PerfilModel.ExcluirPeloId(id));
            }

            //Salvar  e Editar no mesmo método
            [HttpPost]
            [ValidateAntiForgeryToken]
            public JsonResult SalvarPerfil(PerfilModel model)
            {
                var resultado = "Ok";
                var mensagens = new List<string>();
                var idSalvo = string.Empty;

                if (!ModelState.IsValid)
                {
                    resultado = "AVISO";
                    //vou na ModelState e pego os valore, dou um SelectMany caso tenha mais de um para selecionar trago o objeto.
                    //depois Select no ErrorMessage que ai sim trás a minha (string ou mensagem para o usuario do ModelSatate)
                    mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                }
                else
                {
                    try
                    {
                        var id = model.Salvar();
                        //SE O ID FOR MAIOR DO QUE ZERO ... ELE RETORNA O  ID
                        if (id > 0)
                        {
                            idSalvo = id.ToString();
                        }
                        //CASO ACONTEÇA ALGUM ERRO
                        else
                        {
                            resultado = "ERRO";
                        }
                    }
                    catch (Exception ex)
                    {
                        resultado = "ERRO";
                    }

                }
                //vou criar um objeto anonimo para fazer o meu retorno
                return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
            }

        }
    }


var salvar_customizado = null;

function marcar_ordenacao_campo(coluna) {
    var ordem_crescente = true,
        ordem = coluna.find('i');

    if (ordem.length > 0) {
        ordem_crescente = ordem.hasClass('glyphicon-arrow-down');
        if (ordem_crescente) {
            ordem.removeClass('glyphicon-arrow-down');
            ordem.addClass('glyphicon glyphicon-arrow-up');
        }
        else {
            ordem.removeClass('glyphicon-arrow-up');
            ordem.addClass('glyphicon-arrow-down');
        }
    }
    else {
        $('.coluna-ordenacao i').remove();
        coluna.append('&nbsp;<i class="glyphicon glyphicon-arrow-down" style="color: #000000"></i>');
    }
}

function obter_ordem_grid() {
    var colunas_grid = $('.coluna-ordenacao'),
        ret = '';

    colunas_grid.each(function (index, item) {
        var coluna = $(item),
            ordem = coluna.find('i');

        if (ordem.length > 0) {
            ordem_crescente = ordem.hasClass('glyphicon-arrow-down');
            ret = coluna.attr('data-campo') + (ordem_crescente ? '' : ' desc');
            return true;
        }
    });

    return ret;
}

function add_anti_forgery_token(data) {
    data.__RequestVerificationToken = $('[name=__RequestVerificationToken]').val();
    return data;
}

function formatar_mensagem_aviso(mensagens) {
    var template =
        '<ul>' +
        '{{ #. }}' +
        '<li>{{ . }}</li>' +
        '{{ /. }}' +
        '</ul>';

    return Mustache.render(template, mensagens);
}

function abrir_form(dados) {
    set_dados_form(dados);

    var modal_cadastro = $('#modal_cadastro');

    $('#msg_mensagem_aviso').empty();
    $('#msg_aviso').hide();
    $('#msg_mensagem_aviso').hide();
    $('#msg_erro').hide();

    bootbox.dialog({
        title: 'Cadastro de ' + tituloPagina,
        message: modal_cadastro,
        className: 'dialogo',
    })
        .on('shown.bs.modal', function () {
            modal_cadastro.show(0, function () {
                set_focus_form();
            });
        })
        .on('hidden.bs.modal', function () {
            modal_cadastro.hide().appendTo('body');
        });
}

function criar_linha_grid(dados) {
    var template = $('#template-grid').html();
    return Mustache.render(template, dados);
}

function salvar_ok(response, param) {
    if (response.Resultado == "OK") {
        if (param.Id == 0) {
            param.Id = response.IdSalvo;
            $('#grid_cadastro').removeClass('invisivel');
            $('#mensagem_grid').addClass('invisivel');
            $('#quantidade_registros').text(response.Quantidade);

            var btn = $('ul.pagination > li.active').first();
            var pagina = (btn && btn.length == 1) ? parseInt(btn.text()) : 1;
            atualizar_grid(pagina);
        }
        else {
            var linha = $('#grid_cadastro').find('tr[data-id=' + param.Id + ']').find('td');
            preencher_linha_grid(param, linha);
        }

        $('#modal_cadastro').parents('.bootbox').modal('hide');
    }
    else if (response.Resultado == "ERRO") {
        $('#msg_aviso').hide();
        $('#msg_mensagem_aviso').hide();
        $('#msg_erro').show();
    }
    else if (response.Resultado == "AVISO") {
        $('#msg_mensagem_aviso').html(formatar_mensagem_aviso(response.Mensagens));
        $('#msg_aviso').show();
        $('#msg_mensagem_aviso').show();
        $('#msg_erro').hide();
    }
}

function salvar_erro() {
    swal('Aviso', 'Não foi possível salvar. Tente novamente em instantes.', 'warning');
}

function atualizar_grid(pagina, btn) {
    var ordem = obter_ordem_grid(),
        filtro = $('#txt_filtro'),
        tamPag = $('#ddl_tam_pag').val(),
        url = url_page_click,
        param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

    $.post(url, add_anti_forgery_token(param), function (response) {
        if (response) {
            var table = $('#grid_cadastro').find('tbody');

            table.empty();
            if (response.length > 0) {
                $('#grid_cadastro').removeClass('invisivel');
                $('#mensagem_grid').addClass('invisivel');

                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));
                }
            }
            else {
                $('#grid_cadastro').addClass('invisivel');
                $('#mensagem_grid').removeClass('invisivel');
            }

            if (btn) {
                btn.siblings().removeClass('active');
                btn.addClass('active');
            }
        }
    })
        .fail(function () {
            swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
        });
}

$(document).on('click', '#btn_incluir', function () {
    abrir_form(get_dados_inclusao());
})
    .on('click', '.btn-alterar', function () {
        var btn = $(this),
            id = btn.closest('tr').attr('data-id'),
            url = url_alterar,
            param = { 'id': id };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                abrir_form(response);
            }
        })
            .fail(function () {
                swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
            });
    })
    .on('click', '.btn-excluir', function () {
        var btn = $(this),
            tr = btn.closest('tr'),
            id = tr.attr('data-id'),
            url = url_excluir,
            param = { 'id': id };

        bootbox.confirm({
            message: "Realmente deseja excluir o " + tituloPagina + "?",
            buttons: {
                confirm: {
                    label: 'Sim',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'Não',
                    className: 'btn-success'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post(url, add_anti_forgery_token(param), function (response) {
                        if (response.Ok) {
                            tr.remove();
                            var quant = $('#grid_cadastro > tbody > tr').length;
                            if (quant == 0) {
                                $('#grid_cadastro').addClass('invisivel');
                                $('#mensagem_grid').removeClass('invisivel');
                            }
                            $('#quantidade_registros').text(response.Quantidade);
                        }
                    })
                        .fail(function () {
                            swal('Aviso', 'Não foi possível excluir. Tente novamente em instantes.', 'warning');
                        });
                }
            }
        });
    })
    .on('click', '#btn_confirmar', function () {
        var btn = $(this),
            url = url_confirmar,
            param = get_dados_form();
        //foi criado um salvar customizada. com a variavel declarada lá n inicio
        if (salvar_customizado && typeof (salvar_customizado) == 'function') {
            salvar_customizado(url, param, salvar_ok, salvar_erro);
        }
        else {
            $.post(url, add_anti_forgery_token(param), function (response) {
                salvar_ok(response, param);
            })
            .fail(function () {
              salvar_erro();
            });
        }
    })
    .on('click', '.page-item', function () {
        var btn = $(this);
        var pagina = parseInt(btn.text());
        atualizar_grid(pagina, btn);
    })
    .on('change', '#ddl_tam_pag', function () {
        var ordem = obter_ordem_grid(),
            ddl = $(this),
            filtro = $('#txt_filtro'),
            tamPag = ddl.val(),
            pagina = 1,
            url = url_tam_pag_change,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();
                if (response.length > 0) {
                    $('#grid_cadastro').removeClass('invisivel');
                    $('#mensagem_grid').addClass('invisivel');

                    for (var i = 0; i < response.length; i++) {
                        table.append(criar_linha_grid(response[i]));
                    }
                }
                else {
                    $('#grid_cadastro').addClass('invisivel');
                    $('#mensagem_grid').removeClass('invisivel');
                }

                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        })
            .fail(function () {
                swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
            });
    })
    .on('keyup', '#txt_filtro', function () {
        var ordem = obter_ordem_grid(),
            filtro = $(this),
            ddl = $('#ddl_tam_pag'),
            tamPag = ddl.val(),
            pagina = 1,
            url = url_filtro_change,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();
                if (response.length > 0) {
                    $('#grid_cadastro').removeClass('invisivel');
                    $('#mensagem_grid').addClass('invisivel');

                    for (var i = 0; i < response.length; i++) {
                        table.append(criar_linha_grid(response[i]));
                    }
                }
                else {
                    $('#grid_cadastro').addClass('invisivel');
                    $('#mensagem_grid').removeClass('invisivel');
                }

                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        })
            .fail(function () {
                swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
            });
    })
    .on('click', '.coluna-ordenacao', function () {
        marcar_ordenacao_campo($(this));

        var ordem = obter_ordem_grid(),
            filtro = $('#txt_filtro'),
            ddl = $('#ddl_tam_pag'),
            tamPag = ddl.val(),
            pagina = 1,
            url = url_filtro_change,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');

                table.empty();
                if (response.length > 0) {
                    $('#grid_cadastro').removeClass('invisivel');
                    $('#mensagem_grid').addClass('invisivel');

                    for (var i = 0; i < response.length; i++) {
                        table.append(criar_linha_grid(response[i]));
                    }
                }
                else {
                    $('#grid_cadastro').addClass('invisivel');
                    $('#mensagem_grid').removeClass('invisivel');
                }

                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        })
            .fail(function () {
                swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
            });
    });

$(document).ready(function () {
    var grid = $('#grid_cadastro > tbody');
    for (var i = 0; i < linhas.length; i++) {
        grid.append(criar_linha_grid(linhas[i]));
    }

    marcar_ordenacao_campo($('#grid_cadastro thead tr th:nth-child(1) span'));
});





////O QUE FOR GERAL EM JS, VOU FAZER NO CADASTROBASE.JS

////O QUE FOR ESPECIFICO ESTOU DEIXANDO NO GRUPOPRODUTO.JS

//function add_anti_forgery_token(data) {
//    data.__RequestVerificationToken = $('[name=__RequestVerificationToken]').val();
//    return data;
//}

////Ordenação no grid
//function marcar_ordenacao_campo(coluna) {
//    //estou iniciando de ordem crecente
//    var ordem_crescente = true,
//        //Ele vai em busca do elemento html (i)
//        ordem = coluna.find('i');

//    if (ordem.length > 0) {
//        //tendo o elemento (i) o que eu faço. Removo o down glyphicon-arrow-down e coloco o up => glyphicon glyphicon-arrow-up
//        ordem_crescente = ordem.hasClass('glyphicon-arrow-down');
//        if (ordem_crescente) {
//            ordem.removeClass('glyphicon-arrow-down');
//            ordem.addClass('glyphicon glyphicon-arrow-up');
//        } else {
//            ordem.removeClass('glyphicon-arrow-up');
//            ordem.addClass('glyphicon-arrow-down');
//        }
//    }
//    else {
//        //Se não encontrar nenhum remove o elemento (i)
//        $('.coluna-ordenacao i').remove();
//        //e acrescenta este append => esta linha
//        coluna.append('&nbsp;<i class="glyphicon glyphicon-arrow-down" style="color: #000000"></i>');
//    }
//}

//function obter_ordem_grid() {
//    // colunas_grid obtem todas as colunas do grid que estão marcadas para ordenação
//    var colunas_grid = $('.coluna-ordenacao'),
//        ret = '';

//    colunas_grid.each(function (index, item) {
//        var coluna = $(item),
//            //aqui ele tenta buscar o elemento (i)
//            ordem = coluna.find('i');

//        //encontrando o elemento (i) eu vejo se está de forma ordenada, forma crescente ou descendente
//        if (ordem.length > 0) {
//            ordem_crescente = ordem.hasClass('glyphicon-arrow-down');
//            ret = coluna.attr('data-campo') + (ordem_crescente ? '' : ' desc');
//            return true;
//        }
//    });
//    return ret;
//}


////Esta forma estou fazendo uma LISTA com Vanilla JavaScript
//function formatar_mensagem_aviso(mensagens) {
//    var template =
//        '<lu>' +
//        '{{ #. }}' + //para cada item
//        '<li>{{ . }}</li>' + //cada um terá uma linha li
//        '{{ /. }}' +
//        '</lu>';
//    //aqui ele aplica e faz a renderização
//    return Mustache.render(template, mensagens); 
//}


//function abrir_form(dados) {
//    //construi uma função e estou usando ele no GrupoProduto.js
//    set_dados_form(dados);
 
//    //vamos criar uma variavel, vamos usar muitas vezes
//    var modal_cadastro = $('#modal_cadastro');
//    $('#msg_mensagem_aviso').empty();
//    $('#msg_aviso').hide();
//    $('#msg_mensagem_aviso').hide();
//    $('#msg_erro').hide();

//    bootbox.dialog({
//        title: 'Cadastro de ' + tituloPagina,
//        message: modal_cadastro,
//        className: 'dialogo',
//    })
//        .on('shown.bs.modal', function () {
//            modal_cadastro.show(0, function () {
//                set_focus_form();
                
//            });
//        })
//        .on('hidden.bs.modal', function () {
//            modal_cadastro.hide().appendTo('body');
//        });
//}

////Criando Linhas
//function criar_linha_grid(dados) {

//    //FEITO E FUNCIONANDO EM JS VANILLA
//    //var ret =
//    //    '<tr data-id=' + dados.Id + '>' +
//    //    set_dados_grid(dados) +
//    //    '<td>' +
//    //    '<a class="btn btn-primary btn-alterar" role="button" style="margin-right: 3px"><i class="glyphicon glyphicon-pencil"></i> Editar</a>' +
//    //    '<a class="btn btn-danger btn-excluir" role="button"><i class="glyphicon glyphicon-trash"></i> Excluir</a>' +
//    //    '</td>' +
//    //    '</tr>';
//    //return ret;

//    //USANDO MUSTACHE
//    var template = $('#template-grid').html(); //estou criando aqui #template-grid e quero o todo o .html() dele

//    return Mustache.render(template, dados)

//}

//$(document).on('click', '#btn_incluir', function () {
//    abrir_form(get_dados_inclusao());

//})
//    //aqui vammos fazer um ajax
//    .on('click', '.btn-alterar', function () {

//        //tenho que passar três paramentros => url, paramentro, e o que ele vai excultar seria function
//        var btn = $(this),
//            id = btn.closest('tr').attr('data-id'),//estou pegando o Id pelo tr ..dou closest e consigo buscar o mais proximo.

//            url = url_alterar,
//            param = { 'id': id }

//        $.post(url, add_anti_forgery_token(param), function (response) {
//            if (response) {
//                //estou retornando os dados pela função de Post
//                abrir_form(response)
//            }
//        })
//            //Quando houver um erro
//            .fail(function () {
//                swal('Aviso', 'Não foi possivel recuperar as informações ! Tente novamente em instantes.', 'warning');
//            });
//    })




//    //Exibindo um POPAP para excluir
//    .on('click', '.btn-excluir', function () {

//        var btn = $(this),
//            tr = btn.closest('tr'),
//            id = tr.attr('data-id'),//estou pegando o Id pelo tr ..dou closest e consigo buscar o mais proximo.

//            url = url_excluir,
//            param = { 'id': id }

//        bootbox.confirm({
//            message: "Deseja excluir o " + tituloPagina + "?",
//            buttons: {
//                confirm: {
//                    label: 'Sim',
//                    className: 'btn-danger'
//                },
//                cancel: {
//                    label: 'Não',
//                    className: 'btn-success'
//                }
//            },
//            callback: function (result) {

//                if (result) {
//                    $.post(url, add_anti_forgery_token(param), function (response) {
//                        if (response) {
//                            tr.remove();//desta forma foi excluida a linha no back ende
//                            var quantidade = $('#grid_cadastro > tbody > tr').length; //desta forma eu consigo ver a quantidade de linha que existe no gride (length)
//                            if (quantidade == 0) {//Se o tamanho ou quantidade == 0. Quero dizer não tem mais linha. 
//                                $('#grid_cadastro').addClass('invisivel');//ai eu adiciono a class invisivel "Quem vem com a mensagem NENHUM REGISTRO"
//                                $('#mensagem_grid').removeClass('invisivel');//Se ainda tiver linha no gride eu removo a classe e mostro os item as linhas normalmente
//                            }
//                        }
//                    })

//                        //Quando houver um erro
//                        .fail(function () {
//                            swal('Aviso', 'Não foi possivel excluir ! Tente novamente em instantes.', 'warning');
//                        });
//                }
//            }
//        });
//    })

//    //Salvar
//    .on('click', '#btn_confirmar', function () {
//        var btn = $(this),
//            url = url_confirmar,

//            param = get_dados_form();
//        //Estou fazendo o incluir funcionar
//        $.post(url, add_anti_forgery_token(param), function (response) {
//            if (response.Resultado == "Ok") {
//                if (param.Id == 0) {
//                    param.Id = response.IdSalvo;
//                    var table = $('#grid_cadastro').find('tbody'),
//                        //veja que o param tem tudo que eu preciso Id,Nome, Ativo
//                        //Eu passo ele para criar a linha.
//                        linha = criar_linha_grid(param);
//                    table.append(linha);
//                    $('#grid_cadastro').removeClass('invisivel'); //esta removendo a classe invisivel 
//                    $('#mensagem_grid').addClass('invisivel'); //esta adicionando a classe invisivel
//                    //Editar || alterar
//                } else {
//                    var linha = $('#grid_cadastro').find('tr[data-id=' + param.Id + ']').find('td');
//                    preencher_linha_grid(param, linha);
//                }
//                $('#modal_cadastro').parents('.bootbox').modal('hide');
//            }
//            //o que eu vou mostrar quando acontecer um erro
//            else if (response.Resultado == "ERRO") {
//                $('#msg_aviso').hide();
//                $('#msg_mensagem_aviso').hide();
//                $('#msg_erro').show();
//            }
//            else if (response.Resultado == "AVISO") {

//                $('#msg_mensagem_aviso').html(formatar_mensagem_aviso(response.Mensagens));
//                $('#msg_aviso').show();
//                $('#msg_mensagem_aviso').show();
//                $('#msg_erro').hide();
//            }
//        })
//            //Quando houver um erro
//            .fail(function () {
//                swal('Aviso', 'Não foi possivel salvar as informações ! Tente novamente em instantes.', 'warning');
//            });
//    })

//    //Este page-item é a paginação
//    .on('click', '.page-item', function () {
//        var ordem = obter_ordem_grid(),
//            btn = $(this),
//            filtro = $('#txt_filtro'),
//            tamPag = $('#ddl_tam_pag').val(),
//            pagina = btn.text(),
//            url = url_page_click,
//            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

//        //post é o meu envio o que vou passa ao back end => GrupoProdutoPagina
//        $.post(url, add_anti_forgery_token(param), function (response) {
//            if (response) {
//                var table = $('#grid_cadastro').find('tbody');
//                //vou limpar a tabela
//                table.empty();
//                if (response.length > 0) {
//                    $('#grid_cadastro').removeClass('invisivel'); //esta removendo a classe invisivel 
//                    $('#mensagem_grid').addClass('invisivel'); //esta adicionando a classe invisivel
//                    //no for eu já populo a minha lista => montando a nova paginação
//                    for (var i = 0; i < response.length; i++) {
//                        table.append(criar_linha_grid(response[i]));
//                    }
//                }
//                else {
//                    $('#grid_cadastro').addClass('invisivel'); //esta removendo a classe invisivel 
//                    $('#mensagem_grid').removeClass('invisivel'); //esta adicionando a classe invisivel
//                }
//                //para remover e adicionar o item selecionado
//                btn.siblings().removeClass('active');
//                btn.addClass('active');
//            }
//        })
//            //Quando houver um erro
//            .fail(function () {
//                swal('Aviso', 'Não foi possivel recuperar as informações ! Tente novamente em instantes.', 'warning');
//            });

//    })

//    // Este ddl_tam_pag é o seletor que mudamos de pagina
//    //QUANDO CLIKAR NO SELECT MUDAR A PAGINA
//    .on('change', '#ddl_tam_pag', function () {
//        var ordem = obter_ordem_grid(),
//            ddl = $(this),
//            filtro = $('#txt_filtro'),
//            tamPag = ddl.val(),
//            pagina = 1,
//            url = url_tam_pag_change,
//            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };


//        //post é o meu envio o que vou passa ao back end => GrupoProdutoPagina
//        $.post(url, add_anti_forgery_token(param), function (response) {
//            if (response) {
//                var table = $('#grid_cadastro').find('tbody');
//                //vou limpar a tabela
//                table.empty();

//                if (response.length > 0) {
//                    $('#grid_cadastro').removeClass('invisivel'); //esta removendo a classe invisivel 
//                    $('#mensagem_grid').addClass('invisivel'); //esta adicionando a classe invisivel
//                    //no for eu já populo a minha lista => montando a nova paginação
//                    for (var i = 0; i < response.length; i++) {
//                        table.append(criar_linha_grid(response[i]));
//                    }
//                }
//                else {
//                    $('#grid_cadastro').addClass('invisivel'); //esta removendo a classe invisivel 
//                    $('#mensagem_grid').removeClass('invisivel'); //esta adicionando a classe invisivel
//                }
//                //para remover e adicionar o item selecionado
//                ddl.siblings().removeClass('active');
//                ddl.addClass('active');
//            }
//        })
//            //Quando houver um erro
//            .fail(function () {
//                swal('Aviso', 'Não foi possivel recuperar as informações ! Tente novamente em instantes.', 'warning');
//            });

//    })

//    //VAMOS MONTAR A REQUISIÇÃO ATRAVES DE JSON => REQUISIÇÃO DO FRONT END PARA O BACK END

//    .on('keyup', '#txt_filtro', function () { //keyup => pressionar um tecla
//        var ordem = obter_ordem_grid(),
//            filtro = $(this),
//            ddl = $('#ddl_tam_pag'),//Estou buscando o nosso droopdown
//            tamPag = ddl.val(),
//            pagina = 1,
//            url = url_filtro_change, //lá no meu CadGrupoProduto arquivo Index. cshtml esta a minha url no final da pagina... E a minha url, vai até a controller
//            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem }; //eu vou enviar estes parametros para o meu back-end


//        //post é o meu envio o que vou passa ao back end => GrupoProdutoPagina
//        $.post(url, add_anti_forgery_token(param), function (response) {
//            if (response) {
//                var table = $('#grid_cadastro').find('tbody');
//                //vou limpar a tabela
//                table.empty();

//                if (response.length > 0) {
//                    $('#grid_cadastro').removeClass('invisivel'); //esta removendo a classe invisivel 
//                    $('#mensagem_grid').addClass('invisivel'); //esta adicionando a classe invisivel
//                    //no for eu já populo a minha lista => montando a nova paginação
//                    for (var i = 0; i < response.length; i++) {
//                        table.append(criar_linha_grid(response[i]));
//                    }
//                }
//                else {
//                    $('#grid_cadastro').addClass('invisivel'); //esta removendo a classe invisivel 
//                    $('#mensagem_grid').removeClass('invisivel'); //esta adicionando a classe invisivel
//                }

//                //para remover e adicionar o item selecionado
//                ddl.siblings().removeClass('active');
//                ddl.addClass('active');
//            }
//        })
//            //Quando houver um erro
//            .fail(function () {
//                swal('Aviso', 'Não foi possivel recuperar as informações ! Tente novamente em instantes.', 'warning');
//            });

//    })
//    //estamos dando um click na coluna ordenação lá no Index.html (GrupoProduto)
//    //pego a class (.coluna-ordenacao) da coluna html
//    .on('click', '.coluna-ordenacao', function () {
//        //quando acontecer o click eu chamo esta função marcar_ordenacao_campo => no meu click
//        marcar_ordenacao_campo($(this));

        
//        var ordem = obter_ordem_grid(),
//            filtro = $('#txt_filtro'),
//            ddl = $('#ddl_tam_pag'),//Estou buscando o nosso droopdown
//            tamPag = ddl.val(),
//            pagina = 1,
//            url = url_filtro_change, //lá no meu CadGrupoProduto arquivo Index. cshtml esta a minha url no final da pagina... E a minha url, vai até a controller
//            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem }; //eu vou enviar estes parametros para o meu back-end


//        //post é o meu envio o que vou passa ao back end => GrupoProdutoPagina
//        $.post(url, add_anti_forgery_token(param), function (response) {
//            if (response) {
//                var table = $('#grid_cadastro').find('tbody');
//                //vou limpar a tabela
//                table.empty();

//                if (response.length > 0) {
//                    $('#grid_cadastro').removeClass('invisivel'); //esta removendo a classe invisivel 
//                    $('#mensagem_grid').addClass('invisivel'); //esta adicionando a classe invisivel
//                    //no for eu já populo a minha lista => montando a nova paginação
//                    for (var i = 0; i < response.length; i++) {
//                        table.append(criar_linha_grid(response[i]));
//                    }
//                }
//                else {
//                    $('#grid_cadastro').addClass('invisivel'); //esta removendo a classe invisivel 
//                    $('#mensagem_grid').removeClass('invisivel'); //esta adicionando a classe invisivel
//                }

//                //para remover e adicionar o item selecionado
//                ddl.siblings().removeClass('active');
//                ddl.addClass('active');
//            }
//        })
//            //Quando houver um erro
//            .fail(function () {
//                swal('Aviso', 'Não foi possivel recuperar as informações ! Tente novamente em instantes.', 'warning');
//            });

//    });

//$(document).ready(function () {
//    var grid = $('#grid_cadastro > tbody');
//    for (var i = 0; i < linhas.length; i++) {
//        grid.append(criar_linha_grid(linhas[i]));
//    }
//    marcar_ordenacao_campo($('#grid_cadastro thead tr th:nth-child(1) span'));
//});




//O QUE FOR GERAL EM JS, VOU FAZER NO CADASTROBASE.JS

//O QUE FOR ESPECIFICO ESTOU DEIXANDO NO GRUPOPRODUTO.JS

function add_anti_forgery_token(data) {
    data.__RequestVerificationToken = $('[name=__RequestVerificationToken]').val();
    return data;
}

function formatar_mensagem_aviso(mensagens) {
    var ret = '';
    for (var i = 0; i < mensagens.length; i++) {
        //vou criar uma lista não ordenada
        ret += '<li>' + mensagens[i] + '</li>';
    }
    return '<ul>' + ret + '</ul>';
}


function abrir_form(dados) {
    //construi uma função e estou usando ele no GrupoProduto.js
    set_dados_form(dados);
 
    //vamos criar uma variavel, vamos usar muitas vezes
    var modal_cadastro = $('#modal_cadastro');
    $('#msg_mensagem_aviso').empty();
    $('#msg_aviso').hide();
    $('#msg_mensagem_aviso').hide();
    $('#msg_erro').hide();

    bootbox.dialog({
        title: 'Cadastro de ' + tituloPagina,
        message: modal_cadastro
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

//Criando Linhas
function criar_linha_grid(dados) {
    var ret =
        '<tr data-id=' + dados.Id + '>' +
        set_dados_grid(dados) +
        '<td>' +
        '<a class="btn btn-primary btn-alterar" role="button" style="margin-right: 3px"><i class="glyphicon glyphicon-pencil"></i> Editar</a>' +
        '<a class="btn btn-danger btn-excluir" role="button"><i class="glyphicon glyphicon-trash"></i> Excluir</a>' +
        '</td>' +
        '</tr>';
    return ret;
}

$(document).on('click', '#btn_incluir', function () {
    abrir_form(get_dados_inclusao());

})
    //aqui vammos fazer um ajax
    .on('click', '.btn-alterar', function () {

        //tenho que passar três paramentros => url, paramentro, e o que ele vai excultar seria function
        var btn = $(this),
            id = btn.closest('tr').attr('data-id'),//estou pegando o Id pelo tr ..dou closest e consigo buscar o mais proximo.

            url = url_alterar,
            param = { 'id': id }

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                //estou retornando os dados pela função de Post
                abrir_form(response)
            }
        })
            //Quando houver um erro
            .fail(function () {
                swal('Aviso', 'Não foi possivel recuperar as informações ! Tente novamente em instantes.', 'warning');
            });
    })




    //Exibindo um POPAP para excluir
    .on('click', '.btn-excluir', function () {

        var btn = $(this),
            tr = btn.closest('tr'),
            id = tr.attr('data-id'),//estou pegando o Id pelo tr ..dou closest e consigo buscar o mais proximo.

            url = url_excluir,
            param = { 'id': id }

        bootbox.confirm({
            message: "Deseja excluir o " + tituloPagina + "?",
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
                        if (response) {
                            tr.remove();//desta forma foi excluida a linha no back ende
                            var quantidade = $('#grid_cadastro > tbody > tr').length; //desta forma eu consigo ver a quantidade de linha que existe no gride (length)
                            if (quantidade == 0) {//Se o tamanho ou quantidade == 0. Quero dizer não tem mais linha. 
                                $('#grid_cadastro').addClass('invisivel');//ai eu adiciono a class invisivel "Quem vem com a mensagem NENHUM REGISTRO"
                                $('#mensagem_grid').removeClass('invisivel');//Se ainda tiver linha no gride eu removo a classe e mostro os item as linhas normalmente
                            }
                        }
                    })
                
            //Quando houver um erro
            .fail(function () {
                swal('Aviso', 'Não foi possivel excluir ! Tente novamente em instantes.', 'warning');
            });
                }
            }
        });
    })

    //Salvar
    .on('click', '#btn_confirmar', function () {
        var btn = $(this),
            url = url_confirmar,

            param = get_dados_form();
        //Estou fazendo o incluir funcionar
        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response.Resultado == "Ok") {
                if (param.Id == 0) {
                    param.Id = response.IdSalvo;
                    var table = $('#grid_cadastro').find('tbody'),
                        //veja que o param tem tudo que eu preciso Id,Nome, Ativo
                        //Eu passo ele para criar a linha.
                        linha = criar_linha_grid(param);
                    table.append(linha);
                    $('#grid_cadastro').removeClass('invisivel'); //esta removendo a classe invisivel 
                    $('#mensagem_grid').addClass('invisivel'); //esta adicionando a classe invisivel
                    //Editar || alterar
                } else {
                    var linha = $('#grid_cadastro').find('tr[data-id=' + param.Id + ']').find('td');
                    preencher_linha_grid(param, linha);
                }
                $('#modal_cadastro').parents('.bootbox').modal('hide');
            }
            //o que eu vou mostrar quando acontecer um erro
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
        })
            //Quando houver um erro
            .fail(function () {
                swal('Aviso', 'Não foi possivel salvar as informações ! Tente novamente em instantes.', 'warning');
            });
    })

    //Este page-item é a paginação
    .on('click', '.page-item', function () {
        var btn = $(this),
            filtro = $('#txt_filtro'),
            tamPag = $('#ddl_tam_pag').val(),
            pagina = btn.text(),
            url = url_page_click,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val() };

        //post é o meu envio o que vou passa ao back end => GrupoProdutoPagina
        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');
                //vou limpar a tabela
                table.empty();
                if (response.length > 0) {
                    $('#grid_cadastro').removeClass('invisivel'); //esta removendo a classe invisivel 
                    $('#mensagem_grid').addClass('invisivel'); //esta adicionando a classe invisivel
                    //no for eu já populo a minha lista => montando a nova paginação
                    for (var i = 0; i < response.length; i++) {
                        table.append(criar_linha_grid(response[i]));
                    }
                }
                else {
                    $('#grid_cadastro').addClass('invisivel'); //esta removendo a classe invisivel 
                    $('#mensagem_grid').removeClass('invisivel'); //esta adicionando a classe invisivel
                }
                //para remover e adicionar o item selecionado
                btn.siblings().removeClass('active');
                btn.addClass('active');
            }
        })
            //Quando houver um erro
            .fail(function () {
                swal('Aviso', 'Não foi possivel recuperar as informações ! Tente novamente em instantes.', 'warning');
            });

    })

    // Este ddl_tam_pag é o seletor que mudamos de pagina
    //QUANDO CLIKAR NO SELECT MUDAR A PAGINA
    .on('change', '#ddl_tam_pag', function () {
        var ddl = $(this),
            filtro = $('#txt_filtro'),
            tamPag = ddl.val(),
            pagina = 1,
            url = url_tam_pag_change,
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val() };


        //post é o meu envio o que vou passa ao back end => GrupoProdutoPagina
        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');
                //vou limpar a tabela
                table.empty();
                
                if (response.length > 0) {
                    $('#grid_cadastro').removeClass('invisivel'); //esta removendo a classe invisivel 
                    $('#mensagem_grid').addClass('invisivel'); //esta adicionando a classe invisivel
                    //no for eu já populo a minha lista => montando a nova paginação
                    for (var i = 0; i < response.length; i++) {
                        table.append(criar_linha_grid(response[i]));
                    }
                }
                else {
                    $('#grid_cadastro').addClass('invisivel'); //esta removendo a classe invisivel 
                    $('#mensagem_grid').removeClass('invisivel'); //esta adicionando a classe invisivel
                }
                //para remover e adicionar o item selecionado
                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        })
            //Quando houver um erro
            .fail(function () {
                swal('Aviso', 'Não foi possivel recuperar as informações ! Tente novamente em instantes.', 'warning');
            });

    })

//VAMOS MONTAR A REQUISIÇÃO ATRAVES DE JSON => REQUISIÇÃO DO FRONT END PARA O BACK END
 
    .on('keyup', '#txt_filtro', function () { //keyup => pressionar um tecla
        var filtro = $(this),
            ddl = $('#ddl_tam_pag'),//Estou buscando o nosso droopdown
            tamPag = ddl.val(),
            pagina = 1,
            url = url_filtro_change, //lá no meu CadGrupoProduto arquivo Index. cshtml esta a minha url no final da pagina... E a minha url, vai até a controller
            param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val() }; //eu vou enviar estes parametros para o meu back-end


        //post é o meu envio o que vou passa ao back end => GrupoProdutoPagina
        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');
                //vou limpar a tabela
                table.empty();

                if (response.length > 0) {
                    $('#grid_cadastro').removeClass('invisivel'); //esta removendo a classe invisivel 
                    $('#mensagem_grid').addClass('invisivel'); //esta adicionando a classe invisivel
                    //no for eu já populo a minha lista => montando a nova paginação
                    for (var i = 0; i < response.length; i++) {
                        table.append(criar_linha_grid(response[i]));
                    }
                }
                else {
                    $('#grid_cadastro').addClass('invisivel'); //esta removendo a classe invisivel 
                    $('#mensagem_grid').removeClass('invisivel'); //esta adicionando a classe invisivel
                }
               
                //para remover e adicionar o item selecionado
                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        })
            //Quando houver um erro
            .fail(function () {
                swal('Aviso', 'Não foi possivel recuperar as informações ! Tente novamente em instantes.', 'warning');
            });

    });




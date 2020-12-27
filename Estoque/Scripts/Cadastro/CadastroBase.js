
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
                            tr.remove();
                        }
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
        });
    })

    .on('click', '.page-item', function () {
        var btn = $(this),
            tamPag = $('#ddl_tam_pag').val(),
            pagina = btn.text(),
            url = url_page_click,
            param = { 'pagina': pagina, 'tamPag': tamPag };

        //post é o meu envio o que vou passa ao back end => GrupoProdutoPagina
        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');
                //vou limpar a tabela
                table.empty();
                //no for eu já populo a minha lista => montando a nova paginação
                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));
                }
                //para remover e adicionar o item selecionado
                btn.siblings().removeClass('active');
                btn.addClass('active');
            }
        });

    })

    //QUANDO CLIKAR NO SELECT MUDAR A PAGINA
    .on('change', '#ddl_tam_pag', function () {
        var ddl = $(this),
            tamPag = ddl.val(),
            pagina = 1,
            url = url_tam_pag_change,
            param = { 'pagina': pagina, 'tamPag': tamPag };


        //post é o meu envio o que vou passa ao back end => GrupoProdutoPagina
        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response) {
                var table = $('#grid_cadastro').find('tbody');
                //vou limpar a tabela
                table.empty();
                //no for eu já populo a minha lista => montando a nova paginação
                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));
                }
                //para remover e adicionar o item selecionado
                ddl.siblings().removeClass('active');
                ddl.addClass('active');
            }
        });

    });



function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#ddl_pais').val(dados.IdPais);
    $('#cbx_ativo').prop('checked', dados.Ativo);

    $('#ddl_estado').val(dados.IdEstado);
    $('#ddl_estado').prop('disabled', dados.IdEstado <= 0 || dados.IdEstado == undefined);
}

function set_focus_form() {
    $('#txt_nome').focus();
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        IdPais: 0,
        IdEstado: 0,
        Ativo: true
    };
}

function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        IdPais: $('#ddl_pais').val(),
        IdEstado: $('#ddl_estado').val(),
        Ativo: $('#cbx_ativo').prop('checked')
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Ativo ? 'SIM' : 'NÃO');
}

$(document).on('change', '#ddl_pais', function () {
    var ddl_pais = $(this),
        id_pais = parseInt(ddl_pais.val()),
        ddl_estado = $('#ddl_estado');

    if (id_pais > 0) {
        var url = url_listar_estados,
            param = { idPais: id_pais };

        ddl_estado.empty();
        ddl_estado.prop('disabled', true);

        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response && response.length > 0) {
                for (var i = 0; i < response.length; i++) {
                    ddl_estado.append('<option value=' + response[i].Id + '>' + response[i].Nome + '</option>');
                }
                ddl_estado.prop('disabled', false);
            }
        });
    }
});








////FUNÇÃO USADA PARA INCLUIR
//function set_dados_form(dados) {
//    $('#id_cadastro').val(dados.Id);
//    $('#txt_nome').val(dados.Nome);
//    $('#ddl_pais').val(dados.IdPais);
//    $('#ddl_estado').val(dados.IdEstado);
//    $('#cbx_ativo').prop('checked', dados.Ativo);

//    //$('#ddl_estado').val(dados.IdEstado);
//    //vamos habilitar e desabilitar, programando. 
//    //1º Condição => Eu vou desabilitar se meu estado for menor (igual a zero => NÃO TEM NADA SELECIONADO) OU || dados.IdEstado for igual == a indefinido
//    //Nestes casos eu  desabilito.....
//    $('#ddl_estado').prop('disabled', dados.IdEstado <= 0 || dados.IdEstado == undefined);
//}

//function set_focus_form() {
//    $('#txt_nome').focus();
//}

//function set_dados_grid(dados) {
// return '<td>' + dados.Nome + '</td>' +
//        '<td>' + (dados.Ativo ? 'SIM' : 'NÃO') + '</td>';
//}

//function get_dados_inclusao() {
//    return {
//        Id: 0,
//        Nome: '',
//        IdPais: 0,
//        IdEstado: 0,
//        Ativo: true
//    };
//}

//function get_dados_form() {
//    return {
//        Id: $('#id_cadastro').val(),
//        Nome: $('#txt_nome').val(),
//        IdPais: $('#ddl_pais').val(),
//        IdEstado: $('#ddl_estado').val(),
//        Ativo: $('#cbx_ativo').prop('checked')
//    };
//}

//function preencher_linha_grid(param, linha) {
//    linha
//        .eq(0).html(param.Nome).end()
//        .eq(1).html(param.Ativo ? 'SIM' : 'NÃO');
//}

////METODO change USADO PARA ALTERAR
////nome do evento (change) o nome do elemento (id #ddl_pais) logo a minha function
//$(document).on('change', '#ddl_pais', function () {
//    var ddl_pais = $(this), //esta vairavel será o item que foi modificado
//        id_pais = parseInt(ddl_pais.val()),//acrescentando o val() eu obtenho o valor que foi SELECIONADO na dropdown
//        ddl_estado = $('#ddl_estado'); //eu pego só o elemento o (id => #ddl_estado)

//    if (id_pais > 0) { //vamos ver se o pais está maior do que zero ou. No caso ele foi SELECIONADO
//        var url = url_listar_estados, //estamos criando uma url => url_listar_estados
//            param = { idPais: id_pais }; //parametro que será enviado..

//        ddl_estado.empty();//iportante limpar  a dropdown list=> se não toda a vez selecionar ele acaba  aumentando
//        //true será desabilitado
//        ddl_estado.prop('disabled', true);

//        //no meu post que vai enviar eu componho de url=>url_listar_estados,  param = { idPais: id_pais }; e minha função de (resposta)
//        $.post(url, add_anti_forgery_token(param), function (response) {
//            if (response && response.length > 0) { //ele vai retornar um vetor um arra lá no back-end
//                for (var i = 0; i < response.length; i++) {
//                    ddl_estado.append('<option value=' + response[i].Id + '>' + response[i].Nome + '</option>');
//                }
//                //não será desabilitado usamos o false == MOSTRA PARA O USUARIO
//                ddl_estado.prop('disabled', false);
//            }
//        });
//    }
//});
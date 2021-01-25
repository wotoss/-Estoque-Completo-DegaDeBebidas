//fiz uma variavel global referente a estado e cidade
var id_estado = 0,
    id_cidade = 0;

//Esta informações vem da Entidade (dados.InformaçãoDaEntidade)
function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#txt_num_documento').val(dados.NumDocumento);
    $('#txt_razao_social').val(dados.RazaoSocial);
    $('#txt_telefone').val(dados.Telefone);
    $('#txt_contato').val(dados.Contato);
    $('#txt_logradouro').val(dados.Logradouro);
    $('#txt_complemento').val(dados.Complemento);
    $('#txt_cep').val(dados.Cep);   
    $('#cbx_ativo').prop('checked', dados.Ativo);
    $('#cbx_pessoa_juridica').prop('checked', false);
    $('#cbx_pessoa_fisica').prop('checked', false);


    if (dados.Tipo == 2) {
        //O sistema será aberto com pessoa Juridica (Fornecedor), já clikado.
        //O evento trigger simula este click
        $('#cbx_pessoa_juridica').prop('checked', true).trigger('click');
    }
    else {
        $('#cbx_pessoa_fisica').prop('checked', true).trigger('click');
    }

    var inclusao = (dados.Id == 0);
    if (inclusao) {
        $('#ddl_estado').empty();
        $('#ddl_estado').prop('disable', true);

        $('#ddl_cidade').empty();
        $('#ddl_cidade').prop('disable', true);
    } else {
        $('#ddl_pais').val(dados.IdPais);
        mudar_pais(dados.IdEstado, dados.IdCidade);
    }

}

function mudar_pais(id_estado, id_cidade) {
    var ddl_pais = $('#ddl_pais'), //esta vairavel será o item que foi modificado
        id_pais = parseInt(ddl_pais.val()),//acrescentando o val() eu obtenho o valor que foi SELECIONADO na dropdown
        ddl_estado = $('#ddl_estado'); //eu pego só o elemento o (id => #ddl_estado)
        ddl_cidade = $('#ddl_cidade');
    
    if (id_pais > 0) { //vamos ver se o pais está maior do que zero ou. No caso ele foi SELECIONADO
        var url = url_listar_estados, //estamos criando uma url => url_listar_estados
            param = { idPais: id_pais }; //parametro que será enviado..

        ddl_estado.empty();//iportante limpar  a dropdown list=> se não toda a vez selecionar ele acaba  aumentando
        //true será desabilitado
        ddl_estado.prop('disabled', true);

        ddl_cidade.empty();
        ddl_cidade.prop('disabled', true);

        //no meu post que vai enviar eu componho de url=>url_listar_estados,  param = { idPais: id_pais }; e minha função de (resposta)
        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response && response.length > 0) { //ele vai retornar um vetor um arra lá no back-end
                for (var i = 0; i < response.length; i++) {
                    ddl_estado.append('<option value=' + response[i].Id + '>' + response[i].Nome + '</option>');
                }
                //não será desabilitado usamos o false == MOSTRA PARA O USUARIO
                ddl_estado.prop('disabled', false);
            }
            //estou criando um método sel_estado
            sel_estado(id_estado);
            mudar_estado(id_cidade);
        });
    }
}

function mudar_estado(id_cidade) {
    var ddl_estado = $('#ddl_estado'), //esta vairavel será o item que foi modificado
        id_estado = parseInt(ddl_estado.val()),//acrescentando o val() eu obtenho o valor que foi SELECIONADO na dropdown
        ddl_cidade = $('#ddl_cidade'); //eu pego só o elemento o (id => #ddl_estado)

    if (id_estado > 0) { //vamos ver se o pais está maior do que zero ou. No caso ele foi SELECIONADO
        var url = url_listar_cidades, //estamos criando uma url => url_listar_estados
            param = { idEstado: id_estado }; //parametro que será enviado..

        ddl_cidade.empty();//iportante limpar  a dropdown list=> se não toda a vez selecionar ele acaba  aumentando
        //true será desabilitado
        ddl_cidade.prop('disabled', true);

        //no meu post que vai enviar eu componho de url=>url_listar_estados,  param = { idPais: id_pais }; e minha função de (resposta)
        $.post(url, add_anti_forgery_token(param), function (response) {
            if (response && response.length > 0) { //ele vai retornar um vetor um arra lá no back-end
                for (var i = 0; i < response.length; i++) {
                    ddl_cidade.append('<option value=' + response[i].Id + '>' + response[i].Nome + '</option>');
                }
                //não será desabilitado usamos o false == MOSTRA PARA O USUARIO
                ddl_cidade.prop('disabled', false);
            }

            sel_cidade(id_cidade);
        });
    }
}

function sel_estado(id_estado) {
    $('#ddl_estado').val(id_estado);
    //Se for menor ou igual a zero => quer dizer se não tiver estado => inciará desabilitado
    $('#ddl_estado').prop('disabled', $('#ddl_estado option').length == 0);
}

function sel_cidade(id_cidade) {
    $('#ddl_cidade').val(id_cidade);
    $('#ddl_cidade').prop('disabled', $('#ddl_cidade option').length == 0);
}


//tem o id=txt_nome está recebendo um cursor piscando neste input
function set_focus_form() {
    $('#txt_nome').focus();
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        NumDocumento: '',
        RazaoSocial: '',
        Tipo: 2,
        Telefone: '',
        Contato: '',
        Logradouro: '',
        Complemento: '',
        Cep: '',
        IdPais: '',
        IdEstado: 0,
        IdCidade: 0,
        Ativo: true
    };
}

//Esta função esta obtendo os dados do formulário para enviar ao back-end
function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        NumDocumento: $('#txt_num_documento').val(),
        RazaoSocial: $('#txt_razao_social').val(),
        Tipo: $('#cbx_pessoa_juridica').is(':checked') ? 2 : 1, //este é a mesma coisa do ativo que eu pego (prop). Vou pegar o valor enviado pelo usuario 2=> Juridica ou 1=> Fisica
        Telefone: $('#txt_telefone').val(),
        Contato: $('#txt_contato').val(),
        Logradouro: $('#txt_logradouro').val(),
        Complemento: $('#txt_complemento').val(),
        Cep: $('#txt_cep').val(),
        IdPais: $('#ddl_pais').val(),
        IdEstado: $('#ddl_estado').val(),
        IdCidade: $('#ddl_cidade').val(),
        Ativo: $('#cbx_ativo').prop('checked')
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Telefone).end()
        .eq(1).html(param.Contato).end()
        .eq(2).html(param.Ativo ? 'SIM' : 'NÃO');
}

//Vou usar o Jquery lembrando on. é o mesmo do onclik
$(document)
    //Quando entrar na pagina o evento reader vai carregar os as formatações dos inputs,
    .ready(function () {
        $('#txt_telefone').mask('(00) 0000-0000');
        $('#txt_cep').mask('00000-000');
    })
    //basicamente estou dizendo que quando eu der um click em referencia este id, vai acontecer algo
    .on('click', '#cbx_pessoa_juridica', function () {
    //neste momento eu vou selecionar o label
        $('label[for="txt_num_documento"]').text('CNPJ');
        $('#txt_num_documento').mask('00.000.000/0000-00', { reverse: true });
        //Quando eu cliko em pessoaJuridica eu removo a classe invisible e ai eu mostro o input
        $('#container_razao_social').removeClass('invisible'); 
    })
    .on('click', '#cbx_pessoa_fisica', function () {
     //neste momento eu vou selecionar o label
        $('label[for="txt_num_documento"]').text('CPF');
        $('#txt_num_documento').mask('000.000.000-00', { reverse: true });
        //Quando eu cliko em pessoaFisica eu adiciono a classe invisible e ai eu oculto o input PessoaJuridica e mostro o CPF
        $('#container_razao_social').addClass('invisible');
    })

//QUANDO EU MODIFICO O PAÍS ELE VAI SER MODIFICADO
//METODO change USADO PARA ALTERAR
//nome do evento (change) o nome do elemento (id #ddl_pais) logo a minha function
$(document).on('change', '#ddl_pais', function () {
    mudar_pais();
})

//QUANDO EU MODIFICO O PAÍS ELE VAI SER MODIFICADO
//METODO change USADO PARA ALTERAR
//nome do evento (change) o nome do elemento (id #ddl_estado) logo a minha function
$(document).on('change', '#ddl_estado', function () {
    mudar_estado();
});
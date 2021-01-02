
//Este preenche o nossa tabela ou modelo => Trás as informações da base de dados
function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#txt_login').val(dados.Login);
    $('#txt_senha').val(dados.Senha);
    
}

//faz o focu no campo nome
function set_focus_form() {
    $('#txt_nome').focus();
}

function set_dados_grid(dados) {
    return  '<td>' + dados.Nome + '</td>' +
            '<td>' + dados.Login + '</td>';

}

//faz durante a inclusão
function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        Login: '',
        Senha: ''
       
    };
}

//veja que a function tem tudo que eu preciso Id,Nome, Ativo
//para recuperar os valores 
function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Login: $('#txt_login').val(),
        Senha: $('#txt_senha').val()
       
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Login);
}




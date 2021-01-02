
//RESUMO ANOTADO DO FORMULARIO O QUE CADA FUNÇÃO FAZ

//monta os ids do cadastros, passando valores
function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#cbx_ativo').prop('checked', dados.Ativo);

    //vou setar os checkedbox de perfil como false
    var lista_usuario = $('#lista_usuario');
    lista_usuario.find('input[type=checkbox]').prop('checked', false);

    //quando ele vir desta lista do back - end com lista Usuarios
    if (dados.Usuarios) {
        for (var i = 0; i < dados.Usuarios.length; i++) {
            var usuario = dados.Usuarios[i];
            //aqui eu vou obter o checkbox
            var cbx = lista_usuario.find('input[data-id-usuario=' + usuario.Id + ']');
            if (cbx) {
                cbx.prop('checked', true)
            }
        }
    }
}

//seta o focu no campo nome
function set_focus_form() {
    $('#txt_nome').focus();
}

//quando inserir informações no grid, inserir somente o nome e o ativo...
function set_dados_grid(dados) {
    return '<td>' + dados.Nome + '</td>' +
        '<td>' + (dados.Ativo ? 'SIM' : 'NÃO') + '</td>';

}

//nome ativo 
function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        Ativo: true
    };
}

//veja que a function tem tudo que eu preciso Id,Nome, Ativo
//obtendo os dados do form
function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Ativo: $('#cbx_ativo').prop('checked')
    };
}

//preenchendo as linhas do gride....
function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Ativo ? 'SIM' : 'NÃO');
}




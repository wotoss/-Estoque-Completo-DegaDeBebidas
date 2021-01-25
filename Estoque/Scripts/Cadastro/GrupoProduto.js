
//function set_dados_form(dados) {
//$('#id_cadastro').val(dados.Id);
//$('#txt_nome').val(dados.Nome);
//$('#cbx_ativo').prop('checked', dados.Ativo);
//}

//function set_focus_form() {
//  $('#txt_nome').focus();
//}

//function set_dados_grid(dados) {
//    return '<td>' + dados.Nome + '</td>' +
//           '<td>' + (dados.Ativo ? 'SIM' : 'NÃO') + '</td>';
   
//}

//function get_dados_inclusao() {
//    return {
//        Id: 0,
//        Nome: '',
//        Ativo: true
//    };
//}

////veja que a function tem tudo que eu preciso Id,Nome, Ativo
//function get_dados_form() {
//    return {
//        Id: $('#id_cadastro').val(),
//        Nome: $('#txt_nome').val(),
//        Ativo: $('#cbx_ativo').prop('checked')
//    };
//}

//function preencher_linha_grid(param, linha) {
//    linha
//        .eq(0).html(param.Nome).end()
//        .eq(1).html(param.Ativo ? 'SIM' : 'NÃO');
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

function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#cbx_ativo').prop('checked', dados.Ativo);
}

function set_focus_form() {
    $('#txt_nome').focus();
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        Ativo: true
    };
}

function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Ativo: $('#cbx_ativo').prop('checked')
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Ativo ? 'SIM' : 'NÃO');
}

    





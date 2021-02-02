//este arquivo comum.js está sendo usada em todas as paginas...Pois é comum a todas...

function add_anti_forgery_token(data) {
    data.__RequestVerificationToken = $('[name=__RequestVerificationToken]').val();
    return data;
}

//todo o input do tipo number (só receberá numero )
//através do eventoo (e.preventDefault();)
//sendo assim onde for input[type=number] não digitará letra só numero.
$(document)
    .on('keydown', 'input[type=number]', function (e) {
        if (e.key === 'e') {
            e.preventDefault();
        }
    });
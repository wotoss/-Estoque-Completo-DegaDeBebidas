
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Estoque.Models.Binders
{
    public class EntradaSaidaProdutoViewModelModelBinder : DefaultModelBinder
    {

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            if (bindingContext == null)
            {
                //Se o meu bindingContext ele vai apresentar esta excessão ArgumentNullException
                throw new ArgumentNullException(nameof(bindingContext));
            }

            //ele esta indo na quesição que vem do js para buscar a informação do form
            var valores = controllerContext.HttpContext.Request.Form;

            //Estou criando o objeto binder vazio
            var ret = new EntradaSaidaProdutoViewModel() { Produtos = new Dictionary<int, int>() };

            try
            {
                //formato da data que esta vindo do meu js via post
                ret.Data = DateTime.ParseExact(valores.Get("data"), "yyyy-MM-dd", null);

                if (!string.IsNullOrEmpty(valores.Get("produtos")))
                {
                    var produtos = JsonConvert.DeserializeObject<List<dynamic>>(valores.Get("produtos"));

                    foreach (var produto in produtos)
                    {
                        ret.Produtos.Add((int)produto.IdProduto, (int)produto.Quantidade);
                    }
                };
            }
            catch
            {
            }

            return ret;
        }

        //VAMOS FAZER O SALVAR
        public static string SalvarPedidoEntrada(DateTime data, Dictionary<int, int> produtos)
        {
            //Se não der certo eu retorno zero
            var ret = "";
            try 
            { 
            using (var conexao = new SqlConnection())
            {
                //Criar a minha conxeção
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();
                //agora vou dar o comando
                var numPedido = "";
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    //Aqui estamos executando quele sequence que colocamos na base de dados..Aqui esto onome do sequencie(sec_entrada_produto)
                    comando.CommandText = "select next value for sec_entrada_produto";
                    // (D10) significa quero em Decimal com dez digitos
                    numPedido = ((int)comando.ExecuteScalar()).ToString("D10");
                }

                using (var transacao = conexao.BeginTransaction()) {
                    foreach (var produto in produtos)
                    {
                        using (var comando = new SqlCommand())
                        {
                            comando.Connection = conexao;
                            comando.Transaction = transacao;
                            comando.CommandText = "insert into entrada_produto (numero, data, id_produto, quant) values (@numero, @data, @id_produto, @quant)";

                            comando.Parameters.Add("@numero", SqlDbType.VarChar).Value = numPedido;
                            comando.Parameters.Add("@data", SqlDbType.Date).Value = data;
                            comando.Parameters.Add("@id_produto", SqlDbType.Int).Value = produto.Key;
                            comando.Parameters.Add("@quant", SqlDbType.Int).Value = produto.Value;

                            comando.ExecuteNonQuery();

                        }

                        using (var comando = new SqlCommand())
                        {
                            comando.Connection = conexao;
                            comando.Transaction = transacao;
                            comando.CommandText = "update produto set quant_estoque = quant_estoque + @quant_estoque where(id = @id)";

                            comando.Parameters.Add("@id", SqlDbType.Int).Value = produto.Key;
                            comando.Parameters.Add("@quant_estoque", SqlDbType.Int).Value = produto.Value;

                            comando.ExecuteNonQuery();

                        }
                    }
                    transacao.Commit();

                    ret = numPedido;

                }
            }
        }
        catch (Exception ex)
            {
            }
            return ret;
            
        }
    }
}

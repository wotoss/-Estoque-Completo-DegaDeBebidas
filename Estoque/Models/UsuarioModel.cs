using Estoque.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Estoque.Models
{
    /// <summary>
    /// Estamos usando o Framework AdoNet provider SQLServer
    /// </summary>
    public class UsuarioModel
    {
        public static bool ValidarUsuario(string login, string senha)
        {
          var ret = false;

           using (var conexao = new SqlConnection())
            {
                //Criar a minha conxeção
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();
                //agora vou dar o comando
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    //Aqui eu vou definir o meu comando o que vou execultar no banco de dados. Vou fazer a minha query
                    //montando query sql => se o usuario digitado se encontra no banco de dados
                    comando.CommandText = "select count(*) from usuario where login=@login and senha=@senha";
                    //vamos saber se existi algum {0} login com a senha ou {1} com  senha que foi digitado

                    comando.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
                    comando.Parameters.Add("@Senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senha);

                    //retornar o usuario para minha variavel do inicio da tela
                    ret = ((int)comando.ExecuteScalar() > 0);
                   }
                }
                //agor faço o retorno com o usuario encontrado. 
                return ret;

            }
        }
    }

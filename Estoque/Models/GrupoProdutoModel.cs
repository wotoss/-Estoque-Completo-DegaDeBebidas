﻿

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace Estoque.Models
{
    public class GrupoProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o nome !")]
        public string Nome { get; set; }

       //se o usuario informar ou não ele já entra setado como false.
        public bool Ativo { get; set; }

        //COMEÇANDO O ACESSO AO BANCO DE DADOS
        public static List<GrupoProdutoModel> RecuperarLista()
        {
            var ret = new List<GrupoProdutoModel>();

            using (var conexao = new SqlConnection())
            {
                //Criar a minha conxeção//principal
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();
                //agora vou dar o comando
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    //Aqui eu vou definir o meu comando o que vou execultar no banco de dados. Vou fazer a minha query
                    //montando query sql => se o usuario digitado se encontra no banco de dados
                    //Esta fazendo uma consulta e (trazendo ou recuperando) todos as informações do banco de dados oredenado por (nome)
                    comando.CommandText = " select * from grupo_produto order by nome ";
                    var reader = comando.ExecuteReader(); //este (reader) esta recebendo a execução
                    //while => enquanto tiver algo a ser lido
                    //para cada item que for lido eu estou incluido um objeto GrupoProdutoModel
                    while (reader.Read())
                    {
                        //RESUMINDO TUDO QUE EU PEGAR DO BANCO DE DADOS ATRAVES DA QUERY ELA VAI POPULAR E ME RETORNAR =>ret
                        ret.Add(new GrupoProdutoModel
                        {
                            //estou fazendo uma conversão de tipo => reader retorna o objeto e eu tenho que pegar o tipo especific n setagem
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }

                }
            }
            //agor faço o retorno com o usuario encontrado. 
            return ret;
            }


        // AGORA VAMOS RECUPERAR UM UNICO ITEM DA BASE DE DADOS
        public static GrupoProdutoModel RecuperarPeloId(int id)
        {
            //Se não conseguir achar o Id ele retorna nullo
            GrupoProdutoModel ret = null;

            using (var conexao = new SqlConnection())
            {
                //Criar a minha conxeção
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString; 
                conexao.Open();
                //agora vou dar o comando
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    //Query vai ter o Id com parametro qualquer{0} para fazer a recebendo id passado
                    comando.CommandText = "select * from grupo_produto where (id = @id)";
                    //COLOCANDO PARAMETERS PARA EVITAR SQL INJECT
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader(); //este (reader) esta recebendo a execução
                    //while => enquanto tiver algo a ser lido
                    //para cada item que for lido eu estou incluido um objeto GrupoProdutoModel
                     if (reader.Read())
                    {
                        //ATRAVES D QUERY ESTA RETORNANDO UM REGISTRO DA BASE DE DADOS
                        ret = new GrupoProdutoModel
                        {
                            //estou fazendo uma conversão de tipo => reader retorna o objeto e eu tenho que pegar o tipo especific n setagem
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
                        };
                    }

                }
            }
        
            //agor faço o retorno com o usuario encontrado. 
            return ret;
               }


        //RECUPERANDO E EXCLUINDO RETONO BOLL VERDADEIRO OU FALSO
        public static bool ExcluirPeloId(int id)
        {
            //Eu inicio o meu retorno como falso
            var ret = false;
            //Se ele recuperou do banco de dados o Id ai ele faz o processo para excluir
            if (RecuperarPeloId(id) != null)
            {
            using (var conexao = new SqlConnection())
            {
                //Criar a minha conxeção
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                    conexao.Open();
                //agora vou dar o comando
                using (var comando = new SqlCommand())
                {

                    comando.Connection = conexao;
                    //Query vai ter o Id com parametro qualquer{0} para fazer a recebendo id passado
                    comando.CommandText = "delete from grupo_produto where (id = @id)";
                    //COLOCANDO PARAMETRES PARA EVITAR SQL INJECT
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    //Este NonQuery ele retorna um registro afetado ou deletado. Por isto para dizer se o comando execultou eu coloco (maior que zero)
                    ret = comando.ExecuteNonQuery() > 0; 
                }
            }

        }
            //agor faço o retorno com o usuario encontrado. 
            return ret;
        }

        //VAMOS FAZER O SALVAR
        public int Salvar()
        {
            //Se não der certo eu retorno zero
            var ret = 0;

            //SE NÃO RECUPEROU DO BANCO DE DADOS EU INCLUI || SALVAR          
            var model = RecuperarPeloId(this.Id);
           
                using (var conexao = new SqlConnection())
                {
                    //Criar a minha conxeção
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();
                    //agora vou dar o comando
                    using (var comando = new SqlCommand())
                    {

                        comando.Connection = conexao;
                        if(model == null)
                        {
                        //Query vai ter o Id com parametro qualquer{0} para fazer a recebendo id passado
                        comando.CommandText = "insert into grupo_produto (nome, ativo) values (@nome, @ativo); select convert(int, scope_identity())";
                        //como eu colquei no banco bit true || false então. Então para inserir eu faço a conversão passando 1 = true && 0 = false
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                        
                                                                                                                                                                                                            //select convert(int, scoped_identity()) =>  esta função que estou usando na query me retorna o ultimo valor que foi inserido no banco
                            //LEMBRANDO QUE O SCALAR ME RETONA UM OBJETO.LOGO EU CONVERTO PARA INTEIRO.
                            ret = (int)comando.ExecuteScalar();
                        }

                        //SE RECUPEROU O ID DO BANCO DE DADOS EU VOU EDITAR || ALTERAR
                        else
                        {
                        //COLOQUE PARAMETERS E USEI @NOME NAS QUERY PARA EVITAR SQL INJECT
                        comando.CommandText = "update grupo_produto set nome=@nome, ativo=@ativo where id =@id";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = this.Ativo;
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
                        //como eu colquei no banco bit true || false então. Então para inserir eu faço a conversão passando 1 = true && 0 = false

                        //LEMBRANDO QUE ExecuteNonQuery => ELE EXECULTA E RETORNA UM INTEIRO.."por isto eu faço a validação (maior 0) Ele esta fazendo a atualização"
                        if (comando.ExecuteNonQuery() > 0)
                        {
                            //Peço para retornar o Id
                            ret = this.Id;
                        }
                    }
                   }
                }
            //agor faço o retorno com o usuario encontrado. 
            return ret;
        }
    }
 }

        

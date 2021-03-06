﻿using Dapper;
using Estoque.Models.Domain;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Estoque.Models
{
    public class EstadoModel
    {
        #region Atributos

        public int Id { get; set; }
        public string Nome { get; set; }
        public string UF { get; set; }
        public bool Ativo { get; set; }
        public int IdPais { get; set; }
        public virtual PaisModel Pais { get; set; }

        #endregion

        #region Métodos

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var db = new ContextoBD())
            {
                ret = db.Estados.Count();
            }

            return ret;
        }

        public static List<EstadoModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", int idPais = 0)
        {
            var ret = new List<EstadoModel>();

            using (var db = new ContextoBD())
            {
                var filtroWhere = "";
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
                }

                if (idPais > 0)
                {
                    filtroWhere +=
                        (string.IsNullOrEmpty(filtroWhere) ? " where" : " and") +
                        string.Format(" id_pais = {0}", idPais);
                }

                var pos = (pagina - 1) * tamPagina;
                var paginacao = "";
                if (pagina > 0 && tamPagina > 0)
                {
                    paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                        pos, tamPagina);
                }

                var sql =
                    "select id, nome, uf, id_pais as IdPais, ativo" +
                    " from estado" +
                    filtroWhere +
                    " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                    paginacao;

                ret = db.Database.Connection.Query<EstadoModel>(sql).ToList();
            }

            return ret;
        }

        public static EstadoModel RecuperarPeloId(int id)
        {
            EstadoModel ret = null;

            using (var db = new ContextoBD())
            {
                ret = db.Estados.Find(id);
            }

            return ret;
        }

        public static bool ExcluirPeloId(int id)
        {
            var ret = false;

            if (RecuperarPeloId(id) != null)
            {
                using (var db = new ContextoBD())
                {
                    var estado = new EstadoModel { Id = id };
                    db.Estados.Attach(estado);
                    db.Entry(estado).State = EntityState.Deleted;
                    db.SaveChanges();
                    ret = true;
                }
            }

            return ret;
        }

        public int Salvar()
        {
            var ret = 0;

            var model = RecuperarPeloId(this.Id);

            using (var db = new ContextoBD())
            {
                if (model == null)
                {
                    db.Estados.Add(this);
                }
                else
                {
                    db.Estados.Attach(this);
                    db.Entry(this).State = EntityState.Modified;
                }

                db.SaveChanges();
                ret = this.Id;
            }

            return ret;
        }

        #endregion
    }
}


//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Web;

//namespace Estoque.Models
//{
//    public class EstadoModel
//    {
//        public int Id { get; set; }

//        [Required(ErrorMessage = "Informe o nome")]
//        [MaxLength(30, ErrorMessage = "O nome pode ter no máximo 30 caracteres")]
//        public string Nome { get; set; }

//        [Required(ErrorMessage = "Preencha o código internacional.")]
//        [MaxLength(3, ErrorMessage = "O código internacional deve ter 3 caracteres.")]
//        public string UF { get; set; }

//        public bool Ativo { get; set; }

//        [Required(ErrorMessage = "Selecione o país")]
//        public int IdPais { get; set; }

//        //fazendo associação
//        public virtual PaisModel Pais { get; set; }



//        public static int RecuperarQuantidade()
//        {
//            var ret = 0;

//            using (var conexao = new SqlConnection())
//            {
//                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
//                conexao.Open();
//                using (var comando = new SqlCommand())
//                {
//                    comando.Connection = conexao;
//                    comando.CommandText = "select count(*) from estado";
//                    ret = (int)comando.ExecuteScalar();
//                }
//            }
//            return ret;
//        }
//        //Recuperar Lista 
//        public static List<EstadoModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", int idPais = 0)
//        {
//            var ret = new List<EstadoModel>();

//            using (var conexao = new SqlConnection())
//            {
//                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
//                conexao.Open();

//                using (var comando = new SqlCommand())
//                {
//                    var pos = (pagina - 1) * tamPagina;

//                    var filtroWhere = "";
//                    if (!string.IsNullOrEmpty(filtro))
//                    {
//                        filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
//                    }

//                    if (idPais > 0)
//                    {   //VEJA que validação interressante pegamos o filtroWhere que busca o id_pais e dizemos que (string.IsNullOrEmpty(filtroWhere)
//                        //Se vier vazio eu vou de (where) se não vou de (and) são dois trabalho em um só..
//                        filtroWhere += (string.IsNullOrEmpty(filtroWhere) ? " where" : " and") + 
//                            string.Format(" id_pais = {0}", idPais);
//                    }

//                    var paginacao = "";
//                    if (pagina > 0 && tamPagina > 0)
//                    {
//                        paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
//                            pos > 0 ? pos - 1 : 0, tamPagina);


//                        comando.Connection = conexao;
//                        comando.CommandText =
//                            "select *" +
//                            " from pais" +
//                            filtroWhere +
//                            " order by nome" + paginacao;

//                        var reader = comando.ExecuteReader();
//                        while (reader.Read())
//                        {
//                            ret.Add(new EstadoModel
//                            {
//                                Id = (int)reader["id"],
//                                Nome = (string)reader["nome"],
//                                UF = (string)reader["uf"],
//                                Ativo = (bool)reader["ativo"]
//                            });
//                        }

//                    }
//                }
//                return ret;
//            }
//        }
//            //Recuperar por Id
//            public static EstadoModel RecuperarPeloId(int id)
//            {
//                EstadoModel ret = null;

//                using (var conexao = new SqlConnection())
//                {
//                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
//                    conexao.Open();

//                    using (var comando = new SqlCommand())
//                    {
//                        comando.Connection = conexao;
//                        comando.CommandText = "select * from estado where (id = @id)";

//                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

//                        var reader = comando.ExecuteReader();

//                        if (reader.Read())
//                        {
//                            //ATRAVES D QUERY ESTA RETORNANDO UM REGISTRO DA BASE DE DADOS
//                            ret = new EstadoModel
//                            {
//                                //estou fazendo uma conversão de tipo => reader retorna o objeto e eu tenho que pegar o tipo especific n setagem
//                                Id = (int)reader["id"],
//                                Nome = (string)reader["nome"],
//                                UF = (string)reader["uf"],
//                                Ativo = (bool)reader["ativo"]
//                            };
//                        }

//                    }

//                }
//                return ret;
//            }

//        //RECUPERANDO E EXCLUINDO RETONO BOLL VERDADEIRO OU FALSO
//        public static bool ExcluirPeloId(int id)
//        {
//            //Eu inicio o meu retorno como falso
//            var ret = false;
//            //Se ele recuperou do banco de dados o Id ai ele faz o processo para excluir
//            if (RecuperarPeloId(id) != null)
//            {
//                using (var conexao = new SqlConnection())
//                {
//                    //Criar a minha conxeção
//                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
//                    conexao.Open();
//                    //agora vou dar o comando
//                    using (var comando = new SqlCommand())
//                    {

//                        comando.Connection = conexao;
//                        //Query vai ter o Id com parametro qualquer{0} para fazer a recebendo id passado
//                        comando.CommandText = "delete from estado where (id = @id)";
//                        //COLOCANDO PARAMETRES PARA EVITAR SQL INJECT
//                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

//                        //Este NonQuery ele retorna um registro afetado ou deletado. Por isto para dizer se o comando execultou eu coloco (maior que zero)
//                        ret = comando.ExecuteNonQuery() > 0;
//                    }
//                }

//            }
//            //agor faço o retorno com o usuario encontrado. 
//            return ret;
//        }

//        //VAMOS FAZER O SALVAR
//        public int Salvar()
//        {
//            //Se não der certo eu retorno zero
//            var ret = 0;

//            //SE NÃO RECUPEROU DO BANCO DE DADOS EU INCLUI || SALVAR          
//            var model = RecuperarPeloId(this.Id);

//            using (var conexao = new SqlConnection())
//            {
//                //Criar a minha conxeção
//                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
//                conexao.Open();
//                //agora vou dar o comando
//                using (var comando = new SqlCommand())
//                {

//                    comando.Connection = conexao;
//                    if (model == null)
//                    {
//                        //Query vai ter o Id com parametro qualquer{0} para fazer a recebendo id passado
//                        comando.CommandText = "insert into estado (nome, ativo) values (@nome, @ativo); select convert(int, scope_identity())";
//                        //como eu colquei no banco bit true || false então. Então para inserir eu faço a conversão passando 1 = true && 0 = false
//                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
//                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

//                        //select convert(int, scoped_identity()) =>  esta função que estou usando na query me retorna o ultimo valor que foi inserido no banco
//                        //LEMBRANDO QUE O SCALAR ME RETONA UM OBJETO.LOGO EU CONVERTO PARA INTEIRO.
//                        ret = (int)comando.ExecuteScalar();
//                    }

//                    //SE RECUPEROU O ID DO BANCO DE DADOS EU VOU EDITAR || ALTERAR
//                    else
//                    {
//                        //COLOQUE PARAMETERS E USEI @NOME NAS QUERY PARA EVITAR SQL INJECT
//                        comando.CommandText = "update estado set nome=@nome, ativo=@ativo where id =@id";

//                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
//                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = this.Ativo;
//                        comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
//                        //como eu colquei no banco bit true || false então. Então para inserir eu faço a conversão passando 1 = true && 0 = false

//                        //LEMBRANDO QUE ExecuteNonQuery => ELE EXECULTA E RETORNA UM INTEIRO.."por isto eu faço a validação (maior 0) Ele esta fazendo a atualização"
//                        if (comando.ExecuteNonQuery() > 0)
//                        {
//                            //Peço para retornar o Id
//                            ret = this.Id;
//                        }
//                    }
//                }
//            }
//            //agor faço o retorno com o usuario encontrado. 
//            return ret;
//        }
//    }
//    }

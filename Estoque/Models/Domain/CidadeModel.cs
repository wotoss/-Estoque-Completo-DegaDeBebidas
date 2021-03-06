﻿
using Dapper;
using Estoque.Models.Domain;
using System.Collections.Generic;

using System.Data;
using System.Data.Entity;
using System.Linq;

namespace Estoque.Models
{
    public class CidadeModel
    {
        #region Atributos

        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public int IdEstado { get; set; }
        public virtual EstadoModel Estado { get; set; }

        #endregion

        #region Métodos

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var db = new ContextoBD())
            {
                ret = db.Cidades.Count();
            }

            return ret;
        }

        public static List<CidadeViewModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", int idEstado = 0)
        {
            var ret = new List<CidadeViewModel>();

            using (var db = new ContextoBD())
            {
                var pos = (pagina - 1) * tamPagina;

                var filtroWhere = "";
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtroWhere = string.Format(" (lower(c.nome) like '%{0}%') and", filtro.ToLower());
                }

                if (idEstado > 0)
                {
                    filtroWhere += string.Format(" (id_estado = {0}) and", idEstado);
                }

                var paginacao = "";
                if (pagina > 0 && tamPagina > 0)
                {
                    paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                        pos, tamPagina);
                }

                var sql =
                    "select c.id, c.nome, c.ativo, c.id_estado as IdEstado, e.id_pais as IdPais," +
                    " e.nome as NomeEstado, p.nome as NomePais" +
                    " from cidade c, estado e, pais p" +
                    " where" +
                    filtroWhere +
                    " (c.id_estado = e.id) and" +
                    " (e.id_pais = p.id)" +
                    " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "c.nome") +
                    paginacao;

                ret = db.Database.Connection.Query<CidadeViewModel>(sql).ToList();
            }

            return ret;
        }

        public static CidadeViewModel RecuperarPeloId(int id)
        {
            CidadeViewModel ret = null;

            using (var db = new ContextoBD())
            {
                ret = db.Cidades
                    .Include(x => x.Estado)
                    .Where(x => x.Id == id)
                    .Select(x => new CidadeViewModel
                    {
                        Id = x.Id,
                        Nome = x.Nome,
                        Ativo = x.Ativo,
                        IdEstado = x.IdEstado,
                        IdPais = x.Estado.IdPais
                    })
                    .SingleOrDefault();
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
                    var cidade = new CidadeModel { Id = id };
                    db.Cidades.Attach(cidade);
                    db.Entry(cidade).State = EntityState.Deleted;
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
                    db.Cidades.Add(this);
                }
                else
                {
                    db.Cidades.Attach(this);
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



//ESTE ESTÃO FUNCIONANDO OS COMENTARIOS QUE ESTÃO SÃO IMPORTANTES

//public static int RecuperarQuantidade()
//        {
//            var ret = 0;

//            using (var conexao = new SqlConnection())
//            {
//                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
//                conexao.Open();
//                using (var comando = new SqlCommand())
//                {
//                    comando.Connection = conexao;
//                    comando.CommandText = "select count(*) from cidade";
//                    ret = (int)comando.ExecuteScalar();
//                }
//            }
//            return ret;
//        }

//        //Recuperar Lista 
//        public static List<CidadeModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", int idEstado = 0)
//        {
//            var ret = new List<CidadeModel>();

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
//                        filtroWhere = string.Format(" (lower(c.nome) like '%{0}%') and", filtro.ToLower());
//                    }

//                    //Se este idEstado form maior do que zero significa que vou incluir ele no filtro...
//                    if (idEstado > 0)
//                    {
//                        filtroWhere += string.Format(" (id_estado = {0}) and", idEstado);

//                    }

//                    var paginacao = "";
//                    if (pagina > 0 && tamPagina > 0)
//                    {
//                        paginacao = string.Format(" offset {0} rows fetch next {1} rows only ",
//                            pos > 0 ? pos - 1 : 0, tamPagina);

//                        comando.Connection = conexao;
//                        comando.CommandText =
//                            "select c.*, e.id_pais" +
//                            " from cidade c, estado e" +
//                            " where" +
//                            filtroWhere +
//                            " order by c.nome" + paginacao;

//                        var reader = comando.ExecuteReader();
//                        while (reader.Read())
//                        {
//                            ret.Add(new CidadeModel
//                            {
//                                Id = (int)reader["id"],
//                                Nome = (string)reader["nome"],
//                                IdEstado = (int)reader["id_estado"],

//                                Ativo = (bool)reader["ativo"]
//                            });
//                        }

//                    }
//                }
//                return ret;
//            }
//        }
//            //Recuperar por Id
//            public static CidadeModel RecuperarPeloId(int id)
//            {
//                CidadeModel ret = null;

//                using (var conexao = new SqlConnection())
//                {
//                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
//                    conexao.Open();

//                    using (var comando = new SqlCommand())
//                    {
//                        comando.Connection = conexao;
//                        comando.CommandText = "select * from cidade where (id = @id)";

//                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

//                        var reader = comando.ExecuteReader();

//                        if (reader.Read())
//                        {
//                            //ATRAVES D QUERY ESTA RETORNANDO UM REGISTRO DA BASE DE DADOS
//                            ret = new CidadeModel
//                            {
//                                //estou fazendo uma conversão de tipo => reader retorna o objeto e eu tenho que pegar o tipo especific n setagem
//                                Id = (int)reader["id"],
//                                Nome = (string)reader["nome"],
//                                IdEstado = (int)reader["id_estado"],

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
//                        comando.CommandText = "delete from cidade where (id = @id)";
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
//                        comando.CommandText = "insert into cidade (nome, ativo) values (@nome, @ativo); select convert(int, scope_identity())";
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
//                        comando.CommandText = "update cidade set nome=@nome, ativo=@ativo where id =@id";

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
//}

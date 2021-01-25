using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;


namespace Estoque.Models
{
    public class PaisModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome")]
        [MaxLength(30, ErrorMessage = "O nome pode ter no máximo 30 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Preencha o código internacional.")]
        [MaxLength(3, ErrorMessage = "O código internacional deve ter 3 caracteres.")]
        public string Codigo { get; set; }

        public bool Ativo { get; set; }

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();
                ret = conexao.ExecuteScalar<int>("select count(*) from pais");
            }
            return ret;
        }

        //Recuperar Lista 
        public static List<PaisModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "")
        {
            var ret = new List<PaisModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();


                var filtroWhere = "";
                if (!string.IsNullOrEmpty(filtro))
                {
                    filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
                }
                var pos = (pagina - 1) * tamPagina;
                var paginacao = "";
                if (pagina > 0 && tamPagina > 0)
                {
                    paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                        pos > 0 ? pos - 1 : 0, tamPagina);
                }
                var sql =
                     "select *" +
                     " from pais" +
                     filtroWhere +
                     " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                     paginacao;
                ret = conexao.Query<PaisModel>(sql).ToList();
            }
            return ret;
        }
        //Recuperar por Id
        public static PaisModel RecuperarPeloId(int id)
        {
            PaisModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();

                var sql = "select * from pais where (id = @id)";
                //esta variavel parametro é a mesma que estou passando a cima e tambem no sql
                var parametros = new { id };
                ret = conexao.Query<PaisModel>(sql, parametros).SingleOrDefault();

            }
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

                    var sql = "delete from pais where (id = @id)";
                    //crio um (objeto anonimo) e passo o parametro do (sql) ou do meu proprio metodo sendo id
                    var parametros = new { id };
                    ret = (conexao.Execute(sql, parametros) > 0);
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

                if (model == null)
                {
                    var sql = "insert into pais (nome, codigo, ativo) values (@nome, @codigo, @ativo); select convert(int, scope_identity())";

                    var parametros = new { nome = this.Nome, codigo = this.Codigo, ativo = (this.Ativo ? 1 : 0) };
                    ret = conexao.ExecuteScalar<int>(sql, parametros);
                }

                //SE RECUPEROU O ID DO BANCO DE DADOS EU VOU EDITAR || ALTERAR
                else
                {
                    //COLOQUE PARAMETERS E USEI @NOME NAS QUERY PARA EVITAR SQL INJECT
                    var sql = "update pais set nome=@nome, codigo=@codigo, ativo=@ativo where id =@id";

                    var parametros = new { id = this.Id, nome = this.Nome, codigo = this.Codigo, ativo = (this.Ativo ? 1 : 0) };

                    //LEMBRANDO QUE ExecuteNonQuery => ELE EXECULTA E RETORNA UM INTEIRO.."por isto eu faço a validação (maior 0) Ele esta fazendo a atualização"
                    if (conexao.Execute(sql, parametros) > 0)
                    {
                        //Peço para retornar o Id
                        ret = this.Id;
                    }
                }
            }
            return ret;
        }
    }
}

        
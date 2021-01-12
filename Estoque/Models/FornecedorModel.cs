using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace Estoque.Models
{
    public class FornecedorModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o nome")]
        [MaxLength(60, ErrorMessage = "O nome pode ter no máximo 60 caracteres.")]
        public string Nome { get; set; }

        [MaxLength(100, ErrorMessage = "A razão social pode ter no máximo 100 caracteres.")]
        public string RazaoSocial { get; set; }

        [MaxLength(20, ErrorMessage =  "O número do documento pode ter no máximo 20 caracteres.")]
        public string NumDocumento { get; set; }

        [Required]
        public TipoPessoa Tipo { get; set; }

        [Required(ErrorMessage = "Digite o telefone.")]
        [MaxLength(20, ErrorMessage = "O telefone  possui 20 caracteres.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Digite o contato")]
        [MaxLength(60, ErrorMessage = "O telefone deve ter 60 caracteres.")]
        public string Contato { get; set; }

        [MaxLength(100, ErrorMessage = "O logradouro do endereço pode ter no máximo 100 caracteres.")]
        public string Logradouro { get; set; }

        [MaxLength(20, ErrorMessage ="O número do endereço pode ter no máximo 20 caracteres.")]
        public string Numero { get; set; }

        [MaxLength(100, ErrorMessage = "O complemento  do endereço pode ter no máximo 100 caracteres.")]
        public string Complemento { get; set; }

        [MaxLength(10, ErrorMessage = "O Cep do endereço pode ter no máximo 10 caracteres.")]
        public string Cep { get; set; }

        //relacionamento Pais, Estado

        [Required(ErrorMessage = "Selecione o país")]
        public int IdPais { get; set; }

        [Required(ErrorMessage = "Selecione o estado")]
        public int IdEstado { get; set; }

        [Required(ErrorMessage ="Selecione a cidade")]
        public int IdCidade { get; set; }

        public bool Ativo { get; set; }


        //MÉTODOS
        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select count(*) from fornecedor";
                    ret = (int)comando.ExecuteScalar();
                }
            }
            return ret;
        }

        //Recuperar Lista 
        public static List<FornecedorModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "")
        {
            var ret = new List<FornecedorModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;

                    var filtroWhere = "";
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
                    }

                    
                    var paginacao = "";
                    if (pagina > 0 && tamPagina > 0)
                    {
                        paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                            pos > 0 ? pos - 1 : 0, tamPagina);

                    }
                        comando.Connection = conexao;
                        comando.CommandText =
                            "select *" +
                            " from fornecedor" +
                            filtroWhere +
                            " order by nome" + paginacao;

                        var reader = comando.ExecuteReader();
                        while (reader.Read())
                        {
                            ret.Add(new FornecedorModel
                            {
                                Id = (int)reader["id"],
                                Nome = (string)reader["nome"],
                                RazaoSocial = (string)reader["razao_social"],
                                NumDocumento = (string)reader["num_documento"],
                                Tipo = (TipoPessoa)((int)reader["tipo"]),
                                Telefone = (string)reader["telefone"],
                                Contato = (string)reader["contato"],
                                Logradouro = (string)reader["logradouro"],
                                Numero = (string)reader["numero"],
                                Complemento = (string)reader["complemento"],
                                Cep = (string)reader["cep"],
                                IdPais = (int)reader["id_pais"],
                                IdEstado = (int)reader["id_estado"],
                                IdCidade = (int)reader["id_cidade"],
                                Ativo = (bool)reader["ativo"]
                            });
                        }

                    }
                }
                return ret;
            }

        //Recuperar por Id
        public static FornecedorModel RecuperarPeloId(int id)
        {
            FornecedorModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from estado where (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        //ATRAVES D QUERY ESTA RETORNANDO UM REGISTRO DA BASE DE DADOS
                        ret = new FornecedorModel
                        {
                            //estou fazendo uma conversão de tipo => reader retorna o objeto e eu tenho que pegar o tipo especific n setagem
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            RazaoSocial = (string)reader["razao_social"],
                            NumDocumento = (string)reader["num_documento"],
                            Tipo = (TipoPessoa)((int)reader["tipo"]),
                            Telefone = (string)reader["telefone"],
                            Contato = (string)reader["contato"],
                            Logradouro = (string)reader["logradouro"],
                            Numero = (string)reader["numero"],
                            Complemento = (string)reader["complemento"],
                            Cep = (string)reader["cep"],
                            IdPais = (int)reader["id_pais"],
                            IdEstado = (int)reader["id_estado"],
                            IdCidade = (int)reader["id_cidade"],
                            Ativo = (bool)reader["ativo"]
                        };
                    }

                }

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
                    using (var comando = new SqlCommand())
                    {

                        comando.Connection = conexao;
                        //Query vai ter o Id com parametro qualquer{0} para fazer a recebendo id passado
                        comando.CommandText = "delete from fornecedor where (id = @id)";
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
                    if (model == null)
                    {
                        //Query vai ter o Id com parametro qualquer{0} para fazer a recebendo id passado
                        comando.CommandText = "insert into fornecedor (nome, razao_social, num_documento, tipo, telefone, contato, logradouro, numero" +
                            " complemento, cep, id_pais, id_estado, id_cidade, ativo) values (@nome, @razao_social, @num_documento, @tipo, @telefone," +
                            " @contato, @logradouro, @numero, @complemento, @cep, @id_pais, @id_estado, @id_cidade, @ativo; select convert(int, scope_identity())";
                        //como eu colquei no banco bit true || false então. Então para inserir eu faço a conversão passando 1 = true && 0 = false
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@razao_social", SqlDbType.VarChar).Value = this.RazaoSocial ?? "";
                        comando.Parameters.Add("@num_documento", SqlDbType.VarChar).Value = this.NumDocumento ?? "";
                        comando.Parameters.Add("@tipo", SqlDbType.Int).Value = this.Tipo;
                        comando.Parameters.Add("@telefone", SqlDbType.VarChar).Value = this.Telefone ?? "";
                        comando.Parameters.Add("@contato", SqlDbType.VarChar).Value = this.Contato ?? "";
                        comando.Parameters.Add("@logradouro", SqlDbType.VarChar).Value = this.Logradouro ?? "";
                        comando.Parameters.Add("@numero", SqlDbType.VarChar).Value = this.Numero ?? "";
                        comando.Parameters.Add("@complemento", SqlDbType.VarChar).Value = this.Complemento ?? "";
                        comando.Parameters.Add("@cep", SqlDbType.VarChar).Value = this.Cep ?? "";
                        comando.Parameters.Add("@id_pais", SqlDbType.Int).Value = this.IdPais;
                        comando.Parameters.Add("@id_estado", SqlDbType.Int).Value = this.IdEstado;
                        comando.Parameters.Add("@id_cidade", SqlDbType.Int).Value = this.IdCidade;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = this.Ativo;

                        //select convert(int, scoped_identity()) =>  esta função que estou usando na query me retorna o ultimo valor que foi inserido no banco
                        //LEMBRANDO QUE O SCALAR ME RETONA UM OBJETO.LOGO EU CONVERTO PARA INTEIRO.
                        ret = (int)comando.ExecuteScalar();
                    }

                    //SE RECUPEROU O ID DO BANCO DE DADOS EU VOU EDITAR || ALTERAR
                    else
                    {
                        //COLOQUE PARAMETERS E USEI @NOME NAS QUERY PARA EVITAR SQL INJECT
                        comando.CommandText = "update fornecedor set nome=@nome, razao_social=@razao_social, num_documento=@num_documento, tipo=@tipo, telefone=@telefone, contato=@contato, logradouro=@logradouro, numero=@numero" +
                            " complemento=@complemento, cep=@cep, id_pais=@id_pais, id_estado=@id_estado, id_cidade=@id_cidade, ativo=@ativo where id =@id";


                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@razao_social", SqlDbType.VarChar).Value = this.RazaoSocial ?? "";
                        comando.Parameters.Add("@num_documento", SqlDbType.VarChar).Value = this.NumDocumento ?? "";
                        comando.Parameters.Add("@tipo", SqlDbType.Int).Value = this.Tipo;
                        comando.Parameters.Add("@telefone", SqlDbType.VarChar).Value = this.Telefone ?? "";
                        comando.Parameters.Add("@contato", SqlDbType.VarChar).Value = this.Contato ?? "";
                        comando.Parameters.Add("@logradouro", SqlDbType.VarChar).Value = this.Logradouro ?? "";
                        comando.Parameters.Add("@numero", SqlDbType.VarChar).Value = this.Numero ?? "";
                        comando.Parameters.Add("@complemento", SqlDbType.VarChar).Value = this.Complemento ?? "";
                        comando.Parameters.Add("@cep", SqlDbType.VarChar).Value = this.Cep ?? "";
                        comando.Parameters.Add("@id_pais", SqlDbType.Int).Value = this.IdPais;
                        comando.Parameters.Add("@id_estado", SqlDbType.Int).Value = this.IdEstado;
                        comando.Parameters.Add("@id_cidade", SqlDbType.Int).Value = this.IdCidade;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = this.Ativo ? 1 : 0;
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

  

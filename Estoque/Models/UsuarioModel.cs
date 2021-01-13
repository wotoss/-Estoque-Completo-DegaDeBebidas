using Estoque.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace Estoque.Models
{
    /// <summary>
    /// Estamos usando o Framework AdoNet provider SQLServer
    /// Pagina de Cadastro de Usuarios
    /// </summary>
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Digite a senha")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Digite o nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Digite o e-mail")]
        public string Email { get; set; }


        public static UsuarioModel ValidarUsuario(string login, string senha)
        {
            UsuarioModel ret = null;

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
                    comando.CommandText = "select * from usuario where login=@login and senha=@senha";
                    //vamos saber se existi algum {0} login com a senha ou {1} com  senha que foi digitado

                    comando.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
                    comando.Parameters.Add("@Senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senha);

                    //retornar o usuario para minha variavel do inicio da tela
                    var reader = comando.ExecuteReader();
                    //se ele encontrou algum usuario com login e senha
                    if (reader.Read())
                    {
                        //ele encontrando no banco ele vai criar um usuario
                        ret = new UsuarioModel
                        {
                            //Agora eu tenho o meu usuário preenchido...
                            Id = (int)reader["id"],
                            Login = (string)reader["login"],
                            Senha = (string)reader["senha"],
                            Nome = (string)reader["nome"],
                            Email =(string)reader["email"],

                        };

                    }
                }
            }
            //agor faço o retorno com o usuario encontrado. 
            return ret;

        }


        //CRIAR UM MÉTODO APENAS PARA OBTER A QUANTIDADE DE REGISTO QUE TEMOS NA BASE DE DADOS
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
                    comando.CommandText = "select count(*) from usuario";
                    ret = (int)comando.ExecuteScalar();
                }
            }
            return ret;
        }



        //COMEÇANDO O ACESSO AO BANCO DE DADOS
        public static List<UsuarioModel> RecuperarLista(int pagina = -1, int tamPagina = -1)
        {
            var ret = new List<UsuarioModel>();

            using (var conexao = new SqlConnection())
            {
                //Criar a minha conxeção//principal
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();
                //agora vou dar o comando
                using (var comando = new SqlCommand())
                {
                    //estou criando a posição da pagina
                    var pos = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;

                    if (pagina == -1 || tamPagina == -1)
                    {
                        comando.CommandText = "select * from usuario order by nome";

                    }
                    else
                    {
                        //Aqui eu vou definir o meu comando o que vou execultar no banco de dados. Vou fazer a minha query
                        //montando query sql => se o usuario digitado se encontra no banco de dados
                        //Esta fazendo uma consulta e (trazendo ou recuperando) todos as informações do banco de dados oredenado por (nome)
                        comando.CommandText = string.Format(
                            "select * from usuario order by nome offset {0} rows fetch next {1} rows only",
                           pos > 0 ? pos - 1 : 0, tamPagina);


                    }


                    var reader = comando.ExecuteReader(); //este (reader) esta recebendo a execução
                    //while => enquanto tiver algo a ser lido
                    //para cada item que for lido eu estou incluido um objeto UsuarioModel
                    while (reader.Read())
                    {
                        //RESUMINDO TUDO QUE EU PEGAR DO BANCO DE DADOS ATRAVES DA QUERY ELA VAI POPULAR E ME RETORNAR =>ret
                        ret.Add(new UsuarioModel
                        {
                            //estou fazendo uma conversão de tipo => reader retorna o objeto e eu tenho que pegar o tipo especific n setagem
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"],
                            Email = (string)reader["email"],

                            //TODO Senha 
                        });
                    }

                }
            }
            //agor faço o retorno com o usuario encontrado. 
            return ret;
        }


        // AGORA VAMOS RECUPERAR UM UNICO ITEM DA BASE DE DADOS
        public static UsuarioModel RecuperarPeloId(int id)
        {
            //Se não conseguir achar o Id ele retorna nullo
            UsuarioModel ret = null;

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
                    comando.CommandText = "select * from usuario where (id = @id)";
                    //COLOCANDO PARAMETERS PARA EVITAR SQL INJECT
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader(); //este (reader) esta recebendo a execução
                                                          //while => enquanto tiver algo a ser lido
                                                          //para cada item que for lido eu estou incluido um objeto UsuarioModel
                    if (reader.Read())
                    {
                        //ATRAVES D QUERY ESTA RETORNANDO UM REGISTRO DA BASE DE DADOS
                        ret = new UsuarioModel
                        {
                            //estou fazendo uma conversão de tipo => reader retorna o objeto e eu tenho que pegar o tipo especific n setagem
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"],
                            Email = (string)reader["email"],
                        };
                    }

                }
            }

            //agor faço o retorno com o usuario encontrado. 
            return ret;
        }


        // AGORA VAMOS RECUPERAR UM UNICO ITEM DA BASE DE DADOS LOGIN
        public static UsuarioModel RecuperarPeloLogin(string login)
        {
            //Se não conseguir achar o Id ele retorna null
            UsuarioModel ret = null;

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
                    comando.CommandText = "select * from usuario where (login = @login)";
                    //COLOCANDO PARAMETERS PARA EVITAR SQL INJECT
                    comando.Parameters.Add("@login", SqlDbType.VarChar).Value = login;

                    var reader = comando.ExecuteReader(); //este (reader) esta recebendo a execução
                                                          //while => enquanto tiver algo a ser lido
                                                          //para cada item que for lido eu estou incluido um objeto UsuarioModel
                    if (reader.Read())
                    {
                        //ATRAVES D QUERY ESTA RETORNANDO UM REGISTRO DA BASE DE DADOS
                        ret = new UsuarioModel
                        {
                            //estou fazendo uma conversão de tipo => reader retorna o objeto e eu tenho que pegar o tipo especific n setagem
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"],
                            Email = (string)reader["email"],
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
                        comando.CommandText = "delete from usuario where (id = @id)";
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
                        comando.CommandText = "insert into usuario (nome, email, login, senha) values (@nome, @email, @login, @senha); select convert(int, scope_identity())";
                        //como eu colquei no banco bit true || false então. Então para inserir eu faço a conversão passando 1 = true && 0 = false
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@email", SqlDbType.VarChar).Value = this.Email;
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = this.Login;
                        comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(this.Senha); //já gravar na base de dados a senha criptografada


                        //select convert(int, scoped_identity()) =>  esta função que estou usando na query me retorna o ultimo valor que foi inserido no banco
                        //LEMBRANDO QUE O SCALAR ME RETONA UM OBJETO.LOGO EU CONVERTO PARA INTEIRO.
                        ret = (int)comando.ExecuteScalar();
                    }

                    //SE RECUPEROU O ID DO BANCO DE DADOS EU VOU EDITAR || ALTERAR
                    else
                    {
                        //COLOQUE PARAMETERS E USEI @NOME NAS QUERY PARA EVITAR SQL INJECT
                        comando.CommandText =
                            "update usuario set nome=@nome, email=@email, login=@login" +
                            //se a senha veio preenchida eu vou atribuir esta string (senha=@senha") : se não eu paço vazia
                            (!string.IsNullOrEmpty(this.Senha) ? ", senha=@senha" : "") +
                            " where id =@id";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@email", SqlDbType.VarChar).Value = this.Email;
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = this.Login;


                        if (!string.IsNullOrEmpty(this.Senha))
                        {
                            comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(this.Senha); //já gravar na base de dados a senha criptografada
                        }

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

        public string RecuperarStringNomePerfis()
        {

            var ret = string.Empty;

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
                    comando.CommandText = string.Format(
                        "select p.nome " +
                        "from perfil_usuario pu , perfil p " +
                        "where (pu.id_usuario = @id_usuario) and (pu.id_perfil = p.id) and (p.ativo = 1)");

                    comando.Parameters.Add("@id_usuario", SqlDbType.Int).Value = this.Id;

                    var reader = comando.ExecuteReader(); //este (reader) esta recebendo a execução
                    //while => enquanto tiver algo a ser lido
                    //para cada item que for lido eu estou incluido um objeto UsuarioModel
                    while (reader.Read())
                    {
                        //RESUMINDO TUDO QUE EU PEGAR DO BANCO DE DADOS ATRAVES DA QUERY ELA VAI POPULAR E ME RETORNAR =>ret                           
                        ret += (ret != string.Empty ? ";" : string.Empty) + (string)reader["nome"];
                        //TODO Senha 
                    }
                }

            }
            //agor faço o retorno com o usuario encontrado. 
            return ret;
        }

        public bool AlterarSenha(string novaSenha)
        {
            var ret = false;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "update usuario set senha = @senha where id = @id";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
                    //Lembrando que estou critografando a senha com HasMD5
                    comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(novaSenha);

                    ret = (comando.ExecuteNonQuery() > 0);
                }
            }
            return ret;
        }


        //Vou pegar este método lá na minha ContaContoller => AlterarSenhaUsuario
        public bool ValidarSenhaAtual(string senhaAtual)
        {
            var ret = false;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    //este select Count(*) serve para ver a quantidade de registro. Caso retorno seja maior que zero ou seja retorne algum
                    comando.CommandText = "select Count(*) from usuario where senha = @senhaAtual and id = @id";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
                    //Lembrando que estou critografando a senha com HasMD5
                    comando.Parameters.Add("@senhaAtual", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senhaAtual);

                    ret = (comando.ExecuteNonQuery() > 0);
                }
            }
            return ret;
        }
            
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace Estoque.Models
{
    public class ProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o código.")]
        [MaxLength(10, ErrorMessage = "O código pode ter no máximo 10 caracteres.")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Digite o nome.")]
        [MaxLength(50, ErrorMessage = "O nome pode ter no máximo 50 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Digite o preço de custo.")]
        public decimal PrecoCusto { get; set; }

        [Required(ErrorMessage = "Digite o preço de venda.")]
        public decimal PrecoVenda { get; set; }

        [Required(ErrorMessage = "Digite a quantidade em estoque.")]
        public int QuantEstoque { get; set; }

        [Required(ErrorMessage = "Selecione a unidade de medida.")]
        public int IdUnidadeMedida { get; set; }

        //fazendo associação
        public virtual UnidadeMedidaModel UnidadeMedida { get; set; }

        [Required(ErrorMessage = "Selecione o grupo.")]
        public int IdGrupo { get; set; }

        //fazendo associação
        public virtual GrupoProdutoModel Grupo { get; set; }

        [Required(ErrorMessage = "Selecione a marca.")]
        public int IdMarca { get; set; }
        //fazendo associação
        public virtual MarcaProdutoModel Marca { get; set; }

        [Required(ErrorMessage = "Selecione o fornecedor.")]
        public int IdFornecedor { get; set; }

        //fazendo associação
        public virtual FornecedorModel Fornecedor { get; set; }

        [Required(ErrorMessage = "Selecione o local de armazenamento.")]
        public int IdLocalArmazenamento { get; set; }
        //fazendo associação
        public virtual LocalArmazenamentoModel LocalArmazenamento { get; set; }

        public bool Ativo { get; set; }

        public string Imagem { get; set; }


        #region Métodos

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
                    comando.CommandText = "select count(*) from produto";
                    ret = (int)comando.ExecuteScalar();
                }
            }
            return ret;
        }

        //refatorando
        //está como private, porque foi criado a refatoração para usar apenas dentro desta classe
        private static ProdutoModel MontarProduto(SqlDataReader reader)
        {
            return new ProdutoModel
            {
                Id = (int)reader["id"],
                Codigo = (string)reader["codigo"],
                Nome = (string)reader["nome"],
                PrecoCusto = (decimal)reader["preco_custo"],
                PrecoVenda = (decimal)reader["preco_venda"],
                QuantEstoque = (int)reader["quant_estoque"],
                IdUnidadeMedida = (int)reader["id_unidade_medida"],
                IdGrupo = (int)reader["id_grupo"],
                IdMarca = (int)reader["id_marca"],
                IdFornecedor = (int)reader["id_fornecedor"],
                IdLocalArmazenamento = (int)reader["id_local_armazenamento"],
                Ativo = (bool)reader["ativo"],
                Imagem = (string)reader["imagem"],
            };
        }

        //Método criado MontarProdutoInventario
        private static ProdutoInventarioViewModel MontarProdutoInventario(SqlDataReader reader)
        {
            return new ProdutoInventarioViewModel
            {
                Id = (int)reader["id"],
                Codigo = (string)reader["codigo"],
                NomeProduto = (string)reader["nome_produto"],
                QuantEstoque = (int)reader["quant_estoque"],
                NomeUnidadeMedida = (string)reader["nome_unidade_medida"],
                NomeLocalArmazenamento = (string)reader["nome_local_armazenamento"]
            };
        }

        //Recuperar Lista para obter todos os produtos
        public static List<ProdutoModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", bool somenteAtivos = false)
        {
            var ret = new List<ProdutoModel>();

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
                        filtroWhere = string.Format(" where (lower(nome) like '%{0}%')", filtro.ToLower());
                    }

                    if (somenteAtivos)
                    {
                        filtroWhere = (string.IsNullOrEmpty(filtroWhere) ? " where " : " and") + "(ativo = 1)";
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
                        " from produto" +
                        filtroWhere +
                        " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") + paginacao;

                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(MontarProduto(reader));
                    }

                }
            }
            return ret;
        }
        //Recuperar por Id
        public static ProdutoModel RecuperarPeloId(int id)
        {
            ProdutoModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from produto where (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        ret = MontarProduto(reader);
                    }

                }

            }
            return ret;
        }

        /// <summary>
        /// Recuperar Imagem pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public static string RecuperarImagemPeloId(int id)
        {
            string ret = "";

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select imagem from produto where (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        ret = (string)reader["imagem"];
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
                        comando.CommandText = "delete from produto where (id = @id)";
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
                        comando.CommandText = "insert into produto " +
                            "(codigo, nome, preco_custo, preco_venda, quant_estoque, id_unidade_medida, id_grupo, id_marca, id_fornecedor, " +
                            "id_local_armazenamento, ativo, imagem) values " +
                            "(@codigo, @nome, @preco_custo, @preco_venda, @quant_estoque, @id_unidade_medida, @id_grupo, @id_marca, @id_fornecedor," +
                            "@id_local_armazenamento, @ativo, @imagem); select convert(int, scope_identity())";
                        //como eu colquei no banco bit true || false então. Então para inserir eu faço a conversão passando 1 = true && 0 = false
                        comando.Parameters.Add("@codigo", SqlDbType.VarChar).Value = this.Codigo;

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@preco_custo", SqlDbType.Decimal).Value = this.PrecoCusto;
                        comando.Parameters.Add("@preco_venda", SqlDbType.Decimal).Value = this.PrecoVenda;
                        comando.Parameters.Add("@quant_estoque", SqlDbType.Int).Value = this.QuantEstoque;
                        comando.Parameters.Add("@id_unidade_medida", SqlDbType.Int).Value = this.IdUnidadeMedida;
                        comando.Parameters.Add("@id_grupo", SqlDbType.Int).Value = this.IdGrupo;
                        comando.Parameters.Add("@id_marca", SqlDbType.Int).Value = this.IdMarca;
                        comando.Parameters.Add("@id_fornecedor", SqlDbType.Int).Value = this.IdFornecedor;
                        comando.Parameters.Add("@id_local_armazenamento", SqlDbType.Int).Value = this.IdLocalArmazenamento;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                        comando.Parameters.Add("@imagem", SqlDbType.VarChar).Value = (this.Imagem);

                        //select convert(int, scoped_identity()) =>  esta função que estou usando na query me retorna o ultimo valor que foi inserido no banco
                        //LEMBRANDO QUE O SCALAR ME RETONA UM OBJETO.LOGO EU CONVERTO PARA INTEIRO.
                        ret = (int)comando.ExecuteScalar();
                    }

                    //SE RECUPEROU O ID DO BANCO DE DADOS EU VOU EDITAR || ALTERAR
                    else
                    {
                        //COLOQUE PARAMETERS E USEI @NOME NAS QUERY PARA EVITAR SQL INJECT
                        comando.CommandText = "update produto set codigo=@codigo, nome=@nome, preco_custo=@preco_custo, " +
                            "preco_venda=@preco_venda, quant_estoque=@quant_estoque, id_unidade_medida=@id_unidade_medida, " +
                            "id_grupo=@id_grupo, id_marca=@id_marca, id_fornecedor=@id_fornecedor, " +
                            "id_local_armazenamento=@id_local_armazenamento, ativo=@ativo, imagem=@imagem where id =@id";

                        comando.Parameters.Add("@id", SqlDbType.VarChar).Value = this.Id;
                        comando.Parameters.Add("@codigo", SqlDbType.VarChar).Value = this.Codigo;
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@preco_custo", SqlDbType.Decimal).Value = this.PrecoCusto;
                        comando.Parameters.Add("@preco_venda", SqlDbType.Decimal).Value = this.PrecoVenda;
                        comando.Parameters.Add("@quant_estoque", SqlDbType.Int).Value = this.QuantEstoque;
                        comando.Parameters.Add("@id_unidade_medida", SqlDbType.Int).Value = this.IdUnidadeMedida;
                        comando.Parameters.Add("@id_grupo", SqlDbType.Int).Value = this.IdGrupo;
                        comando.Parameters.Add("@id_marca", SqlDbType.Int).Value = this.IdMarca;
                        comando.Parameters.Add("@id_fornecedor", SqlDbType.Int).Value = this.IdFornecedor;
                        comando.Parameters.Add("@id_local_armazenamento", SqlDbType.Int).Value = this.IdLocalArmazenamento;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                        comando.Parameters.Add("@imagem", SqlDbType.VarChar).Value = (this.Imagem);
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

        #endregion
        public static string SalvarPedidoEntrada(DateTime data, Dictionary<int, int> produtos)
        {
            //Eu coloco como (true) porque é entrada
            //Neste ele faz o incremento no estoque (inclui )
            return SalvarPedido(data, produtos, "entrada_produto", true);

        }

        //FAZENDO O PEDIDO DE SAÍDA
        public static string SalvarPedidoSaida(DateTime data, Dictionary<int, int> produtos)
        {
            //Eu coloco como (false) porque é saida
            //Neste método ele faz  decremento no estoque, (diminui)
            return SalvarPedido(data, produtos, "saida_produto", false);
        }

        //Vamos criar o método Salvar pedido 
        public static string SalvarPedido(DateTime data, Dictionary<int, int> produtos, string nomeTabela, bool entrada)
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
                        //neste moment estams execultando o sequencie(sec_entrada_produto) que foi feito no banco de dados
                        /////////////////////////////////////////////////////////////////
                        //VOU COLOCAR DE TRÊS FORMAS PARA EXEMPLO AO FAZER A QUERY
                        ////////////////////////////////////////////////////////////////
                        //1º DENTRO DA STRING EU CONSIGO USAR A VARIAVEL
                        comando.CommandText = $"select next value for sec_{nomeTabela}";
                        //2º NESTA EU ADICINA A VARIAVEL + nomeTabela;
                        //comando.CommandText = "select next value for sec_" + nomeTabela;

                        //3º FORMA DE FAZER EU ACRESCENTO O STRING.FORMAT E COLOCO O INDICE {0}, {1} => CONFORME  TANTO DE VARIAVEL QUE EU TENHO.
                        //comando.CommandText = string.Format("select next value for sec_{0}", nomeTabela);

                        //(D)=> decimal (10) seria dez digitos. E o formato é cm zeros frmatdos a esquerda

                        numPedido = ((int)comando.ExecuteScalar()).ToString("D10");
                    }

                    //neste momento com Begin nós fazemos a iniciação de uma transação
                    using (var transacao = conexao.BeginTransaction())
                    {
                        //RESUMO DESTA TRANSAÇÃO
                        //APARTIR DO MOMENTO QUE ENTRA PRDUTO (na tabela entrada_poduto)  EU  FAÇO O INSERT, SIMUTANEAMENTE ELE ATUALIZA A TABELA (produto o campo quant_estoque)
                        foreach (var produto in produtos)
                        {
                            using (var comando = new SqlCommand())
                            {
                                comando.Connection = conexao;
                                comando.Transaction = transacao;
                                comando.CommandText = $"insert into {nomeTabela} (numero, data, id_produto, quant) values (@numero, @data, @id_produto, @quant)";

                                comando.Parameters.Add("@numero", SqlDbType.VarChar).Value = numPedido;
                                comando.Parameters.Add("@data", SqlDbType.Date).Value = data;
                                comando.Parameters.Add("@id_produto", SqlDbType.Int).Value = produto.Key;
                                comando.Parameters.Add("@quant", SqlDbType.Int).Value = produto.Value;

                                comando.ExecuteNonQuery();
                            }

                            using (var comando = new SqlCommand())
                            {
                                //////////////////////////////////////////////////////
                                //VEJA ! que é interressante eu criei um método 
                                //com de sinal mais e menos ai eu passo a variavel 
                                //dentro da query E tambem declaro o (bool entrada )
                                //no inicio do método. Muito bom !
                                //////////////////////////////////////////////////////
                                var sinal = (entrada ? "+" : "-");

                                comando.Connection = conexao;
                                comando.Transaction = transacao;
                                //Perceba que ele encrementa (aumenta) (+) a quantidade de estoque
                                comando.CommandText = $"update produto set quant_estoque = quant_estoque {sinal} @quant_estoque where (id = @id)";

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

        //implementando retorno de lista de Inventário
        public static List<ProdutoInventarioViewModel> RecuperarListaParaInventario()
        {
            //este será o nosso retorno...
            var ret = new List<ProdutoInventarioViewModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    //VAMOS FAZER UM SELECT DE INFORMAÇÕES DE MAIS DE UMA TABELA...
                    comando.CommandText = "select " +
                        "p.id, p.codigo, p.nome as nome_produto, p.quant_estoque, " +
                        "l.nome as nome_local_armazenamento, u.nome as nome_unidade_medida  " +
                        "from produto p, local_armazenamento l, unidade_medida u " +
                        //aqui (where) estou fazendo um filtro para trazer somente os meu produtos ativos
                        "where (p.ativo = 1) and " +
                        //Neste momento estou fazendo os Join
                        "(p.id_local_armazenamento = l.id) and " +
                        "(p.id_unidade_medida = u.id) " +
                        "order by l.nome, p.nome";

                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(MontarProdutoInventario(reader));
                    }
                }
            }

            return ret;
        }
        
        //public static bool SalvarInventario(List<ItemInventarioViewModel> dados)
        //{
            
        //    var ret = true;

        //    try
        //    {
        //        var data = DateTime.Now;

        //        using (var conexao = new SqlConnection())
        //        {
        //            conexao.ConnectionString = ConfigurationManager.ConnectionStrings["real"].ConnectionString;
        //            conexao.Open();

        //            using (var transacao = conexao.BeginTransaction())
        //            {
        //                foreach (var produtoInventario in dados)
        //                {
        //                    using (var comando = new SqlCommand())
        //                    {
        //                        comando.Connection = conexao;
        //                        comando.Transaction = transacao;
        //                        comando.CommandText = "insert into inventario_estoque (data, id_produto, quant_estoque, quant_inventario, motivo)";

        //                        comando.Parameters.Add("@data", SqlDbType.DateTime).Value = data;
        //                        comando.Parameters.Add("@id_produto", SqlDbType.Int).Value = produtoInventario.IdProduto;
        //                        comando.Parameters.Add("@quant_estoque", SqlDbType.Int).Value = produtoInventario.QuantidadeEstoque;
        //                        comando.Parameters.Add("@quant_inventario", SqlDbType.Int).Value = produtoInventario.QuantidadeInventario;
        //                        comando.Parameters.Add("@motivo", SqlDbType.VarChar).Value = produtoInventario.Motivo;

        //                        comando.ExecuteNonQuery();
        //                    }
        //                }

        //                transacao.Commit();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ret = false;

        //    }
           
        //}
    }
}

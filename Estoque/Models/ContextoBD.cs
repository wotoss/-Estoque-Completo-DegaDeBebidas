

using Estoque.Models.Maps;
using System.Data.Entity;

namespace Estoque.Models.Domain
{
    public class ContextoBD : DbContext
    {
        public ContextoBD() : base("name=real")
        {

        }

    // É MUITO IMPORTANTE E TEMOS DOIS PASSO (OnModelCreating)
    //1º Este é muito importante é o momento que faz o mapeamento (OnModelCreating)
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        //2º E aqui nos incluimos o mapeamento
        //lEMBRANDO QUE LÁ NO CidadeMap EU TENHO A DEFINIÇÃO DA BASE DE DADOS
        modelBuilder.Configurations.Add(new CidadeMap());
        modelBuilder.Configurations.Add(new EntradaProdutoMap());
        modelBuilder.Configurations.Add(new EstadoMap());
        modelBuilder.Configurations.Add(new FornecedorMap());
        modelBuilder.Configurations.Add(new GrupoProdutoMap());
        modelBuilder.Configurations.Add(new InventarioEstoqueMap());
        modelBuilder.Configurations.Add(new LocalArmazenamentoMap());
        modelBuilder.Configurations.Add(new MarcaProdutoMap());
        modelBuilder.Configurations.Add(new PaisMap());
        modelBuilder.Configurations.Add(new PerfilMap());
        modelBuilder.Configurations.Add(new ProdutoMap());
        modelBuilder.Configurations.Add(new SaidaProdutoMap());
        modelBuilder.Configurations.Add(new UnidadeMedidaMap());
        modelBuilder.Configurations.Add(new UsuarioMap());
    }
    //Atraves desta  Cidades nos conseguimos fazer a query e as consultas no banco de dados.
    public DbSet<CidadeModel> Cidades { get; set; }
    public DbSet<EntradaProdutoModel> EntradasProdutos { get; set; }
    public DbSet<EstadoModel> Estados { get; set; }
    public DbSet<FornecedorModel> Fornecedores { get; set; }
    public DbSet<GrupoProdutoModel> GruposProdutos { get; set; }
    public DbSet<InventarioEstoqueModel> InventariosEstoque { get; set; }
    public DbSet<LocalArmazenamentoModel> LocaisArmazenamentos { get; set; }
    public DbSet<MarcaProdutoModel> MarcasProdutos { get; set; }
    public DbSet<PaisModel> Paises { get; set; }
    public DbSet<PerfilModel> PerfisUsuarios { get; set; }
    public DbSet<ProdutoModel> Produtos { get; set; }
    public DbSet<SaidaProdutoModel> SaidasProdutos { get; set; }
    public DbSet<UnidadeMedidaModel> UnidadesMedida { get; set; }
    public DbSet<UsuarioModel> Usuarios { get; set; }
}
}
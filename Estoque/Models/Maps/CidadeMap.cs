

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Estoque.Models.Maps
{
    // => EntityTypeConfiguration É a base dos mapeamento
    /// <summary>
    /// Aqui ela esta dizendo CidadeMap esta mapeando CidadeModel atraves do (EntityTypeConfiguration)
    /// </summary>
    public class CidadeMap : EntityTypeConfiguration<CidadeModel>
    {
        public CidadeMap()
        {
            //ToTable eu estou indicando o nome da tabela
            ToTable("cidade");

            // Estou dizendo que a chave primaria (HasKey) é este campo Id
            HasKey(x => x.Id);

            //Eu estou pegando a minha propriedade Id da entidade e tranformando em (id minusculo para a base de dados)
            // Depois estou dizendo usando (HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity) "para dizer que o id terá autoincremento")
            Property(x => x.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //Já neste passo a propriedade como a acima e o que vai para base de dados (nome). só que eu digo que eu quero 30 caracters no maximo e de forma obrigatoria.
            Property(x => x.Nome).HasColumnName("nome").HasMaxLength(30).IsRequired();

            Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

            Property(x => x.IdEstado).HasColumnName("id_estado").IsRequired();
            HasRequired(x => x.Estado).WithMany().HasForeignKey(x => x.IdEstado).WillCascadeOnDelete(false);
        }
    }
}
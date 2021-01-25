

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Estoque.Models.Maps
{
    public class PerfilMap : EntityTypeConfiguration<PerfilModel>
    {
        public PerfilMap()
        {
            ToTable("perfil");

            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Nome).HasColumnName("nome").HasMaxLength(20).IsRequired();
            Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

            //Este mapeamento e de muito para muitos eu escolhi colocar na classe Pai, poderia ter cido em outra. 
            //Observação não pode colocar o mapeameto M para M nas duas
            HasMany(x => x.Usuarios).WithMany(x => x.Perfis)
                .Map(x =>
                {
                    x.ToTable("perfil_usuario");
                    x.MapLeftKey("id_perfil");
                    x.MapRightKey("id_usuario");
                });
        }
    }
}

using AutoMapper;

namespace Estoque.AutoMapper
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            //vou inicializar o mapeador = com o Mapper
            Mapper.Initialize(x =>
            {
                //e aqui eu estou adicionando o Profile
                x.AddProfile<AutoMapperProfile>();
            });
        }
    }
}
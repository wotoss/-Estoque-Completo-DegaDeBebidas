

using AutoMapper;
using Estoque.Models;
using Estoque.Models.ViewModel;

namespace Estoque.AutoMapper
{
    //Auto Mapper faz todos os mapeamentos entre classes 
    //Por exemplo pode transformar um CidadeModel em CidadeViewModel
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CidadeViewModel, CidadeModel>().ReverseMap();
            CreateMap<EstadoViewModel, EstadoModel>().ReverseMap();
            CreateMap<FornecedorViewModel, FornecedorModel>().ReverseMap();
            CreateMap<GrupoProdutoViewModel, GrupoProdutoModel>().ReverseMap();
            CreateMap<LocalArmazenamentoViewModel, LocalArmazenamentoModel>().ReverseMap();
            CreateMap<MarcaProdutoViewModel, MarcaProdutoModel>().ReverseMap();
            CreateMap<PaisModel, PaisViewModel>().ReverseMap();
            CreateMap<PerfilViewModel, PerfilModel>().ReverseMap();
            CreateMap<MarcaProdutoViewModel, MarcaProdutoModel>().ReverseMap();//vamos
            CreateMap<ProdutoViewModel, ProdutoModel>().ReverseMap();
            CreateMap<UnidadeMedidaViewModel, UnidadeMedidaModel>().ReverseMap();
            CreateMap<UsuarioModel, UsuarioViewModel>().ReverseMap();

        }
    }
}
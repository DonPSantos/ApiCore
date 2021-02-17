using ApiCore.Api.ViewModel;
using ApiCore.Business.Models;
using AutoMapper;

namespace ApiCore.Api.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
            CreateMap<ProdutoViewModel, Produto>();
            CreateMap<Produto, ProdutoViewModel>()
                .ForMember(dest => dest.NomeFornecedor,opt => opt.MapFrom(src => src.Fornecedor.Nome));
        }
    }
}
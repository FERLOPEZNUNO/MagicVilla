using AutoMapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;

namespace MagicVilla_API
{

    public class ConfigMapeador : Profile
    {

        public ConfigMapeador()
        {
            //fuente y destino del mapeador.
            CreateMap<Villa, VillaDto>();
            //tb hay que hacer lo inverso:
            CreateMap<VillaDto, Villa>();

            //el "inverso" se puede hacer en 1 sola linea:
            CreateMap<Villa, VillaCreateDto>().ReverseMap();
            CreateMap<Villa, VillaUpdateDto>().ReverseMap();
        }
    }
}
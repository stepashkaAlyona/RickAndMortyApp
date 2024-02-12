using AutoMapper;

using RM.BLL.Models.ThirdPartyApi;
using RM.BLL.Models.ViewModel;
using RM.DAL.Models;

namespace RM.API.Mapper
{
    public class MappingProfile : Profile
    {
        private const string UnknownTextStub = "unknown";

        private const string UnknownImageStub = "https://rickandmortyapi.com/api/character/avatar/19.jpeg";

        public MappingProfile()
        {
            CreateMap<ApiCharacter, CharacterDb>()
                .ForMember(x => x.Status, x => x.MapFrom(y => string.IsNullOrWhiteSpace(y.Status) ? UnknownTextStub : y.Status))
                .ForMember(x => x.Species, x => x.MapFrom(y => string.IsNullOrWhiteSpace(y.Species) ? UnknownTextStub : y.Species))
                .ForMember(x => x.Type, x => x.MapFrom(y => string.IsNullOrWhiteSpace(y.Type) ? UnknownTextStub : y.Type))
                .ForMember(x => x.Gender, x => x.MapFrom(y => string.IsNullOrWhiteSpace(y.Gender) ? UnknownTextStub : y.Gender))
                .ForMember(x => x.Image, x => x.MapFrom(y => string.IsNullOrWhiteSpace(y.Image) ? UnknownImageStub : y.Image))
                .ForMember(x => x.Url, x => x.MapFrom(y => string.IsNullOrWhiteSpace(y.Url) ? UnknownTextStub : y.Url))
                .ForMember(x => x.Episode, x => x.MapFrom(y => string.Join(", ", y.Episode)))
                .ForMember(x => x.LocationName, x => x.MapFrom(y => string.IsNullOrWhiteSpace(y.Location.Name) ? UnknownTextStub : y.Location.Name))
                .ForMember(x => x.OriginName, x => x.MapFrom(y => string.IsNullOrWhiteSpace(y.Origin.Name) ? UnknownTextStub : y.Origin.Name));

            CreateMap<CharacterDb, CharacterVM>()
                .ForMember(
                    x => x.Episode,
                    x => x.MapFrom(
                        y => string.IsNullOrWhiteSpace(y.Episode)
                                 ? Array.Empty<string>()
                                 : y.Episode.Split(',', StringSplitOptions.None).Select(s => s.Trim())));

            CreateMap<CharacterDb, CharacterListVM>();

            CreateMap<CreateCharacterDb, CreateCharacterVM>();
            CreateMap<CreateCharacterVM, CreateCharacterDb>();
        }
    }
}

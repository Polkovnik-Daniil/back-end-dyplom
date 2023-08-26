using AutoMapper;
using DTO;
using Models;

namespace WebAPI.Helpers
{
    public class AutoMappingProfiles : Profile
    {
        public AutoMappingProfiles() {
            CreateMap<Author, DTOAuthor>().ReverseMap();
            CreateMap<Book, DTOBook>().ReverseMap();
            CreateMap<Reader, DTOReader>().ReverseMap();
        }
    }
}

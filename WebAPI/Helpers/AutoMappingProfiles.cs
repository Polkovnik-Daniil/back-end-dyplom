using AutoMapper;
using DTO;
using Models;

namespace WebAPI.Helpers
{
    public class AutoMappingProfiles : Profile
    {
        public AutoMappingProfiles() {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Reader, ReaderDTO>().ReverseMap();
        }
    }
}

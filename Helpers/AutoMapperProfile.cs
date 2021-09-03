using AutoMapper;
using contactos.DTOs;
using contactos.Models;

namespace contactos.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Contacto, ContactoDto>().ReverseMap();
        }
    }
}

using AutoMapper;
using FinanceAPI.Data.DTOs;
using FinanceAPI.Models;

namespace FinanceAPI.Data.Mappers
{
    public class UsuarioProfile: Profile
    {
        public UsuarioProfile()
        {
            CreateMap<UserDTO, Usuario>();
            CreateMap<Usuario, UserDTO>();
        }
    }
}
